
using System;

namespace BehaviorTree
{
	public class Until : Decorator
	{
		private readonly Result result;

		public Until(Result result, Func<Result> node)
			: this(result, new Node(node)) { }

		public Until(Result result, INode node) : base(node)
		{
			if (result == Result.Running)
				throw new ArgumentException("Specified result must be success or failure");

			this.result = result;
		}

		public override Result Run()
		{
			var status = base.Run();
			if (status == this.result)
				return Result.Success;

			return Result.Running;
		}
	}

	public class Until<T> : Decorator<T>
	{
		private readonly Result result;

		public Until(Result result, Func<T, Result> node)
			: this(result, new Node<T>(node)) { }

		public Until(Result result, INode<T> node) : base(node)
		{
			if (result == Result.Running)
				throw new ArgumentException("Specified result must be success or failure");

			this.result = result;
		}

		public override Result Run(T data)
		{
			var status = base.Run(data);
			if (status == this.result)
				return Result.Success;

			return Result.Running;
		}
	}
}
