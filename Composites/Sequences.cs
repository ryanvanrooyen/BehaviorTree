
namespace BehaviorTree
{
	public class Sequence : Composite
	{
		private int lastRunningChild;

		public Sequence(params INode[] children) : base(children)
		{ }

		public override Result Run()
		{
			return Iterate(Result.Success, this.lastRunningChild,
				out this.lastRunningChild);
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

	public class RandomSequence : Composite
	{
		private int lastRunningChild;
		private readonly int[] indexes;

		public RandomSequence(params INode[] children) : base(children)
		{
			this.indexes = Indexes.Create(children.Length);
		}

		public override Result Run()
		{
			return RandomIterate(Result.Success, this.indexes,
				this.lastRunningChild, out this.lastRunningChild);
		}
	}
}
