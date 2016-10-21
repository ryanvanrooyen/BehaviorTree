
using System;

namespace BehaviorTree
{
	internal static class Indexes
	{
		private static readonly Random random = new Random();

		// Simple Fisher-Yates Shuffle
		// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
		public static void Shuffle(int[] indexes)
		{
			if (indexes == null || indexes.Length == 0)
				return;

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
