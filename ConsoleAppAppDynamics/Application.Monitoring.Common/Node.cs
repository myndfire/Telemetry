using System;
using System.Text;

namespace Application.Monitoring.Common
{
    /*
     *   {
    "appAgentVersion": "4.4.0.0",
    "machineAgentVersion": "4.4.0.0",
    "agentType": "DOT_NET_APP_AGENT",
    "type": "Other",
    "machineName": "PSTXWB129",
    "appAgentPresent": true,
    "nodeUniqueLocalId": "",
    "machineId": 1285561,
    "machineOSType": "Windows",
    "tierId": 4829,
    "tierName": "azureapi.sureaddress.net",
    "machineAgentPresent": true,
    "name": "PSTXWB129-azureapi.sureaddress.net",
    "ipAddresses": {
      "ipAddresses": [
        "10.232.209.179"
      ]
    },
    "id": 43144
  }
  */

    public class Node
    {
        public string id { get; set; }
        public string type { get; set; }
        public string machineName { get; set; }
        public string machineId { get; set; }
        private string machineOSType { get; set; }
        public string tierId { get; set; }
        public string tierName { get; set; }
        public string name { get; set; }
        public IpAddresses ipAddresses { get; set; }

        public override string ToString()
        {
            return $"{nameof(id)}: {id}, {nameof(type)}: {type}, {nameof(machineName)}: {machineName}, {nameof(machineId)}: {machineId}, {nameof(machineOSType)}: {machineOSType}, {nameof(tierId)}: {tierId}, {nameof(tierName)}: {tierName}, {nameof(name)}: {name}, {nameof(ipAddresses)}: {ipAddresses}";
        }
    }
}
