using Azure.AI.Inference;
using OpenAI.Chat;

namespace McpSample.BlazorChat.Services;

public static class ChatMessageHelper
{
    public static IList<ChatRequestMessage> GetSampleChatMessages ()
    {
        IList<ChatRequestMessage> messages =
        [
            new ChatRequestUserMessage("2+2"),
            new ChatRequestAssistantMessage(@"<think>
Okay, the user asked ""2+2"". Let me think. That's a basic arithmetic question. Let's confirm the answer step by step.

First, adding 2 and 2. When you put two things together with another two things, you end up with four things. So 2 plus 2 equals 4. 

Wait, is there any chance of a trick here? The question seems straightforward. Maybe the user is testing if I can handle simple math. I should make sure there's no hidden element. No, it's just addition. 

Another way to check: 2 is a number, adding another 2 increments it by 2. Starting at 2, adding 1 makes 3, adding another 1 makes 4. Yep, that's correct. 

I should also consider different number systems, but unless specified, the default is base 10. In base 10, 2+2=4. In other bases like base 3, 2+2 would be 11, but that's probably not the case here. The user didn't mention any base, so I'll stick with the standard answer. 

Alright, confident the answer is 4. Let me present it clearly.
</think>

The sum of 2 + 2 is **4**. 

Let me know if you have more questions! 😊"),
            new ChatRequestUserMessage("3+3"),
            new ChatRequestAssistantMessage(@"<think>

Okay, the user asked ""3+3"" after previously getting an answer for ""2+2"". They probably want a quick answer again. Let me check the calculation first. 3 plus 3 is definitely 6. The last response was friendly with an emoji and an offer to help more. I should keep the same tone. Maybe add the bold for the number and the emoji again. Short and helpful, no need for extra info unless they ask. Yep, that's it.

Alright, I'll format it similarly. Start with ""The sum of 3 + 3 is **6**."" Then the same friendly message. Make sure the emoji is there to keep it consistent. Quick response, no mistakes. Double-check the math. Yep, 6 is correct. All set.
</think>

The sum of 3 + 3 is **6**.  

Feel free to ask if you need anything else! 😊"),
        ];
        return messages;
    }
}
