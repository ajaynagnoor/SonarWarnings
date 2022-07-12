using System.Collections.Generic;

namespace SonarQube.BusinessLogic
{
    public class Metrics
	{
		public string Metric
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public List<Period> Periods
		{
			get;
			set;
		}
	}
}
