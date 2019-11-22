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
		public void IsInRange()
		{
			var range = new DateRange(day19, day21);

			Assert.IsTrue(range.IsInRange(day19));
			Assert.IsTrue(range.IsInRange(day20));
			Assert.IsTrue(range.IsInRange(day21));
		}
		
		
		[TestMethod]
		public void IsNotInRange()
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
		
		[TestMethod]
		public void IntersectingInRange()
		{
			var first = new DateRange(day19, day21);
			var second = new DateRange(day20, day22);
			var expected = new DateRange(day20, day21);

			var result = first.Intersect(second);
			var resultCommutativity = second.Intersect(first);

			Assert.AreEqual(resultCommutativity, result);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void IntersectingOverlap()
		{
			var first = new DateRange(day19, day22);
			var second = new DateRange(day20, day21);
			var expected = new DateRange(day20, day21);

			var result = first.Intersect(second);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void IntersectingInOneDate()
		{
			var first = new DateRange(day19, day20);
			var second = new DateRange(day20, day21);
			var expected = new DateRange(day20, day20);
			var result = first.Intersect(second);

			Assert.AreEqual(expected, result);
		}
		
		[TestMethod]
		public void IntersectingSameRange()
		{
			var first = new DateRange(day19, day20);
			var second = new DateRange(day19, day20);
			var expected = new DateRange(day19, day20);
			var result = first.Intersect(second);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void EqualOperatorTest()
		{
			var first = new DateRange(day19, day21);
			var second = new DateRange(day19, day21);
			DateRange nullable = null;

			Assert.IsTrue(first == first);
			Assert.IsTrue(first == second);
			Assert.IsTrue(second == first);
			Assert.IsTrue(nullable == null);
			Assert.IsTrue(null == nullable);
		}
		
		[TestMethod]
		public void NotEqualOperatorTest()
		{
			var first = new DateRange(day20, day21);
			var second = new DateRange(day19, day21);
			
			Assert.IsTrue(first != second);
			Assert.IsTrue(second != first);
			Assert.IsTrue(first != null);
			Assert.IsTrue(null != first);
		}
	}
}