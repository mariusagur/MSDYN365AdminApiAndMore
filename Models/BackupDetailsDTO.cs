using System;

namespace MSDYN365AdminApiAndMore.Models
{
    public class BackupDetailsDTO
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string Version { get; set; }
        public string Notes { get; set; }
        public string Label { get; set; }
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
