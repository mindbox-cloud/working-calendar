using System;
using System.Net;

namespace Mindbox.WorkingCalendar
{
	/// <summary>
	/// Represent of date range. Uses only date information for processing.
	/// </summary>
	public class DateRange
	{
		public DateTime Start { get; }
		public DateTime End { get; }

		
		/// <summary>
		/// Represent date range. Uses only date information for processing.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public DateRange(DateTime startDate, DateTime endDate)
		{
			if (startDate > endDate)
				throw new ArgumentException("start date must be greater then or equal to end date.");

			Start = startDate.Date;
			End = endDate.Date;
		}

		public bool IsInRange(DateTime date)
		{
			return Start <= date && date <= End;
		}

		public override string ToString()
		{
			return $"{Start.ToShortDateString()} - {End.ToShortDateString()}";
		}

		public override bool Equals(object obj) => obj is DateRange range && Equals(range);

		public bool Equals(DateRange other) => this == other;

		public override int GetHashCode()
		{
			unchecked
			{
				return (Start.GetHashCode() * 397) ^ End.GetHashCode();
			}
		}

		public static bool operator ==(DateRange right, DateRange left)
		{
			return !ReferenceEquals(right, null) 
				&& !ReferenceEquals(left, null)
				&& right.Start == left.Start && right.End == left.End;
		}

		public static bool operator !=(DateRange right, DateRange left)
		{
			return !(right == left);
		}
	}
}