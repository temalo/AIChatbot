using System;
using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;

namespace AzureAI;

internal class Program
{
    static void Main(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Setup dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Get services from DI container
        var chatClient = serviceProvider.GetRequiredService<ChatClient>();
        var requestOptions = serviceProvider.GetRequiredService<ChatCompletionOptions>();
        var systemMessage = configuration["SystemMessage"] ?? 
            "You are ASP.NET Core developer. I have experience with C# 14. I am going to build a web application.";

        List<ChatMessage> messages = new List<ChatMessage>()
        {
            new SystemChatMessage(systemMessage),
        };

        string? userInput;
        while (true)
        {
            // Get user input
            Console.Write("You: ");
            userInput = Console.ReadLine();

            // Check if user wants to exit
            if (string.IsNullOrWhiteSpace(userInput) || 
                userInput.Equals("exit", StringComparison.OrdinalIgnoreCase) || 
                userInput.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            // Add user message to conversation history
            messages.Add(new UserChatMessage(userInput));

            // Show thinking message
            Console.WriteLine("GPT 5 mini is thinking ...");

            // Get AI response
            var response = chatClient.CompleteChat(messages, requestOptions);
            string assistantResponse = response.Value.Content[0].Text;

            // Display AI response
            Console.WriteLine($"GPT 5 mini: {assistantResponse}");
            Console.WriteLine();

            // Append the model response to the chat history
            messages.Add(new AssistantChatMessage(assistantResponse));
        }

        Console.WriteLine("Thank you for using GPT 5 mini");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Get configuration values
        var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"] ?? 
            throw new InvalidOperationException("AzureOpenAI:Endpoint is not configured"));
        var apiKey = configuration["AzureOpenAI:ApiKey"] ?? 
            throw new InvalidOperationException("AzureOpenAI:ApiKey is not configured");
        var deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? 
            throw new InvalidOperationException("AzureOpenAI:DeploymentName is not configured");
        var maxOutputTokenCount = int.Parse(configuration["AzureOpenAI:MaxOutputTokenCount"] ?? "1000");

        // Register Azure OpenAI Client
        var azureClient = new AzureOpenAIClient(
            endpoint,
            new AzureKeyCredential(apiKey));
        var chatClient = azureClient.GetChatClient(deploymentName);
        services.AddSingleton(chatClient);

        // Register ChatCompletionOptions
        var requestOptions = new ChatCompletionOptions()
        {
            MaxOutputTokenCount = maxOutputTokenCount,
        };

        // The SetNewMaxCompletionTokensPropertyEnabled() method is an [Experimental] opt-in to use
        // the new max_completion_tokens JSON property instead of the legacy max_tokens property.
        // This extension method will be removed and unnecessary in a future service API version;
        // please disable the [Experimental] warning to acknowledge.
#pragma warning disable AOAI001
        requestOptions.SetNewMaxCompletionTokensPropertyEnabled(true);
#pragma warning restore AOAI001

        services.AddSingleton(requestOptions);
    }
}
