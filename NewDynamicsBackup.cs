using MSDYN365AdminApiAndMore.Helpers;
using MSDYN365AdminApiAndMore.Models;
using Newtonsoft.Json;
using System.Management.Automation;
using System.Net.Http;
using System.Text;
using static MSDYN365AdminApiAndMore.Models.BackupRequestDTO;

namespace MSDYN365AdminApiAndMore
{
    [Cmdlet(VerbsCommon.New, "DynamicsBackup", DefaultParameterSetName = "Default")]
    public class NewDynamicsBackup : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public InstanceDTO Instance;
        [Parameter(Mandatory = true)]
        public string Description;
        [Parameter(Mandatory = true, ParameterSetName = _azureBackupName)]
        public string ContainerName;
        [Parameter(Mandatory = true, ParameterSetName = _azureBackupName)]
        public string StorageAccountKey;
        [Parameter(Mandatory = true, ParameterSetName = _azureBackupName)]
        public string StorageAccountName;

        private const string _azureBackupName = "AzureBackup";
        private AuthenticationHelper _auth = null;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();


            _auth = SessionState.PSVariable.Get("adminauth").Value as AuthenticationHelper;
            var serverUrl = SessionState.PSVariable.Get("serverurl").Value as string;
            serverUrl = serverUrl + "/api/v1/InstanceBackups";

            using (var httpClient = new HttpClient(_auth.Handler))
            {
                var req = new BackupRequestDTO
                {
                    InstanceId = Instance.Id,
                    Notes = Description
                };
                if (ParameterSetName == _azureBackupName)
                {
                    req.AzureStorageInformation = new AzureStorage
                    {
                        ContainerName = "YourContainerName",
                        StorageAccountKey = "YourStorageAccountKey",
                        StorageAccountName = "YourStorageAccountName"
                    };
                    req.IsAzureBackup = true;
                }

                req.Label = Instance.UniqueName;
                
                var result = httpClient.PostAsync(serverUrl, new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json"));
                var response = result.Result;
            }

            SessionState.PSVariable.Set("adminauth", _auth);
        }
    }
}
