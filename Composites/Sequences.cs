
namespace BehaviorTree
{
	public class Sequence : Composite
	{
		public Sequence(params INode[] children)
			: this("Sequence", children) { }

		public Sequence(string name, params INode[] children)
			: base(name, Result.Success, children) { }
	}

	public class RandomSequence : RandomComposite
	{
		public RandomSequence(params INode[] children)
			: this("RandomSequence", children) { }

		public RandomSequence(string name, params INode[] children)
			: base(name, Result.Success, children) { }
	}

	public class ParallelSequence : ParallelComposite
	{
		public ParallelSequence(params INode[] children)
			: this("ParallelSequence", children) { }

		public ParallelSequence(string name, params INode[] children)
			: base(name, Result.Success, ChildRunPolicy.ParallelOnce, children) { }

		public ParallelSequence(ChildRunPolicy runPolicy, params INode[] children)
			: base("ParallelSequence", Result.Success, runPolicy, children) { }
		
		public ParallelSequence(string name, ChildRunPolicy runPolicy, params INode[] children)
			: base(name, Result.Success, runPolicy, children) { }
	}
}
