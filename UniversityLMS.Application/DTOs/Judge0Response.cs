using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniversityLMS.Application.DTOs
{
    public class Judge0Response
    {
        [JsonPropertyName("source_code")]
        public string SourceCode { get; set; }

        [JsonPropertyName("language_id")]
        public int LanguageId { get; set; }

        [JsonPropertyName("stdin")]
        public string Stdin { get; set; }

        [JsonPropertyName("expected_output")]
        public string ExpectedOutput { get; set; }

        [JsonPropertyName("stdout")]
        public string Stdout { get; set; }

        [JsonPropertyName("status_id")]
        public int StatusId { get; set; }

        [JsonPropertyName("stderr")]
        public string Stderr { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("compile_output")]
        public string CompileOutput { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
