
using NUnit.Framework;

namespace BehaviorTree.Composites
{
	[TestFixture]
	public class SequenceTests
	{
		[Test]
		public void Success()
		{
			Asserts.Success(new Sequence());
			Asserts.Success(new Sequence(Node.Success));
			Asserts.Success(new Sequence(Node.Success, Node.Success));
			Asserts.Success(new Sequence(Node.Success, Node.Success, Node.Success));
		}

		[Test]
		public void Failure()
		{
			Asserts.Fail(new Sequence(Node.Fail));
			Asserts.Fail(new Sequence(Node.Fail, Node.Success));
			Asserts.Fail(new Sequence(Node.Success, Node.Fail));
			Asserts.Fail(new Sequence(Node.Fail, Node.Running));
			Asserts.Fail(new Sequence(Node.Success, Node.Fail));
			Asserts.Fail(new Sequence(Node.Success, Node.Fail, Node.Success));
			Asserts.Fail(new Sequence(Node.Success, Node.Fail, Node.Running));
		}

		[Test]
		public void Running()
		{
			Asserts.Running(new Sequence(Node.Running),
				"Behavior/Sequence/Running");
			Asserts.Running(new Sequence(Node.Success, Node.Running),
				"Behavior/Sequence/Running");
			Asserts.Running(new Sequence(Node.Success, Node.Running, Node.Success),
				"Behavior/Sequence/Running");
			Asserts.Running(new Sequence(Node.Success, Node.Running, Node.Fail),
				"Behavior/Sequence/Running");
		}

		[Test]
		public void LongRunning1()
		{
			var callCount = 0;
			var runTwice = new Act("RunTwice", () =>
			{
				callCount++;
				return callCount > 2 ? Result.Success : Result.Running;
			});

			var behavior = new Behavior(new Sequence(Node.Success, runTwice, Node.Fail));
			Asserts.Running(behavior, "Behavior/Sequence/RunTwice");
			Asserts.Running(behavior, "Behavior/Sequence/RunTwice");
			Asserts.Fail(behavior);
			Asserts.Fail(behavior);
			Assert.AreEqual(4, callCount);
		}

		[Test]
		public void LongRunning2()
		{
			var node1CallCount = 0;
			var node1 = new Act(() =>
			{
				node1CallCount++;
				return Result.Success;
			});

			var node2CallCount = 0;
			var node2 = new Act("Act2", () =>
			{
				node2CallCount++;
				return node2CallCount > 4 ? Result.Success : Result.Running;
			});

			var seq = new Behavior("Seq",
				new Sequence(node1, node2, node1, node2));

			Asserts.Running(seq, "Seq/Sequence/Act2");
			Asserts.Running(seq, "Seq/Sequence/Act2");
			Asserts.Running(seq, "Seq/Sequence/Act2");
			Asserts.Running(seq, "Seq/Sequence/Act2");
			Asserts.Success(seq);

			Assert.AreEqual(node1CallCount, 2);
			Assert.AreEqual(node2CallCount, 6);
		}

		[Test]
		public void DeepSequenceTree()
		{
			var onStartCount = 0;
			var onCompletedCount = 0;

			var mockObserver = new MockObserver(
				() => onStartCount++,
				() => onCompletedCount++);

			Result result = Result.Running;

			var behavior = new Behavior(
				CreateSequence(mockObserver,
					CreateNode(mockObserver, () => Result.Success),
					CreateSequence(mockObserver,
						CreateNode(mockObserver, () => Result.Success),
						CreateNode(mockObserver, () => Result.Success),
						CreateSequence(mockObserver,
							CreateNode(mockObserver, () => Result.Success),
							CreateNode(mockObserver, () => result),
							CreateNode(mockObserver, () => Result.Running)))));

			Assert.AreEqual(Result.Running, behavior.Run());
			Assert.AreEqual(1, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/Sequence/Sequence/Sequence/Act",
				behavior.RunningNodePaths[0]);

			Assert.AreEqual(8, onStartCount);
			Assert.AreEqual(4, onCompletedCount);

			onStartCount = 0;
			onCompletedCount = 0;

			Assert.AreEqual(Result.Running, behavior.Run());
			Assert.AreEqual(1, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/Sequence/Sequence/Sequence/Act",
				behavior.RunningNodePaths[0]);

			Assert.AreEqual(0, onStartCount);
			Assert.AreEqual(0, onCompletedCount);

			result = Result.Success;

			Assert.AreEqual(Result.Running, behavior.Run());
			Assert.AreEqual(1, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/Sequence/Sequence/Sequence/Act",
				behavior.RunningNodePaths[0]);

			Assert.AreEqual(1, onStartCount);
			Assert.AreEqual(1, onCompletedCount);
		}

		private INode CreateNode(INodeObserver observer, Delegates.Func<Result> act)
		{
			var node = new Act(act);
			node.AddObserver(observer);
			return node;
		}

		private INode CreateSequence(INodeObserver observer, params INode[] nodes)
		{
			var seq = new Sequence(nodes);
			seq.AddObserver(observer);
			return seq;
		}
	}

	public class MockObserver : INodeObserver
	{
		private readonly Delegates.Func onStarted;
		private readonly Delegates.Func onCompleted;

		public MockObserver(Delegates.Func onStarted, Delegates.Func onCompleted)
		{
			this.onStarted = onStarted;
			this.onCompleted = onCompleted;
		}

		public void OnStarted(INode node)
		{
			this.onStarted();
		}

		public void OnCompleted(INode node, Result result)
		{
			this.onCompleted();
		}
	}
}