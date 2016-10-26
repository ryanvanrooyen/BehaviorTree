
using NUnit.Framework;

namespace BehaviorTree.Composites
{
	[TestFixture]
	public class SequenceTests
	{
		[Test]
		public void Success()
		{
			Asserts.Sequence(Result.Success);
			Asserts.Sequence(Result.Success, Node.Success);
			Asserts.Sequence(Result.Success, Node.Success, Node.Success);
			Asserts.Sequence(Result.Success, Node.Success, Node.Success, Node.Success);
		}

		[Test]
		public void Failure()
		{
			Asserts.Sequence(Result.Failure, Node.Fail);
			Asserts.Sequence(Result.Failure, Node.Fail, Node.Success);
			Asserts.Sequence(Result.Failure, Node.Success, Node.Fail);
			Asserts.Sequence(Result.Failure, Node.Fail, Node.Running);
			Asserts.Sequence(Result.Failure, Node.Success, Node.Fail);
			Asserts.Sequence(Result.Failure, Node.Success, Node.Fail, Node.Success);
			Asserts.Sequence(Result.Failure, Node.Success, Node.Fail, Node.Running);
		}

		[Test]
		public void Running()
		{
			Asserts.Sequence(Result.Running, Node.Running);
			Asserts.Sequence(Result.Running, Node.Success, Node.Running);
			Asserts.Sequence(Result.Running, Node.Success, Node.Running, Node.Success);
			Asserts.Sequence(Result.Running, Node.Success, Node.Running, Node.Fail);
		}

		[Test]
		public void LongRunning()
		{
			var callCount = 0;
			var runOnce = new Act("RunOnce", () =>
			{
				callCount++;
				return callCount > 2 ? Result.Success : Result.Running;
			});

			var behavior = new Behavior(new Sequence(Node.Success, runOnce, Node.Fail));
			Assert.AreEqual(Result.Running, behavior.Run());
			Assert.AreEqual(1, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/Sequence/RunOnce", behavior.RunningNodePaths[0]);
			Assert.AreEqual(Result.Running, behavior.Run());
			Assert.AreEqual(Result.Failure, behavior.Run());
			Assert.AreEqual(Result.Failure, behavior.Run());
			Assert.AreEqual(4, callCount);

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

			Asserts.Equal(Result.Running, seq);
			Assert.AreEqual(1, seq.RunningNodePaths.Length);
			Assert.AreEqual("Seq/Sequence/Act2", seq.RunningNodePaths[0]);

			Asserts.Equal(Result.Running, seq);
			Assert.AreEqual(1, seq.RunningNodePaths.Length);
			Assert.AreEqual("Seq/Sequence/Act2", seq.RunningNodePaths[0]);

			Asserts.Equal(Result.Running, seq);
			Assert.AreEqual(1, seq.RunningNodePaths.Length);
			Assert.AreEqual("Seq/Sequence/Act2", seq.RunningNodePaths[0]);

			Asserts.Equal(Result.Running, seq);
			Assert.AreEqual(1, seq.RunningNodePaths.Length);
			Assert.AreEqual("Seq/Sequence/Act2", seq.RunningNodePaths[0]);

			Asserts.Equal(Result.Success, seq);
			Assert.AreEqual(0, seq.RunningNodePaths.Length);

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