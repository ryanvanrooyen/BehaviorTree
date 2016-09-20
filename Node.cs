
namespace BehaviorTree
{
	public enum Result
	{
		Success,
		Running,
		Failure
	}

	public interface INode
	{
		Result Run();
	}

	public abstract class Node : INode
	{
		public abstract Result Run();
	}

	public class Succeed : Node
	{
		public override Result Run()
		{
			return Result.Success;
		}
	}

	public class Fail : Node
	{
		public override Result Run()
		{
			return Result.Failure;
		}
	}

	public class Running : Node
	{
		public override Result Run()
		{
			return Result.Running;
		}
	}
}
