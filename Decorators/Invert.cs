﻿
namespace BehaviorTree
{
	public class Invert : Decorator
	{
		public Invert(INode node) : base(node)
		{ }

		protected override Result RunNode()
		{
			var result = this.node.Run();
			if (result == Result.Success)
				return Result.Failure;
			if (result == Result.Failure)
				return Result.Success;

			return result;
		}
	}
}
