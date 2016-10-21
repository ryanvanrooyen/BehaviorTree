
using System;

namespace BehaviorTree
{
	public class Until : Decorator
	{
		private readonly Result result;

		public Until(Result result, Func<Result> node)
			: this(result, new Node(node)) { }

		public Until(Result result, INode node) : base(node)
		{
			if (result == Result.Running)
				throw new ArgumentException("Specified result must be success or failure");

			this.result = result;
		}

		public override Result Run()
		{
			var status = base.Run();
			if (status == this.result)
				return Result.Success;

			return Result.Running;
		}
	}
}
