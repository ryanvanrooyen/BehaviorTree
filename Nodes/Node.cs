
using System;
using System.Collections.Generic;

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
		string Name { get; }
		INode[] Children { get; }
		bool RunChildrenInParallel { get; }
		void AddObserver(INodeObserver observer);
		void RemoveObserver(INodeObserver observer);
	}

	public interface INodeObserver
	{
		void OnStarted(INode node);
		void OnCompleted(INode node, Result result);
	}

	public class Delegates
	{
		public delegate T Func<T>();
		public delegate void Func();
	}

	public abstract class Node : INode
	{
		private readonly string name;
		private bool hasStarted;
		private IList<INodeObserver> observers;
		protected bool runChildreninParallel = false;

		public Node(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			this.name = name;
		}

		public virtual string Name { get { return this.name; } }
		public virtual INode[] Children { get { return null; } }
		public virtual bool RunChildrenInParallel { get { return false; } }

		public void AddObserver(INodeObserver observer)
		{
			if (observer == null)
				return;
			if (this.observers == null)
				this.observers = new List<INodeObserver>();
			if (!this.observers.Contains(observer))
				this.observers.Add(observer);
		}

		public void RemoveObserver(INodeObserver observer)
		{
			if (observer != null && this.observers != null)
				this.observers.Remove(observer);
		}

		public Result Run()
		{
			if (!this.hasStarted)
			{
				this.hasStarted = true;

				if (this.observers != null)
				{
					for (var i = 0; i < this.observers.Count; i++)
						this.observers[i].OnStarted(this);
				}
			}

			var result = RunNode();

			if (result != Result.Running)
			{
				this.hasStarted = false;

				if (this.observers != null)
				{
					for (var i = 0; i < this.observers.Count; i++)
						this.observers[i].OnCompleted(this, result);
				}
			}

			return result;
		}

		protected abstract Result RunNode();

		public static INode Success
		{
			get { return new Act("Success", () => Result.Success); }
		}

		public static INode Fail
		{
			get { return new Act("Fail", () => Result.Failure); }
		}

		public static INode Running
		{
			get { return new Act("Running", () => Result.Running); }
		}
	}
}
