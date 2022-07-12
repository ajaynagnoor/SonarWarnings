using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SonarWarnings
{
    public static class ProjectsReader
    {
        internal static List<Project> GetProjects(string url, string UserName, string Password)
        {
            List<Project> projects = new List<Project>();
            Project project = null;
            string response = SonarQubeAuthentication.GetResponseFromRequest(url, UserName, Password);
            int projectCounter = 0;

            JObject allProjects = (JObject)JsonConvert.DeserializeObject(response);
            List<JToken> tokens = allProjects.Children().ToList()[1].ToList()[0].Children().ToList();

            foreach (JToken token in tokens)
            {
                project = JsonConvert.DeserializeObject<Project>(Convert.ToString(token));
                project.Index = Convert.ToString(projectCounter);
                projects.Add(project);
                projectCounter++;
            }

            return projects.OrderBy(p => p.Name).ToList();
        }
    }
}