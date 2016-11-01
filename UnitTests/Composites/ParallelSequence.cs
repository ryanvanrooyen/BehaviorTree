
using NUnit.Framework;

namespace BehaviorTree.Composites
{
	[TestFixture]
	public class ParallelSequenceTests
	{
		[Test]
		public void Success()
		{
			Asserts.Success(new ParallelSequence());
			Asserts.Success(new ParallelSequence(Node.Success));
			Asserts.Success(new ParallelSequence(new Invert(Node.Fail)));
			Asserts.Success(new ParallelSequence(Node.Success, Node.Success));
			Asserts.Success(new ParallelSequence(Node.Success, Node.Success, Node.Success));
		}

		[Test]
		public void Failure()
		{
			Asserts.Fail(new ParallelSequence(Node.Fail));
			Asserts.Fail(new ParallelSequence(new Invert(Node.Success)));
			Asserts.Fail(new ParallelSequence(Node.Fail, Node.Success));
			Asserts.Fail(new ParallelSequence(Node.Success, Node.Fail));
			Asserts.Fail(new ParallelSequence(Node.Fail, Node.Running));
			Asserts.Fail(new ParallelSequence(Node.Running, Node.Fail));
			Asserts.Fail(new ParallelSequence(Node.Success, Node.Fail));
			Asserts.Fail(new ParallelSequence(Node.Success, Node.Fail, Node.Success));
			Asserts.Fail(new ParallelSequence(Node.Success, Node.Fail, Node.Running));
			Asserts.Fail(new ParallelSequence(Node.Running, Node.Running, Node.Fail));
		}

		[Test]
		public void Running()
		{
			Asserts.Running(new ParallelSequence(Node.Running),
				"Behavior/ParallelSequence/Running");
			Asserts.Running(new ParallelSequence(Node.Success, Node.Running),
				"Behavior/ParallelSequence/Success",
				"Behavior/ParallelSequence/Running");
			Asserts.Running(new ParallelSequence(Node.Success, Node.Running, Node.Success),
				"Behavior/ParallelSequence/Success",
				"Behavior/ParallelSequence/Running",
				"Behavior/ParallelSequence/Success");
			Asserts.Running(new ParallelSequence(new Invert(Node.Fail), Node.Running, Node.Running),
				"Behavior/ParallelSequence/!Fail",
				"Behavior/ParallelSequence/Running",
				"Behavior/ParallelSequence/Running");
			Asserts.Running(new ParallelSequence(Node.Running, Node.Success, Node.Running),
				"Behavior/ParallelSequence/Running",
				"Behavior/ParallelSequence/Success",
				"Behavior/ParallelSequence/Running");
			Asserts.Running(new ParallelSequence(Node.Running, Node.Running, Node.Success),
				"Behavior/ParallelSequence/Running",
				"Behavior/ParallelSequence/Running",
				"Behavior/ParallelSequence/Success");
		}

