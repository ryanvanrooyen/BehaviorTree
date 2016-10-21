
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
			AssertSeq(Result.Failure, Node.Success, Node.Fail);
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
			AssertParallel(Result.Running, Node.Running);
			AssertParallel(Result.Running, Node.Success, Node.Running);
			AssertParallel(Result.Running, Node.Success, Node.Running, Node.Success);
			AssertParallel(Result.Running, Node.Success, Node.Running, Node.Running);
		}

		[Test]
		public void LongRunningSequence()
		{
			var node1CallCount = 0;
			var node1 = new Node(() =>
			{
				node1CallCount++;
				return Result.Success;
			});

			var node2CallCount = 0;
			var node2 = new Node(() =>
			{
				node2CallCount++;
				return node2CallCount > 4 ? Result.Success : Result.Running;
			});

			var seq = new Sequence(node1, node2, node1, node2);
			AssertSequence(Result.Running, seq);
			AssertSequence(Result.Running, seq);
			AssertSequence(Result.Running, seq);
			AssertSequence(Result.Running, seq);
			AssertSequence(Result.Success, seq);

			Assert.AreEqual(node1CallCount, 2);
			Assert.AreEqual(node2CallCount, 6);
		}

		private void AssertSeq(Result expected, params INode[] nodes)
		{
			AssertSequence(expected, new Sequence(nodes));
		}

		private void AssertParallel(Result expected, params INode[] nodes)
		{
			AssertSequence(expected, new ParallelSequence(nodes));
		}

		private void AssertSequence(Result expected, INode sequence)
		{
			var actual = sequence.Run();
			Assert.AreEqual(expected, actual,
			   "Sequence .Run returned unexpected result.");
		}
	}
}