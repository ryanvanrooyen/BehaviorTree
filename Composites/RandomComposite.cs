
namespace BehaviorTree
{
	public abstract class RandomComposite : Composite
	{
		private readonly int[] indexes;

		public RandomComposite(string name, Result endResult, params INode[] children)
			: base(name, endResult, children)
		{
			this.indexes = Indexes.Create(children.Length);
		}

		protected override Result RunNode()
		{
			// If we're starting at the beginning of the children again,
			// make sure to re-shuffle the indexes to get a new, random order.
			if (this.currentChild == 0)
				Indexes.Shuffle(this.indexes);

			return base.RunNode();
		}

		protected override INode GetCurrentChild()
		{
			if (this.currentChild < this.children.Length)
				return this.children[this.indexes[this.currentChild]];

			return null;
		}
	}
}
