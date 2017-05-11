using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindbox.WorkingCalendar
{
	public static class WorkingDaysExceptions
	{
		public static Dictionary<DateTime, DayType> GetForRussia()
		{
			var exceptionsStringData = new HttpClient().GetStringAsync("http://basicdata.ru/api/json/calend/").Result;
			var exceptionsJsonData = (JObject) JsonConvert.DeserializeObject<JObject>(exceptionsStringData)["data"];

			var result = new Dictionary<DateTime, DayType>();
			foreach (var yearExceptionProperty in exceptionsJsonData.Properties())
			{
				var year = int.Parse(yearExceptionProperty.Name);
				var yearExceptionsData = (JObject) yearExceptionProperty.Value;

				foreach (var monthExceptionsProperty in yearExceptionsData.Properties())
				{
					var month = int.Parse(monthExceptionsProperty.Name);
					var monthExceptionsData = (JObject) monthExceptionsProperty.Value;

					foreach (var dayExceptionProperty in monthExceptionsData.Properties())
					{
						var day = int.Parse(dayExceptionProperty.Name);
						var exceptionType = (DayType) (int) dayExceptionProperty.Value["isWorking"];

						result.Add(new DateTime(year, month, day), exceptionType);
					}
				}
			}

			return result;
		}
	}
}