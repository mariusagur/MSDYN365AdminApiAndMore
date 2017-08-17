using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MSDYN365AdminApiAndMore.Helpers
{
    public class AuthenticationHelper
    {
        private static string _clientId = "b954ae2b-8130-4b0e-a45a-d91ef9faec59";
        private static string _redirectUrl = "urn:ietf:wg:oauth:2.0:oob";

        private string _endpoint = null;
        private string _resource = null;
        private string _authority = null;
        private AuthenticationContext _authContext = null;
        private AuthenticationResult _authResult = null;

        public AuthenticationHelper(Uri endpoint)
        {
            _endpoint = endpoint.GetLeftPart(UriPartial.Authority);
        }

        public string Authority
        {
            get
            {
                if (_authority == null)
                {
                    DiscoverAuthority(new Uri(_endpoint + "/api/aad/challenge"));
                }
                return _authority;
            }
        }

        public AuthenticationContext AuthContext
        {
            get
            {
                if (_authContext == null)
                {
                    _authContext = new AuthenticationContext(Authority, false);
                }
                return _authContext;
            }
        }

        public AuthenticationResult AuthResult
        {
            get
            {
                Authorize();
                return _authResult;
            }
        }

        private void DiscoverAuthority(Uri discoveryUrl)
        {
            try
            {
                Task.Run(async () =>
                {
                    var ap = await AuthenticationParameters.CreateFromResourceUrlAsync(discoveryUrl);
                    _resource = ap.Resource;
                    _authority = ap.Authority;
                }).Wait();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void Authorize()
        {
            if (_authResult == null || _authResult.ExpiresOn.AddMinutes(-30) < DateTime.Now)
            {
                Task.Run(async () =>
                {
                    _authResult = await AuthContext.AcquireTokenAsync(_resource, _clientId, new Uri(_redirectUrl),
                    new PlatformParameters(PromptBehavior.Always));
                }).Wait();
            }
        }
    }
}