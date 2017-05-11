using System;
using System.Collections.Generic;
using System.Globalization;

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
	}
}