		[Test]
		public void LongRunningSucceed()
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
				if (node2CallCount < 3)
					return Result.Running;
				return Result.Success;
			});

			var node3CallCount = 0;
			var node3 = new Act(() =>
			{
				node3CallCount++;
				return Result.Success;
			});

			var behavior = new Behavior(new ParallelSequence(node1, node2, node3));

			Asserts.Running(behavior,
				"Behavior/ParallelSequence/Act",
				"Behavior/ParallelSequence/Act2",
				"Behavior/ParallelSequence/Act");
			Asserts.Counts(1, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Running(behavior,
				"Behavior/ParallelSequence/Act",
				"Behavior/ParallelSequence/Act2",
				"Behavior/ParallelSequence/Act");
			Asserts.Counts(2, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Success(behavior);
			Asserts.Counts(3, node1CallCount, node2CallCount, node3CallCount);
		}

		[Test]
		public void LongRunningFail()
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
				return Result.Failure;
			});

			var node3CallCount = 0;
			var node3 = new Act(() =>
			{
				node3CallCount++;
				return Result.Success;
			});

			var behavior = new Behavior(new ParallelSequence(node1, node2, node3));

			Asserts.Running(behavior,
				"Behavior/ParallelSequence/Act1",
				"Behavior/ParallelSequence/Act2",
				"Behavior/ParallelSequence/Act");
			Asserts.Counts(1, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Running(behavior,
				"Behavior/ParallelSequence/Act1",
				"Behavior/ParallelSequence/Act2",
				"Behavior/ParallelSequence/Act");
			Asserts.Counts(2, node1CallCount, node2CallCount, node3CallCount);

			Asserts.Fail(behavior);
			Asserts.Counts(3, node1CallCount, node2CallCount, node3CallCount);
		}

		[Test]
		public void DeepTreeRunning()
		{
			var behavior = new Behavior(
				new Sequence(
					Node.Success,
					new Selector(Node.Fail,
						new ParallelSequence(
							new Sequence(
								Node.Success,
								new Selector(
									Node.Fail,
									new Sequence(
										Node.Success,
										Node.Running,
										Node.Success))),
							new Sequence(
								Node.Success,
								new Sequence(
									Node.Success,
									Node.Success,
									new Selector(
										Node.Fail,
										Node.Running,
										Node.Fail))))),
					Node.Success));

			Asserts.Running(behavior,
				"Behavior/Sequence/Selector/ParallelSequence/Sequence/Selector/Sequence/Running",
				"Behavior/Sequence/Selector/ParallelSequence/Sequence/Sequence/Selector/Running");
		}

		[Test]
		public void RunningExample1()
		{
			var patrol = new Act("Patrol", () => Result.Running);
			var attackTarget = new Act("Attack", () => Result.Running);

			var canSeeTarget = false;

			var behavior = new Behavior(new Selector(
				new Sequence(new If("IfCanSeeTarget", () => canSeeTarget),
					 new ParallelSequence(new If("IfCanSeeTarget", () => canSeeTarget), attackTarget)),
				new Sequence(new Invert(new If("IfCanSeeTarget", () => canSeeTarget)),
					new ParallelSequence(new Invert(new If("IfCanSeeTarget", () => canSeeTarget)), patrol))));

			Asserts.Running(behavior,
				"Behavior/Selector/Sequence/ParallelSequence/!IfCanSeeTarget",
				"Behavior/Selector/Sequence/ParallelSequence/Patrol");

			canSeeTarget = true;

			Asserts.Fail(behavior);

			Asserts.Running(behavior,
				"Behavior/Selector/Sequence/ParallelSequence/IfCanSeeTarget",
				"Behavior/Selector/Sequence/ParallelSequence/Attack");

			canSeeTarget = false;

			Asserts.Running(behavior,
				"Behavior/Selector/Sequence/ParallelSequence/!IfCanSeeTarget",
				"Behavior/Selector/Sequence/ParallelSequence/Patrol");
		}

		[Test]
		public void WhileExample()
		{
			var patrol = new Act("Patrol", () => Result.Running);
			var attackTarget = new Act("Attack", () => Result.Running);

			var canSeeTarget = false;

			var behavior = new Behavior(new Selector(
				new While(() => canSeeTarget, attackTarget),
				new While(() => !canSeeTarget, patrol)));

			Asserts.Running(behavior,
				"Behavior/Selector/While/Parallel/If",
				"Behavior/Selector/While/Parallel/Patrol");

			canSeeTarget = true;

			Asserts.Fail(behavior);

			Asserts.Running(behavior,
				"Behavior/Selector/While/Parallel/If",
				"Behavior/Selector/While/Parallel/Attack");

			canSeeTarget = false;

			Asserts.Running(behavior,
				"Behavior/Selector/While/Parallel/If",
				"Behavior/Selector/While/Parallel/Patrol");
		}
	}
}
