
using NUnit.Framework;

namespace BehaviorTree.Composites
{
	[TestFixture]
	public class WhileTests
	{
		[Test]
		// This test makes sure the While node only calls
		// the condition once per behavior.Run() call.
		public void WhileCountTests()
		{
			// This test makes sure
			var ifValue = true;
			var result = Result.Success;
			var ifCount = 0;
			var trueCount = 0;
			var falseCount = 0;

			var behavior = new Behavior(new While(() => { ifCount++; return ifValue; },
				new Act(() => { trueCount++; return result; }),
				new Act(() => { falseCount++; return result; })));

			Asserts.Success(behavior);
			Asserts.Counts(1, ifCount);
			Asserts.Counts(1, trueCount);
			Asserts.Counts(0, falseCount);

			result = Result.Failure;
			Asserts.Fail(behavior);
			Asserts.Counts(2, ifCount);
			Asserts.Counts(2, trueCount);
			Asserts.Counts(0, falseCount);

			ifValue = false;

			result = Result.Success;
			Asserts.Success(behavior);
			Asserts.Counts(3, ifCount);
			Asserts.Counts(2, trueCount);
			Asserts.Counts(1, falseCount);

			result = Result.Failure;
			Asserts.Fail(behavior);
			Asserts.Counts(4, ifCount);
			Asserts.Counts(2, trueCount);
			Asserts.Counts(2, falseCount);

			ifValue = true;

			Asserts.Fail(behavior);
			Asserts.Counts(5, ifCount);
			Asserts.Counts(3, trueCount);
			Asserts.Counts(2, falseCount);

			result = Result.Success;
			Asserts.Success(behavior);
			Asserts.Counts(6, ifCount);
			Asserts.Counts(4, trueCount);
			Asserts.Counts(2, falseCount);
		}

		[Test]
		public void WhileExample1()
		{
			var attack1Count = 0;
			var attack2Count = 0;

			var attackTarget = new Sequence(
				new Act("Attack1", () => { attack1Count++; return Result.Success; }),
				new Act("Attack2", () => { attack2Count++; return Result.Running; }));

			var canSeeTarget = false;

			var behavior = new Behavior(new While(() => canSeeTarget, attackTarget));

			Asserts.Fail(behavior);
			Asserts.Counts(0, attack1Count, attack2Count);
			Asserts.Fail(behavior);
			Asserts.Counts(0, attack1Count, attack2Count);

			canSeeTarget = true;

			Asserts.Running(behavior,
				"While/Parallel/If",
				"While/Parallel/Sequence/Attack2");

			Asserts.Counts(1, attack1Count, attack2Count);

			Asserts.Running(behavior,
				"While/Parallel/If",
				"While/Parallel/Sequence/Attack2");

			Asserts.Counts(1, attack1Count);
			Asserts.Counts(2, attack2Count);

			canSeeTarget = false;

			Asserts.Fail(behavior);
			Asserts.Counts(1, attack1Count);
			Asserts.Counts(3, attack2Count);

			Asserts.Fail(behavior);
			Asserts.Counts(1, attack1Count);
			Asserts.Counts(3, attack2Count);

			canSeeTarget = true;

			Asserts.Running(behavior,
				"While/Parallel/If",
				"While/Parallel/Sequence/Attack2");

			Asserts.Counts(2, attack1Count);
			Asserts.Counts(4, attack2Count);

			Asserts.Running(behavior,
				"While/Parallel/If",
				"While/Parallel/Sequence/Attack2");

			Asserts.Counts(2, attack1Count);
			Asserts.Counts(5, attack2Count);
		}

		[Test]
		public void WhileExample2()
		{
			var patrol = new Act("Patrol", () => Result.Running);
			var attackTarget = new Act("Attack", () => Result.Running);

			var canSeeTarget = false;
			var behavior = new Behavior(new While(() => canSeeTarget, attackTarget, patrol));

			Asserts.Running(behavior,
				"While/Sequence/Parallel/!If",
				"While/Sequence/Parallel/Patrol");

			canSeeTarget = true;

			Asserts.Fail(behavior);

			Asserts.Running(behavior,
				"While/Sequence/Parallel/If",
				"While/Sequence/Parallel/Attack");

			canSeeTarget = false;

			Asserts.Running(behavior,
				"While/Sequence/Parallel/!If",
				"While/Sequence/Parallel/Patrol");
		}
	}
}
