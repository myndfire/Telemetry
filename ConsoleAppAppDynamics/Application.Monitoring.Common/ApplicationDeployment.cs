using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Monitoring.Common
{
    public class ApplicationDeployment
    {
        public ApplicationDeployment(string environment)
        {
            _environment = environment;
        }
        public string _environment { get; set; }
        public IList<Cluster> _clusters { get; set; }
    }
}
