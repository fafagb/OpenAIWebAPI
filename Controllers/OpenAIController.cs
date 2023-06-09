using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace OpenAIWebAPI.Controllers;

class OpenAIResponse {
    public OpenAIChoice[] Choices { get; set; }
}

class OpenAIChoice {
    public string Text { get; set; }
    public double? Logprobs { get; set; }
    public string[] Tokens { get; set; }
}

[ApiController]
[Route ("[controller]/[action]")]
public class OpenAIController : ControllerBase {
    private readonly ILogger<OpenAIController> _logger;
    private readonly HttpClient _httpClient;

    public OpenAIController (ILogger<OpenAIController> logger, HttpClient httpClient) {
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpGet]
    public async Task<string> Call2 (string prompt) {
        string apiKey = "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";
        using (var httpClient = new HttpClient ()) {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue ("Bearer", apiKey);

            var apiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";
            var requestData = new {
                prompt = prompt,
                max_tokens = 50,
                n = 1,
                stop = "",
                temperature = 1.0
            };

            var content = new StringContent (JsonConvert.SerializeObject (requestData), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");
            var response = await httpClient.PostAsync (apiUrl, content);

            if (response.IsSuccessStatusCode) {
                var jsonResponse = await response.Content.ReadAsStringAsync ();
                dynamic resultObject = JsonConvert.DeserializeObject (jsonResponse);
                string result = resultObject.choices[0].text.ToString ();
                return result;
            } else {
                throw new Exception ("Error calling OpenAI API: " + response.StatusCode);
            }
        }
    }

    //写一个异步接口来调用上面的方法
    [HttpGet]
    public async Task<string> Get (string str) {
        string model = "davinci";
        string apiKey = "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";
        string apiUrl = "https://api.openai.com/v1/engines/" + model + "/completions";

        var request = new {
            prompt = str,
            temperature = 0.1,
            max_tokens = 150,
            top_p = 0.1,
            frequency_penalty = 0,
            presence_penalty = 0
        };

        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonRequest = System.Text.Json.JsonSerializer.Serialize (request, options);
        var client = new HttpClient ();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", apiKey);
        var response = await client.PostAsync (apiUrl, new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
        var jsonResponse = await response.Content.ReadAsStringAsync ();
        var output = System.Text.Json.JsonSerializer.Deserialize<OpenAIResponse> (jsonResponse, options);

        return output.Choices[0].Text;
    }

    //写一个异步方法
    [HttpGet]
    public async Task<string> Call (string str) {
        // string expectedReferer = "ai.myi.cn";

        // // 检查请求的 Referer 字段
        // string referer = Request.Headers["Host"].ToString ();

        // if (referer != expectedReferer) {

        //     return $"Access denied:{referer}";
        // }
        try {
            var openAiService = new OpenAIService (new OpenAiOptions () {
                // ApiKey = "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"
                ApiKey = "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"
            });
            var completionResult = await openAiService.ChatCompletion.CreateCompletion (new ChatCompletionCreateRequest {
                Messages = new List<ChatMessage> {
                        ChatMessage.FromSystem ($"{str}"),

                    },
                    Model = Models.ChatGpt3_5Turbo,
                    MaxTokens = 2048, //optional,
                    Temperature = 1, TopP = 1

            });
            if (completionResult.Successful) {
                return completionResult.Choices.First ().Message.Content;
            }
            return "失败" + completionResult.Error.Message;
        } catch (System.Exception ex) {
            // TODO

            return "错误：" + ex.Message;
        }

    }

    [HttpGet]
    public async Task<string> Question (string str) {
        
        var openAiService = new OpenAIService (new OpenAiOptions () {
            // ApiKey = "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"
            ApiKey = "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"
            // ApiVersion="v1/chat/completions"
            
        });
        var completionResult = openAiService.Completions.CreateCompletionAsStream (new CompletionCreateRequest () {
            Prompt = "写一份详细的语宙gpt的功能说明",
                MaxTokens = 2048
        }, Models.ChatGpt3_5Turbo);

        await
        foreach (var completion in completionResult) {
            if (completion.Successful) {
                Console.Write (completion.Choices.FirstOrDefault ()?.Text);
            } else {
                if (completion.Error == null) {
                    throw new Exception ("Unknown Error");
                }

                Console.WriteLine ($"{completion.Error.Code}: {completion.Error.Message}");
            }
        }
        Console.WriteLine ("Complete");
        return "Complete";
    }

    [HttpGet]
    public async Task<string> Test (string str) {
        string apiKey = "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";
        string url = "https://api.openai.com/v1/chat/completions";

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue ("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add ("OpenAI-Organization", "org-6rXJXBf9tP8J0QdOA0WBUfeI");
        string json = "";
        //         """
        //             {"model ": "
        //                gpt - 3.5 - turbo ",
        //                "
        //         messages ": [{"
        //         role ": "
        //         user ", "
        //         content ": "
        //         德国使用强人工智能的农业机器人有哪些 "}],
        //      "
        //         temperature ": 0.7
        //    }
        //    "
        //         """;

        var content = new StringContent (json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");
        var response = await _httpClient.PostAsync (url, content);
        if (response.IsSuccessStatusCode) {
            var jsonResponse = await response.Content.ReadAsStringAsync ();
            dynamic resultObject = JsonConvert.DeserializeObject (jsonResponse);
            string result = resultObject.choices[0].message.content;
            return result;
        } else {
            return "错误";

        }
    }

    /// <inheritdoc />

}