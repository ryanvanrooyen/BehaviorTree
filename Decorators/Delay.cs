
using System;

namespace BehaviorTree
{
	public class Delay : Decorator
	{
		private readonly TimeSpan delay;
		private readonly ITime time;
		private DateTime? startTime;

		public Delay(TimeSpan delay, Func<Result> node)
			: this(delay, new Node(node)) { }

		public Delay(TimeSpan delay, INode node)
			: this(delay, node, Time.Real)
		{ }

		internal Delay(TimeSpan delay, INode node, ITime time) : base(node)
		{
			this.delay = delay;
			this.time = time;
		}

		public override Result Run()
		{
			var currentTime = this.time.Now;

			if (this.startTime == null)
				this.startTime = currentTime + this.delay;

			if (this.startTime > currentTime)
				return Result.Running;

			this.startTime = null;
			return base.Run();
		}
	}

	public class Delay<T> : Decorator<T>
	{
		private readonly TimeSpan delay;
		private readonly ITime time;
		private DateTime? startTime;

		public Delay(TimeSpan delay, Func<T, Result> node)
			: this(delay, new Node<T>(node)) { }

		public Delay(TimeSpan delay, INode<T> node)
			: this(delay, node, Time.Real)
		{ }

		internal Delay(TimeSpan delay, INode<T> node, ITime time) : base(node)
		{
			this.delay = delay;
			this.time = time;
		}

		public override Result Run(T data)
		{
			var currentTime = this.time.Now;

			if (this.startTime == null)
				this.startTime = currentTime + this.delay;

			if (this.startTime > currentTime)
				return Result.Running;

			this.startTime = null;
			return base.Run(data);
		}
	}
}
