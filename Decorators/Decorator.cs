
using System;

namespace BehaviorTree
{
	public abstract class Decorator : Node
	{
		protected readonly INode node;

		public Decorator(INode node) : base("")
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));
			if (node.Children != null || node.RunChildrenInParallel)
				throw new ArgumentException("Decorators currently do not work on Sequence & Selector node types.");

			this.node = node;
		}

		public override string Name
		{
			get
			{
				return "Decorator(" + this.node.Name + ")";
			}
		}
	}
}
