# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Comprehensive error handling for API calls
  - Rate limit (429) detection and user-friendly messages
  - Authentication error (401) handling
  - Deployment not found (404) handling
  - Network error handling
  - Timeout handling
- Configuration validation with clear error messages
  - URL format validation for endpoint
  - API key validation (checks for placeholder values)
  - Token count validation (must be positive integer)
- Conversation history limit to prevent memory issues (max 50 messages)
- Constants for magic strings (exit commands, bot name, etc.)
- Welcome message and improved user prompts
- Build support without appsettings.json file
- GitHub Actions CI/CD workflow for automated builds
- LICENSE file (MIT License)
- CONTRIBUTING.md with contribution guidelines
- CODE_REVIEW.md with comprehensive code review findings
- CHANGELOG.md (this file)

### Changed
- Configuration loading now supports both appsettings.json and appsettings.example.json
- Refactored main application logic into separate RunChatLoop method
- Improved ConfigureServices with comprehensive validation
- Enhanced error messages with emoji indicators (❌)
- Application now recovers from errors instead of crashing

### Fixed
- Build failure when appsettings.json doesn't exist
- Application crashes on network errors
- Application crashes on API errors
- Unlimited conversation history causing potential memory issues

## [1.0.0] - Initial Release

### Added
- Console-based chatbot using Azure OpenAI GPT-5-mini
- Dependency injection architecture
- Configuration-based setup with appsettings.json
- Continuous conversation loop
- Support for custom system messages
- Proper .gitignore for sensitive files
- Comprehensive README with setup instructions
