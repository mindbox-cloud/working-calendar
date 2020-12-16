using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mindbox.WorkingCalendar.Tests
{
	[TestClass]
	public class CountWorkingDaysInPeriodTests
	{
		private WorkingCalendar CreateCalendar(Dictionary<DateTime, DayType> exceptions = null)
		{
			return new WorkingCalendar(exceptions ?? new Dictionary<DateTime, DayType>());
		}

		[TestMethod]
		public void FromMondayToFirday()
		{
			var calendar = CreateCalendar();
			var mondayMorning = new DateTime(2017, 04, 10, 11, 0, 0);
			var fridayMorning = new DateTime(2017, 04, 14, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(mondayMorning, fridayMorning);
			Assert.AreEqual(4, workingDays);
		}

		[TestMethod]
		public void FromMondayToMonday()
		{
			var calendar = CreateCalendar();
			var mondayMorning = new DateTime(2017, 04, 10, 11, 0, 0);
			var nextMondayMorning = new DateTime(2017, 04, 17, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(mondayMorning, nextMondayMorning);
			Assert.AreEqual(5, workingDays);
		}

		[TestMethod]
		public void FromFridayToMonday()
		{
			var calendar = CreateCalendar();
			var fridayMorning = new DateTime(2017, 04, 14, 11, 0, 0);
			var mondayMorning = new DateTime(2017, 04, 17, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayMorning, mondayMorning);
			Assert.AreEqual(1, workingDays);
		}

		[TestMethod]
		public void FromFridayEveningToMondayMorning()
		{
			var calendar = CreateCalendar();
			var fridayEvening = new DateTime(2017, 04, 14, 20, 0, 0);
			var mondayMorning = new DateTime(2017, 04, 17, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayEvening, mondayMorning);
			Assert.AreEqual(0, workingDays);
		}

		[TestMethod]
		public void FromThursdayToSunday()
		{
			var calendar = CreateCalendar();
			var thursdayMorning = new DateTime(2017, 04, 13, 11, 0, 0);
			var sundayMorning = new DateTime(2017, 04, 16, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(thursdayMorning, sundayMorning);
			Assert.AreEqual(1, workingDays);
		}

		[TestMethod]
		public void FromThursdayMidnightToSundayMidnight()
		{
			var calendar = CreateCalendar();
			var thursdayMorning = new DateTime(2017, 04, 13, 0, 0, 0);
			var sundayMorning = new DateTime(2017, 04, 16, 0, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(thursdayMorning, sundayMorning);
			Assert.AreEqual(2, workingDays);
		}

		[TestMethod]
		public void FromThursdayToNextWeekMonday()
		{
			var calendar = CreateCalendar();
			var thursdayMorning = new DateTime(2017, 04, 13, 11, 0, 0);
			var mondayMorning = new DateTime(2017, 04, 24, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(thursdayMorning, mondayMorning);
			Assert.AreEqual(7, workingDays);
		}

		[TestMethod]
		public void HolidayInMiddleOfWeek()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 03, 08), DayType.Holiday }
				});
			var mondayMorning = new DateTime(2017, 03, 06, 11, 0, 0);
			var fridayMorning = new DateTime(2017, 03, 10, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(mondayMorning, fridayMorning);
			Assert.AreEqual(3, workingDays);
		}

		[TestMethod]
		public void FromFridayToHolidayMonday()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 05, 01), DayType.Holiday }
				});
			var fridayMorning = new DateTime(2017, 04, 28, 11, 0, 0);
			var mondayMorning = new DateTime(2017, 05, 01, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayMorning, mondayMorning);
			Assert.AreEqual(0, workingDays);
		}

		[TestMethod]
		public void FromFridayToTuesdayMondayIsHoliday()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 05, 01), DayType.Holiday }
				});
			var fridayMorning = new DateTime(2017, 04, 28, 11, 0, 0);
			var tuesdayMorning = new DateTime(2017, 05, 02, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayMorning, tuesdayMorning);
			Assert.AreEqual(1, workingDays);
		}

		[TestMethod]
		public void FromFridayToMondaySaturdayIsWorking()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 04, 29), DayType.Working }
				});
			var fridayMorning = new DateTime(2017, 04, 28, 11, 0, 0);
			var mondayMorning = new DateTime(2017, 05, 01, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayMorning, mondayMorning);
			Assert.AreEqual(2, workingDays);
		}

		[TestMethod]
		public void FromFridayToMondaySaturdayIsShortDay()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 04, 29), DayType.Short }
				});
			var fridayMorning = new DateTime(2017, 04, 28, 11, 0, 0);
			var mondayMorning = new DateTime(2017, 05, 01, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayMorning, mondayMorning);
			Assert.AreEqual(2, workingDays);
		}

		[TestMethod]
		public void FromFridayToMondayFridayShortDay()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 04, 28), DayType.Short }
				});
			var fridayMorning = new DateTime(2017, 04, 28, 11, 0, 0);
			var mondayMorning = new DateTime(2017, 05, 01, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayMorning, mondayMorning);
			Assert.AreEqual(1, workingDays);
		}

		[TestMethod]
		public void FromFridayToTuesdayMondayIsHolidaySaturdayIsWorking()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 04, 29), DayType.Working },
					{ new DateTime(2017, 05, 01), DayType.Holiday }
				});
			var fridayMorning = new DateTime(2017, 04, 28, 11, 0, 0);
			var tuesdayMorning = new DateTime(2017, 05, 02, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayMorning, tuesdayMorning);
			Assert.AreEqual(2, workingDays);
		}

		[TestMethod]
		public void FromFridayToMondaySaturdayIsHoliday()
		{
			var calendar = CreateCalendar(
				new Dictionary<DateTime, DayType>
				{
					{ new DateTime(2017, 04, 29), DayType.Holiday }
				});
			var fridayMorning = new DateTime(2017, 04, 28, 11, 0, 0);
			var mondayMorning = new DateTime(2017, 05, 01, 11, 0, 0);

			var workingDays = calendar.CountWorkingDaysInPeriod(fridayMorning, mondayMorning);
			Assert.AreEqual(1, workingDays);
		}

		[TestMethod]
		public void CountWorkingDaysInPeriod_ForRussia_IncludingTwoYears()
		{
			var calendar = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());
			var startDateTime = new DateTime(2018, 12, 28);
			var endDateTime = new DateTime(2019, 01, 10);

			var workingDays = calendar.CountWorkingDaysInPeriod(startDateTime, endDateTime);
			Assert.AreEqual(3, workingDays);
		}

		[DataRow(13, 0)]
		[DataRow(14, 0)]
		[DataRow(15, 1)]
		[DataRow(16, 2)]
		[DataRow(17, 3)]
		[DataRow(18, 4)]
		[DataRow(19, 5)]
		[DataRow(20, 5)]
		[DataRow(21, 5)]
		[DataRow(22, 6)]
		[DataRow(29, 11)]
		[DataTestMethod]
		public void CountWorkingDaysInPeriod_FromSaturday(int endDay, int expected)
		{
			var calendar = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());
			var startDateTime = new DateTime(2020, 12, 12);
			var endDateTime = new DateTime(2020, 12, endDay);

			var workingDays = calendar.CountWorkingDaysInPeriod(startDateTime, endDateTime);
			Assert.AreEqual(expected, workingDays);
		}

		[DataRow(14, 0)]
		[DataRow(15, 1)]
		[DataRow(16, 2)]
		[DataRow(17, 3)]
		[DataRow(18, 4)]
		[DataRow(19, 5)]
		[DataRow(20, 5)]
		[DataRow(21, 5)]
		[DataRow(22, 6)]
		[DataRow(29, 11)]
		[DataTestMethod]
		public void CountWorkingDaysInPeriod_FromSunday(int endDay, int expected)
		{
			var calendar = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());
			var startDateTime = new DateTime(2020, 12, 13);
			var endDateTime = new DateTime(2020, 12, endDay);

			var workingDays = calendar.CountWorkingDaysInPeriod(startDateTime, endDateTime);
			Assert.AreEqual(expected, workingDays);
		}

		[DataRow(15, 1)]
		[DataRow(16, 2)]
		[DataRow(17, 3)]
		[DataRow(18, 4)]
		[DataRow(19, 5)]
		[DataRow(20, 5)]
		[DataRow(21, 5)]
		[DataRow(22, 6)]
		[DataRow(23, 7)]
		[DataRow(29, 11)]
		[DataTestMethod]
		public void CountWorkingDaysInPeriod_FromMonday(int endDay, int expected)
		{
			var calendar = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());
			var startDateTime = new DateTime(2020, 12, 14);
			var endDateTime = new DateTime(2020, 12, endDay);

			var workingDays = calendar.CountWorkingDaysInPeriod(startDateTime, endDateTime);
			Assert.AreEqual(expected, workingDays);
		}

		[DataRow(16, 1)]
		[DataRow(17, 2)]
		[DataRow(18, 3)]
		[DataRow(19, 4)]
		[DataRow(20, 4)]
		[DataRow(21, 4)]
		[DataRow(22, 5)]
		[DataRow(23, 6)]
		[DataRow(24, 7)]
		[DataRow(30, 11)]
		[DataTestMethod]
		public void CountWorkingDaysInPeriod_FromTuesday(int endDay, int expected)
		{
			var calendar = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());
			var startDateTime = new DateTime(2020, 12, 15);
			var endDateTime = new DateTime(2020, 12, endDay);

			var workingDays = calendar.CountWorkingDaysInPeriod(startDateTime, endDateTime);
			Assert.AreEqual(expected, workingDays);
		}

		[DataRow(17, 1)]
		[DataRow(18, 2)]
		[DataRow(19, 3)]
		[DataRow(20, 3)]
		[DataRow(21, 3)]
		[DataRow(22, 4)]
		[DataRow(23, 5)]
		[DataRow(24, 6)]
		[DataRow(25, 7)]
		[DataRow(31, 11)]
		[DataTestMethod]
		public void CountWorkingDaysInPeriod_FromWednesday(int endDay, int expected)
		{
			var calendar = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());
			var startDateTime = new DateTime(2020, 12, 16);
			var endDateTime = new DateTime(2020, 12, endDay);

			var workingDays = calendar.CountWorkingDaysInPeriod(startDateTime, endDateTime);
			Assert.AreEqual(expected, workingDays);
		}

		[DataRow(18, 1)]
		[DataRow(19, 2)]
		[DataRow(20, 2)]
		[DataRow(21, 2)]
		[DataRow(22, 3)]
		[DataRow(23, 4)]
		[DataRow(24, 5)]
		[DataRow(25, 6)]
		[DataRow(26, 7)]
		[DataRow(31, 10)]
		[DataTestMethod]
		public void CountWorkingDaysInPeriod_FromThursday(int endDay, int expected)
		{
			var calendar = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());
			var startDateTime = new DateTime(2020, 12, 17);
			var endDateTime = new DateTime(2020, 12, endDay);

			var workingDays = calendar.CountWorkingDaysInPeriod(startDateTime, endDateTime);
			Assert.AreEqual(expected, workingDays);
		}

		[DataRow(19, 1)]
		[DataRow(20, 1)]
		[DataRow(21, 1)]
		[DataRow(22, 2)]
		[DataRow(23, 3)]
		[DataRow(24, 4)]
		[DataRow(25, 5)]
		[DataRow(26, 6)]
		[DataRow(27, 6)]
		[DataRow(31, 9)]
		[DataTestMethod]
		public void CountWorkingDaysInPeriod_FromFriday(int endDay, int expected)
		{
			var calendar = new WorkingCalendar(new RussianWorkingDaysExceptionsProvider());
			var startDateTime = new DateTime(2020, 12, 18);
			var endDateTime = new DateTime(2020, 12, endDay);

			var workingDays = calendar.CountWorkingDaysInPeriod(startDateTime, endDateTime);
			Assert.AreEqual(expected, workingDays);
		}
	}
}
