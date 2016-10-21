
namespace BehaviorTree
{
	public class Invert : Decorator
	{
		public Invert(Func<Result> node)
			: this(new Node(node)) { }

		public Invert(INode node) : base(node)
		{ }

		public override Result Run()
		{
			var result = base.Run();
			if (result == Result.Success)
				return Result.Failure;
			if (result == Result.Failure)
				return Result.Success;

			return result;
		}
	}
}
