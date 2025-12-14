# Contributing to AIChatbot

Thank you for your interest in contributing to AIChatbot! This document provides guidelines for contributing to the project.

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for all contributors.

## How to Contribute

### Reporting Bugs

If you find a bug, please create an issue with the following information:
- A clear, descriptive title
- Steps to reproduce the issue
- Expected behavior
- Actual behavior
- Your environment (OS, .NET version, etc.)
- Any relevant logs or error messages

### Suggesting Enhancements

Enhancement suggestions are welcome! Please create an issue with:
- A clear, descriptive title
- Detailed description of the proposed enhancement
- Rationale for why this enhancement would be useful
- Any examples or mockups if applicable

### Pull Requests

1. **Fork the repository** and create your branch from `main`
2. **Make your changes** following the coding standards below
3. **Test your changes** - ensure the application builds and runs correctly
4. **Update documentation** if you've changed functionality
5. **Ensure appsettings.json is not committed** - only commit appsettings.example.json
6. **Submit a pull request** with a clear description of changes

## Coding Standards

### C# Guidelines

- Follow standard C# naming conventions
- Use meaningful variable and method names
- Add comments for complex logic
- Keep methods focused and concise
- Use constants for magic strings and numbers
- Handle exceptions appropriately
- Use async/await for asynchronous operations where appropriate

### Code Quality

- Maintain existing code structure and patterns
- Write clean, readable code
- Add error handling for edge cases
- Validate input parameters
- Use dependency injection where appropriate

### Security

- **Never commit API keys or sensitive data**
- Always use configuration files for sensitive information
- Follow the existing .gitignore patterns
- Use appropriate authentication methods
- Validate and sanitize user input where necessary

## Project Structure

```
AIChatbot/
├── .github/
│   └── workflows/      # CI/CD workflows
├── AzureAI/
│   ├── Program.cs      # Main application
│   ├── AzureAI.csproj  # Project file
│   └── appsettings.example.json  # Configuration template
├── CODE_REVIEW.md      # Code review documentation
├── LICENSE             # Project license
└── README.md           # Project readme
```

## Development Setup

1. Clone the repository
2. Install .NET 10.0 SDK or later
3. Copy `appsettings.example.json` to `appsettings.json`
4. Configure your Azure OpenAI credentials
5. Build with `dotnet build`
6. Run with `dotnet run`

## Testing

Currently, the project does not have automated tests. If you're adding tests:
- Use xUnit as the testing framework
- Follow the naming convention: `[ClassName]Tests.cs`
- Place tests in a separate `AzureAI.Tests` project
- Mock external dependencies (Azure OpenAI API)

## Commit Messages

- Use clear, descriptive commit messages
- Start with a verb in present tense (e.g., "Add", "Fix", "Update")
- Keep the first line under 72 characters
- Add detailed description if necessary

Examples:
```
Add error handling for API rate limits
Fix build issue when appsettings.json is missing
Update README with new configuration options
```

## Review Process

1. All pull requests require review before merging
2. Reviewers will check for:
   - Code quality and adherence to standards
   - Proper error handling
   - Security considerations
   - Documentation updates
   - Build success

## Questions?

If you have questions about contributing, feel free to open an issue for discussion.

Thank you for contributing to AIChatbot! 🎉
