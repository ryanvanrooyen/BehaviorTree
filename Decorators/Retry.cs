
namespace BehaviorTree
{
	public class Retry : Decorator
	{
		private readonly uint maxAttempts;
		private uint attempt;

		public Retry(uint maxAttempts, Func<Result> node)
			: this(maxAttempts, new Node(node)) { }

		public Retry(uint maxAttempts, INode node) : base(node)
		{
			this.maxAttempts = maxAttempts;
			this.attempt = 0;
		}

		public override Result Run()
		{
			var result = base.Run();

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

	public class Retry<T> : Decorator<T>
	{
		private readonly uint maxAttempts;
		private uint attempt;

		public Retry(uint maxAttempts, Func<T, Result> node)
			: this(maxAttempts, new Node<T>(node)) { }

		public Retry(uint maxAttempts, INode<T> node) : base(node)
		{
			this.maxAttempts = maxAttempts;
			this.attempt = 0;
		}

		public override Result Run(T data)
		{
			var result = base.Run(data);

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
