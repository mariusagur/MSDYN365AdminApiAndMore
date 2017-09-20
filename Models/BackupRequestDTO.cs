using System;

namespace MSDYN365AdminApiAndMore.Models
{
    public class BackupRequestDTO
    {
        public Guid InstanceId { get; set; }
        public string Label { get; set; }
        public string Notes { get; set; }
        public bool IsAzureBackup { get; set; }
        public AzureStorage AzureStorageInformation { get; set; }
        public class AzureStorage
        {
            public string ContainerName { get; set; }
            public string StorageAccountKey { get; set; }
            public string StorageAccountName { get; set; }
        }

    }
}
