
namespace BehaviorTree
{
	public class Selector : Composite
	{
		public Selector(params INode[] children)
			: base("Selector", Result.Failure, children) { }
	}

	public class RandomSelector : RandomComposite
	{
		public RandomSelector(params INode[] children)
			: base("RandomSelector", Result.Failure, children) { }
	}

	public class ParallelSelector : ParallelComposite
	{
		public ParallelSelector(params INode[] children)
			: base("ParallelSelector", Result.Failure, children) { }
	}
}
