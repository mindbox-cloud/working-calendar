using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

using RestSharp;

namespace Mindbox.WorkingCalendar
{
	public class XmlCalendarClient
	{
		private readonly IRestClient restClient;

		public XmlCalendarClient(IRestClient restClient)
		{
			this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
		}

		public XmlCalendarClient(): this(new RestClient("http://xmlcalendar.ru"))
		{
		}

		public string GetWorkingDaysExceptionsData(int year, int maxRetryCount = 3)
		{
			var exceptions = new List<Exception>();
			for (var attempt = 0; attempt < maxRetryCount; attempt++)
			{
				var response = restClient.Get(new RestRequest($"data/ru/{year}/calendar.xml"));

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
					exceptions.Add(new XmlCalendarException(
						$"Request to \"{response.ResponseUri}\" " +
						$"returned status {(int)response.StatusCode} [{response.StatusDescription}]"));
				}
			}
			throw new AggregateException(exceptions);

		}

		private class XmlCalendarException : WebException
		{
			private readonly StackTrace manuallyCapturedStackTrace;

			public override string StackTrace => manuallyCapturedStackTrace.ToString();

			public XmlCalendarException(string message) : base(message)
			{
				manuallyCapturedStackTrace = new StackTrace(skipFrames: 1, fNeedFileInfo: true);
			}
		}

	}
}