
using System;

namespace BehaviorTree
{
	public abstract class Decorator<T> : INode<T>
	{
		private readonly INode<T> node;

		public Decorator(INode<T> node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			this.node = node;
		}

		public virtual Result Run(T data)
		{
			return this.node.Run(data);
		}
	}

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
