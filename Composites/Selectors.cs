
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

	public class Selector<T> : Composite<T>
	{
		public Selector(params INode<T>[] children) : base(children)
		{ }

		public override Result Run(T data)
		{
			return Iterate(data, Result.Failure);
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

	public class ParallelSelector<T> : Composite<T>
	{
		public ParallelSelector(params INode<T>[] children) : base(children)
		{ }

		public override Result Run(T data)
		{
			return ParallelIterate(data, Result.Failure, Result.Success);
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

	public class RandomSelector<T> : Composite<T>
	{
		private readonly int[] indexes;

		public RandomSelector(params INode<T>[] children) : base(children)
		{
			this.indexes = Indexes.Create(children.Length);
		}

		public override Result Run(T data)
		{
			return RandomIterate(data, Result.Failure, this.indexes);
		}
	}
}
