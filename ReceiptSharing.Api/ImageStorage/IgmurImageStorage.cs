using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Repositories{
    public class IgmurImageStorage : IImageStorage{

        private const string URL = "https://api.imgur.com/3/upload";
        private readonly IConfiguration configuration;

        //private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        public IgmurImageStorage(IConfiguration configuration){

            this.configuration = configuration;

        }

        public async Task<string?> PostImage(IFormFile File){

            var ms = new MemoryStream();
            File.CopyTo(ms);
            var fileBytes = ms.ToArray();
            string result = Convert.ToBase64String(fileBytes);


            HttpClient client = new HttpClient();


            var requestData = new Dictionary<string, string>
            {  
                { "image", result },
                { "type", "base64" }
            };
     
            var request = new HttpRequestMessage() {
                RequestUri = new Uri("https://api.imgur.com/3/upload"),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(requestData)
            };

            var clientId = configuration.GetSection("imgur:clientId").Value;

            request.Headers.Add("Authorization", $"Client-ID {clientId}");
            
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine($"Error Message: {errorMessage}");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStreamAsync();
                var contentData = await System.Text.Json.JsonSerializer.DeserializeAsync<ResponseIgmur>(content, 
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if(contentData == null) return null;

                var imagePath = contentData.Data.link;
                return imagePath;
            }
            else
            {
                return null;
            }
            
  
        }

    } 
}