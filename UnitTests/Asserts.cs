
using NUnit.Framework;

namespace BehaviorTree
{
	public static class Asserts
	{
		public static void Success(IBehavior behavior)
		{
			Run(behavior, Result.Success);
			Assert.AreEqual(false, behavior.HasRunningNodes);
			Assert.AreEqual(0, behavior.RunningNodePaths.Length);
		}

		public static void Success(INode rootNode)
		{
			Success(new Behavior(rootNode));
		}

		public static void Fail(IBehavior behavior)
		{
			Run(behavior, Result.Failure);
			Assert.AreEqual(false, behavior.HasRunningNodes);
			Assert.AreEqual(0, behavior.RunningNodePaths.Length);
		}

		public static void Fail(INode rootNode)
		{
			Fail(new Behavior(rootNode));
		}

		public static void Running(IBehavior behavior, params string[] runningNodes)
		{
			Run(behavior, Result.Running);
			Assert.AreEqual(true, behavior.HasRunningNodes);
			Assert.AreEqual(runningNodes.Length, behavior.RunningNodePaths.Length);

			for (var i = 0; i < runningNodes.Length; i++)
				Assert.AreEqual(runningNodes[i], behavior.RunningNodePaths[i]);
		}

		public static void Running(INode rootNode, params string[] runningNodes)
		{
			Running(new Behavior(rootNode), runningNodes);
		}

		public static void Counts(int expected, params int[] counts)
		{
			for (var i = 0; i < counts.Length; i++)
				Assert.AreEqual(expected, counts[i]);
		}

		private static void Run(IBehavior behavior, Result expected)
		{
			var actual = behavior.Run();
			Assert.AreEqual(expected, actual,
			   "Behavior .Run returned unexpected result.");
		}
	}
}
