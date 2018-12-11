using System;
using System.Net;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using RestSharp;

namespace Mindbox.WorkingCalendar.Tests
{
	[TestClass]
	public class XmlCalendarClientTests
	{
		[TestMethod]
		public void GetWorkingDaysExceptionsData_BadRequest_ThrowsWithStackTrace()
		{
			var responseMock = new Mock<IRestResponse>();
			responseMock.Setup(r => r.StatusCode).Returns(HttpStatusCode.BadGateway);

			var clientMock = new Mock<IRestClient>();
			clientMock.Setup(c => c.Execute(It.IsAny<IRestRequest>())).Returns(responseMock.Object);

			try
			{
				new XmlCalendarClient(clientMock.Object)
					.GetWorkingDaysExceptionsData(
						year: 2018,
						maxRetryCount: 1);
			}
			catch (AggregateException aggregateException)
			{
				Assert.AreEqual(1, aggregateException.InnerExceptions.Count);

				var innerException = aggregateException.InnerException;
				Assert.IsNotNull(innerException);
				Assert.IsNotNull(innerException.StackTrace);

				Console.WriteLine(innerException.StackTrace);
				Assert.IsTrue(
					innerException.StackTrace.Contains(
						$"{nameof(XmlCalendarClient)}.{nameof(XmlCalendarClient.GetWorkingDaysExceptionsData)}"));
			}
		}
	}
}