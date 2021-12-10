using Azure.Storage.Blobs;

namespace FileLoader.Services;

public interface IBlogStorage
{
    Task UploadFile(string name, byte[] data);
    Task<List<string>> GetFileList();
    Task<byte[]> GetFile(string name);
    Task DeleteFile(string name);
}

public class BlogStorage : IBlogStorage
{
    private const string connectionString =
        "DefaultEndpointsProtocol=https;AccountName=thebakwaistorage;AccountKey=RlTknnhH0dAw/9eNCHJOVsuqpJOQFw/dhjKkoOkPheKlNrfgA3TxZ3sm4cbUVMrll/N2EocHvhmZTbBwpWVh4g==;EndpointSuffix=core.windows.net";

    private const string containerName = "testingblobs";

    public BlogStorage()
    {
    }

    private async Task<BlobContainerClient> GetContainer()
    {
        var serviceClient = new BlobServiceClient(connectionString);

        var blobClient = serviceClient.GetBlobContainerClient(containerName);

        await blobClient.CreateIfNotExistsAsync();
        return blobClient;
    }

    public async Task UploadFile(string name, byte[] data)
    {
        var client = await GetContainer();
        await using var ms = new MemoryStream(data);
        var response = await client.UploadBlobAsync(name, ms);
    }

    public async Task<List<string>> GetFileList()
    {
        var client = await GetContainer();
        var list = new List<string>();
        await foreach (var blob in client.GetBlobsAsync())
        {
            list.Add(blob.Name);
        }

        return list;
    }

    public async Task<byte[]> GetFile(string name)
    {
        var client = await GetContainer();
        var blobClient = client.GetBlobClient(name);
        await using var ms = new MemoryStream();
        await blobClient.DownloadToAsync(ms);
        return ms.ToArray();
    }

    public async Task DeleteFile(string name)
    {
        var client = await GetContainer();
        await client.DeleteBlobAsync(name);
    }
}