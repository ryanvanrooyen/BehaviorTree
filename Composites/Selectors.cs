
namespace BehaviorTree
{
	public class Selector : Composite
	{
		private int lastRunningChild;

		public Selector(params INode[] children) : base(children)
		{ }

		public override Result Run()
		{
			return Iterate(Result.Failure, this.lastRunningChild,
				out this.lastRunningChild);
		}
	}

	public class ParallelSelector : Composite
	{
		public ParallelSelector(params INode[] children) : base(children)
		{ }

		public override Result Run()
		{
			return ParallelIterate(Result.Failure, Result.Success);
		}
	}

	public class RandomSelector : Composite
	{
		private int lastRunningChild;
		private readonly int[] indexes;

		public RandomSelector(params INode[] children) : base(children)
		{
			this.indexes = Indexes.Create(children.Length);
		}

		public override Result Run()
		{
			return RandomIterate(Result.Failure, this.indexes,
				this.lastRunningChild, out this.lastRunningChild);
		}
	}
}
