using System;
using System.IO;
using System.Net;
using System.Text;
using NLog;

namespace SonarWarnings
{
    public static class SonarQubeAuthentication
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static string GetResponseFromRequest(string url, string username, string password)
        {
            string responseInText = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "Get";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(string.Concat(username, ":", password))); ////username, ":", password

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    responseInText = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                responseInText = ex.Message;
                logger.Error(ex);
            }

            return responseInText;
        }

        public static string GetResponseFromRequest(string url)
        {
            string responseInText = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "Get";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("admin:Harbinger#2020"));

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    responseInText = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                responseInText = ex.Message;
                logger.Error(ex);
            }

            return responseInText;
        }
        public static Tuple<bool, string> ValidateUser(string url, string username, string password)
        {
            bool isUserAuthenticated = false;
            string responseMessage = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "Get";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(string.Concat(username, ":", password)));
                request.GetResponse();
                isUserAuthenticated = true;
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
                logger.Error(ex);
            }

            return new Tuple<bool, string>(isUserAuthenticated, responseMessage);
        }
    }
}