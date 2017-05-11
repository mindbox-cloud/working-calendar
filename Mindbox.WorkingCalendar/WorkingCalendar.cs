using System;
using System.Collections.Generic;
using System.Linq;

namespace Mindbox.WorkingCalendar
{
	public class WorkingCalendar
	{
		private static readonly Dictionary<DayOfWeek, int> dayOfWeekOffsets = new Dictionary<DayOfWeek, int>
		{
			[DayOfWeek.Monday] = 0,
			[DayOfWeek.Tuesday] = 1,
			[DayOfWeek.Wednesday] = 2,
			[DayOfWeek.Thursday] = 3,
			[DayOfWeek.Friday] = 4,
			[DayOfWeek.Saturday] = 5,
			[DayOfWeek.Sunday] = 6
		};

		public static WorkingCalendar Russia { get; } = new WorkingCalendar(WorkingDaysExceptions.GetForRussia);

		
		private readonly Func<IDictionary<DateTime, DayType>> exceptionsProvider;
		private IDictionary<DateTime, DayType> exceptions;


		public WorkingCalendar(Func<IDictionary<DateTime, DayType>> exceptionsProvider)
		{
			if (exceptionsProvider == null)
				throw new ArgumentNullException(nameof(exceptionsProvider));
			
			this.exceptionsProvider = exceptionsProvider;
		}

		public WorkingCalendar(IDictionary<DateTime, DayType> exceptions)
		{
			if (exceptions == null)
				throw new ArgumentNullException(nameof(exceptions));

			this.exceptions = exceptions;
		}

		private IDictionary<DateTime, DayType> Exceptions => exceptions ?? (exceptions = exceptionsProvider());

		public bool IsWorkingDay(DateTime dateTime)
		{
			switch (GetDayType(dateTime))
			{
				case DayType.Working:
				case DayType.Short:
					return true;
				case DayType.Holiday:
				case DayType.Weekend:
					return false;
				default:
					throw new NotSupportedException($"Unknonwn DayType value: {GetDayType(dateTime)}");
			}
		}

		public DayType GetDayType(DateTime dateTime)
		{
			if (exceptions.TryGetValue(dateTime.Date, out var result))
			{
				return result;
			}
			else if (dateTime.DayOfWeek.IsWeekend())
			{
				return DayType.Weekend;
			}
			else
			{
				return DayType.Working;
			}
		}

		/// <summary>
		/// Вычисляет количество полных рабочих дней в периоде, учитывая праздничные дни.
		/// </summary>
		/// <param name="startDateTime">Дата начала периода (включительно).</param>
		/// <param name="endDateTime">Дата окончания периода (не включительно).</param>
		public int CountWorkingDaysInPeriod(DateTime startDateTime, DateTime endDateTime)
		{
			if (endDateTime < startDateTime)
				throw new ArgumentException("startDateTime must be greater then or equal to endDateTime.");

			var startDateTimeRounded = startDateTime.Date == startDateTime
				? startDateTime
				: startDateTime.Date.AddDays(1);
			var endDateTimeRounded = endDateTime.Date;

			var totalElapsedDays = (int) (endDateTimeRounded - startDateTimeRounded).TotalDays;
			
			var daysFromNearestMonday = dayOfWeekOffsets[startDateTimeRounded.DayOfWeek] + totalElapsedDays;
			var weekendDays = (daysFromNearestMonday / 7) * 2 + (daysFromNearestMonday % 7 == 6 ? 1 : 0);

			var exceptionsInPeriod = GetExceptionsInPeriod(startDateTimeRounded, endDateTimeRounded);
			var holidays = exceptionsInPeriod.Count(pair => pair.Value == DayType.Holiday);
			var workingWeekendDays = 
				exceptionsInPeriod.Count(pair => pair.Value == DayType.Working) + 
				exceptionsInPeriod.Where(pair => pair.Value == DayType.Short).Count(pair => pair.Key.DayOfWeek.IsWeekend());

			var dateParts = TimeSpan.Zero;
			if (IsWorkingDay(startDateTime))
			{
				dateParts = dateParts.Add(startDateTimeRounded - startDateTime);
			}
			if (IsWorkingDay(endDateTime))
			{
				dateParts = dateParts.Add(endDateTime - endDateTimeRounded);
			}
			totalElapsedDays += (int)dateParts.TotalDays;

			return totalElapsedDays - weekendDays - holidays + workingWeekendDays;
		}

		/// <summary>
		/// Получает исключения за период.
		/// </summary>
		/// <param name="startDateTime">Дата начала периода (включительно).</param>
		/// <param name="endDateTime">Дата окончания периода (не включительно).</param>
		private IReadOnlyCollection<KeyValuePair<DateTime, DayType>> GetExceptionsInPeriod(
			DateTime startDateTime, DateTime endDateTime)
		{
			return Exceptions
				.Where(pair => pair.Key >= startDateTime)
				.Where(pair => pair.Key < endDateTime)
				.ToArray();
		}
	}
}