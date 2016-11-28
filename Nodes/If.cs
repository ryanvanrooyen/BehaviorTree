
using System;

namespace BehaviorTree
{
	public class If : Node
	{
		private readonly Delegates.Func<bool> condition;

		public If(Delegates.Func<bool> condition) : this("If", condition) { }

		public If(string name, Delegates.Func<bool> condition) : base(name)
		{
			if (condition == null)
				throw new ArgumentNullException("condition");

			this.condition = condition;
		}

		protected override Result RunNode()
		{
			return this.condition() ? Result.Success : Result.Failure;
		}
	}
}
