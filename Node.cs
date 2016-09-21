
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

	public class Node2 : INode
	{
		private readonly Func<Result> action;

		public Node2(Func<Result> action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			this.action = action;
		}

		public Result Run()
		{
			return this.action();
		}
	}

	public abstract class Node : INode
	{
		public abstract Result Run();
	}

	public class Succeed : Node
	{
		public override Result Run()
		{
			return Result.Success;
		}
	}

	public class Fail : Node
	{
		public override Result Run()
		{
			return Result.Failure;
		}
	}

	public class Running : Node
	{
		public override Result Run()
		{
			return Result.Running;
		}
	}
}
