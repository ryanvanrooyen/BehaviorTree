
using NUnit.Framework;

namespace BehaviorTree.Composites
{
	[TestFixture]
	public class ParallelSelectorTests
	{
		[Test]
		public void Success()
		{
			Asserts.Success(new ParallelSelector(Node.Success));
			Asserts.Success(new ParallelSelector(Node.Success, Node.Fail));
			Asserts.Success(new ParallelSelector(Node.Success, Node.Running));
			Asserts.Success(new ParallelSelector(Node.Fail, Node.Success));
			Asserts.Success(new ParallelSelector(Node.Fail, Node.Fail, Node.Success));
		}

		[Test]
		public void Failure()
		{
			Asserts.Fail(new ParallelSelector());
			Asserts.Fail(new ParallelSelector(Node.Fail));
			Asserts.Fail(new ParallelSelector(Node.Fail, Node.Fail));
		}

		[Test]
		public void Running()
		{
			Asserts.Running(new ParallelSelector(Node.Running),
				"Behavior/ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Fail, Node.Running),
				"Behavior/ParallelSelector/Fail",
				"Behavior/ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Running, Node.Fail),
				"Behavior/ParallelSelector/Running",
				"Behavior/ParallelSelector/Fail");
			Asserts.Running(new ParallelSelector(Node.Fail, Node.Fail, Node.Running),
				"Behavior/ParallelSelector/Fail",
				"Behavior/ParallelSelector/Fail",
				"Behavior/ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Fail, Node.Running, Node.Fail),
				"Behavior/ParallelSelector/Fail",
				"Behavior/ParallelSelector/Running",
				"Behavior/ParallelSelector/Fail");
			Asserts.Running(new ParallelSelector(Node.Fail, Node.Running, Node.Running),
				"Behavior/ParallelSelector/Fail",
				"Behavior/ParallelSelector/Running",
				"Behavior/ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Running, Node.Fail, Node.Running),
				"Behavior/ParallelSelector/Running",
				"Behavior/ParallelSelector/Fail",
				"Behavior/ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Running, Node.Running, Node.Fail),
				"Behavior/ParallelSelector/Running",
				"Behavior/ParallelSelector/Running",
				"Behavior/ParallelSelector/Fail");
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

			Asserts.Running(behavior,
				"Behavior/ParallelSelector/Act1",
				"Behavior/ParallelSelector/Act2",
				"Behavior/ParallelSelector/Act");
			Asserts.Counts(1, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Running(behavior,
				"Behavior/ParallelSelector/Act1",
				"Behavior/ParallelSelector/Act2",
				"Behavior/ParallelSelector/Act");
			Asserts.Counts(2, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Success(behavior);
			Asserts.Counts(3, node1CallCount, node2CallCount, node3CallCount);
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

			Asserts.Running(behavior,
				"Behavior/ParallelSelector/Act",
				"Behavior/ParallelSelector/Act2",
				"Behavior/ParallelSelector/Act");
			Asserts.Counts(1, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Running(behavior,
				"Behavior/ParallelSelector/Act",
				"Behavior/ParallelSelector/Act2",
				"Behavior/ParallelSelector/Act");
			Asserts.Counts(2, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Fail(behavior);
			Asserts.Counts(3, node1CallCount, node2CallCount, node3CallCount);
		}
	}
}