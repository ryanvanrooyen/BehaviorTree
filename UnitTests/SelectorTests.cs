
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
		public void FailureSelectors()
		{
			AssertSel(Result.Failure);
			AssertSel(Result.Failure, Node.Fail);
			AssertSel(Result.Failure, Node.Fail, Node.Fail);
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

		private void AssertSel(Result expected, params INode[] nodes)
		{
			var sel = new Selector(nodes);
			var actual = sel.Run();
			Assert.AreEqual(expected, actual,
			   "Selector .Run returned unexpected result.");
		}
	}
}