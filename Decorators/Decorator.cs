
using System;

namespace BehaviorTree
{
	public abstract class Decorator : INode
	{
		private readonly INode node;

		public Decorator(INode node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			this.node = node;
		}

		public virtual Result Run()
		{
			return this.node.Run();
		}
	}
}
