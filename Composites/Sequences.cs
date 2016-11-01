
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
			: base(name, Result.Success, children) { }
	}

	public class While : Sequence
	{
		public While(Delegates.Func<bool> condition, INode longRunningAction)
			: base("While", new If(condition),
					new ParallelSequence("Parallel", new If(condition), longRunningAction))
		{
		}
	}
}
