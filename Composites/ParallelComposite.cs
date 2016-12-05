
using System;

namespace BehaviorTree
{
	public abstract class ParallelComposite : Composite
	{
		private readonly ChildRunPolicy childRunPolicy;
		private readonly Result[] results;
		private Result totalResult;

		public ParallelComposite(string name, Result endResult,
			ChildRunPolicy childRunPolicy, params INode[] children)
			: base(name, endResult, children)
		{
			if (childRunPolicy == ChildRunPolicy.Sequential)
				throw new ArgumentException("Child run policy cannot be sequential for parallel composites.");
			
			this.childRunPolicy = childRunPolicy;
			this.results = new Result[children.Length];
			ResetResults();
		}

		public override ChildRunPolicy ChildRunPolicy { get { return this.childRunPolicy; } }

		public override void Reset()
		{
			base.Reset();
			ResetResults();
		}

		public override void OnCompleted(INode node, Result result)
		{
			var childIndex = GetChildIndex(node);
			if (childIndex != -1)
			{
				this.results[childIndex] = result;
				this.totalResult = GetTotalResult();
			}
		}

		protected override Result RunNode()
		{
			if (this.totalResult != Result.Running)
			{
				var lastResult = this.totalResult;
				ResetResults();
				return lastResult;
			}

			this.totalResult = GetTotalResult();
			return this.totalResult;
		}

		private Result GetTotalResult()
		{
			var anyRunning = false;

			var exitResult = Result.Success;
			if (this.endResult == Result.Success)
				exitResult = Result.Failure;

			for (var i = 0; i < this.results.Length; i++)
			{
				var result = this.results[i];
				if (result == exitResult)
					return exitResult;

				anyRunning = anyRunning || result == Result.Running;
			}

			if (anyRunning)
				return Result.Running;

			return this.endResult;
		}

		private void ResetResults()
		{
			this.totalResult = Result.Running;
			for (var i = 0; i < this.results.Length; i++)
				this.results[i] = Result.Running;
		}
	}
}
