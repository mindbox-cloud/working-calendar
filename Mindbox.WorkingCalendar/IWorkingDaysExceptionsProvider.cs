using System;
using System.Collections.Generic;
using System.Text;

namespace Mindbox.WorkingCalendar
{
	public interface IWorkingDaysExceptionsProvider
	{
		bool TryGet(DateTime date, out DayType dayType);

		IEnumerable<(DateTime Date, DayType DayType)> GetExceptionsInPeriod(DateTime startDateTime, DateTime endDateTime);
	}
}
