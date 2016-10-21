
namespace BehaviorTree
{
	public class Repeat : Decorator
	{
		private readonly uint times;
		private uint iteration;

		public Repeat(uint times, Func<Result> node)
			: this(times, new Node(node)) { }

		public Repeat(uint times, INode node) : base(node)
		{
			this.times = times;
			this.iteration = 0;
		}

		public override Result Run()
		{
			var result = base.Run();

			if (result != Result.Success)
				return result;

			this.iteration += 1;
			if (this.iteration > this.times)
			{
				this.iteration = 0;
				return Result.Success;
			}

			return Result.Running;
		}
	}

	public class Repeat<T> : Decorator<T>
	{
		private readonly uint times;
		private uint iteration;

		public Repeat(uint times, Func<T, Result> node)
			: this(times, new Node<T>(node)) { }

		public Repeat(uint times, INode<T> node) : base(node)
		{
			this.times = times;
			this.iteration = 0;
		}

		public override Result Run(T data)
		{
			var result = base.Run(data);

			if (result != Result.Success)
				return result;

			this.iteration += 1;
			if (this.iteration > this.times)
			{
				this.iteration = 0;
				return Result.Success;
			}

			return Result.Running;
		}
	}
}
