
using System;

namespace BehaviorTree
{
	public class Repeat : Decorator
	{
		private readonly TimeSpan? duration;
		private DateTime? endTime;
		private readonly uint times;
		private uint iteration;

		public Repeat(uint times, INode node) : base(node)
		{
			this.times = times;
			this.iteration = 0;
		}

		public Repeat(TimeSpan duration, INode node) : base(node)
		{
			this.duration = duration;
		}

		protected override Result RunNode()
		{
			var result = this.node.Run();

			if (result != Result.Success)
				return result;

			if (this.duration.HasValue)
			{
				var currentTime = DateTime.Now;

				if (!this.endTime.HasValue)
					this.endTime = currentTime + this.duration.Value;

				if (currentTime > this.endTime.Value)
				{
					this.endTime = null;
					return Result.Success;
				}
			}
			else
			{
				this.iteration += 1;
				if (this.iteration > this.times)
				{
					this.iteration = 0;
					return Result.Success;
				}
			}

			return Result.Running;
		}
	}
}
