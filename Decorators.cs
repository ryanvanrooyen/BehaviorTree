
using System;

namespace BehaviorTree
{
    public abstract class Decorator : INode
    {
        private readonly INode node;

        public Decorator(INode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            
            this.node = node;
        }

        public virtual Result Run()
        {
            return this.node.Run();
        }
    }

    public class Always : Decorator
    {
        private readonly Result result;

        public Always(Result result, INode node) : base(node)
        {
            if (result == Result.Running)
                throw new ArgumentException("Specified result must be success or failure");

            this.result = result;
        }

        public override Result Run()
        {
            var status = base.Run();
            if (status == Result.Running)
                return status;

            return this.result;
        }
    }

    public class Until : Decorator
    {
        private readonly Result result;

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

    public class Invert : Decorator
    {
        public Invert(INode node) : base(node)
        {}

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
            
            if (result == Result.Running)
                return result;
            
            this.iteration += 1;
            if (this.iteration >= this.times)
            {
                this.iteration = 0;
                return Result.Success;
            }

            return Result.Running;
        }
    }

    public class Retry : Decorator
    {
        private readonly uint maxAttempts;
        private uint attempt;

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
            if (this.attempt >= this.maxAttempts)
            {
                this.attempt = 0;
                return Result.Failure;
            }
            
            return Result.Running;
        }
    }

    public class Delay : Decorator
    {
        private readonly TimeSpan delay;
        private DateTime? startTime;

        public Delay(TimeSpan delay, INode node) : base(node)
        {
            this.delay = delay;
        }

        public override Result Run()
        {
            var currentTime = DateTime.Now;

            if (this.startTime == null)
                this.startTime = currentTime + this.delay;

            if (this.startTime > currentTime)
                return Result.Running;

            this.startTime = null;
            return base.Run();
        }
    }

    public class Limit : Decorator
    {
        private readonly TimeSpan maxRunTime;
        private DateTime? endTime;

        public Limit(TimeSpan maxRunTime, INode node) : base(node)
        {
            this.maxRunTime = maxRunTime;
        }

        public override Result Run()
        {
            var result = base.Run();

            if (result != Result.Running)
            {
                this.endTime = null;
                return result;
            }
            
            var currentTime = DateTime.Now;

            if (this.endTime == null)
            {
                this.endTime = currentTime + this.maxRunTime;
                return Result.Running;
            }

            // If the max time to wait has passed, return failed.
            if (currentTime > this.endTime)
            {
                this.endTime = null;
                return Result.Failure;
            }

            return Result.Running;
        }
    }
}
