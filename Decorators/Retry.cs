
namespace BehaviorTree
{
	public class Retry : Decorator
	{
		private readonly uint maxAttempts;
		private uint attempt;

		public Retry(uint maxAttempts, INode node) : base(node)
		{
			this.maxAttempts = maxAttempts;
			this.attempt = 0;
		}

		protected override Result RunNode()
		{
			var result = this.node.Run();

			if (result == Result.Success)
				this.attempt = 0;

			if (result != Result.Failure)
				return result;

			this.attempt += 1;
			if (this.attempt > this.maxAttempts)
			{
				this.attempt = 0;
				return Result.Failure;
			}

			return Result.Running;
		}
	}
}
