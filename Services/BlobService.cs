namespace Caketime.Services;

public interface IBlobService
{
    Task CreateBlob(string filename, string filepath);
}

public class BlobService : IBlobService
{
    private readonly BlobSettings _settings;
    private readonly BlobServiceClient _client;
    private readonly BlobContainerClient _container;

    public BlobService(IOptions<BlobSettings> blobOptions)
    {
        _settings = blobOptions.Value;
        _client = new BlobServiceClient(_settings.ConnectionString);
        _container = _client.GetBlobContainerClient(_settings.ContainerName);
    }


    public async Task CreateBlob(string filename, string filepath)
    {
        BlobClient blobClient = _container.GetBlobClient(filename);
        using FileStream uploadFileStream = File.OpenRead(filepath);
        await blobClient.UploadAsync(uploadFileStream, true);
        uploadFileStream.Close();
    }
}