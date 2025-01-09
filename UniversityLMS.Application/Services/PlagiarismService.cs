using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;

namespace UniversityLMS.Application.Services
{
    public class PlagiarismService : IPlagiarism
    {
        private readonly HttpClient _client;

        public PlagiarismService(HttpClient client)
        {
            _client = client;
        }

        public async Task<PlagiarismRequest> PlagiarismCodeAsync(string code)
        {

            var requestBody = new
            {
                text = code,
                language = "en",
                includeCitations = false,
                scrapeSources = false
            };


            string jsonPayload = System.Text.Json.JsonSerializer.Serialize(requestBody);


            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://plagiarism-checker-and-auto-citation-generator-multi-lingual.p.rapidapi.com/plagiarism"),
                Headers =
                {
                    { "x-rapidapi-key", "d8f903eba4mshb80da5d96f5e331p124396jsna61eb85e0cd1" },
                    { "x-rapidapi-host", "plagiarism-checker-and-auto-citation-generator-multi-lingual.p.rapidapi.com" },
                },

                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };


            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<PlagiarismRequest>(body);
            }
        }
    }
}
