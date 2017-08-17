using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDYN365AdminApiAndMore.Helpers
{
    public static class UrlFactory
    {
        public static string BaseUrl = "https://{0}.crm{1}.dynamics.com{2}";
        public static Uri GetUrl(string subdomain, DataCenterLocations location, string resource = "")
        {
            if (location == DataCenterLocations.NorthAmerica)
            {
                return new Uri(
                    string.Format(BaseUrl, subdomain, "", resource)
                    );
            }
            else
            {
                return new Uri(
                    string.Format(BaseUrl, subdomain, (int)location, resource)
                    );
            }
        }
    }

    public enum DataCenterLocations
    {
        NorthAmerica = 1,
        SouthAmerica = 2,
        Canada = 3,
        EMEA = 4,
        APAC = 5,
        Oceania = 6,
        Japan = 7,
        India = 8,
        NorthAmerica2 = 9,
        UnitedKingdom = 11
    }
}
