
using NUnit.Framework;

namespace BehaviorTree
{
	[TestFixture]
	public class SelectorTests
	{
		[Test]
		public void SuccessSelectors()
		{
			AssertSel(Result.Success, Node.Success);
			AssertSel(Result.Success, Node.Success, Node.Fail);
			AssertSel(Result.Success, Node.Success, Node.Running);
			AssertSel(Result.Success, Node.Fail, Node.Success);
			AssertSel(Result.Success, Node.Fail, Node.Fail, Node.Success);
		}

		[Test]
		public void ParallelSuccess()
		{
			AssertParallel(Result.Success, Node.Success);
			AssertParallel(Result.Success, Node.Success, Node.Fail);
			AssertParallel(Result.Success, Node.Success, Node.Running);
			AssertParallel(Result.Success, Node.Fail, Node.Success);
			AssertParallel(Result.Success, Node.Fail, Node.Fail, Node.Success);
		}

		[Test]
		public void RunningSelectors()
		{
			AssertSel(Result.Running, Node.Running);
			AssertSel(Result.Running, Node.Fail, Node.Running);
			AssertSel(Result.Running, Node.Fail, Node.Fail, Node.Running);
			AssertSel(Result.Running, Node.Fail, Node.Running, Node.Fail);
			AssertSel(Result.Running, Node.Fail, Node.Running, Node.Success);
		}

		[Test]
		public void ParallelRunning()
		{
			AssertSel(Result.Running, Node.Running);
			AssertSel(Result.Running, Node.Fail, Node.Running);
			AssertSel(Result.Running, Node.Running, Node.Fail);
			AssertSel(Result.Running, Node.Fail, Node.Fail, Node.Running);
			AssertSel(Result.Running, Node.Fail, Node.Running, Node.Fail);
		}

		[Test]
		public void LongRunningSelector()
		{
			var node1CallCount = 0;
			var node1 = new Node(() =>
			{
				node1CallCount++;
				return Result.Failure;
			});

			var node2CallCount = 0;
			var node2 = new Node(() =>
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

			var selector = new Selector(node1, node2, node1, node2);
			AssertSelector(Result.Running, selector);
			AssertSelector(Result.Running, selector);
			AssertSelector(Result.Success, selector);

			Assert.AreEqual(node1CallCount, 2);
			Assert.AreEqual(node2CallCount, 4);
		}

		[Test]
		public void FailureSelectors()
		{
			AssertSel(Result.Failure);
			AssertSel(Result.Failure, Node.Fail);
			AssertSel(Result.Failure, Node.Fail, Node.Fail);
		}

		[Test]
		public void ParallelFailure()
		{
			AssertParallel(Result.Failure);
			AssertParallel(Result.Failure, Node.Fail);
			AssertParallel(Result.Failure, Node.Fail, Node.Fail);
		}

		private void AssertSel(Result expected, params INode[] nodes)
		{
			AssertSelector(expected, new Selector(nodes));
		}

		private void AssertParallel(Result expected, params INode[] nodes)
		{
			AssertSelector(expected, new ParallelSelector(nodes));
		}

		private void AssertSelector(Result expected, INode selector)
		{
			var actual = selector.Run();
			Assert.AreEqual(expected, actual,
			   "Selector .Run returned unexpected result.");
		}
	}
}