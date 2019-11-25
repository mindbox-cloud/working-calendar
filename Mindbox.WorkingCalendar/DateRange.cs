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

		public DateRange Intersect(DateTime startDate, DateTime endDate)
		{
			var range = new DateRange(startDate, endDate);
			return Intersect(this, range);
		}

		/// <summary>
		/// Return intersection with current date range.
		/// </summary>
		/// <returns>Return null if no intersection.</returns>
		public DateRange Intersect(DateRange other)
		{
			return Intersect(this, other);
		}

		/// <summary>
		/// Return intersection with current date range.
		/// </summary>
		/// <returns>Return null if no intersection.</returns>
		public static DateRange Intersect(DateRange first, DateRange second)
		{
			if (first == null)
				throw new ArgumentNullException(nameof(first));
			if (second == null)
				throw new ArgumentNullException(nameof(second));

			if (first.Start > second.Start)
				(first, second) = (second, first);

			if (second.Start <= first.End)
			{
				var start = Min(second.Start, first.End);
				var end = Min(first.End, second.End);
				return new DateRange(start, end);
			}

			return null;
		}

		public override string ToString()
		{
			return $"{Start.ToShortDateString()} - {End.ToShortDateString()}";
		}
		
		public override int GetHashCode()
		{
			unchecked
			{
				return (Start.GetHashCode() * 397) ^ End.GetHashCode();
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((DateRange)obj);
		}
		
		public static bool operator ==(DateRange left, DateRange right)
		{
			if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;

			return left.Equals(right);
		}

		public static bool operator !=(DateRange left, DateRange right)
		{
			return !(left == right);
		}

		protected bool Equals(DateRange other)
		{
			return Start.Equals(other.Start) && End.Equals(other.End);
		}

		private static DateTime Min(DateTime first, DateTime second)
		{
			return first <= second ? first : second;
		}
	}
}