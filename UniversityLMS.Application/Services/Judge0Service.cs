using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;

namespace UniversityLMS.Application.Services
{
    public class Judge0Service : IJudgeService
    {
        private readonly HttpClient _client;
        

        public Judge0Service(HttpClient client)
        {
            _client = client;
        }


        public async Task<string> EvaluateCode (string code, int language, string input)
        {          
            var requestData = new
            {
                source_code = code,
                language_id = language, 
                stdin = input
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://judge029.p.rapidapi.com/submissions?base64_encoded=false&wait=true&fields=*"),
                Headers =
                {
                    { "x-rapidapi-key", "d8f903eba4mshb80da5d96f5e331p124396jsna61eb85e0cd1" },
                    { "x-rapidapi-host", "judge029.p.rapidapi.com" }
                },
                Content = requestContent
            };

            using (var response = await _client.SendAsync(request))
            {

                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);

                return body;

            }
        }





        public async Task<int> AssignmentResult (string sourceCode, IEnumerable<TestCaseDTO> testCases, int L)
        {
            int score = 0;

            foreach (var testCase in testCases)
            {
                try
                {
                    var result = await EvaluateCode(sourceCode, L,testCase.Input);
                    var response = JsonConvert.DeserializeObject<Judge0Response>(result);

                    if (!string.IsNullOrEmpty(response?.Stdout) && response.Stdout.Trim() == testCase.ExpectedOutput.Trim())
                    {
                        score++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing test case: {ex.Message}");
                }
            }
            return score;
        }
    }
}

