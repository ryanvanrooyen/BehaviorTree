
using System;

namespace BehaviorTree
{
	public interface IBehavior
	{
		Result Run();
		bool HasRunningNodes { get; }
		string[] RunningNodePaths { get; }
	}

	public class Behavior : IBehavior
	{
		private readonly string name;
		private readonly INode rootNode;
		private readonly IScheduler scheduler;

		public Behavior(INode rootNode)
			: this(rootNode, new StackScheduler()) { }

		public Behavior(string name, INode rootNode)
			: this(name, rootNode, new StackScheduler()) { }

		public Behavior(INode rootNode, IScheduler scheduler)
			: this("Behavior", rootNode, scheduler) { }

		public Behavior(string name, INode rootNode, IScheduler scheduler)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (rootNode == null)
				throw new ArgumentNullException(nameof(rootNode));
			if (scheduler == null)
				throw new ArgumentNullException(nameof(scheduler));

			this.name = name;
			this.rootNode = rootNode;
			this.scheduler = scheduler;
		}

		public Result Run()
		{
			if (!this.scheduler.HasRunningNodes)
				this.scheduler.Add(this.rootNode);

			return this.scheduler.Run();
		}

		public bool HasRunningNodes
		{
			get { return this.scheduler.HasRunningNodes; }
		}

		public string[] RunningNodePaths
		{
			get
			{
				var paths = this.scheduler.RunningNodePaths;
				if (paths == null)
					return paths;

				for (var i = 0; i < paths.Length; i++)
					paths[i] = this.name + "/" + paths[i];

				return paths;
			}
		}
	}
}
