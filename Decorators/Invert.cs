
namespace BehaviorTree
{
	public class Invert : Decorator
	{
		public Invert(Func<Result> node)
			: this(new Node(node)) { }

		public Invert(INode node) : base(node)
		{ }

		public override Result Run()
		{
			var result = base.Run();
			if (result == Result.Success)
				return Result.Failure;
			if (result == Result.Failure)
				return Result.Success;

			return result;
		}
	}

	public class Invert<T> : Decorator<T>
	{
		public Invert(Func<T, Result> node)
			: this(new Node<T>(node)) { }

		public Invert(INode<T> node) : base(node)
		{ }

		public override Result Run(T data)
		{
			var result = base.Run(data);
			if (result == Result.Success)
				return Result.Failure;
			if (result == Result.Failure)
				return Result.Success;

			return result;
		}
	}
}
