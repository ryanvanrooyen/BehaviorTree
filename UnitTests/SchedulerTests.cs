
using System;
using NUnit.Framework;

namespace BehaviorTree
{
	[TestFixture]
	public class SchedulerTests
	{
		[Test]
		public void MultiDecoratorTest()
		{
			var callCount = 0;
			var time = new MockTime();

			var node = new Repeat(TimeSpan.FromSeconds(8), new Wait(TimeSpan.FromSeconds(5),
				new Act(() => { callCount++; return Result.Success; }), time), time);

			var behavior = new Behavior(node);

			Asserts.Running(behavior, "Act(ThenWait5secs)(Repeat8secs)");
			Asserts.Counts(1, callCount);

			Asserts.Running(behavior, "Act(ThenWait5secs)(Repeat8secs)");
			Asserts.Counts(1, callCount);

			time.Now += TimeSpan.FromSeconds(6);

			// The next call, the Wait will return the original value from the Act node.
			Asserts.Running(behavior, "Act(ThenWait5secs)(Repeat8secs)");
			Asserts.Counts(1, callCount);

			// The time after that, the Wait will actually call the Act node again.
			Asserts.Running(behavior, "Act(ThenWait5secs)(Repeat8secs)");
			Asserts.Counts(2, callCount);

			time.Now += TimeSpan.FromSeconds(9);

			Asserts.Success(behavior);
		}
	}
}
