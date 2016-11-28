
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
		private readonly INode rootNode;
		private readonly IScheduler scheduler;

		public Behavior(INode rootNode)
			: this(rootNode, new StackScheduler()) { }

		public Behavior(INode rootNode, IScheduler scheduler)
		{
			if (rootNode == null)
				throw new ArgumentNullException("rootNode");
			if (scheduler == null)
				throw new ArgumentNullException("scheduler");

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
			get { return this.scheduler.RunningNodePaths; }
		}
	}
}
