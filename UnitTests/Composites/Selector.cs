
using NUnit.Framework;

namespace BehaviorTree.Composites
{
	[TestFixture]
	public class SelectorTests
	{
		[Test]
		public void Success()
		{
			Asserts.Selector(Result.Success, Node.Success);
			Asserts.Selector(Result.Success, Node.Success, Node.Fail);
			Asserts.Selector(Result.Success, Node.Success, Node.Running);
			Asserts.Selector(Result.Success, Node.Fail, Node.Success);
			Asserts.Selector(Result.Success, Node.Fail, Node.Fail, Node.Success);
		}

		[Test]
		public void Failure()
		{
			Asserts.Selector(Result.Failure);
			Asserts.Selector(Result.Failure, Node.Fail);
			Asserts.Selector(Result.Failure, Node.Fail, Node.Fail);
		}

		[Test]
		public void Running()
		{
			Asserts.Selector(Result.Running, Node.Running);
			Asserts.Selector(Result.Running, Node.Fail, Node.Running);
			Asserts.Selector(Result.Running, Node.Fail, Node.Fail, Node.Running);
			Asserts.Selector(Result.Running, Node.Fail, Node.Running, Node.Fail);
			Asserts.Selector(Result.Running, Node.Fail, Node.Running, Node.Success);
		}

		[Test]
		public void LongRunning()
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
				if (node2CallCount == 1)
					return Result.Running;
				if (node2CallCount == 2)
					return Result.Failure;
				if (node2CallCount == 3)
					return Result.Running;
				return Result.Success;
			});

			var behavior = new Behavior(
				new Selector(node1, node2, node1, node2));

			Asserts.Equal(Result.Running, behavior);
			Assert.AreEqual(1, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/Selector/Act2", behavior.RunningNodePaths[0]);

			Asserts.Equal(Result.Running, behavior);
			Assert.AreEqual(1, behavior.RunningNodePaths.Length);
			Assert.AreEqual("Behavior/Selector/Act2", behavior.RunningNodePaths[0]);

			Asserts.Equal(Result.Success, behavior);

			Assert.AreEqual(node1CallCount, 2);
			Assert.AreEqual(node2CallCount, 4);
		}
	}
}