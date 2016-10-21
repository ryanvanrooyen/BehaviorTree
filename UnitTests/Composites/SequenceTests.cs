
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

		[Test]
		public void ParallelSuccess()
		{
			AssertParallel(Result.Success);
			AssertParallel(Result.Success, Node.Success);
			AssertParallel(Result.Success, Node.Success, Node.Success);
			AssertParallel(Result.Success, Node.Success, Node.Success, Node.Success);
		}

		[Test]
		public void ParallelFailure()
		{
			AssertParallel(Result.Failure, Node.Fail);
			AssertParallel(Result.Failure, Node.Fail, Node.Success);
			AssertParallel(Result.Failure, Node.Success, Node.Fail);
			AssertParallel(Result.Failure, Node.Fail, Node.Running);
			AssertParallel(Result.Failure, Node.Running, Node.Fail);
			AssertParallel(Result.Failure, Node.Success, Node.Fail);
			AssertParallel(Result.Failure, Node.Success, Node.Fail, Node.Success);
			AssertParallel(Result.Failure, Node.Success, Node.Fail, Node.Running);
		}

		[Test]
		public void ParallelRunning()
		{
			AssertSeq(Result.Running, Node.Running);
			AssertSeq(Result.Running, Node.Success, Node.Running);
			AssertSeq(Result.Running, Node.Success, Node.Running, Node.Success);
			AssertSeq(Result.Running, Node.Success, Node.Running, Node.Running);
		}

		private void AssertSeq(Result expected, params INode[] nodes)
		{
			var seq = new Sequence(nodes);
			var actual = seq.Run();
			Assert.AreEqual(expected, actual,
			   "Sequence .Run returned unexpected result.");
		}

		private void AssertParallel(Result expected, params INode[] nodes)
		{
			var seq = new ParallelSequence(nodes);
			var actual = seq.Run();
			Assert.AreEqual(expected, actual,
			   "ParralelSequence .Run returned unexpected result.");
		}
	}
}