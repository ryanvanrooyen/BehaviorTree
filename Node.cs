
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
	public delegate void Func();

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
			try
			{
				return this.action();
			}
			catch (Exception) { }

			return Result.Failure;
		}

		public static readonly INode Success
			= new Node(() => Result.Success);

		public static readonly INode Fail
			= new Node(() => Result.Failure);

		public static readonly INode Running
			= new Node(() => Result.Running);
	}

	public class If : INode
	{
		private readonly Func<bool> condition;

		public If(Func<bool> condition)
		{
			if (condition == null)
				throw new ArgumentNullException(nameof(condition));

			this.condition = condition;
		}

		public Result Run()
		{
			var success = this.condition();
			if (success)
				return Result.Success;

			return Result.Failure;
		}
	}

	public class Act : INode
	{
		private readonly Func action;

		public Act(Func action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			this.action = action;
		}

		public Result Run()
		{
			try
			{
				this.action();
				return Result.Success;
			}
			catch (Exception) { }

			return Result.Failure;
		}
	}
}
