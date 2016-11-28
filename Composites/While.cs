
using System;

namespace BehaviorTree
{
	public class While : Composite
	{
		public While(Delegates.Func<bool> condition, INode trueAction)
			: base("While", Result.Success, new If(condition),
				new ParallelSequence("Parallel", new If(condition), trueAction))
		{
		}

		public While(Delegates.Func<bool> condition, INode trueAction, INode falseAction)
			: base("While", Result.Failure,
				   CreateTrueBranch(condition, trueAction),
				   CreateFalseBranch(condition, falseAction))
		{
		}

		private static INode CreateTrueBranch(Delegates.Func<bool> condition, INode trueAction)
		{
			if (condition == null)
				throw new ArgumentNullException("condition");
			if (trueAction == null)
				throw new ArgumentNullException("trueAction");

			return new Sequence(new If(condition),
				new ParallelSequence("Parallel", new If(condition), trueAction));
		}

		private static INode CreateFalseBranch(Delegates.Func<bool> condition, INode falseAction)
		{
			if (condition == null)
				throw new ArgumentNullException("condition");
			if (falseAction == null)
				throw new ArgumentNullException("falseAction");

			return new Sequence(new Invert(new If(condition)),
				new ParallelSequence("Parallel", new Invert(new If(condition)), falseAction));
		}
	}
}
