using MSDYN365AdminApiAndMore.Helpers;
using MSDYN365AdminApiAndMore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net.Http;

namespace MSDYN365AdminApiAndMore
{
    [Cmdlet(VerbsCommon.Get, "DynamicsInstances")]
    public class GetDynamicsInstances : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        [ValidateSet("NorthAmerica", "SouthAmerica", "Canada", "EMEA", "APAC", "Oceania", "Japan", "India", "NorthAmerica2", "UnitedKingdom", IgnoreCase = true)]
        public string Location;
        [Parameter]
        public string UniqueName;

        private AuthenticationHelper _auth = null;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            Enum.TryParse(Location, out DataCenterLocations tenantLocation);
            var serverUrl = UrlFactory.GetUrl("admin.services", tenantLocation, "/api/v1/instances");

            if (SessionState.PSVariable.Get("adminauth") != null)
            {
                _auth = SessionState.PSVariable.Get("adminauth").Value as AuthenticationHelper;
            }
            else
            {
                var discoveryUrl = UrlFactory.GetDiscoveryUrl(serverUrl, ApiType.Admin);
                _auth = new AuthenticationHelper(discoveryUrl);
            }

            using (var httpClient = new HttpClient(_auth.Handler))
            {
                var result = httpClient.GetStringAsync(serverUrl).Result;
                var instances = JsonConvert.DeserializeObject<List<InstanceDTO>>(result);
                SessionState.PSVariable.Set("serverurl", serverUrl.GetLeftPart(UriPartial.Authority));
                SessionState.PSVariable.Set("adminauth", _auth);
                
                if (string.IsNullOrWhiteSpace(UniqueName))
                {
                    WriteObject(instances);
                }
                else
                {
                    var instance = instances.First(i => i.UniqueName.Equals(UniqueName, StringComparison.InvariantCultureIgnoreCase));
                    WriteObject(instance);
                }
            }
        }
    }
}
