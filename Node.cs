
using System;

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

    public class Node : INode
    {
        private readonly Func<Result> behavior;

        public Node(Func<Result> behavior)
        {
            if (behavior == null)
                throw new ArgumentNullException("behavior");

            this.behavior = behavior;
        }

        public Result Run()
        {
            return this.behavior();
        }
    }

    public class Succeed : Node
    {
        public Succeed() : base(() => Result.Success)
        {}
    }

    public class Fail : Node
    {
        public Fail() : base(() => Result.Failure)
        {}
    }

    public class Running : Node
    {
        public Running() : base(() => Result.Running)
        {}
    }
}
