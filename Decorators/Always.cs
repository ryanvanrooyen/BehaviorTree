
using System;

namespace BehaviorTree
{
	public class Always : Decorator
	{
		private readonly Result result;

		public Always(Result result, INode node) : base(node)
		{
			if (result == Result.Running)
				throw new ArgumentException(
					"Specified result must be success or failure");

			this.result = result;
		}

		public override string Name
		{
			get
			{
				return "Always" + this.result + "-" + this.node.Name;
			}
		}

		protected override Result RunNode()
		{
			var status = this.node.Run();
			if (status == Result.Running)
				return status;

			return this.result;
		}
	}
}
