/// <summary>
/// Helper class for prompts related to video analysis.
/// </summary>
public static class PromptsHelper
{
    /// <summary>
    /// Defines the number of frames that the model will analyze.
    /// </summary>
    public static int NumberOfFrames = 10;

    /// <summary>
    /// The system prompt for the assistant.
    /// </summary>
    public static string SystemPrompt = @"You are a useful assistant. When you receive a group of images, they are frames of a unique video.";

    /// <summary>
    /// The user prompt to describe the whole video story.
    /// </summary>
    public static string UserPromptDescribeVideo = @"The attached frames represent a video. Describe the whole video story, do not describe frame by frame.";

    /// <summary>
    /// The user prompt to create an incident report for car damage analysis.
    /// </summary>
    public static string UserPromptInsuranceCarAnalysis = @"You are an expert in evaluating car damage from car accidents for auto insurance reporting. 
Create an incident report for the accident shown in the video with 3 sections. 
- Section 1 will include the car details (license plate, car make, car model, approximate model year, color, mileage).
- Section 2 list the car damage, per damage in a list.
- Section 3 will only include exactly 6 sentence description of the car damage.";

    public static string UserPromptInsuranceCarAnalysisPhi35 = @"You are an expert in evaluating car damage from car accidents for auto insurance reporting. 
Create an incident report for the accident shown in the video with 3 sections. 
- Section 1 will include the car details (license plate, car make, car model, approximate model year, color, mileage).
- Section 2 list the car damage, per damage in a list.
- Section 3 will only include exactly 6 sentence description of the car damage.
====================================
The texts below represets a video analysis from different frames from the video. ";

    /// <summary>
    /// The user prompt to create an incident report for car damage analysis and return a JSON object.
    /// </summary>
    public static string UserPromptInsuranceCarAnalysisJson = @"You are an expert in evaluating car damage from car accidents for auto insurance reporting. 
Create an incident report for the accident shown in the video with 3 sections in JSON format. 
- Section 1 will include the car details (license plate, car make, car model, approximate model year, color, mileage).
- Section 2 list the car damage, per damage in a list.
- Section 3 will only include exactly 6 sentence description of the car damage.";
}
