using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MSDYN365AdminApiAndMore.Helpers
{
    public class AuthenticationHelper
    {
        private string _clientId = "2ad88395-b77d-4561-9441-d0e40824f9bc";
        private string _redirectUrl = "app://5d3e90d6-aa8e-48a8-8f2c-58b45cc67315/";

        private Uri _endpoint = null;
        private X509Certificate2 _certificate = null;
        private string _resource = null;
        private string _authority = null;
        private string _userName = null;
        private SecureString _password = null;
        private AuthenticationContext _authContext = null;
        private AuthenticationResult _authResult = null;

        public AuthenticationHelper(Uri endpoint, string userName = null, SecureString password = null)
        {
            _endpoint = endpoint;
            _userName = userName;
            _password = password;
        }

        public AuthenticationHelper(Uri endpoint, string clientId, X509Certificate2 certificate)
        {
            _endpoint = endpoint;
            _clientId = clientId;
            _certificate = certificate;
        }

        public string Authority
        {
            get
            {
                if (_authority == null)
                {
                    DiscoverAuthority(_endpoint);
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

        public HttpMessageHandler Handler
        {
            get
            {
                return new OAuthMessageHandler(this, new HttpClientHandler());
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
                    if (string.IsNullOrWhiteSpace(_userName) || _password == null)
                    {
                        _authResult = await AuthContext.AcquireTokenAsync(_resource, _clientId, new Uri(_redirectUrl),
                            new PlatformParameters(PromptBehavior.Always));
                    }
                    else if (_certificate != null)
                    {
                        var assertion = new ClientAssertionCertificate(_clientId, _certificate);
                        _authResult = await AuthContext.AcquireTokenAsync(_resource, assertion);
                    }
                    else
                    {
                        var creds = new UserPasswordCredential(_userName, _password);
                        _authResult = await AuthContext.AcquireTokenAsync(_resource, _clientId, creds);
                    }

                }).Wait();
            }
        }

        class OAuthMessageHandler : DelegatingHandler
        {
            AuthenticationHelper _auth = null;
            public OAuthMessageHandler(AuthenticationHelper auth, HttpMessageHandler innerHandler) : base(innerHandler)
            {
                _auth = auth;
            }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Version = HttpVersion.Version11;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.AuthResult.AccessToken);
                request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("NORWEGIAN-VIKING"));
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}