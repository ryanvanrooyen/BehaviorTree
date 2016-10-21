
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

	public interface INode<T>
	{
		Result Run(T data);
	}

	public delegate T Func<T>();
	public delegate T Func<D, T>(D data);

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

	public class Node<T> : INode<T>
	{
		private readonly Func<T, Result> action;

		public Node(Func<T, Result> action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			this.action = action;
		}

		public Result Run(T data)
		{
			return this.action(data);
		}

		public static readonly INode<T> Success
			= new Node<T>(data => Result.Success);

		public static readonly INode<T> Fail
			= new Node<T>(data => Result.Failure);

		public static readonly INode<T> Running
			= new Node<T>(data => Result.Running);
	}
}
