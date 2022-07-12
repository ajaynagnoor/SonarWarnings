using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SonarQube.DataAccess;

namespace SonarQube.BusinessLogic
{
    public class ActivityBL
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public bool AddStatistics(List<Tuple<string, int>> warningsCount, List<Tuple<string, int>> warningsCountAsonAnalysisDate, string response, string componentInfo, string projectName)
        {
            bool result = false;
            try
            {
                DataTable metricswithvalues = CreateMetricsDataTable(GetMetrics(response));
                DateTime analysisDate = GetAnalysisDate(componentInfo);
                DataTable dataTable = CreateDataTable();

                DataRow dataRow = dataTable.NewRow();
                dataRow["ScanDate"] = analysisDate;
                dataRow["Blocker"] = warningsCount[0].Item2;
                dataRow["Critical"] = warningsCount[1].Item2;
                dataRow["Major"] = warningsCount[2].Item2;
                dataRow["Minor"] = warningsCount[3].Item2;
                dataRow["Info"] = warningsCount[4].Item2;
                dataRow["ServiceLeakPeriod"] = 0;
                dataRow["Rating"] = "Z";
                dataRow["ProjectName"] = projectName;
                dataRow["WarningType"] = "CodeSmell";
                dataTable.Rows.Add(dataRow);
                dataRow = dataTable.NewRow();
                dataRow["ScanDate"] = analysisDate;
                dataRow["Blocker"] = warningsCountAsonAnalysisDate[0].Item2;
                dataRow["Critical"] = warningsCountAsonAnalysisDate[1].Item2;
                dataRow["Major"] = warningsCountAsonAnalysisDate[2].Item2;
                dataRow["Minor"] = warningsCountAsonAnalysisDate[3].Item2;
                dataRow["Info"] = warningsCountAsonAnalysisDate[4].Item2;
                dataRow["ServiceLeakPeriod"] = 1;
                dataRow["Rating"] = "Z";
                dataRow["ProjectName"] = projectName;
                dataRow["WarningType"] = "CodeSmell";
                dataTable.Rows.Add(dataRow);
                dataRow = dataTable.NewRow();
                dataRow["ScanDate"] = analysisDate;
                dataRow["Blocker"] = warningsCount[5].Item2;
                dataRow["Critical"] = warningsCount[6].Item2;
                dataRow["Major"] = warningsCount[7].Item2;
                dataRow["Minor"] = warningsCount[8].Item2;
                dataRow["Info"] = warningsCount[9].Item2;
                dataRow["ServiceLeakPeriod"] = 0;
                dataRow["Rating"] = "Z";
                dataRow["ProjectName"] = projectName;
                dataRow["WarningType"] = "Bugs";
                dataTable.Rows.Add(dataRow);
                dataRow = dataTable.NewRow();
                dataRow["ScanDate"] = analysisDate;
                dataRow["Blocker"] = warningsCountAsonAnalysisDate[5].Item2;
                dataRow["Critical"] = warningsCountAsonAnalysisDate[6].Item2;
                dataRow["Major"] = warningsCountAsonAnalysisDate[7].Item2;
                dataRow["Minor"] = warningsCountAsonAnalysisDate[8].Item2;
                dataRow["Info"] = warningsCountAsonAnalysisDate[9].Item2;
                dataRow["ServiceLeakPeriod"] = 1;
                dataRow["Rating"] = "Z";
                dataRow["ProjectName"] = projectName;
                dataRow["WarningType"] = "Bugs";
                dataTable.Rows.Add(dataRow);
                dataRow = dataTable.NewRow();
                dataRow["ScanDate"] = analysisDate;
                dataRow["Blocker"] = warningsCount[10].Item2;
                dataRow["Critical"] = warningsCount[11].Item2;
                dataRow["Major"] = warningsCount[12].Item2;
                dataRow["Minor"] = warningsCount[13].Item2;
                dataRow["Info"] = warningsCount[14].Item2;
                dataRow["ServiceLeakPeriod"] = 0;
                dataRow["Rating"] = "Z";
                dataRow["ProjectName"] = projectName;
                dataRow["WarningType"] = "Vulnerability";
                dataTable.Rows.Add(dataRow);
                dataRow = dataTable.NewRow();
                dataRow["ScanDate"] = analysisDate;
                dataRow["Blocker"] = warningsCountAsonAnalysisDate[10].Item2;
                dataRow["Critical"] = warningsCountAsonAnalysisDate[11].Item2;
                dataRow["Major"] = warningsCountAsonAnalysisDate[12].Item2;
                dataRow["Minor"] = warningsCountAsonAnalysisDate[13].Item2;
                dataRow["Info"] = warningsCountAsonAnalysisDate[14].Item2;
                dataRow["ServiceLeakPeriod"] = 1;
                dataRow["Rating"] = "Z";
                dataRow["ProjectName"] = projectName;
                dataRow["WarningType"] = "Vulnerability";
                dataTable.Rows.Add(dataRow);
                DataTable dataTable2 = CreateSecurityHotspotTable();
                dataRow = dataTable2.NewRow();
                dataRow["ScanDate"] = analysisDate;
                dataRow["Total"] = warningsCount[15].Item2;
                dataRow["ServiceLeakPeriod"] = 0;
                dataRow["ProjectName"] = projectName;
                dataTable2.Rows.Add(dataRow);
                dataRow = dataTable2.NewRow();
                dataRow["ScanDate"] = analysisDate;
                dataRow["Total"] = warningsCountAsonAnalysisDate[15].Item2;
                dataRow["ServiceLeakPeriod"] = 1;
                dataRow["ProjectName"] = projectName;
                dataTable2.Rows.Add(dataRow);
                result = ActivityDA.AddStatistics(dataTable, dataTable2, metricswithvalues);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error<Exception>(ex);
                return result;
            }
        }

