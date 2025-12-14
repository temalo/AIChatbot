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
    // Constants
    private const string EXIT_COMMAND_1 = "exit";
    private const string EXIT_COMMAND_2 = "quit";
    private const string BOT_NAME = "GPT 5 mini";
    private const string DEFAULT_SYSTEM_MESSAGE = "You are ASP.NET Core developer. I have experience with C# 14. I am going to build a web application.";
    private const int MAX_CONVERSATION_HISTORY = 50; // Maximum messages to keep in history
    private const int DEFAULT_MAX_OUTPUT_TOKENS = 1000;

    static void Main(string[] args)
    {
        try
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.example.json", optional: true, reloadOnChange: true)
                .Build();

            // Setup dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Get services from DI container
            var chatClient = serviceProvider.GetRequiredService<ChatClient>();
            var requestOptions = serviceProvider.GetRequiredService<ChatCompletionOptions>();
            var systemMessage = configuration["SystemMessage"] ?? DEFAULT_SYSTEM_MESSAGE;

            RunChatLoop(chatClient, requestOptions, systemMessage);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Configuration Error: {ex.Message}");
            Console.WriteLine("\nPlease ensure appsettings.json is properly configured.");
            Console.WriteLine("You can copy appsettings.example.json to appsettings.json and update with your Azure OpenAI credentials.");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Environment.Exit(1);
        }
    }

    private static void RunChatLoop(ChatClient chatClient, ChatCompletionOptions requestOptions, string systemMessage)
    {
        List<ChatMessage> messages = new List<ChatMessage>()
        {
            new SystemChatMessage(systemMessage),
        };

        Console.WriteLine($"Welcome to {BOT_NAME}!");
        Console.WriteLine($"Type '{EXIT_COMMAND_1}' or '{EXIT_COMMAND_2}' to exit.\n");

        string? userInput;
        while (true)
        {
            try
            {
                // Get user input
                Console.Write("You: ");
                userInput = Console.ReadLine();

                // Check if user wants to exit
                if (string.IsNullOrWhiteSpace(userInput) ||
                    userInput.Equals(EXIT_COMMAND_1, StringComparison.OrdinalIgnoreCase) ||
                    userInput.Equals(EXIT_COMMAND_2, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                // Add user message to conversation history
                messages.Add(new UserChatMessage(userInput));

                // Limit conversation history to prevent memory issues
                if (messages.Count > MAX_CONVERSATION_HISTORY)
                {
                    // Keep system message and most recent messages
                    var systemMsg = messages[0];
                    var recentMessages = messages.Skip(messages.Count - MAX_CONVERSATION_HISTORY + 1).ToList();
                    messages = new List<ChatMessage> { systemMsg };
                    messages.AddRange(recentMessages);
                }

                // Show thinking message
                Console.WriteLine($"{BOT_NAME} is thinking ...");

                // Get AI response with error handling
                ChatCompletion response;
                try
                {
                    response = chatClient.CompleteChat(messages, requestOptions);
                }
                catch (Azure.RequestFailedException ex)
                {
                    Console.WriteLine($"\n❌ API Error: {ex.Message}");
                    
                    if (ex.Status == 429)
                    {
                        Console.WriteLine("Rate limit exceeded. Please wait a moment and try again.");
                    }
                    else if (ex.Status == 401)
                    {
                        Console.WriteLine("Authentication failed. Please check your API key in appsettings.json.");
                    }
                    else if (ex.Status == 404)
                    {
                        Console.WriteLine("Deployment not found. Please check your DeploymentName in appsettings.json.");
                    }
                    
                    Console.WriteLine();
                    // Remove the failed user message from history
                    messages.RemoveAt(messages.Count - 1);
                    continue;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("\n❌ Request timed out. The API took too long to respond.");
                    Console.WriteLine();
                    messages.RemoveAt(messages.Count - 1);
                    continue;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"\n❌ Network Error: {ex.Message}");
                    Console.WriteLine("Please check your internet connection and try again.");
                    Console.WriteLine();
                    messages.RemoveAt(messages.Count - 1);
                    continue;
                }

                string assistantResponse = response.Content[0].Text;

                // Display AI response
                Console.WriteLine($"{BOT_NAME}: {assistantResponse}");
                Console.WriteLine();

                // Append the model response to the chat history
                messages.Add(new AssistantChatMessage(assistantResponse));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Unexpected Error: {ex.Message}");
                Console.WriteLine("Please try again or type 'exit' to quit.");
                Console.WriteLine();
                // Try to recover by removing the last user message if it exists
                if (messages.Count > 1 && messages[^1] is UserChatMessage)
                {
                    messages.RemoveAt(messages.Count - 1);
                }
            }
        }

        Console.WriteLine($"Thank you for using {BOT_NAME}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Get configuration values with validation
        var endpointString = configuration["AzureOpenAI:Endpoint"] ??
            throw new InvalidOperationException("AzureOpenAI:Endpoint is not configured. Please check your appsettings.json file.");
        
        if (!Uri.TryCreate(endpointString, UriKind.Absolute, out var endpoint))
        {
            throw new InvalidOperationException($"AzureOpenAI:Endpoint is not a valid URL: {endpointString}");
        }

        var apiKey = configuration["AzureOpenAI:ApiKey"] ??
            throw new InvalidOperationException("AzureOpenAI:ApiKey is not configured. Please check your appsettings.json file.");
        
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "YOUR_API_KEY_HERE")
        {
            throw new InvalidOperationException("AzureOpenAI:ApiKey is not set to a valid value. Please update your appsettings.json with your actual API key.");
        }

        var deploymentName = configuration["AzureOpenAI:DeploymentName"] ??
            throw new InvalidOperationException("AzureOpenAI:DeploymentName is not configured. Please check your appsettings.json file.");

        var maxOutputTokenCountString = configuration["AzureOpenAI:MaxOutputTokenCount"] ?? DEFAULT_MAX_OUTPUT_TOKENS.ToString();
        if (!int.TryParse(maxOutputTokenCountString, out var maxOutputTokenCount) || maxOutputTokenCount <= 0)
        {
            throw new InvalidOperationException($"AzureOpenAI:MaxOutputTokenCount must be a positive integer. Current value: {maxOutputTokenCountString}");
        }

        try
        {
            // Register Azure OpenAI Client
            var azureClient = new AzureOpenAIClient(
                endpoint,
                new AzureKeyCredential(apiKey));
            var chatClient = azureClient.GetChatClient(deploymentName);
            services.AddSingleton(chatClient);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Azure OpenAI client: {ex.Message}", ex);
        }

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
