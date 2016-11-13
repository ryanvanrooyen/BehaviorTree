
using System.Collections.Generic;

namespace BehaviorTree
{
	public interface IScheduler
	{
		void Add(INode node);
		bool HasRunningNodes { get; }
		string[] RunningNodePaths { get; }
		Result Run();
	}

	public class StackScheduler : IScheduler, INodeObserver
	{
		private readonly StackScheduler parent;
		private readonly Stack<INode> runningNodes;
		private readonly IList<StackScheduler> subSchedulers;
		private INode currentNode;

		public StackScheduler() : this(null) { }

		private StackScheduler(StackScheduler parent)
		{
			this.parent = parent;
			this.runningNodes = new Stack<INode>();
			this.subSchedulers = new List<StackScheduler>();
		}

		public void OnStarted(INode node)
		{
			Add(node);
		}

		public void OnCompleted(INode node, Result result)
		{
			Remove(node, keepRootNode: this.parent != null);
		}

		public string[] RunningNodePaths
		{
			get
			{
				if (this.currentNode == null)
					return new string[] { };

				var currentPath = this.currentNode.Name;
				foreach (var node in this.runningNodes)
					currentPath = node.Name + "/" + currentPath;

				var paths = new List<string>();

				if (!HasRunningSubSchedulers())
				{
					paths.Add(currentPath);
					return paths.ToArray(); ;
				}

				for (var i = 0; i < this.subSchedulers.Count; i++)
				{
					var subPaths = this.subSchedulers[i].RunningNodePaths;
					for (var j = 0; j < subPaths.Length; j++)
					{
						var subPath = subPaths[j];
						paths.Add(currentPath + "/" + subPath);
					}
				}

				return paths.ToArray();
			}
		}

		public bool HasRunningNodes
		{
			get
			{
				if (this.currentNode != null)
					return true;

				for (var i = 0; i < this.subSchedulers.Count; i++)
					if (this.subSchedulers[i].HasRunningNodes)
						return true;

				return false;
			}
		}

		public void Add(INode node)
		{
			if (node == null || node == this.currentNode)
				return;

			if (this.currentNode != null)
				this.runningNodes.Push(this.currentNode);

			node.AddObserver(this);
			this.currentNode = node;

			var children = node.Children;
			if (children == null || children.Length == 0)
				return;

			if (node.RunChildrenInParallel)
			{
				for (var i = 0; i < children.Length; i++)
				{
					var subScheduler = GetEmptySubScheduler();
					subScheduler.Add(children[i]);
				}
			}
			else
			{
				for (var i = 0; i < children.Length; i++)
					children[i].AddObserver(this);
			}
		}

		public void Remove(INode node, bool keepRootNode)
		{
			if (node == null || this.currentNode != node)
				return;

			// Clear any data with current-sub schedulers that
			// may have been registered from this node we're removing.
			ClearSubSchedulers();

			// If this is a sub-scheduler and is the last node,
			// Leave the node running until the parent clears it
			// otherwise parallel root nodes won't be re-validated.
			if (keepRootNode && this.runningNodes.Count == 0)
				return;

			if (this.currentNode.Children != null)
			{
				for (var i = 0; i < this.currentNode.Children.Length; i++)
					this.currentNode.Children[i].RemoveObserver(this);
			}

			if (this.runningNodes.Count > 0)
			{
				this.currentNode = this.runningNodes.Pop();
			}
			else
			{
				this.currentNode.RemoveObserver(this);
				this.currentNode = null;
			}
		}

		public Result Run()
		{
			var hadSubSchedulers = HasRunningSubSchedulers();
			if (hadSubSchedulers)
			{
				for (var i = 0; i < this.subSchedulers.Count; i++)
					this.subSchedulers[i].Run();
			}

			INode previousNode = null;
			var result = Result.Success;

			while (previousNode != this.currentNode && this.currentNode != null)
			{
				previousNode = this.currentNode;
				result = this.currentNode.Run();
				if (result == Result.Running)
					break;
			}

			// If sub-schedulers were added during the last run
			// make sure call Run one more time to process them.
			if (!hadSubSchedulers && HasRunningSubSchedulers())
			{
				return Run();
			}

			return result;
		}

		private bool HasRunningSubSchedulers()
		{
			for (var i = 0; i < this.subSchedulers.Count; i++)
				if (this.subSchedulers[i].HasRunningNodes)
					return true;

			return false;
		}

		private StackScheduler GetEmptySubScheduler()
		{
			for (var i = 0; i < this.subSchedulers.Count; i++)
				if (!this.subSchedulers[i].HasRunningNodes)
					return this.subSchedulers[i];

			var subScheduler = new StackScheduler(this);
			this.subSchedulers.Add(subScheduler);
			return subScheduler;
		}

		private void ClearSubSchedulers()
		{
			for (var i = 0; i < this.subSchedulers.Count; i++)
			{
				var subScheduler = this.subSchedulers[i];
				while (subScheduler.currentNode != null)
				{
					subScheduler.currentNode.Reset();
					subScheduler.Remove(subScheduler.currentNode, false);
				}
			}
		}
	}
}
