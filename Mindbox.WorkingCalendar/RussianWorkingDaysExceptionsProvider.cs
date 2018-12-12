using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Mindbox.WorkingCalendar
{
	public class RussianWorkingDaysExceptionsProvider : IWorkingDaysExceptionsProvider
	{
		private readonly ConcurrentBag<int> loadedYears = new ConcurrentBag<int>();
		private readonly ConcurrentDictionary<DateTime, DayType> workingDaysExceptions = new ConcurrentDictionary<DateTime, DayType>();

		public bool TryGet(DateTime date, out DayType dayType)
		{
			LazyLoadForYear(date.Year);

			return workingDaysExceptions.TryGetValue(date, out dayType);
		}

		public IEnumerable<(DateTime Date, DayType DayType)> GetExceptionsInPeriod(DateTime startDateTime, DateTime endDateTime)
		{
			for (int year = startDateTime.Year; year <= endDateTime.Year; year++)
			{
				LazyLoadForYear(year);
			}

			return workingDaysExceptions
				.Where(pair => pair.Key >= startDateTime)
				.Where(pair => pair.Key < endDateTime)
				.Select(pair => (pair.Key, pair.Value));
		}

		private void LazyLoadForYear(int year)
		{
			if (!loadedYears.Contains(year))
			{
				foreach (var workingDayException in GetWorkingDaysExceptions(year))
				{
					workingDaysExceptions.TryAdd(workingDayException.Date, workingDayException.DayType);
				}
				loadedYears.Add(year);
			}
		}
		private IEnumerable<(DateTime Date, DayType DayType)> GetWorkingDaysExceptions(int year)
		{
			var exceptionsXml = GetXmlCalendar(year);
			var exceptionDayElements = exceptionsXml
				.Root
				.Element("days")
				.Elements("day");

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
				DayType dayType;

				switch (exceptionDayElement.Attribute("t").Value)
				{
					case "1":
						dayType = DayType.Holiday;
						break;
					case "2":
						dayType = DayType.Short;
						break;
					default:
						throw new NotSupportedException();
				}

				yield return (Date: new DateTime(year, month, day), DayType: dayType);
			}
		}

		private static XDocument GetXmlCalendar(int year)
		{
			var calendarFileName = $"Mindbox.WorkingCalendar.Calendars.{year}.xml";
			var assembly = Assembly.GetExecutingAssembly();

			if (assembly.GetManifestResourceNames().All(resourceName => resourceName != calendarFileName))
				throw new FileNotFoundException(
					$"Xml calendar for year {year} was not found");

			using (var stream = assembly.GetManifestResourceStream(calendarFileName))
			{
				return XDocument.Load(stream);
			}
		}
	}
}