
using System;

namespace BehaviorTree
{
	public abstract class Composite : INode
	{
		private readonly INode[] children;

		public Composite(INode[] children)
		{
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			this.children = children;
		}

		public abstract Result Run();

		protected Result Iterate(Result endResult)
		{
			for (var i = 0; i < this.children.Length; i++)
			{
				var status = this.children[i].Run();
				if (status != endResult)
					return status;
			}

			return endResult;
		}

		protected Result ParallelIterate(Result startResult, Result endResult)
		{
			var isRunning = false;
			var hitEndResult = false;

			for (var i = 0; i < this.children.Length; i++)
			{
				var status = this.children[i].Run();
				isRunning = isRunning || status == Result.Running;
				hitEndResult = hitEndResult || status == endResult;
			}

			if (hitEndResult)
				return endResult;
			else if (isRunning)
				return Result.Running;

			return startResult;
		}

		protected Result RandomIterate(Result endResult, int[] indexes)
		{
			Indexes.Shuffle(indexes);

			for (var i = 0; i < indexes.Length; i++)
			{
				var index = indexes[i];
				var status = this.children[index].Run();
				if (status != endResult)
					return status;
			}

			return endResult;
		}
	}

	public abstract class Composite<T> : INode<T>
	{
		private readonly INode<T>[] children;

		public Composite(INode<T>[] children)
		{
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			this.children = children;
		}

		public abstract Result Run(T data);

		protected Result Iterate(T data, Result endResult)
		{
			for (var i = 0; i < this.children.Length; i++)
			{
				var status = this.children[i].Run(data);
				if (status != endResult)
					return status;
			}

			return endResult;
		}

		protected Result ParallelIterate(T data, Result startResult, Result endResult)
		{
			var isRunning = false;
			var hitEndResult = false;

			for (var i = 0; i < this.children.Length; i++)
			{
				var status = this.children[i].Run(data);
				isRunning = isRunning || status == Result.Running;
				hitEndResult = hitEndResult || status == endResult;
			}

			if (hitEndResult)
				return endResult;
			else if (isRunning)
				return Result.Running;

			return startResult;
		}

		protected Result RandomIterate(T data, Result endResult, int[] indexes)
		{
			Indexes.Shuffle(indexes);

			for (var i = 0; i < indexes.Length; i++)
			{
				var index = indexes[i];
				var status = this.children[index].Run(data);
				if (status != endResult)
					return status;
			}

			return endResult;
		}
	}
}
