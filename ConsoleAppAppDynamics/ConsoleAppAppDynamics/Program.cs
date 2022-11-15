using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using Application.Monitoring.Common;
using Newtonsoft.Json;


namespace ConsoleAppAppDynamics
{
    class Program
    {
        private static string apiUsername = "Manoj.Patil@wkapmusp01";
        private static string apiPassword = "Welcome1";
        static async Task Main(string[] args)
        {
            AppDynamics _appDynamics = new AppDynamics(new AppDynamicsConfig());
            string _environmentName = "SureTax-prod";

            //var r = GetJsonResponse();
            var clusters = _appDynamics.GetClusters(_environmentName);
            foreach (var cluster in clusters)
            {
                //Console.WriteLine($"CLUSTER: {cluster.ToString()}");
                if (!cluster.numberOfNodes.Equals("0"))
                {
                    IList<Node> n = _appDynamics.GetNodesInCluster(_environmentName, clusterId: cluster.id);
                    foreach (var node in n)
                    {
                        //Console.WriteLine($"\tNODE: {node.ToString()}");
                        DiskMetric diskmetric = _appDynamics.GetNodeDiskMetrics(_environmentName,node);
                        Console.WriteLine($"\tDISK USAGE: {diskmetric.ToString()}");
                    }
                }

            }
        }


    }
}
