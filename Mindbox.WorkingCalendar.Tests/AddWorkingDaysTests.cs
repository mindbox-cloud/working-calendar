using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mindbox.WorkingCalendar.Tests
{
	[TestClass]
	public class AddWorkingDaysTests
	{
		[TestMethod]
		public void AddWorkingDays_Add1Day_NextDayIsWorkday()
		{
			var today = new DateTime(2018, 01, 01);

			var workingCalendar = new WorkingCalendar(new Dictionary<DateTime, DayType>());
			var nextWorkingDay = workingCalendar.AddWorkingDays(today, 1);

			Assert.AreEqual(new DateTime(2018, 01, 02), nextWorkingDay);
		}

		[TestMethod]
		public void AddWorkingDays_Add1Day_NextDayIsHoliday()
		{
			var today = new DateTime(2018, 01, 01);

			var workingCalendar = new WorkingCalendar(new Dictionary<DateTime, DayType>
			{
				{ new DateTime(2018, 01, 02), DayType.Holiday }
			});
			var nextWorkingDay = workingCalendar.AddWorkingDays(today, 1);

			Assert.AreEqual(new DateTime(2018, 01, 03), nextWorkingDay);
		}

		[TestMethod]
		public void AddWorkingDays_Subtract1Day_PreviosDayIsWorkday()
		{
			var today = new DateTime(2018, 01, 03);

			var workingCalendar = new WorkingCalendar(new Dictionary<DateTime, DayType>());
			var previousWorkingDay = workingCalendar.AddWorkingDays(today, -1);

			Assert.AreEqual(new DateTime(2018, 01, 02), previousWorkingDay);
		}

		[TestMethod]
		public void AddWorkingDays_Subtract1Day_PreviosDayIsHoliday()
		{
			var today = new DateTime(2018, 01, 03);

			var workingCalendar = new WorkingCalendar(new Dictionary<DateTime, DayType>
			{
				{ new DateTime(2018, 01, 02), DayType.Holiday }
			});
			var previousWorkingDay = workingCalendar.AddWorkingDays(today, -1);

			Assert.AreEqual(new DateTime(2018, 01, 01), previousWorkingDay);
		}
	}
}