        private DataTable CreateMetricsDataTable(List<Metrics> metrics)
        {
            DataTable dataTable = new DataTable("metrics");
            dataTable.Columns.Add("Metric");
            dataTable.Columns.Add("Value");
            foreach (Metrics metric in metrics)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["Metric"] = metric.Metric;
                dataRow["Value"] = ((metric.Periods == null) ? metric.Value : metric.Periods[0].Value);
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        private DateTime GetAnalysisDate(string componentInfo)
        {
            JToken token = ((JObject)JsonConvert.DeserializeObject(componentInfo)).Last.Children().ToList()[0].Children().Last();

            JToken jt = (from r in token.Children()
                         where ((JProperty)r).Name.Equals("lastAnalysisDate")
                         select r).Children().ToList()[0];

            return Convert.ToDateTime(((JValue)jt).Value);
        }

        private DataTable CreateSecurityHotspotTable()
        {
            return new DataTable("warnings")
            {
                Columns =
            {
                "ScanDate",
                "Total",
                "ServiceLeakPeriod",
                "ProjectName"
            }
            };
        }

        private DataTable CreateDataTable()
        {
            return new DataTable("warnings")
            {
                Columns =
            {
                "ScanDate",
                "Critical",
                "Blocker",
                "Major",
                "Minor",
                "Info",
                "ServiceLeakPeriod",
                "Rating",
                "ProjectName",
                "WarningType"
            }
            };
        }

        private static List<Metrics> GetMetrics(string resp)
        {
            List<JToken> list = ((JObject)JsonConvert.DeserializeObject(resp)).Children().ToList()[0].ToList()[0].Children().ToList();
            List<Metrics> list2 = new List<Metrics>();
            foreach (JToken item in list[list.Count - 1].Children().Children().ToList())
            {
                list2.Add(JsonConvert.DeserializeObject<Metrics>(Convert.ToString(item)));
            }
            return list2;
        }
    }
}
