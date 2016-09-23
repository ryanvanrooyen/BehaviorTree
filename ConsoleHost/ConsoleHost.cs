
using System;
using System.Threading;

namespace BehaviorTree
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var i = 0;
			var playerVisible = new Node(() =>
			{
				Console.WriteLine("Checking if Player visible.");
				i++;
				var visible = i > 3;
				Console.WriteLine("Player is visible: " + visible);
				return visible ? Result.Success : Result.Failure;
			});

			var chasePlayer = new Repeat(3, new Node(() =>
			{
				Console.WriteLine("Chasing player...");
				return Result.Success;
			}));

			var attackPlayer = new Node(() =>
			{
				Console.WriteLine("Attacked player.");
				return Result.Success;
			});

			var attack = new Sequence(playerVisible, chasePlayer, attackPlayer);

			var patrol = new Node(() =>
			{
				Console.WriteLine("Patrolling around.");
				return Result.Running;
			});

			var root = new Selector(attack, patrol);

			Result result = Result.Running;
			while (result == Result.Running)
			{
				Thread.Sleep(1000);
				root.Run();
			}
		}
	}
}
