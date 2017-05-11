using System;

namespace Statistics
{
	public static class Extensions
	{
		public static bool IsWeekend(this DayOfWeek dayOfWeek)
		{
			return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
		}
	}
}