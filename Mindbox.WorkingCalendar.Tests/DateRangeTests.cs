using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mindbox.WorkingCalendar.Tests
{
	[TestClass]
	public class DateRangeTests
	{
		private static readonly DateTime day19 = new DateTime(2019, 11, 19);

		private static readonly DateTime day20 = new DateTime(2019, 11, 20);

		private static readonly DateTime day21 = new DateTime(2019, 11, 21);

		private static readonly DateTime day22 = new DateTime(2019, 11, 22);

		[TestMethod]
		public void IsInRage()
		{
			var range = new DateRange(day19, day21);

			Assert.IsTrue(range.IsInRange(day19));
			Assert.IsTrue(range.IsInRange(day20));
			Assert.IsTrue(range.IsInRange(day21));
		}
		
		
		[TestMethod]
		public void IsNotInRage()
		{
			var range = new DateRange(day20, day21);

			Assert.IsFalse(range.IsInRange(day22));
			Assert.IsFalse(range.IsInRange(day19));
		}
		
		[TestMethod]
		public void DateRangeEqual()
		{
			var first = new DateRange(day19, day21);
			var second = new DateRange(day19, day21);

			Assert.AreEqual(first, second);
			Assert.AreNotEqual(first, null);
		}
	}
}