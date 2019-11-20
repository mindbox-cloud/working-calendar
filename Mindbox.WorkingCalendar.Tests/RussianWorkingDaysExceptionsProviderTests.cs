using System;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mindbox.WorkingCalendar.Tests
{
	[TestClass]
	public class RussianWorkingDaysExceptionsProviderTests
	{
		[TestMethod]
		public void GetSupportedDateRangeTest()
		{
			var provider = new RussianWorkingDaysExceptionsProvider();
			var years = new[] { 2019, 2020 };
			var expectedStart = new DateTime(2019, 1, 1);
			var expectedEnd = new DateTime(2020, 12, 31);

			var actualRange = provider.GetSupportedDateRange(years);
			Assert.AreEqual(expectedStart, actualRange.Start);
			Assert.AreEqual(expectedEnd, actualRange.End);
		}
	}
}