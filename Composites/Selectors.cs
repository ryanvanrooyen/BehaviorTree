
namespace BehaviorTree
{
	public class Selector : Composite
	{
		public Selector(params INode[] children) : base(children)
		{ }

		public override Result Run()
		{
			return Iterate(Result.Failure);
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

	public class MemorySelector : Composite
	{
		private int lastRunningChildIndex = 0;

		public MemorySelector(params INode[] children) : base(children) { }

		public override Result Run()
		{
			return MemoryIterate(Result.Failure,
				this.lastRunningChildIndex, out this.lastRunningChildIndex);
		}
	}

	public class RandomSelector : Composite
	{
		private readonly int[] indexes;

		public RandomSelector(params INode[] children) : base(children)
		{
			this.indexes = Indexes.Create(children.Length);
		}

		public override Result Run()
		{
			return RandomIterate(Result.Failure, this.indexes);
		}
	}
}
