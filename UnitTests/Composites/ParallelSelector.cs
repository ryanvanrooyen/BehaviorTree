
using NUnit.Framework;

namespace BehaviorTree.Composites
{
	[TestFixture]
	public class ParallelSelectorTests
	{
		[Test]
		public void Success()
		{
			Asserts.ParallelSel(Result.Success, Node.Success);
			Asserts.ParallelSel(Result.Success, Node.Success, Node.Fail);
			Asserts.ParallelSel(Result.Success, Node.Success, Node.Running);
			Asserts.ParallelSel(Result.Success, Node.Fail, Node.Success);
			Asserts.ParallelSel(Result.Success, Node.Fail, Node.Fail, Node.Success);
		}

		[Test]
		public void Failure()
		{
			Asserts.ParallelSel(Result.Failure);
			Asserts.ParallelSel(Result.Failure, Node.Fail);
			Asserts.ParallelSel(Result.Failure, Node.Fail, Node.Fail);
		}

		[Test]
		public void Running()
		{
			Asserts.ParallelSel(Result.Running, Node.Running);
			Asserts.ParallelSel(Result.Running, Node.Fail, Node.Running);
			Asserts.ParallelSel(Result.Running, Node.Running, Node.Fail);
			Asserts.ParallelSel(Result.Running, Node.Fail, Node.Fail, Node.Running);
			Asserts.ParallelSel(Result.Running, Node.Fail, Node.Running, Node.Fail);
		}

		[Test]
		public void LongRunningSucceed()
		{
			var node1CallCount = 0;
			var node1 = new Act("Act1", () =>
			{
				node1CallCount++;
				return Result.Running;
			});

			var node2CallCount = 0;
			var node2 = new Act("Act2", () =>
			{
				node2CallCount++;
				if (node2CallCount < 3)
					return Result.Running;
				return Result.Success;
			});

			var node3CallCount = 0;
			var node3 = new Act(() =>
			{
				node3CallCount++;
				return Result.Failure;
			});

			var behavior = new Behavior(new ParallelSelector(node1, node2, node3));

			Asserts.Equal(Result.Running, behavior);
			Assert.AreEqual(true, behavior.HasRunningNodes);
			Assert.AreEqual(3, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/ParallelSelector/Act1", behavior.RunningNodePaths[0]);
			Assert.AreEqual("Behavior/ParallelSelector/Act2", behavior.RunningNodePaths[1]);
			Assert.AreEqual("Behavior/ParallelSelector/Act", behavior.RunningNodePaths[2]);

			Assert.AreEqual(node1CallCount, 1);
			Assert.AreEqual(node2CallCount, 1);
			Assert.AreEqual(node3CallCount, 1);

			Asserts.Equal(Result.Running, behavior);
			Assert.AreEqual(true, behavior.HasRunningNodes);
			Assert.AreEqual(3, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/ParallelSelector/Act1", behavior.RunningNodePaths[0]);
			Assert.AreEqual("Behavior/ParallelSelector/Act2", behavior.RunningNodePaths[1]);
			Assert.AreEqual("Behavior/ParallelSelector/Act", behavior.RunningNodePaths[2]);

			Assert.AreEqual(node1CallCount, 2);
			Assert.AreEqual(node2CallCount, 2);
			Assert.AreEqual(node3CallCount, 2);

			Asserts.Equal(Result.Success, behavior);
			Assert.AreEqual(false, behavior.HasRunningNodes);
			Assert.AreEqual(0, behavior.RunningNodePaths.Length);

			Assert.AreEqual(node1CallCount, 3);
			Assert.AreEqual(node2CallCount, 3);
			Assert.AreEqual(node3CallCount, 3);
		}

		[Test]
		public void LongRunningFail()
		{
			var node1CallCount = 0;
			var node1 = new Act(() =>
			{
				node1CallCount++;
				return Result.Failure;
			});

			var node2CallCount = 0;
			var node2 = new Act("Act2", () =>
			{
				node2CallCount++;
				if (node2CallCount < 3)
					return Result.Running;
				return Result.Failure;
			});

			var node3CallCount = 0;
			var node3 = new Act(() =>
			{
				node3CallCount++;
				return Result.Failure;
			});

			var behavior = new Behavior(new ParallelSelector(node1, node2, node3));

			Asserts.Equal(Result.Running, behavior);
			Assert.AreEqual(true, behavior.HasRunningNodes);
			Assert.AreEqual(3, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/ParallelSelector/Act", behavior.RunningNodePaths[0]);
			Assert.AreEqual("Behavior/ParallelSelector/Act2", behavior.RunningNodePaths[1]);
			Assert.AreEqual("Behavior/ParallelSelector/Act", behavior.RunningNodePaths[2]);

			Assert.AreEqual(node1CallCount, 1);
			Assert.AreEqual(node2CallCount, 1);
			Assert.AreEqual(node3CallCount, 1);

			Asserts.Equal(Result.Running, behavior);
			Assert.AreEqual(true, behavior.HasRunningNodes);
			Assert.AreEqual(3, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/ParallelSelector/Act", behavior.RunningNodePaths[0]);
			Assert.AreEqual("Behavior/ParallelSelector/Act2", behavior.RunningNodePaths[1]);
			Assert.AreEqual("Behavior/ParallelSelector/Act", behavior.RunningNodePaths[2]);

			Assert.AreEqual(node1CallCount, 2);
			Assert.AreEqual(node2CallCount, 2);
			Assert.AreEqual(node3CallCount, 2);

			Asserts.Equal(Result.Failure, behavior);
			Assert.AreEqual(false, behavior.HasRunningNodes);
			Assert.AreEqual(0, behavior.RunningNodePaths.Length);

			Assert.AreEqual(node1CallCount, 3);
			Assert.AreEqual(node2CallCount, 3);
			Assert.AreEqual(node3CallCount, 3);
		}
	}
}