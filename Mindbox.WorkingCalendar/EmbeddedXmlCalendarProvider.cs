using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Mindbox.WorkingCalendar
{
	public class EmbeddedXmlCalendarProvider
	{
		private const string DefaultCalendarsLocation = "Mindbox.WorkingCalendar.Calendars";

		private readonly Assembly effectiveAssembly;

		private readonly string calendarsLocation;

		public EmbeddedXmlCalendarProvider() : this(Assembly.GetExecutingAssembly(), DefaultCalendarsLocation)
		{
		}

		public EmbeddedXmlCalendarProvider(Assembly effectiveAssembly, string calendarsLocation)
		{
			if (string.IsNullOrWhiteSpace(calendarsLocation))
				throw new ArgumentNullException(nameof(calendarsLocation));

			this.calendarsLocation = calendarsLocation;
			this.effectiveAssembly = effectiveAssembly ?? throw new ArgumentNullException(nameof(effectiveAssembly));
		}

		public XDocument GetCalendar(int year)
		{
			var calendarFileName = $"{calendarsLocation}.{year}.xml";

			if (effectiveAssembly.GetManifestResourceNames().All(resourceName => resourceName != calendarFileName))
				throw new FileNotFoundException(
					$"Xml calendar for year {year} was not found");

			using (var stream = effectiveAssembly.GetManifestResourceStream(calendarFileName))
			{
				return XDocument.Load(stream);
			}
		}
	}
}