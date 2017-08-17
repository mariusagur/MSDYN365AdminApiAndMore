using MSDYN365AdminApiAndMore.Helpers;
using System;
using System.Management.Automation;
using System.Net.Http;

namespace MSDYN365AdminApiAndMore
{
    [Cmdlet(VerbsCommon.Get, "DynamicsInstanceTypeInfo")]
    public class GetDynamicsDynamicsInstanceTypeInfo : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        [ValidateSet("NorthAmerica", "SouthAmerica", "Canada", "EMEA", "APAC", "Oceania", "Japan", "India", "NorthAmerica2", "UnitedKingdom", IgnoreCase = true)]
        public string Location;

        private AuthenticationHelper _auth = null;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            Enum.TryParse(Location, out DataCenterLocations tenantLocation);
            var serverUrl = UrlFactory.GetUrl("admin.services", tenantLocation, "/api/v1/InstanceTypeInfo");

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
                Console.WriteLine(result);
                Console.ReadLine();
            }

            SessionState.PSVariable.Set("adminauth", _auth);
        }
    }
}
