using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.AzureQueueStorage;

internal static class Constants
{
    private const string AzureStorageAccountName = "STORAGE-HERE";
    private const string AzureStorageAccountKey = "KEY-HERE";

    public const string AzureQueueStorageKey = $"DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName={AzureStorageAccountName};AccountKey={AzureStorageAccountKey}";
}
