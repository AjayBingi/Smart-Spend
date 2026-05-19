using System.Net.Http;
using Azure;
using LangChain.Providers.OpenAI;
using Microsoft.Extensions.Options;
using SmartSpendService.Helpers;

namespace SmartSpendService.Repositories;

public class AILibrary
{
    private readonly OpenAiProvider _provider;

    public AILibrary(IOptions<AISettings> aiSettings)
    {
        var settings = aiSettings.Value;

        _provider = new OpenAiProvider(
            settings.ApiKey
        );
    }

    public async Task<string> AskAI(string prompt)
    {
        try
        {
            var model = new OpenAiChatModel(
                _provider,
                "gpt-4o-mini"
            );
            var response = await model.GenerateAsync(prompt);
            return response.ToString();
        }
        catch (Exception ex)
        {
            return $"AI Error: {ex.Message}";
        }
    }
}