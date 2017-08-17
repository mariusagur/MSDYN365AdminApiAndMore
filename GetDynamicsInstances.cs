using MSDYN365AdminApiAndMore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MSDYN365AdminApiAndMore
{
    [Cmdlet(VerbsCommon.Get, "DynamicsInstances")]
    public class GetDynamicsInstances : Cmdlet
    {
        public string InstanceName;
        [Parameter(Mandatory = true)]
        [ValidateSet("NorthAmerica", "SouthAmerica", "Canada", "EMEA", "APAC", "Oceania", "Japan", "India", "NorthAmerica2", "UnitedKingdom", IgnoreCase = true)]
        public string Location;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Enum.TryParse(Location, out DataCenterLocations tenantLocation);
            var serverUrl = UrlFactory.GetUrl("admin.services", tenantLocation, "/api/v1/instances");
            var auth = new AuthenticationHelper(serverUrl);
        }
    }
}
