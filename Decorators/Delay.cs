
using System;

namespace BehaviorTree
{
	public class Delay : Decorator
	{
		private readonly TimeSpan delay;
		private readonly ITime time;
		private DateTime? startTime;

		public Delay(TimeSpan delay, INode node)
			: this(delay, node, Time.Real)
		{ }

		internal Delay(TimeSpan delay, INode node, ITime time) : base(node)
		{
			this.delay = delay;
			this.time = time;
		}

		public override string Name
		{
			get
			{
				return "Delay(" + this.delay + ")-" + this.node.Name;
			}
		}

		protected override Result RunNode()
		{
			var currentTime = this.time.Now;

			if (this.startTime == null)
				this.startTime = currentTime + this.delay;

			if (this.startTime > currentTime)
				return Result.Running;

			this.startTime = null;
			return this.node.Run();
		}
	}
}
