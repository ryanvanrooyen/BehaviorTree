
using NUnit.Framework;

namespace BehaviorTree
{
	[TestFixture]
	public class SequenceTests
	{
		[Test]
		public void SuccessSequences()
		{
			AssertSeq(Result.Success);
			AssertSeq(Result.Success, Node.Success);
			AssertSeq(Result.Success, Node.Success, Node.Success);
			AssertSeq(Result.Success, Node.Success, Node.Success, Node.Success);
		}

		[Test]
		public void FailureSequences()
		{
			AssertSeq(Result.Failure, Node.Fail);
			AssertSeq(Result.Failure, Node.Fail, Node.Success);
			AssertSeq(Result.Failure, Node.Fail, Node.Running);
			AssertSeq(Result.Failure, Node.Success, Node.Fail);
			AssertSeq(Result.Failure, Node.Success, Node.Fail, Node.Success);
			AssertSeq(Result.Failure, Node.Success, Node.Fail, Node.Running);
		}

		[Test]
		public void RunningSequences()
		{
			AssertSeq(Result.Running, Node.Running);
			AssertSeq(Result.Running, Node.Success, Node.Running);
			AssertSeq(Result.Running, Node.Success, Node.Running, Node.Success);
			AssertSeq(Result.Running, Node.Success, Node.Running, Node.Fail);
		}

		private void AssertSeq(Result expected, params INode[] nodes)
		{
			var seq = new Sequence(nodes);
			var actual = seq.Run();
			Assert.AreEqual(expected, actual,
			   "Sequence .Run returned unexpected result.");
		}
	}
}