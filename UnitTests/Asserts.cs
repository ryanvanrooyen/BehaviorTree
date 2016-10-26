
using NUnit.Framework;

namespace BehaviorTree
{
	public static class Asserts
	{
		public static void Equal(Result expected, IBehavior behavior)
		{
			var actual = behavior.Run();
			Assert.AreEqual(expected, actual,
			   "Behavior .Run returned unexpected result.");
		}

		public static void Sequence(Result expected, params INode[] nodes)
		{
			Equal(expected, new Behavior(new Sequence(nodes)));
		}

		public static void ParallelSeq(Result expected, params INode[] nodes)
		{
			Equal(expected, new Behavior(new ParallelSequence(nodes)));
		}

		public static void Selector(Result expected, params INode[] nodes)
		{
			Equal(expected, new Behavior(new Selector(nodes)));
		}

		public static void ParallelSel(Result expected, params INode[] nodes)
		{
			Equal(expected, new Behavior(new ParallelSelector(nodes)));
		}
	}
}
