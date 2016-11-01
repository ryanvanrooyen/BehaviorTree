
using NUnit.Framework;

namespace BehaviorTree.Composites
{
	[TestFixture]
	public class SelectorTests
	{
		[Test]
		public void Success()
		{
			Asserts.Success(new Selector(Node.Success));
			Asserts.Success(new Selector(Node.Success, Node.Fail));
			Asserts.Success(new Selector(Node.Success, Node.Running));
			Asserts.Success(new Selector(Node.Fail, Node.Success));
			Asserts.Success(new Selector(Node.Fail, Node.Fail, Node.Success));
		}

		[Test]
		public void Failure()
		{
			Asserts.Fail(new Selector());
			Asserts.Fail(new Selector(Node.Fail));
			Asserts.Fail(new Selector(Node.Fail, Node.Fail));
		}

		[Test]
		public void Running()
		{
			Asserts.Running(new Selector(Node.Running),
				"Behavior/Selector/Running");
			Asserts.Running(new Selector(Node.Fail, Node.Running),
				"Behavior/Selector/Running");
			Asserts.Running(new Selector(Node.Fail, Node.Fail, Node.Running),
				"Behavior/Selector/Running");
			Asserts.Running(new Selector(Node.Fail, Node.Running, Node.Fail),
				"Behavior/Selector/Running");
			Asserts.Running(new Selector(Node.Fail, Node.Running, Node.Success),
				"Behavior/Selector/Running");
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

			Asserts.Running(behavior, "Behavior/Selector/Act2");
			Asserts.Running(behavior, "Behavior/Selector/Act2");
			Asserts.Success(behavior);

			Assert.AreEqual(node1CallCount, 2);
			Assert.AreEqual(node2CallCount, 4);
		}
	}
}