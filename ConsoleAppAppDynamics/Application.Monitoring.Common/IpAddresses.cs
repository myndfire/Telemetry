using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Application.Monitoring.Common
{
    public class IpAddresses
    {
        [JsonProperty("ipAddresses")]
        public IList<string> ipAddresses { get; set; }

        public override string ToString()
        {
            return $"{nameof(ipAddresses)}: {String.Join(",", ipAddresses)}";
        }
    }
}