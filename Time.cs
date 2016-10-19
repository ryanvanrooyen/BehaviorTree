
using System;

namespace BehaviorTree
{
	internal interface ITime
	{
		DateTime Now { get; }
	}

	internal static class Time
	{
		public static readonly ITime Real = new RealTime();

		private class RealTime : ITime
		{
			public DateTime Now
			{
				get { return DateTime.Now; }
			}
		}
	}
}
