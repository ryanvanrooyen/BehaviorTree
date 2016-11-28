
using System;

namespace BehaviorTree
{
	public abstract class Composite : Node, INodeObserver
	{
		private Result result;
		protected readonly Result endResult;
		protected readonly INode[] children;
		protected int currentChild;

		public Composite(string name, Result endResult, params INode[] children)
			: base(name)
		{
			if (children == null)
				throw new ArgumentNullException("children");

			this.endResult = endResult;
			this.children = children;
			foreach (var child in this.children)
				child.AddObserver(this);
		}

		public override INode[] Children { get { return this.children; } }

		public void OnStarted(INode node) { }

		public override void Reset()
		{
			base.Reset();
			this.currentChild = 0;
		}

		public virtual void OnCompleted(INode node, Result result)
		{
			if (GetCurrentChild() == node)
			{
				this.result = result;
				this.currentChild++;
			}
		}

		protected override Result RunNode()
		{
			if (this.currentChild == 0)
				this.result = this.endResult;

			while (this.currentChild < this.children.Length && this.result == this.endResult)
			{
				var child = GetCurrentChild();
				this.result = child.Run();
			}

			if (this.result != Result.Running)
				this.currentChild = 0;

			return this.result;
		}

		protected virtual INode GetCurrentChild()
		{
			if (this.currentChild < this.children.Length)
				return this.children[this.currentChild];

			return null;
		}

		protected int GetChildIndex(INode node)
		{
			var index = -1;
			if (node != null)
			{
				for (var i = 0; i < this.children.Length; i++)
				{
					if (this.children[i] == node)
						return i;
				}
			}

			return index;
		}
	}
}
