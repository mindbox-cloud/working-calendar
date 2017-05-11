using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mindbox.WorkingCalendar.Tests
{
	[TestClass]
	public class IsWorkingDayTests
	{
		private WorkingCalendar CreateCalendar(Dictionary<DateTime, DayType> exceptions = null)
		{
			return new WorkingCalendar(exceptions ?? new Dictionary<DateTime, DayType>());
		}

		[TestMethod]
		public void MondayIsWorking()
		{
			var calendar = CreateCalendar();

			Assert.IsTrue(calendar.IsWorkingDay(new DateTime(2017, 04, 10)));
		}

		[TestMethod]
		public void TuesdayIsWorking()
		{
			var calendar = CreateCalendar();

			Assert.IsTrue(calendar.IsWorkingDay(new DateTime(2017, 04, 11)));
		}

		[TestMethod]
		public void WednesdayIsWorking()
		{
			var calendar = CreateCalendar();

			Assert.IsTrue(calendar.IsWorkingDay(new DateTime(2017, 04, 12)));
		}

		[TestMethod]
		public void ThursdayIsWorking()
		{
			var calendar = CreateCalendar();

			Assert.IsTrue(calendar.IsWorkingDay(new DateTime(2017, 04, 13)));
		}

		[TestMethod]
		public void FridayIsWorking()
		{
			var calendar = CreateCalendar();

			Assert.IsTrue(calendar.IsWorkingDay(new DateTime(2017, 04, 14)));
		}

		[TestMethod]
		public void SaturdayIsNotWorking()
		{
			var calendar = CreateCalendar();

			Assert.IsFalse(calendar.IsWorkingDay(new DateTime(2017, 04, 15)));
		}

		[TestMethod]
		public void SundayIsNotWorking()
		{
			var calendar = CreateCalendar();

			Assert.IsFalse(calendar.IsWorkingDay(new DateTime(2017, 04, 16)));
		}

		[TestMethod]
		public void HolidayMorningIsNotWorking()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 05, 01), DayType.Holiday }
				});

			Assert.IsFalse(calendar.IsWorkingDay(new DateTime(2017, 05, 01)));
		}

		[TestMethod]
		public void ShortFridayIsWorking()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 04, 28), DayType.Short }
				});

			Assert.IsTrue(calendar.IsWorkingDay(new DateTime(2017, 04, 28)));
		}

		[TestMethod]
		public void ShortSaturdayIsWorking()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 04, 29), DayType.Short }
				});

			Assert.IsTrue(calendar.IsWorkingDay(new DateTime(2017, 04, 29)));
		}

		[TestMethod]
		public void WorkingSaturdayIsWorking()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 04, 29), DayType.Working }
				});

			Assert.IsTrue(calendar.IsWorkingDay(new DateTime(2017, 04, 29)));
		}
	}
}
