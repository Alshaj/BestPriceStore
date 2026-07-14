using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BestPriceStore.Services.ImageService
{
    public class R2HttpClientFactory : Amazon.Runtime.HttpClientFactory
    {
        private static readonly HttpClient _httpClient;

        static R2HttpClientFactory()
        {
            var handler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, cancellationToken) =>
                {
                    var host = context.DnsEndPoint.Host;
                    var port = context.DnsEndPoint.Port;

                    if (host.EndsWith("r2.cloudflarestorage.com", StringComparison.OrdinalIgnoreCase))
                    {
                        // Redirect connection to a working Cloudflare IP to bypass regional ISP block (e.g. in Egypt)
                        var ipAddress = System.Net.IPAddress.Parse("104.18.54.45");
                        var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                        try
                        {
                            await socket.ConnectAsync(new System.Net.IPEndPoint(ipAddress, port), cancellationToken);
                            return new System.Net.Sockets.NetworkStream(socket, ownsSocket: true);
                        }
                        catch
                        {
                            socket.Dispose();
                            throw;
                        }
                    }

                    var defaultSocket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    try
                    {
                        await defaultSocket.ConnectAsync(host, port, cancellationToken);
                        return new System.Net.Sockets.NetworkStream(defaultSocket, ownsSocket: true);
                    }
                    catch
                    {
                        defaultSocket.Dispose();
                        throw;
                    }
                }
            };

            _httpClient = new HttpClient(handler);
        }

        public override HttpClient CreateHttpClient(IClientConfig clientConfig)
        {
            return _httpClient;
        }
    }

    public class R2ImageService : IImageService
    {
        private readonly IConfiguration _configuration;
        private readonly AmazonS3Client _s3Client;

        public R2ImageService(IConfiguration configuration)
        {
            _configuration = configuration;

            var accessKey = _configuration["R2:AccessKey"];
            var secretKey = _configuration["R2:SecretKey"];
            var serviceUrl = _configuration["R2:ServiceUrl"];

            var config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true, // Required for Cloudflare R2
                AuthenticationRegion = "auto", // Required for Cloudflare R2
                HttpClientFactory = new R2HttpClientFactory() // Custom factory to bypass DNS/ISP block
            };

            _s3Client = new AmazonS3Client(accessKey, secretKey, config);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be empty", nameof(file));

            var bucketName = _configuration["R2:BucketName"];
            var publicUrl = _configuration["R2:PublicUrl"];

            // Generate a unique filename
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            using var newMemoryStream = new MemoryStream();
            await file.CopyToAsync(newMemoryStream);
            newMemoryStream.Position = 0; // CRITICAL: Reset stream position to beginning before upload!

            var putRequest = new PutObjectRequest
            {
                InputStream = newMemoryStream,
                Key = uniqueFileName,
                BucketName = bucketName,
                ContentType = file.ContentType,
                DisablePayloadSigning = true
            };

            await _s3Client.PutObjectAsync(putRequest);

            // Construct and return the public URL
            return $"{publicUrl.TrimEnd('/')}/{uniqueFileName}";
        }
    }
}
