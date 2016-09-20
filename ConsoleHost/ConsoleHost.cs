
using System;

namespace BehaviorTree
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			var indexes = Indicies.Create(5);
			Indicies.Shuffle(indexes);

			foreach (var i in indexes)
				Console.WriteLine(i);
		}
	}
}
