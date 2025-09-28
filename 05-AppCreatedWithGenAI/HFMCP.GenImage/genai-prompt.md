# GitHub Copilot Agent Prompt: Add Hugging Face MCP Server Integration to Blazor Web App

## Overview
Transform the existing .NET Aspire Blazor web application to integrate with the Hugging Face MCP (Model Context Protocol) Server for AI image generation and text responses.

## Context

- Starting with a .NET Aspire solution containing 4 projects: ApiService, AppHost, ServiceDefaults, and Web (Blazor)
- Web project currently has basic Weather and Counter pages (to be removed)
- Need to add AI capabilities using Hugging Face MCP Server
- Reference implementation: https://elbruno.com/2025/07/21/%f0%9f%a4%96-using-c-to-call-hugging-face-mcp-server-and-generate-ai-images/
- Sample code: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/03-CoreGenerativeAITechniques/src/MCP-01-HuggingFace/Program.cs

## Requirements

### 1. Home Page Enhancement (Main AI Interface)
- **Replace current Home.razor** with an AI interaction interface
- **User Input**: Text box for prompts (e.g., "create a pixelated image of a beaver")
- **Predefined Suggestions**: Include quick-start suggestion buttons:
  - `who am I`
  - `create a pixelated image of a beaver`
- **AI Response Display**: 
  - Show generated images when AI responds with images
  - Show text responses when AI responds with text
- **Chat History**: Display conversation history between user and AI
- **Loading States**: Show loading indicators during AI processing
- **Error Handling**: Display user-friendly error messages

### 2. Settings Page Implementation
Create a new Settings.razor page with secure configuration management:

#### Required Settings Fields

- **Hugging Face Access Token** (masked input, required)
- **Model Name** (text input, default: "gpt-4.1-mini") - allows user to type any model name

#### AI Provider Settings (Choose One)

Either configure **GitHub Models** OR **Azure OpenAI** (at least one required):

**GitHub Models (Optional)**:
- **GitHub Model Access Token** (masked input)

**Azure OpenAI (Optional)**:
- **Azure OpenAI service endpoint** (text input, not masked)
- **Azure OpenAI service ApiKey** (masked input)

#### Field Order

1. Hugging Face Access Token
2. Model Name
3. GitHub Models Access Token
4. Azure OpenAI (optional)

#### Settings Page UX Requirements

- **Save Confirmation**: Display success message when settings are saved
- **Visual Indicators**: Show configuration status badges for each service:
  - Green "Configured" badge when tokens are provided
  - Yellow "Required" badge for missing Hugging Face token
  - Clear indication of which services are ready to use
- **User Feedback**: Clear messaging about what was saved and current configuration state

#### Security Requirements

- Use ASP.NET Core's User Secrets for development
- Use secure configuration providers for production
- All sensitive data must be encrypted at rest
- Settings should persist between application restarts
- Implement validation for all inputs

### 3. Navigation Updates
- Remove Counter and Weather pages from the navigation and delete the page files
- Add Settings page to NavMenu.razor
- Update navigation to include AI-focused branding
- Ensure responsive design for both desktop and mobile

### 4. Technical Implementation

#### Required NuGet Packages:
```xml
<PackageReference Include="Microsoft.Extensions.AI" />
<PackageReference Include="Azure.AI.Inference" />
<PackageReference Include="Azure.AI.OpenAI" />
<PackageReference Include="ModelContextProtocol.Client" />
<PackageReference Include="System.ClientModel" />
```

#### Core Components to Implement:

1. **AI Service Layer**:
   - Create `IAIService` interface
   - Implement `AIService` with MCP client integration AND Azure OpenAI support
   - Support multiple AI providers (Azure OpenAI, GitHub Models, with fallback options)
   - Handle image generation and text responses
   - Prioritize Azure OpenAI when configured, fallback to GitHub Models

2. **Configuration Service**:
   - Create `IConfigurationService` interface
   - Implement secure settings storage and retrieval
   - Support environment-specific configurations
   - Validate that at least one AI provider (Azure OpenAI OR GitHub Models) is configured along with Hugging Face

