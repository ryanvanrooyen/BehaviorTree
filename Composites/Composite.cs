
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

		protected Result Iterate(Result endResult, int startIndex, out int stoppedIndex)
		{
			stoppedIndex = 0;

			for (var i = startIndex; i < this.children.Length; i++)
			{
				var status = this.children[i].Run();
				if (status != endResult)
				{
					if (status == Result.Running)
						stoppedIndex = i;

					return status;
				}
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

		protected Result RandomIterate(Result endResult, int[] indexes,
			int startIndex, out int stoppedIndex)
		{
			stoppedIndex = 0;

			if (startIndex == 0)
				Indexes.Shuffle(indexes);

			for (var i = startIndex; i < indexes.Length; i++)
			{
				var index = indexes[i];
				var status = this.children[index].Run();
				if (status != endResult)
				{
					if (status == Result.Running)
						stoppedIndex = i;

					return status;
				}
			}

			return endResult;
		}
	}
}
