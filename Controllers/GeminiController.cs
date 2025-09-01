using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlantDiaganoseDisease.Models;
using PlantDiaganoseDisease.Models.RequestModels;
using System.Text;
using System.Text.Json;

namespace PlantDiaganoseDisease.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GeminiSettings _geminiSettings;

        public GeminiController(IHttpClientFactory httpClientFactory, IOptions<GeminiSettings> geminiOptions)
        {
            _httpClientFactory = httpClientFactory;
            _geminiSettings = geminiOptions.Value;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskGemini([FromBody] GeminiRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest(new { error = "Question is required." });

            var client = _httpClientFactory.CreateClient();

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = request.Question }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";
            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = "Gemini API error", details = errorBody });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonDocument.Parse(responseBody);

            var answer = geminiResponse
                            .RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();

            return Ok(new { answer });
        }

        [HttpPost("TreatmentDosageCalculator")]
        public async Task<IActionResult> CalculateTreatment([FromBody] TreatmentDosageReq input)
        {
            if (input == null)
                return BadRequest("Input is required.");

            string prompt = $@"
                You are an intelligent agricultural treatment dosage assistant.

                The user will provide treatment details in any language. Your job is to:
                - Understand the language used by the user (do NOT translate it)
                - Respond in the SAME language as the input
                - Output only the following JSON fields:

                {{
                  ""productRequired"": ""in ml or L"",
                  ""waterRequired"": ""in L"",
                  ""totalSolution"": ""in L"",
                  ""coverage"": ""in acres"",
                  ""safetyNote"": ""one concise safety sentence""
                }}

                Use numbers with proper units. Do NOT include any extra text or explanations.
                Input values:

                - Treatment Type: {input.TreatmentType}
                - Product Name: {input.ProductName}
                - Farm Area: {input.FarmArea}
                - Dilution Ratio: {input.DilutionRatio}
                - Product Concentration: {input.ProductConcentration}
                - Water Available: {input.WaterAvailable}
                ";


            var payload = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
            };

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";
            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = "Gemini API error", details = errorBody });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonDocument.Parse(responseBody);

            var answer = geminiResponse
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            try
            {
                var cleanJson = JsonDocument.Parse(answer).RootElement;
                return Ok(new
                {
                    productRequired = cleanJson.GetProperty("productRequired").GetString(),
                    waterRequired = cleanJson.GetProperty("waterRequired").GetString(),
                    totalSolution = cleanJson.GetProperty("totalSolution").GetString(),
                    coverage = cleanJson.GetProperty("coverage").GetString(),
                    safetyNote = cleanJson.GetProperty("safetyNote").GetString()
                });
            }
            catch
            {
                return Ok(new { rawGeminiOutput = answer });
            }
        }

        [HttpPost("TreatmentType")]
        public async Task<IActionResult> TreatmentType([FromBody] TreatmentTypeReq input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Name))
                return BadRequest("Treatment name is required.");

            string prompt = $@"
            You are an expert in agricultural treatment recommendations.

            Based on the treatment type provided by the user, generate a response in the same language as the input.

            Respond with this exact JSON structure:
            {{
              ""recommendedProducts"": [
                {{ ""productName"": ""Product1"", ""dosage"": ""in ml or L"" }},
                {{ ""productName"": ""Product2"", ""dosage"": ""in ml or L"" }}
              ],
              ""generalInstructions"": ""One concise usage or safety note""
            }}

            Limit to maximum 10 recommended products only.
            Do not include extra commentary or text outside the JSON.
            
            Treatment Type: {input.Name}
        ";

            var payload = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
            };

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";
            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = "Gemini API error", details = errorBody });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonDocument.Parse(responseBody);

            var answer = geminiResponse
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            try
            {
                // Remove markdown formatting if present
                var cleanedJson = answer
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var treatmentResponse = JsonSerializer.Deserialize<TreatmentTypeResponse>(cleanedJson, options);

                if (treatmentResponse?.RecommendedProducts != null)
                {
                    treatmentResponse.RecommendedProducts = treatmentResponse.RecommendedProducts
                        .Take(100)
                        .ToList();
                }

                return Ok(treatmentResponse);
            }
            catch
            {
                return Ok(new { rawGeminiOutput = answer });
            }
        }
        [HttpPost("FertilizerRecommendation")]
        public async Task<IActionResult> FertilizerRecommendation([FromBody] FertilizerReq input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.CropType) || string.IsNullOrWhiteSpace(input.SoilType))
                return BadRequest("Crop type and soil type are required.");

            string prompt = $@"
                You are an expert in agricultural fertilizer recommendations.

                Based on the input details, provide fertilizer recommendations.
                Respond in the same language as the user input (e.g., Hindi, English, Tamil).
                Use this exact JSON structure:
                {{
                  ""primaryFertilizer"": ""string"",
                  ""quantityRequired"": ""string"",
                  ""applicationTime"": ""string"",
                  ""applicationMethod"": ""string"",
                  ""applicationSchedule"": [
                    {{ ""stage"": ""string"", ""fertilizer"": ""string"", ""quantity"": ""string"", ""timing"": ""string"" }}
                  ],
                  ""applicationTips"": [""tip1"", ""tip2""]
                }}

                Input Details:
                Crop: {input.CropType}
                Soil Type: {input.SoilType}
                Farm Area: {input.FarmArea} acres
                Growth Stage: {input.GrowthStage}
                Previous Fertilizer Used: {input.PreviousFertilizer}
                ";

            var payload = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
            };

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";
            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = "Gemini API error", details = errorBody });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonDocument.Parse(responseBody);

            var answer = geminiResponse
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            try
            {
                // Clean markdown if present
                var cleanedJson = answer
                    .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var fertilizerResponse = JsonSerializer.Deserialize<FertilizerResponse>(cleanedJson, options);

                return Ok(fertilizerResponse);
            }
            catch (Exception ex)
            {
                return Ok(new { rawGeminiOutput = answer, error = ex.Message });
            }
        }
        [HttpPost("IrrigationRecommendation")]
        public async Task<IActionResult> IrrigationRecommendation([FromBody] IrrigationReq input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.CropType) || string.IsNullOrWhiteSpace(input.SoilType))
                return BadRequest("Crop type and soil type are required.");

            string prompt = $@"
        You are an expert in agricultural irrigation recommendations.

        Based on the input details, provide irrigation recommendations.
        Respond strictly in JSON only, with no extra commentary.
        Use the same language as the input request (English, Hindi, Tamil, etc.).

        JSON format:
        {{
          ""irrigationFrequency"": ""string"",
          ""waterRequired"": ""string"",
          ""duration"": ""string"",
          ""bestTime"": ""string"",
          ""irrigationSchedule"": [
            {{ ""stage"": ""string"", ""frequency"": ""string"", ""waterDepth"": ""string"", ""duration"": ""string"" }}
          ],
          ""irrigationTips"": [""tip1"", ""tip2""]
        }}

        Input Details:
        Crop: {input.CropType}
        Soil Type: {input.SoilType}
        Farm Area: {input.FarmArea} acres
        Irrigation Method: {input.IrrigationMethod}
        Water Source: {input.WaterSource}
    ";

            var payload = new
            {
                contents = new[]
                {
            new
            {
                parts = new[] { new { text = prompt } }
            }
        }
            };

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";
            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = "Gemini API error", details = errorBody });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonDocument.Parse(responseBody);

            var answer = geminiResponse
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            try
            {
                var cleanedJson = answer
                    .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var irrigationResponse = JsonSerializer.Deserialize<IrrigationResponse>(cleanedJson, options);

                return Ok(irrigationResponse);
            }
            catch (Exception ex)
            {
                return Ok(new { rawGeminiOutput = answer, error = ex.Message });
            }
        }

        [HttpPost("GetWeather")]
        public async Task<IActionResult> GetWeather([FromBody] LocationReq request)
        {
            try
            {
                // Step 1: Extract latitude & longitude from model
                double latitude = request.Latitude;
                double longitude = request.Longitude;

                // Step 2: Create Gemini prompt
                string prompt = $@"
        You are a weather assistant.
        Based on the given location (lat: {latitude}, lon: {longitude}),
        provide the current weather information.

        Use this exact JSON structure:
        {{
          ""temperature"": ""string"",
          ""condition"": ""string"",
          ""humidity"": ""string"",
          ""windSpeed"": ""string"",
          ""visibility"": ""string""
        }}
        ";

                var payload = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
                };

                // Step 3: Call Gemini API
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";
                var response = await client.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { error = "Gemini API error", details = errorBody });
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonDocument.Parse(responseBody);

                var answer = geminiResponse
                    .RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                // Step 4: Clean JSON response
                int start = answer.IndexOf("{");
                int end = answer.LastIndexOf("}");
                string cleanedJson = (start >= 0 && end > start) ? answer.Substring(start, end - start + 1) : answer;

                return Ok(cleanedJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Invalid request", details = ex.Message });
            }
        }




    }

}
