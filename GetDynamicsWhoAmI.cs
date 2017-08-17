using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.WebServiceClient;
using MSDYN365AdminApiAndMore.Helpers;
using System;
using System.Management.Automation;

namespace MSDYN365AdminApiAndMore
{
    [Cmdlet(VerbsCommon.Get, "DynamicsWhoAmI")]
    public class GetDynamicsWhoAmI : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Organization;
        [Parameter(Mandatory = true)]
        [ValidateSet("NorthAmerica", "SouthAmerica", "Canada", "EMEA", "APAC", "Oceania", "Japan", "India", "NorthAmerica2", "UnitedKingdom", IgnoreCase = true)]
        public string Location;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Enum.TryParse(Location, out DataCenterLocations tenantLocation);
            var customerEngagementUrl = UrlFactory.GetUrl(Organization, tenantLocation, "/XRMServices/2011/organization.svc/web");
            AuthenticationHelper customerEngagementAuth = null;
            if (SessionState.PSVariable.Get("customerengagementauth") != null)
            {
                customerEngagementAuth = SessionState.PSVariable.Get("customerengagementauth").Value as AuthenticationHelper;
            }
            else
            {
                var customerEngagementDiscovery = UrlFactory.GetDiscoveryUrl(customerEngagementUrl, ApiType.CustomerEngagement);
                customerEngagementAuth = new AuthenticationHelper(customerEngagementDiscovery);
            }
            var client = new OrganizationWebProxyClient(customerEngagementUrl, false)
            {
                HeaderToken = customerEngagementAuth.AuthResult.AccessToken,
                SdkClientVersion = "8.2"
            };
            var whoAmI = client.Execute(new WhoAmIRequest());
            foreach (var att in whoAmI.Results)
            {
                Console.WriteLine($"{att.Key}: {att.Value}");
            }
            Console.ReadLine();

            SessionState.PSVariable.Set("customerengagementauth", customerEngagementAuth);
        }
    }
}
