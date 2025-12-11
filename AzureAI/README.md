# Azure AI Chatbot

A console-based chatbot application that uses Azure OpenAI GPT-5-mini model for interactive conversations.

## Features

- Interactive chat interface with Azure OpenAI GPT-5-mini
- Dependency injection for clean architecture
- Configuration-based setup using `appsettings.json`
- Continuous conversation loop until user exits
- Support for custom system messages

## Prerequisites

- .NET 10.0 SDK or later
- Azure OpenAI resource with GPT-5-mini deployment
- Azure OpenAI API key

## Setup Instructions

### 1. Clone the Repository

```bash
git clone git@github.com:duytm12/AIChatbot.git
cd AIChatbot/AzureAI
```

### 2. Configure Application Settings

1. Copy the example configuration file:
   ```bash
   cp appsettings.example.json appsettings.json
   ```

2. Edit `appsettings.json` and update the following values:
   ```json
   {
     "AzureOpenAI": {
       "Endpoint": "https://your-resource-name.cognitiveservices.azure.com/",
       "ApiKey": "YOUR_API_KEY_HERE",
       "DeploymentName": "gpt-5-mini",
       "MaxOutputTokenCount": 1000
     },
     "SystemMessage": "Your custom system message here"
   }
   ```

   **Important:** 
   - Replace `YOUR_API_KEY_HERE` with your actual Azure OpenAI API key
   - Replace `your-resource-name` with your Azure OpenAI resource name
   - Adjust `MaxOutputTokenCount` as needed (default: 1000)
   - Customize `SystemMessage` to define the AI's role and behavior

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Application

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

## Usage

1. Start the application - it will display a prompt: `You: `
2. Type your message and press Enter
3. The AI will process your message and respond
4. Continue the conversation by typing more messages
5. Type `exit` or `quit` to end the conversation and close the application

### Example Conversation

```
You: Hello, can you help me learn ASP.NET Core?
GPT 5 mini is thinking ...
GPT 5 mini: [AI response here]

You: What are the key concepts I should know?
GPT 5 mini is thinking ...
GPT 5 mini: [AI response here]

You: exit
Thank you for using GPT 5 mini
Press any key to exit...
```

## Configuration

### Azure OpenAI Settings

- **Endpoint**: Your Azure OpenAI resource endpoint URL
- **ApiKey**: Your Azure OpenAI API key (keep this secret!)
- **DeploymentName**: The name of your GPT-5-mini deployment
- **MaxOutputTokenCount**: Maximum number of tokens in the AI's response (default: 1000)

### System Message

The `SystemMessage` field defines the AI's role and behavior. You can customize it to:
- Set the AI's expertise area
- Define conversation style
- Provide context about the user
- Set specific instructions

## Security Notes

⚠️ **Important Security Reminders:**

1. **Never commit `appsettings.json`** - It contains your API key and is excluded from git via `.gitignore`
2. **Use `appsettings.example.json`** - This template file is safe to commit
3. **Keep your API key secure** - Don't share it publicly or commit it to version control
4. **Use environment variables or User Secrets** for production deployments

## Project Structure

```
AzureAI/
├── Program.cs                 # Main application entry point
├── AzureAI.csproj            # Project file with dependencies
├── appsettings.json          # Configuration file (not in git)
├── appsettings.example.json  # Configuration template (safe to commit)
└── README.md                 # This file
```

## Dependencies

- **Azure.AI.OpenAI** (2.2.0-beta.4) - Azure OpenAI SDK
- **Azure.Core** (1.50.0) - Azure Core library
- **Microsoft.Extensions.Configuration** - Configuration framework
- **Microsoft.Extensions.Configuration.Json** - JSON configuration provider
- **Microsoft.Extensions.DependencyInjection** - Dependency injection container

## Troubleshooting

### Error: "AzureOpenAI:Endpoint is not configured"
- Make sure `appsettings.json` exists and contains the `AzureOpenAI:Endpoint` field
- Verify the file is in the same directory as the executable

### Error: "HTTP 400 (invalid_request_error: unsupported_parameter)"
- Ensure you're using Azure.AI.OpenAI version 2.2.0-beta.4 or later
- Verify your deployment name matches your Azure OpenAI deployment

### Error: "API key is invalid"
- Double-check your API key in `appsettings.json`
- Ensure your Azure OpenAI resource is active and accessible

## License

See the LICENSE file in the root of the repository.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Ensure `appsettings.json` is not committed
5. Submit a pull request

