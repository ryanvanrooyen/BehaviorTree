
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
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Fail, Node.Running),
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Running, Node.Fail),
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Fail, Node.Fail, Node.Running),
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Fail, Node.Running, Node.Fail),
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Fail, Node.Running, Node.Running),
				"ParallelSelector/Running",
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Running, Node.Fail, Node.Running),
				"ParallelSelector/Running",
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(Node.Running, Node.Running, Node.Fail),
				"ParallelSelector/Running",
				"ParallelSelector/Running");
		}

		[Test]
		public void RunningRevalidate()
		{
			Asserts.Running(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, Node.Running),
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, Node.Fail, Node.Running),
				"ParallelSelector/Fail",
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, Node.Running, Node.Fail),
				"ParallelSelector/Running",
				"ParallelSelector/Fail");
			Asserts.Running(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, Node.Fail, Node.Fail, Node.Running),
				"ParallelSelector/Fail",
				"ParallelSelector/Fail",
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, Node.Fail, Node.Running, Node.Fail),
				"ParallelSelector/Fail",
				"ParallelSelector/Running",
				"ParallelSelector/Fail");
			Asserts.Running(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, Node.Fail, Node.Running, Node.Running),
				"ParallelSelector/Fail",
				"ParallelSelector/Running",
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, Node.Running, Node.Fail, Node.Running),
				"ParallelSelector/Running",
				"ParallelSelector/Fail",
				"ParallelSelector/Running");
			Asserts.Running(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, Node.Running, Node.Running, Node.Fail),
				"ParallelSelector/Running",
				"ParallelSelector/Running",
				"ParallelSelector/Fail");
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
				"ParallelSelector/Act1",
                "ParallelSelector/Act2");
			Asserts.Counts(1, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Running(behavior,
				"ParallelSelector/Act1",
				"ParallelSelector/Act2");
			Asserts.Counts(1, node3CallCount);
			Asserts.Counts(2, node1CallCount, node2CallCount);

			Asserts.Success(behavior);
			Asserts.Counts(1, node3CallCount);
			Asserts.Counts(3, node1CallCount, node2CallCount);
		}

		[Test]
		public void LongRunningSucceedRevalidate()
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

			var behavior = new Behavior(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate,  node1, node2, node3));

			Asserts.Running(behavior,
				"ParallelSelector/Act1",
				"ParallelSelector/Act2",
				"ParallelSelector/Act");
			Asserts.Counts(1, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Running(behavior,
				"ParallelSelector/Act1",
				"ParallelSelector/Act2",
				"ParallelSelector/Act");
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

			Asserts.Running(behavior, "ParallelSelector/Act2");
			Asserts.Counts(1, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Running(behavior, "ParallelSelector/Act2");
			Asserts.Counts(1, node1CallCount, node3CallCount);
			Asserts.Counts(2, node2CallCount);

			Asserts.Fail(behavior);
			Asserts.Counts(1, node1CallCount, node3CallCount);
			Asserts.Counts(3, node2CallCount);
		}

		[Test]
		public void LongRunningFailRevalidate()
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

			var behavior = new Behavior(new ParallelSelector(
				ChildRunPolicy.ParallelRevalidate, node1, node2, node3));

			Asserts.Running(behavior,
				"ParallelSelector/Act",
				"ParallelSelector/Act2",
				"ParallelSelector/Act");
			Asserts.Counts(1, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Running(behavior,
				"ParallelSelector/Act",
				"ParallelSelector/Act2",
				"ParallelSelector/Act");
			Asserts.Counts(2, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Fail(behavior);
			Asserts.Counts(3, node1CallCount, node2CallCount, node3CallCount);
		}
	}
}