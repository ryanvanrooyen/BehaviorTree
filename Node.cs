
using System;

namespace BehaviorTree
{
	public enum Result
	{
		Success,
		Running,
		Failure
	}

	public interface INode
	{
		Result Run();
	}

	public delegate T Func<T>();

	public class Node : INode
	{
		private readonly Func<Result> action;

		public Node(Func<Result> action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			this.action = action;
		}

		public Result Run()
		{
			return this.action();
		}

		public static readonly INode Success
			= new Node(() => Result.Success);

		public static readonly INode Fail
			= new Node(() => Result.Failure);

		public static readonly INode Running
			= new Node(() => Result.Running);
	}
}
