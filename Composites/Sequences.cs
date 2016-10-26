
namespace BehaviorTree
{
	public class Sequence : Composite
	{
		public Sequence(params INode[] children)
			: base("Sequence", Result.Success, children) { }
	}

	public class RandomSequence : RandomComposite
	{
		public RandomSequence(params INode[] children)
			: base("RandomSequence", Result.Success, children) { }
	}

	public class ParallelSequence : ParallelComposite
	{
		public ParallelSequence(params INode[] children)
			: base("ParallelSequence", Result.Success, children) { }
	}
}
