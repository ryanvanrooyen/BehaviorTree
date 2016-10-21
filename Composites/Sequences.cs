
namespace BehaviorTree
{
	public class Sequence : Composite
	{
		public Sequence(params INode[] children) : base(children)
		{ }

		public override Result Run()
		{
			return Iterate(Result.Success);
		}
	}

	public class ParallelSequence : Composite
	{
		public ParallelSequence(params INode[] children) : base(children)
		{ }

		public override Result Run()
		{
			return ParallelIterate(Result.Success, Result.Failure);
		}
	}

	public class MemorySequence : Composite
	{
		private int lastRunningChildIndex = 0;

		public MemorySequence(params INode[] children) : base(children) { }

		public override Result Run()
		{
			return MemoryIterate(Result.Success,
				this.lastRunningChildIndex, out this.lastRunningChildIndex);
		}
	}

	public class RandomSequence : Composite
	{
		private readonly int[] indexes;

		public RandomSequence(params INode[] children) : base(children)
		{
			this.indexes = Indexes.Create(children.Length);
		}

		public override Result Run()
		{
			return RandomIterate(Result.Success, this.indexes);
		}
	}
}
