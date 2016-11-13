
using System;
using NUnit.Framework;

namespace BehaviorTree
{
	[TestFixture]
	public class Decorators
	{
		[Test]
		public void AlwaysConstructor()
		{
			Assert.Throws<ArgumentException>(() =>
				new Always(Result.Running, Node.Success));
		}

		[Test]
		public void Always()
		{
			Run(new Always(Result.Success, Node.Success), Result.Success);
			Run(new Always(Result.Success, Node.Fail), Result.Success);
			Run(new Always(Result.Success, Node.Running), Result.Running);

			Run(new Always(Result.Failure, Node.Success), Result.Failure);
			Run(new Always(Result.Failure, Node.Fail), Result.Failure);
			Run(new Always(Result.Failure, Node.Running), Result.Running);
		}

		[Test]
		public void UntilConstructor()
		{
			Assert.Throws<ArgumentException>(() =>
				new Until(Result.Running, Node.Success));
		}

		[Test]
		public void Until()
		{
			Run(new Until(Result.Success, Node.Success), Result.Success);
			Run(new Until(Result.Success, Node.Fail), Result.Running);
			Run(new Until(Result.Success, Node.Running), Result.Running);

			Run(new Until(Result.Failure, Node.Fail), Result.Success);
			Run(new Until(Result.Failure, Node.Success), Result.Running);
			Run(new Until(Result.Failure, Node.Running), Result.Running);
		}

		[Test]
		public void Invert()
		{
			Run(new Invert(Node.Success), Result.Failure);
			Run(new Invert(Node.Fail), Result.Success);
			Run(new Invert(Node.Running), Result.Running);
		}

		[Test]
		public void Repeat()
		{
			Run(new Repeat(0, Node.Fail), Result.Failure);
			Run(new Repeat(1, Node.Fail), Result.Failure);
			Run(new Repeat(2, Node.Fail), Result.Failure);

			Run(new Repeat(0, Node.Success), Result.Success);
			Run(new Repeat(1, Node.Success), Result.Running);
			var repeat = new Repeat(2, Node.Success);
			Run(repeat, Result.Running);
			Run(repeat, Result.Running);
			Run(repeat, Result.Success);

			repeat = new Repeat(2, MockNode(Result.Running, Result.Failure));
			Run(repeat, Result.Running);
			Run(repeat, Result.Failure);

			repeat = new Repeat(2, MockNode(Result.Running, Result.Success, Result.Success));
			Run(repeat, Result.Running);
			Run(repeat, Result.Running);
			Run(repeat, Result.Running);
			Run(repeat, Result.Success);
		}

		[Test]
		public void Retry()
		{
			Run(new Retry(0, Node.Success), Result.Success);
			Run(new Retry(1, Node.Success), Result.Success);
			Run(new Retry(2, Node.Success), Result.Success);

			Run(new Retry(0, Node.Fail), Result.Failure);
			Run(new Retry(1, Node.Fail), Result.Running);
			Run(new Retry(2, Node.Fail), Result.Running);

			var retry = new Retry(2, Node.Fail);
			Run(retry, Result.Running);
			Run(retry, Result.Running);
			Run(retry, Result.Failure);

			retry = new Retry(2, MockNode(Result.Failure, Result.Success));
			Run(retry, Result.Running);
			Run(retry, Result.Success);

			retry = new Retry(1, MockNode(Result.Running, Result.Failure, Result.Success));
			Run(retry, Result.Running);
			Run(retry, Result.Running);
			Run(retry, Result.Success);
		}

		[Test]
		public void Delay()
		{
			Run(new Delay(TimeSpan.Zero, Node.Success), Result.Success);
			Run(new Delay(TimeSpan.Zero, Node.Fail), Result.Failure);
			Run(new Delay(TimeSpan.Zero, Node.Running), Result.Running);

			DelayTest(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.99), Result.Running);
			DelayTest(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Result.Success);
			DelayTest(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.01), Result.Success);

			DelayTest2(TimeSpan.FromMilliseconds(1));
			DelayTest2(TimeSpan.FromSeconds(1));
			DelayTest2(TimeSpan.FromMinutes(1));
			DelayTest2(TimeSpan.FromHours(1));
			DelayTest2(TimeSpan.FromDays(1));
		}

		[Test]
		public void Limit()
		{
			var status = Result.Success;
			INode node = new Act(() => status);

			var time = new MockTime(DateTime.Now);
			var maxRunTime = TimeSpan.FromSeconds(10);

			node = new Limit(maxRunTime, node, time);
			Run(node, Result.Success);

			status = Result.Failure;
			Run(node, Result.Failure);

			status = Result.Running;
			Run(node, Result.Running);

			time.Now = time.Now + maxRunTime;
			Run(node, Result.Running);

			time.Now = time.Now + TimeSpan.FromTicks(1);
			Run(node, Result.Failure);
		}

		private void DelayTest(TimeSpan delay, TimeSpan waitTime, Result result)
		{
			var currentTime = DateTime.Now;
			var time = new MockTime(currentTime);

			var node = new Delay(delay, Node.Success, time);
			// Trigger the node to run once, should be in a Running state.
			Run(node, Result.Running);

			// Set the mock time to the future time.
			time.Now = time.Now + waitTime;
			Run(node, result);
		}

		private void DelayTest2(TimeSpan delay)
		{
			var currentTime = DateTime.Now;
			var time = new MockTime(currentTime);
			var halfDelay = TimeSpan.FromTicks(delay.Ticks / 2);

			var node = new Delay(delay, Node.Success, time);
			// Trigger the node to run once, should be in a Running state.
			Run(node, Result.Running);

			time.Now = time.Now + halfDelay;
			Run(node, Result.Running);

			// Set the mock time to the future time.
			time.Now = time.Now + delay;
			Run(node, Result.Success);

			// Run again, should be in a Running state.
			Run(node, Result.Running);

			time.Now = time.Now + halfDelay;
			Run(node, Result.Running);

			// Set the mock time to the future time.
			time.Now = time.Now + delay;
			Run(node, Result.Success);
		}

		private INode MockNode(params Result[] results)
		{
			var currentResult = -1;

			return new Act(() =>
			{
				currentResult++;
				if (currentResult >= results.Length)
					currentResult = results.Length - 1;

				return results[currentResult];
			});
		}

		private void Run(INode node, Result expected)
		{
			var actual = node.Run();
			Assert.AreEqual(expected, actual,
			   "Node .Run returned unexpected result.");
		}
	}

	internal class MockTime : ITime
	{
		public MockTime() : this(DateTime.Now) { }

		public MockTime(DateTime now)
		{
			this.Now = now;
		}

		public DateTime Now { get; set; }
	}
}
