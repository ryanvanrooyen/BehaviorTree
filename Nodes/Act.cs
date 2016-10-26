
using System;

namespace BehaviorTree
{
	public class Act : Node
	{
		private readonly Delegates.Func<Result> action;

		public Act(Delegates.Func action) : this("Act", action) { }

		public Act(string name, Delegates.Func action)
			: this(name, () => { action(); return Result.Success; })
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));
		}

		public Act(Delegates.Func<Result> action) : this("Act", action) { }

		public Act(string name, Delegates.Func<Result> action) : base(name)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			this.action = action;
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
	}
}
