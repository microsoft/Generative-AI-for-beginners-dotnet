# SpaceAINet

> **Note:** The core functionality of this game was created using [this prompt](https://aka.ms/spaceainet/prompt) and GitHub Copilot Agent Mode with GPT-4.1. For an overview and walkthrough on how to create and use the game, check out our [YouTube video](https://www.youtube.com/watch?v=XLg9Qt61RVs).

**SpaceAINet** is an AI-powered Space Battle game for .NET 9, designed to showcase how modern AI models can play classic games. The solution allows you to run the game with either local AI models (via Ollama) or cloud-based models (via Azure AI Foundry), which analyze the game state and predict the next best action to win.

![Demo of the game running with Azure OpenAI models](./images/01-demo.gif)

## Solution Structure

- **SpaceAINet.Console**: Main console game project. Handles game logic, rendering, user/AI input, and the main game loop.
- **SpaceAINet.GameActionProcessor**: Library for integrating AI models (Ollama or Azure AI Foundry) to analyze game frames and suggest actions.
- **SpaceAINet.Screenshot**: Provides screenshot capture functionality for the game screen.
- **SpaceAINet.ServiceDefaults**: Contains shared configuration and service defaults for the solution.

## Requirements

- [.NET 9 or newer](https://dotnet.microsoft.com/download/)
- **For local AI (Ollama):**
  - [Ollama](https://ollama.com/) running locally (default: `http://localhost:11434`)
  - At least one supported model pulled (e.g., `ollama run phi4-mini`)
- **For cloud [Azure AI Foundry](https://ai.azure.com/) (using OpenAI models):**
  - Access to Azure OpenAI with a deployed chat model
  - Azure AI endpoint, model name, and API key

## Setup

### 1. Clone the repository

```bash
git clone <this-repo-url>
cd SpaceAINet
```

### 2. Configure Azure OpenAI (optional)

If you want to use Azure OpenAI, set up [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) for the main project:

```bash
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<your-endpoint>"
dotnet user-secrets set "AZURE_OPENAI_MODEL" "<your-model-name>"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "<your-api-key>"
```

### 3. Run Ollama (optional)

If you want to use local models, install and start Ollama:

- [Install Ollama](https://ollama.com/download)
- Pull a model: `ollama pull phi4-mini`
- Start Ollama (it usually runs at `http://localhost:11434`)

### 4. Build and run the game

```bash
cd SpaceAINet.Console
dotnet build
dotnet run
```

## Usage

| Key(s)                | Behavior/Action                                              |
|-----------------------|-------------------------------------------------------------|
| LEFT / RIGHT Arrows   | Move your ship left or right                                 |
| SPACE                 | Shoot                                                        |
| Q                     | Quit the game                                               |
| A                     | Toggle Azure OpenAI AI mode (if configured)                 |
| O                     | Toggle Ollama AI mode (if Ollama is running)                |
| F                     | Toggle FPS display (shows both game FPS and AI FPS)         |
| S                     | Save a screenshot of the current game frame (any mode)      |
| 1 / 2 / 3             | Select game speed (1 = Slow, 2 = Medium, 3 = Fast) at start |
| ENTER                 | Start game with default speed                               |

You can switch between manual, Azure OpenAI, and Ollama AI modes at any time during gameplay. All tool keys (F, S) work in any mode.

## How it works

- The game sends the last three frames and the last action to the selected AI provider.
- The AI model returns the next action (`move_left`, `move_right`, `fire`, or `stop`) and an explanation.
- The game applies the action and displays the AI's reasoning.

## Notes

- **Screenshot Capture:** Press `S` at any time to save a screenshot of the current game frame. The screenshot captures both the character and color buffers, preserving the visual state of the game. Screenshots are saved using the `ScreenshotService`.
- **FPS Display:** Press `F` to toggle the FPS display. Two values are shown:
  - **Game FPS:** The number of game frames rendered per second (all rendering, regardless of AI involvement).
  - **AI FPS:** The number of frames processed by the AI per second (only increments when AI mode is active and the AI processes a frame).
- For best results, ensure your chosen AI provider is running and properly configured before starting the game.

### Feedback

If you encounter any issues or have suggestions, please report them to the repository's issue tracker.

---

Enjoy experimenting with AI-powered Retro Invaders!
