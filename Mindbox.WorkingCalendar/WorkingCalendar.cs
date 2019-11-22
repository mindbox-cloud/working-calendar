using System;
using System.Collections.Generic;
using System.Linq;

namespace Mindbox.WorkingCalendar
{
	public sealed class WorkingCalendar
	{
		private static readonly IReadOnlyDictionary<DayOfWeek, int> dayOfWeekOffsets = new Dictionary<DayOfWeek, int>
		{
			[DayOfWeek.Monday] = 0,
			[DayOfWeek.Tuesday] = 1,
			[DayOfWeek.Wednesday] = 2,
			[DayOfWeek.Thursday] = 3,
			[DayOfWeek.Friday] = 4,
			[DayOfWeek.Saturday] = 5,
			[DayOfWeek.Sunday] = 6
		};

		public static WorkingCalendar Russia { get; } = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());

		public DateRange SupportedDateRange { get; }
		
		private readonly IWorkingDaysExceptionsProvider exceptionsProvider;

		public WorkingCalendar(IWorkingDaysExceptionsProvider exceptionsProvider)
		{
			if (exceptionsProvider == null)
				throw new ArgumentNullException(nameof(exceptionsProvider));
			
			this.exceptionsProvider = exceptionsProvider;
			SupportedDateRange = exceptionsProvider.SupportedDateRange;
		}


		public WorkingCalendar(IDictionary<DateTime, DayType> exceptions) : this(new FixedWorkingDaysExceptionsProvider(exceptions))
		{
		}

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
					throw new NotSupportedException($"Unknown DayType value: {GetDayType(dateTime)}");
			}
		}

		public DayType GetDayType(DateTime dateTime)
		{
			if (exceptionsProvider.TryGet(dateTime.Date, out var result))
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
		/// Computes whole working days count in period considering holidays and extra working days.
		/// </summary>
		/// <param name="startDateTime">Period start date and time.</param>
		/// <param name="endDateTime">Period end date and time.</param>
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

			var exceptionsInPeriod = exceptionsProvider.GetExceptionsInPeriod(startDateTimeRounded, endDateTimeRounded).ToArray();

			var nonWeekendHolidays = exceptionsInPeriod
				.Where(exception => exception.DayType == DayType.Holiday)
				.Count(exception => !exception.Date.DayOfWeek.IsWeekend());

			var workingWeekendDays = exceptionsInPeriod
				.Where(exception => exception.DayType == DayType.Working || exception.DayType == DayType.Short)
				.Count(exception => exception.Date.DayOfWeek.IsWeekend());

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

			return totalElapsedDays - weekendDays - nonWeekendHolidays + workingWeekendDays;
		}

		/// <summary>
		/// Computes DateTime in `daysToAdd` working days after `dateTime`.
		/// </summary>
		public DateTime AddWorkingDays(DateTime dateTime, int daysToAdd)
		{
			if (daysToAdd == 0)
				return dateTime;

			var result = dateTime;

			var oneDayPeriod = daysToAdd > 0 ? 1 : -1;
			var addedDays = 0;

			while (addedDays != daysToAdd)
			{
				result += TimeSpan.FromDays(oneDayPeriod);
				if (IsWorkingDay(result))
				{
					addedDays += oneDayPeriod;
				}
			}

			return result;
		}
	}
}