3. **Azure OpenAI Integration**:
   - Support standard Azure OpenAI REST API endpoints
   - Handle proper authentication with API keys
   - Support deployment-based model calls
   - Implement error handling for Azure OpenAI specific errors

4. **MCP Integration**:
   - Initialize Hugging Face MCP client with proper authentication
   - Connect to MCP server: https://huggingface.co/mcp
   - Handle tool discovery and invocation
   - Implement proper error handling and retry logic

#### Key Implementation Patterns:
```csharp
// MCP Client Setup
var hfHeaders = new Dictionary<string, string>
{
    { "Authorization", $"Bearer {hfApiKey}" }
};
var clientTransport = new SseClientTransport(new()
{
    Name = "HF Server",
    Endpoint = new Uri("https://huggingface.co/mcp"),
    AdditionalHeaders = hfHeaders
});
await using var mcpClient = await McpClientFactory.CreateAsync(clientTransport);

// Chat Client with MCP Tools
var tools = await mcpClient.ListToolsAsync();
IChatClient client = GetChatClient(); // GitHub Models or Azure OpenAI
var chatOptions = new ChatOptions
{
    Tools = [.. tools],
    ModelId = deploymentName
};
```

#### Response Handling:
- Check response content type
- Display images with proper formatting and download options
- Format text responses with markdown support
- Handle streaming responses for better UX

### 5. User Experience Requirements

#### Home Page UX:
- Clean, modern chat interface
- Clear prompt input with placeholder text
- Responsive image display with zoom capabilities
- Copy/share functionality for generated content
- Clear visual distinction between user messages and AI responses

#### Settings Page UX:
- Intuitive form layout with proper labeling
- Password visibility toggles for sensitive fields
- Save confirmation messages
- Input validation with helpful error messages
- Quick setup buttons for common configurations (GitHub Models, Azure OpenAI, Ollama)

### 6. Error Handling & Validation
- Network connectivity issues
- Invalid API tokens
- Rate limiting from AI services
- Malformed responses
- Image loading failures
- Configuration validation

### 7. Security Considerations
- Never log sensitive tokens
- Implement proper CORS policies
- Use HTTPS for all external communications
- Validate all user inputs
- Implement rate limiting on AI requests
- Secure token storage using ASP.NET Core Data Protection

### 8. Testing Requirements
- Unit tests for AI service integration
- Integration tests for MCP client
- UI tests for critical user flows
- Configuration validation tests
- Error scenario testing

## Acceptance Criteria

1. ✅ User can enter prompts and receive AI responses
2. ✅ Generated images display properly in the interface
3. ✅ Text responses render with appropriate formatting
4. ✅ Settings page allows secure configuration of all required tokens
5. ✅ Settings persist between application restarts
6. ✅ Navigation includes new Settings page
7. ✅ Error states provide helpful feedback to users
8. ✅ Application handles both successful and failed AI requests gracefully
9. ✅ Security best practices are implemented for sensitive data
10. ✅ Responsive design works on desktop and mobile devices
11. ✅ **Azure OpenAI Integration**: Users can configure and use Azure OpenAI instead of GitHub Models
12. ✅ **Provider Prioritization**: Azure OpenAI is prioritized when both providers are configured
13. ✅ **Flexible Configuration**: Users can choose either GitHub Models OR Azure OpenAI (with Hugging Face required)
14. ✅ **Error Resolution**: Fixed "No configured AI provider available for chat" error when Azure OpenAI is configured
15. ✅ **MCP Tools Panel**: Collapsible panel showing Hugging Face MCP server tools when configured
16. ✅ **Proper MCP Integration**: Following Microsoft sample pattern from GitHub repository

## Implementation Status: COMPLETED ✅

### Final Session Completion:
- ✅ **Package Version Alignment**: Successfully updated to Microsoft.Extensions.AI 9.7.1 to match GitHub sample
- ✅ **Extension Method Resolution**: Resolved AsIChatClient() extension method availability issues
- ✅ **Microsoft.Extensions.AI.OpenAI**: Added preview package (9.7.1-preview.1.25365.4)
- ✅ **Microsoft.Extensions.AI.AzureAIInference**: Added preview package (9.7.1-preview.1.25365.4)
- ✅ **System.ClientModel**: Updated to version 1.4.2 to resolve package conflicts
- ✅ **Builder Pattern Implementation**: Using proper AsIChatClient().AsBuilder().UseFunctionInvocation().Build()
- ✅ **Runtime Testing**: Application builds clean and runs successfully
- ✅ **No Casting Errors**: Resolved InvalidCastException by using proper extension methods

