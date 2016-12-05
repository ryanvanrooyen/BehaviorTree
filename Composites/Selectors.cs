
namespace BehaviorTree
{
	public class Selector : Composite
	{
		public Selector(params INode[] children)
			: this("Selector", children) { }

		public Selector(string name, params INode[] children)
			: base(name, Result.Failure, children) { }
	}

	public class RandomSelector : RandomComposite
	{
		public RandomSelector(params INode[] children)
			: this("RandomSelector", children) { }

		public RandomSelector(string name, params INode[] children)
			: base(name, Result.Failure, children) { }
	}

	public class ParallelSelector : ParallelComposite
	{
		public ParallelSelector(params INode[] children)
			: this("ParallelSelector", children) { }

		public ParallelSelector(string name, params INode[] children)
			: base(name, Result.Failure, ChildRunPolicy.ParallelOnce, children) { }

		public ParallelSelector(ChildRunPolicy runPolicy, params INode[] children)
			: base("ParallelSelector", Result.Failure, runPolicy, children) { }
		
		public ParallelSelector(string name, ChildRunPolicy runPolicy, params INode[] children)
			: base(name, Result.Failure, runPolicy, children) { }
	}
}
