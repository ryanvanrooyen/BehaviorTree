
using System;

namespace BehaviorTree
{
	public class Wait : Decorator
	{
		private readonly TimeSpan waitTime;
		private readonly ITime time;
		private DateTime? startTime;
		private Result? result;

		public Wait(TimeSpan waitTime, INode node)
			: this(waitTime, node, Time.Real)
		{ }

		internal Wait(TimeSpan waitTime, INode node, ITime time) : base(node)
		{
			this.waitTime = waitTime;
			this.time = time;
		}

		public override string Name
		{
			get
			{
				return this.node.Name + "(ThenWait" + this.waitTime.TotalSeconds + "secs)";
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.result = null;
		}

		protected override Result RunNode()
		{
			var currentTime = this.time.Now;

			if (!this.result.HasValue)
			{
				var nodeResult = this.node.Run();
				if (nodeResult == Result.Running)
					return nodeResult;

				this.result = nodeResult;
				this.startTime = currentTime + this.waitTime;
			}

			if (this.startTime > currentTime)
				return Result.Running;

			var lastResult = this.result.Value;
			this.result = null;
			return lastResult;
		}
	}
}
