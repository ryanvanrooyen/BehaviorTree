
using System;

namespace BehaviorTree
{
	public class Always : Decorator
	{
		private readonly Result result;

		public Always(Result result, Func<Result> node)
			: this(result, new Node(node)) { }

		public Always(Result result, INode node) : base(node)
		{
			if (result == Result.Running)
				throw new ArgumentException(
					"Specified result must be success or failure");

			this.result = result;
		}

		public override Result Run()
		{
			var status = base.Run();
			if (status == Result.Running)
				return status;

			return this.result;
		}
	}

	public class Always<T> : Decorator<T>
	{
		private readonly Result result;

		public Always(Result result, Func<T, Result> node)
			: this(result, new Node<T>(node)) { }

		public Always(Result result, INode<T> node) : base(node)
		{
			if (result == Result.Running)
				throw new ArgumentException(
					"Specified result must be success or failure");

			this.result = result;
		}

		public override Result Run(T data)
		{
			var status = base.Run(data);
			if (status == Result.Running)
				return status;

			return this.result;
		}
	}
}
