using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using Newtonsoft.Json;

namespace Application.Monitoring.Common
{
    public class AppDynamics
    {
        private AppDynamicsConfig _appDynamicsConfig;
        private HttpClient _httpClient;

        public AppDynamics(AppDynamicsConfig appDynamicsConfig)
        {
            _appDynamicsConfig = appDynamicsConfig;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", _appDynamicsConfig.apiUserName, _appDynamicsConfig.apiPassword))));
        }

        public DiskMetric GetNodeDiskMetrics(string environmentName, Node node)
        {
            IList<DiskMetric> resultlist;
            string path = $"https://wkapmusp01.saas.appdynamics.com/controller/rest/applications/{environmentName}/metric-data?metric-path=Application%20Infrastructure%20Performance%7C{node.tierName}%7CHardware%20Resources%7CDisks%7C%25%20Free&time-range-type=BEFORE_NOW&duration-in-mins=43800&output=JSON";
            HttpResponseMessage response = _httpClient.GetAsync(path).Result;
            using (HttpContent content = response.Content)
            {
                var responseBody = response.Content.ReadAsStringAsync().Result;
                resultlist = JsonConvert.DeserializeObject<List<DiskMetric>>(responseBody);
            }

            return resultlist.ToList().FirstOrDefault();
        }
        public IList<Node> GetNodesInCluster(string environmentName,  string clusterId)
        {
            IList<Node> resultlist;
            string path = $"https://wkapmusp01.saas.appdynamics.com/controller/rest/applications/{environmentName}/tiers/{clusterId}/nodes?output=JSON";
            HttpResponseMessage response = _httpClient.GetAsync(path).Result;
            using (HttpContent content = response.Content)
            {
                var responseBody = response.Content.ReadAsStringAsync().Result;
                resultlist = JsonConvert.DeserializeObject<List<Node>>(responseBody);
            }

            return resultlist.ToList();
        }

        public  IList<Cluster> GetClusters(string environmentName)
        {
            string path = $"https://wkapmusp01.saas.appdynamics.com/controller/rest/applications/{environmentName}/tiers?output=JSON";
            IList<Cluster> resultlist;
            HttpResponseMessage response = _httpClient.GetAsync(path).Result;
            using (HttpContent content = response.Content)
            {
                var responseBody = response.Content.ReadAsStringAsync().Result;
                resultlist = JsonConvert.DeserializeObject<List<Cluster>>(responseBody);

            }
            //dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody, typeof(object));
            //foreach (var VARIABLE in jsonResponse)
            //{
                
            //}
            return resultlist.ToList();
        }

        public  void GetXMLResponse()
        {
            string path = "https://wkapmusp01.saas.appdynamics.com/controller/rest/applications/SureTax-prod/tiers";
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", _appDynamicsConfig.apiUserName, _appDynamicsConfig.apiPassword))));
            HttpResponseMessage response = client.GetAsync(path).Result;
            string xml = response.Content.ReadAsStringAsync().Result;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var json = JsonConvert.SerializeXmlNode(doc);
            dynamic jsonResponse = JsonConvert.DeserializeObject(json);

            Console.WriteLine(json);
        }
    }
}
