using System;
using System.Collections.Generic;
using System.Linq;

namespace Mindbox.WorkingCalendar
{
	public sealed class FixedWorkingDaysExceptionsProvider : IWorkingDaysExceptionsProvider
	{
		private readonly IDictionary<DateTime, DayType> workingDaysExceptions;

		public FixedWorkingDaysExceptionsProvider(IDictionary<DateTime, DayType> workingDaysExceptions)
		{
			this.workingDaysExceptions = workingDaysExceptions ?? throw new ArgumentNullException(nameof(workingDaysExceptions));
		}

		public bool TryGet(DateTime date, out DayType dayType)
		{
			return workingDaysExceptions.TryGetValue(date, out dayType);
		}

		public IEnumerable<(DateTime Date, DayType DayType)> GetExceptionsInPeriod(DateTime startDateTime, DateTime endDateTime)
		{
			return workingDaysExceptions
				.Where(pair => pair.Key >= startDateTime)
				.Where(pair => pair.Key < endDateTime)
				.Select(pair => (pair.Key, pair.Value));
		}
	}
}