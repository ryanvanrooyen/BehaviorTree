

using System;

namespace BehaviorTree
{
	public abstract class Composite : INode
	{
		private readonly INode[] children;

		public Composite(params INode[] children)
		{
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			this.children = children;
		}

		public abstract Result Run();

		protected Result Iterate(Result endResult)
		{
			foreach (var child in this.children)
			{
				var status = child.Run();
				if (status != endResult)
					return status;
			}

			return endResult;
		}

		protected Result RandomIterate(Result endResult, int[] indexes)
		{
			Indicies.Shuffle(indexes);

			for (var i = 0; i < indexes.Length; i++)
			{
				var index = indexes[i];
				var status = this.children[index].Run();
				if (status != endResult)
					return status;
			}

			return endResult;
		}
	}

	public class Selector : Composite
	{
		public Selector(params INode[] children) : base(children)
		{ }

		public override Result Run()
		{
			return Iterate(Result.Failure);
		}
	}

	public class RandomSelector : Composite
	{
		private readonly int[] indexes;

		public RandomSelector(params INode[] children) : base(children)
		{
			this.indexes = Indicies.Create(children.Length);
		}

		public override Result Run()
		{
			return RandomIterate(Result.Failure, this.indexes);
		}
	}

	public class Sequence : Composite
	{
		public Sequence(params INode[] children) : base(children)
		{ }

		public override Result Run()
		{
			return Iterate(Result.Success);
		}
	}

	public class RandomSequence : Composite
	{
		private readonly int[] indexes;

		public RandomSequence(params INode[] children) : base(children)
		{
			this.indexes = Indicies.Create(children.Length);
		}

		public override Result Run()
		{
			return RandomIterate(Result.Success, this.indexes);
		}
	}

	public static class Indicies
	{
		// Simple Fisher-Yates Shuffle
		// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
		public static void Shuffle(int[] indexes)
		{
			if (indexes == null || indexes.Length == 0)
				return;

			var random = new Random();
			var length = indexes.Length;

			for (var i = 0; i < length - 1; i++)
			{
				var j = random.Next(i, length - 1);
				var temp = indexes[i];
				indexes[i] = indexes[j];
				indexes[j] = temp;
			}
		}

		public static int[] Create(int length)
		{
			var indexes = new int[length];
			for (var i = 0; i < length; i++)
				indexes[i] = i;

			return indexes;
		}
	}
}