### Key Technical Resolution:
- **Issue**: System.InvalidCastException when casting Azure OpenAI clients to IChatClient
- **Root Cause**: Direct casting instead of using Microsoft.Extensions.AI extension methods
- **Solution**: Used proper AsIChatClient() extension methods following the Microsoft GitHub sample pattern
- **Pattern**: chatClient.AsIChatClient().AsBuilder().UseFunctionInvocation().Build()
- **Verification**: Clean build with no warnings, application running successfully

### Previous Session Achievements:
- ✅ **Fixed MCP Package Issues**: Updated to use correct ModelContextProtocol packages (v0.3.0-preview.3)
- ✅ **MCP Service Implementation**: Created proper MCP service following GitHub sample pattern
- ✅ **AI Service Refactoring**: Complete rewrite to use MCP integration with Microsoft.Extensions.AI
- ✅ **Home Page Enhancement**: Added collapsible MCP tools panel
- ✅ **Application Architecture**: Complete implementation following Microsoft patterns

### Key Features Implemented:
1. **Proper MCP Integration**: Following exact pattern from Microsoft GitHub sample
2. **SseClientTransport**: Server-Sent Events transport for MCP communication
3. **McpClientFactory**: Proper MCP client initialization
4. **Function Invocation**: Microsoft.Extensions.AI pattern with MCP tools
5. **Dual Provider Support**: Azure OpenAI and GitHub Models with MCP tools
6. **Tools Panel**: Collapsible UI showing available Hugging Face MCP tools
7. **Error Handling**: Comprehensive error handling for MCP operations
8. **Image Display**: Automatic detection and display of images from AI responses
9. **Configurable MCP Server**: Settings allow customization of Hugging Face MCP server endpoint

### Latest Enhancements (Current Session):
- ✅ **Smart Image Detection**: Automatically extracts image URLs from AI responses using multiple patterns:
  - Markdown image syntax: `![alt text](url)`
  - Direct HTTP/HTTPS URLs with image extensions
  - Hugging Face specific URLs (e.g., evalstate-flux1-schnell.hf.space)
- ✅ **Enhanced Image Display**: 
  - Proper image sizing and responsive design
  - View full size and copy URL functionality
  - Error handling for failed image loads
  - Removes markdown syntax from text when images are displayed separately
- ✅ **Configurable MCP Server**: Added setting for Hugging Face MCP Server with default value `https://huggingface.co/mcp`
- ✅ **Settings Page Enhancement**: Added Hugging Face MCP Server field to configuration
- ✅ **Code Consistency**: Updated all services to use the configurable MCP server endpoint

## Implementation Priority: COMPLETED ✅
1. ✅ **Phase 1**: Basic MCP integration and Home page AI interface
2. ✅ **Phase 2**: Settings page with secure configuration
3. ✅ **Phase 3**: Enhanced UX, error handling, and responsive design
4. ✅ **Phase 4**: Image detection, display, and MCP server configuration

## Additional Notes
- Follow .NET Aspire patterns and conventions ✅
- Ensure compatibility with existing ServiceDefaults ✅
- Maintain clean separation of concerns ✅
- Use dependency injection throughout ✅
- Implement proper logging for debugging and monitoring ✅
- Consider performance implications of image generation and display ✅
- **MCP Integration**: Follows Microsoft's recommended patterns for Model Context Protocol ✅
- **Package Versions**: Uses stable and preview packages as specified in Microsoft samples ✅
- **Image Processing**: Smart extraction and display of images from AI responses ✅
- **User Experience**: Clean image gallery with actions for viewing and copying ✅

This implementation successfully transforms the basic Aspire starter into a fully functional AI-powered image generation and chat application with proper MCP integration, smart image detection and display, while maintaining security and user experience best practices.