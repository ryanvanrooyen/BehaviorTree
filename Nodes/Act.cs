
using System;

namespace BehaviorTree
{
	public class Act : Node
	{
		private readonly Delegates.Func<Result> action;
		private readonly Delegates.Func cancel;

		public Act(Delegates.Func action, Delegates.Func cancel = null) : this("Act", action, cancel) { }

		public Act(string name, Delegates.Func action, Delegates.Func cancel = null)
			: this(name, () => { action(); return Result.Success; }, cancel)
		{
			if (action == null)
				throw new ArgumentNullException("action");
		}

		public Act(Delegates.Func<Result> action, Delegates.Func cancel = null)
			: this("Act", action, cancel) { }

		public Act(string name, Delegates.Func<Result> action,
			Delegates.Func cancel = null) : base(name)
		{
			if (action == null)
				throw new ArgumentNullException("action");

			this.action = action;
			this.cancel = cancel;
		}

		protected override Result RunNode()
		{
			try
			{
				return this.action();
			}
			catch (Exception)
			{
				return Result.Failure;
			}
		}

		public override void Reset()
		{
			base.Reset();
			if (this.cancel != null)
				this.cancel();
		}
	}
}
