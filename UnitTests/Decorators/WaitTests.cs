
using System;
using NUnit.Framework;

namespace BehaviorTree
{
	[TestFixture]
	public class WaitTests
	{
		[Test]
		public void ResetsCorrectly()
		{
			var attackCount = 0;
			var resultValue = Result.Success;
			var time = new MockTime(DateTime.Now);
			var attackTarget = new Wait(TimeSpan.FromHours(1),
				new Act("Attack", () => { attackCount++; return resultValue; }), time);

			var canSeeTarget = false;
			var behavior = new Behavior(new While(() => canSeeTarget, attackTarget));

			Asserts.Fail(behavior);
			Asserts.Counts(0, attackCount);
			Asserts.Fail(behavior);
			Asserts.Counts(0, attackCount);

			canSeeTarget = true;

			Asserts.Running(behavior,
				"While/Parallel/If",
				"While/Parallel/Attack(ThenWait3600secs)");

			Asserts.Counts(1, attackCount);

			Asserts.Running(behavior,
				"While/Parallel/If",
				"While/Parallel/Attack(ThenWait3600secs)");

			Asserts.Counts(1, attackCount);

			canSeeTarget = false;

			Asserts.Fail(behavior);
			Asserts.Counts(1, attackCount);

			canSeeTarget = true;
			resultValue = Result.Failure;

			// Move the clock up so the while is finished.
			time.Now = time.Now + TimeSpan.FromHours(2);

			Asserts.Running(behavior,
				"While/Parallel/If",
				"While/Parallel/Attack(ThenWait3600secs)");
		}
	}
}
