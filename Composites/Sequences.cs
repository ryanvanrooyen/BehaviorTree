
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

	public class Sequence<T> : Composite<T>
	{
		public Sequence(params INode<T>[] children) : base(children)
		{ }

		public override Result Run(T data)
		{
			return Iterate(data, Result.Success);
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

	public class ParallelSequence<T> : Composite<T>
	{
		public ParallelSequence(params INode<T>[] children) : base(children)
		{ }

		public override Result Run(T data)
		{
			return ParallelIterate(data, Result.Success, Result.Failure);
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

	public class RandomSequence<T> : Composite<T>
	{
		private readonly int[] indexes;

		public RandomSequence(params INode<T>[] children) : base(children)
		{
			this.indexes = Indexes.Create(children.Length);
		}

		public override Result Run(T data)
		{
			return RandomIterate(data, Result.Success, this.indexes);
		}
	}
}
