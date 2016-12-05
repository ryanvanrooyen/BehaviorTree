
using System;

namespace BehaviorTree
{
	public class While : Composite
	{
		public While(Delegates.Func<bool> condition, INode trueAction)
			: base("While", Result.Success, CreateIfNodes(condition, trueAction))
		{ }

		public While(Delegates.Func<bool> condition, INode trueAction, INode falseAction)
			: base("While", Result.Failure, CreateIfElseNodes(condition, trueAction, falseAction))
		{ }

		private static INode[] CreateIfNodes(Delegates.Func<bool> condition, INode trueAction)
		{
			if (condition == null)
				throw new ArgumentNullException("condition");
			if (trueAction == null)
				throw new ArgumentNullException("trueAction");
			
			var whileIf = new WhileIf(condition);

			return new INode[]
			{
				whileIf.InitialIfTrue,
				new ParallelSequence("Parallel",
					ChildRunPolicy.ParallelRevalidate, whileIf.ParallelIfTrue, trueAction)
			};
		}

		private static INode[] CreateIfElseNodes(Delegates.Func<bool> condition,
			INode trueAction, INode falseAction)
		{
			if (condition == null)
				throw new ArgumentNullException("condition");
			if (trueAction == null)
				throw new ArgumentNullException("trueAction");
			if (falseAction == null)
				throw new ArgumentNullException("falseAction");

			var whileIf = new WhileIf(condition);

			return new INode[]
			{
				new Sequence(whileIf.InitialIfTrue,
					new ParallelSequence("Parallel",
						ChildRunPolicy.ParallelRevalidate, whileIf.ParallelIfTrue, trueAction)),
				new Sequence(whileIf.InitialIfFalse,
					new ParallelSequence("Parallel",
						ChildRunPolicy.ParallelRevalidate, new Invert(whileIf.ParallelIfFalse), falseAction))
			};
		}

		private class WhileIf
		{
			public readonly INode InitialIfTrue;
			public readonly INode InitialIfFalse;
			public readonly INode ParallelIfTrue;
			public readonly INode ParallelIfFalse;
			private bool lastVal;
			private bool useLastInitialIfTrue;
			private bool useLastInitialIfFalse;

			public WhileIf(Delegates.Func<bool> condition)
			{
				this.InitialIfTrue = new If(() =>
				{
					this.lastVal = condition();
					this.useLastInitialIfTrue = this.lastVal == true;

					return this.lastVal;
				});

				this.InitialIfFalse = new If(() =>
				{
					this.useLastInitialIfFalse = this.lastVal == false;
					return !this.lastVal;
				});

				this.ParallelIfTrue = new If(() =>
				{
					if (this.useLastInitialIfTrue)
					{
						this.useLastInitialIfTrue = false;
						return true;
					}

					this.lastVal = condition();
					return this.lastVal;
				});

				this.ParallelIfFalse = new If(() =>
				{
					if (this.useLastInitialIfFalse)
					{
						this.useLastInitialIfFalse = false;
						return false;
					}

					this.lastVal = condition();
					return this.lastVal;
				});
			}
		}
	}
}
