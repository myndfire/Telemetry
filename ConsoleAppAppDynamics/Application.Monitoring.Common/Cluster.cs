using System.Collections.Generic;

namespace Application.Monitoring.Common
{
    public class Cluster
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string numberOfNodes { get; set; }

        public override string ToString()
        {
            return $"{nameof(id)}: {id}, {nameof(name)}: {name}, {nameof(type)}: {type}, {nameof(description)}: {description}, {nameof(numberOfNodes)}: {numberOfNodes}";
        }
    }
}