
namespace BehaviorTree
{
	public class Repeat : Decorator
	{
		private readonly uint times;
		private uint iteration;

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
}
