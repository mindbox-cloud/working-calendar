using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Xml.Linq;

namespace Mindbox.WorkingCalendar
{
	public static class WorkingDaysExceptions
	{
		public static Dictionary<DateTime, DayType> GetForRussia()
		{
			var exceptionsStringData = GetCalendarStringData();
			var exceptionsXml = XDocument.Parse(exceptionsStringData);
			var exceptionDayElements = exceptionsXml
				.Root
				.Element("days")
				.Elements("day");

			var year = int.Parse(
				exceptionsXml
					.Root
					.Attribute("year")
					.Value);

			var result = new Dictionary<DateTime, DayType>();
			foreach (var exceptionDayElement in exceptionDayElements)
			{
				var dateComponents = exceptionDayElement
					.Attribute("d")
					.Value
					.Split('.')
					.Select(int.Parse)
					.ToArray();

				var month = dateComponents[0];
				var day = dateComponents[1];
				DayType exceptionType;

				switch (exceptionDayElement.Attribute("t").Value)
				{
					case "1":
						exceptionType = DayType.Holiday;
						break;
					case "2":
						exceptionType = DayType.Short;
						break;
					default:
						throw new NotSupportedException();
				}

				result.Add(new DateTime(year, month, day), exceptionType);
			}

			return result;
		}

		private static string GetCalendarStringData()
		{
			var exceptions = new List<Exception>();
			for (var attempt = 0; attempt < 3; attempt++)
			{
				try
				{
					return new HttpClient()
						.GetStringAsync($"http://xmlcalendar.ru/data/ru/{DateTime.Now.Year}/calendar.xml")
						.Result;
				}
				catch (SocketException ex)
				{
					exceptions.Add(ex);
				}
			}
			throw new AggregateException(exceptions);
		}
	}
}