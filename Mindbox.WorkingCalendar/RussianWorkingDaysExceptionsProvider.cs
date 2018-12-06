﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Xml.Linq;

using RestSharp;

namespace Mindbox.WorkingCalendar
{
	public class RussianWorkingDaysExceptionsProvider : IWorkingDaysExceptionsProvider
	{
		private const int MaxRequestsAttempts = 3;

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
			var exceptionsStringData = GetWorkingDaysExceptionsData(year);
			var exceptionsXml = XDocument.Parse(exceptionsStringData);
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

		private string GetWorkingDaysExceptionsData(int year)
		{
			var exceptions = new List<Exception>();
			for (var attempt = 0; attempt < MaxRequestsAttempts; attempt++)
			{
				var response = new RestClient("http://xmlcalendar.ru").Get(new RestRequest($"data/ru/{year}/calendar.xml"));

				if (response.IsSuccessful)
				{
					return response.Content;
				}
				else if (response.ErrorException != null)
				{
					exceptions.Add(response.ErrorException);
				}
				else
				{
					exceptions.Add(new WebException(
						$"Request to \"{response.ResponseUri}\" " +
						$"returned status {(int)response.StatusCode} [{response.StatusDescription}]"));
				}
			}
			throw new AggregateException(exceptions);
		}
	}
}