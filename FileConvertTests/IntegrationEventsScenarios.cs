using FileConvert.Model;
using FileConvertTests.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FileConvertTests
{
    public class IntegrationEventsScenarios
    {
        public ITestOutputHelper Output { get; }

        public IntegrationEventsScenarios(ITestOutputHelper outputHelper)
        {
            Output = outputHelper;
        }
        [Fact]
        public async Task  The_test_run()
        {
            using (var fileConvertServer  = new FileConvertScenariosBase().CreateServer())
            {
                var client = fileConvertServer.CreateIdempotentClient();
                //GetTokenAsync
                var token = await GetTokenAsync(client);
                if (token == null)
                {
                    Assert.False(true, $"The token service has not been complited.");
                }
                else
                {
                    Output.WriteLine(JsonSerializer.Serialize<TokenObject>(token));
                    Assert.NotNull(token);
                }

                client.DefaultRequestHeaders.Add("Authorization", $"bearer {token.token}");
                //PostVideoCompressByTextAsync
                var videoCompressedText = await PostVideoCompressByTextAsync(client);
                Assert.NotNull(videoCompressedText);

                //PostVideoCompressByFormAsync
                var videoCompressedFrom = await PostVideoCompressByFormAsync(client);
                Assert.NotNull(videoCompressedFrom);

                //PostImageCompressByTextAsync
                var imageCompressedText = await PostImageCompressByTextAsync(client);
                Assert.NotNull(imageCompressedText);

                //PostImageCompressByFormAsync
                var imageCompressedForm = await PostImageCompressByFormAsync(client);
                Assert.NotNull(imageCompressedForm);
            }
        }

        private async Task<TokenObject> GetTokenAsync(HttpClient client)
        {
            var response = await client.GetAsync(FileConvertScenariosBase.Get.GetToken);
            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenObject>(result, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private async Task<string> PostVideoCompressByTextAsync(HttpClient client)
        {
            using (FileStream fs = new FileStream(FileConvertScenariosBase.Post.VideoFilePath,FileMode.Open,FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
                var response = await client.PostAsync(
                    FileConvertScenariosBase.Post.PostVideoCompressByText,
                    new StringContent(Convert.ToBase64String(bytes),Encoding.UTF8, "text/plain"));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            }
          
        }
        private async Task<string> PostVideoCompressByFormAsync(HttpClient client)
        {
            using (FileStream fs = new FileStream(FileConvertScenariosBase.Post.VideoFilePath, FileMode.Open, FileAccess.Read))
            {
                var postContent = new MultipartFormDataContent();
                string boundary = string.Format("--{0}", DateTime.Now.Ticks.ToString("x"));
                postContent.Headers.Add("ContentType", $"multipart/form-data, boundary={boundary}");
                postContent.Add(new StreamContent(fs,(int)fs.Length), "files",Path.GetFileName(fs.Name));
                var response = await client.PostAsync(
                    FileConvertScenariosBase.Post.PostVideoCompressByForm, postContent);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            }

        }


        private async Task<string> PostImageCompressByTextAsync(HttpClient client)
        {
            using (FileStream fs = new FileStream(FileConvertScenariosBase.Post.ImageFilePath, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
                var response = await client.PostAsync(
                    FileConvertScenariosBase.Post.PostImageCompressByText,
                    new StringContent(Convert.ToBase64String(bytes), Encoding.UTF8, "text/plain"));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            }

        }
        private async Task<string> PostImageCompressByFormAsync(HttpClient client)
        {
            using (FileStream fs = new FileStream(FileConvertScenariosBase.Post.ImageFilePath, FileMode.Open, FileAccess.Read))
            {
                var postContent = new MultipartFormDataContent();
                string boundary = string.Format("--{0}", DateTime.Now.Ticks.ToString("x"));
                postContent.Headers.Add("ContentType", $"multipart/form-data, boundary={boundary}");
                postContent.Add(new StreamContent(fs, (int)fs.Length), "files", Path.GetFileName(fs.Name));
                var response = await client.PostAsync(
                    FileConvertScenariosBase.Post.PostImageCompressByForm, postContent);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            }

        }

        class TokenObject
        {
            public string token { get; set; }
            public DateTime expiration { get; set; }
        }
    }
}
