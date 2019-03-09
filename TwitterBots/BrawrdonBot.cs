﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pyxcell;

namespace TwitterBots
{

  
    public class BrawrdonBot : TwitterBot
    {

        public BrawrdonBot(HttpClient client) : base(client, Environment.GetEnvironmentVariable("BRAWRDONBOT_CONSUMER_KEY"), Environment.GetEnvironmentVariable("BRAWRDONBOT_OAUTH_TOKEN"),  Environment.GetEnvironmentVariable("BRAWRDONBOT_CONSUMER_KEY_SECRET"), Environment.GetEnvironmentVariable("BRAWRDONBOT_OAUTH_TOKEN_SECRET"))
        {
           Console.WriteLine("hello");
        }

        public async Task<JObject> PostTweet(string status)
        {
            var mediaBase64 = await UploadImage(status);
            return await PostTweet(status, mediaBase64: mediaBase64);
        }
        

        private async Task<string> UploadImage(string status)
        {
            const string url = "https://upload.twitter.com/1.1/media/upload.json";
            var filePath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "image.png");
            var generator = new CommandGenerator();
            generator.Generate(status);
            generator.Draw(filePath);

            var base64Image = Convert.ToBase64String(File.ReadAllBytes(filePath));
            var requestData = new SortedDictionary<string, string> { { "media_data", base64Image } };

            Authenticate(url, requestData);

            var content = new FormUrlEncodedContent(requestData);
            
            var response = await _client.PostAsync(url, content);
            var responseData = await response.Content.ReadAsStringAsync();
            var responseDataJson = (JObject)JsonConvert.DeserializeObject(responseData);

            File.Delete(filePath);
            return responseDataJson.Value<string>("media_id_string");

        }

    }
}