
using System;

namespace BehaviorTree
{
	public class Limit : Decorator
	{
		private readonly TimeSpan maxRunTime;
		private readonly ITime time;
		private DateTime? endTime;

		public Limit(TimeSpan maxRunTime, INode node)
			: this(maxRunTime, node, Time.Real)
		{ }

		internal Limit(TimeSpan maxRunTime, INode node, ITime time) : base(node)
		{
			this.maxRunTime = maxRunTime;
			this.time = time;
		}

		protected override Result RunNode()
		{
			var result = this.node.Run();

			if (result != Result.Running)
			{
				this.endTime = null;
				return result;
			}

			var currentTime = this.time.Now;

			if (this.endTime == null)
			{
				this.endTime = currentTime + this.maxRunTime;
				return Result.Running;
			}

			// If the max time to wait has passed, return failed.
			if (currentTime > this.endTime)
			{
				this.endTime = null;
				return Result.Failure;
			}

			return Result.Running;
		}
	}
}
