using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Mindbox.WorkingCalendar.Tests
{
	[TestClass]
	public class EmbeddedXmlCalendarProviderTests
	{
		private static EmbeddedXmlCalendarProvider GetTestXmlCalendarProvider()
			=> new EmbeddedXmlCalendarProvider(
				Assembly.GetExecutingAssembly(),
				"Mindbox.WorkingCalendar.Tests.Calendars");

		[TestMethod]
		public void GetWorkingDaysExceptionsData_FileExists_Success()
		{
			var provider = GetTestXmlCalendarProvider();

			var actualData = provider.GetCalendar(1961);

			var expectedData = new XDocument(
				new XDeclaration(version: "1.0", encoding: "UTF-8", standalone: "yes"),
				new XElement("calendar",
					new XAttribute("year", "1961"),
					new XAttribute("lang", "ru"),
					new XAttribute("date", "1961.01.01"),
					new XElement("holidays",
						new XElement("holiday",
							new XAttribute("id", "6"),
							new XAttribute("title", "Cosmonautics Day"))),
					new XElement("days",
						new XElement("day",
							new XAttribute("d", "01.01"),
							new XAttribute("t", "1"),
							new XAttribute("h", "1")))));

			Assert.AreEqual(expectedData.ToString(), actualData.ToString());
		}

		[TestMethod]
		public void GetWorkingDaysExceptionsData_FileDoesNotExist_Throws()
		{
			var provider = GetTestXmlCalendarProvider();

			try
			{
				provider.GetCalendar(1812);
			}
			catch (FileNotFoundException e)
			{
				Assert.AreEqual("Xml calendar for year 1812 was not found", e.Message);
				return;
			}

			Assert.Fail();
		}
	}
}