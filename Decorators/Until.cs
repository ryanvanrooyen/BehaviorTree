
using System;

namespace BehaviorTree
{
	public class Until : Decorator
	{
		private readonly Result result;

		public Until(Result result, INode node) : base(node)
		{
			if (result == Result.Running)
				throw new ArgumentException("Specified result must be success or failure");

			this.result = result;
		}

		public override string Name
		{
			get
			{
				return "Until" + this.result + "-" + this.node.Name;
			}
		}

		protected override Result RunNode()
		{
			var status = this.node.Run();
			if (status == this.result)
				return Result.Success;

			return Result.Running;
		}
	}
}
