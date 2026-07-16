# Vision-03: Video analysis with Azure OpenAI

This sample extracts frames from a **local video file** using [OpenCvSharp](https://github.com/shimat/opencvsharp) and sends them to an Azure OpenAI vision-capable model (via [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)) to describe what happens in the video.

## Provide your own video

> [!IMPORTANT]
> Sample video files are **not** included in this repository to keep it lightweight. You need to supply your own `.mp4` clip before running the sample.

The sample looks for a folder named `videos` by walking up from the current directory (see `VideosHelper.cs`). By default it uses a file named `firetruck.mp4`.

To run it:

1. Create a `videos` folder. A good location is `samples/CoreSamples/videos` (it will be found automatically), for example:

   ```bash
   mkdir samples/CoreSamples/videos
   ```

2. Add a short `.mp4` clip named `firetruck.mp4` to that folder (any short video works — smaller/shorter clips process faster):

   ```
   samples/CoreSamples/videos/firetruck.mp4
   ```

   `VideosHelper` also has helpers for `racoon.mp4` and `insurance_v3.mp4` if you want to switch the source video in `Program.cs`.

If no video is found, the sample prints a friendly message and exits instead of failing.

## Configure Azure OpenAI

Set the following [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) for the project:

```bash
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<your-endpoint>"
dotnet user-secrets set "AzureOpenAI:Deployment" "<your-vision-model-deployment>"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "<your-api-key>"
```

> Use a vision-capable deployment (for example `gpt-4o` or `gpt-4o-mini`).

## Run

```bash
cd samples/CoreSamples/Vision-03MEAI-AOAI
dotnet run
```

The sample extracts frames, saves them under a local `data/frames` folder, and prints the model's description of the video.
