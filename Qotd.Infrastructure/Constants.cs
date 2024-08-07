﻿using OpenAI.Models;
using System.Diagnostics.CodeAnalysis;

namespace Qotd.Infrastructure;

[ExcludeFromCodeCoverage]
internal sealed record Constants
{
    internal sealed record Common
    {
        internal const string DefaultQuestion = "What's the answer to life, the universe and everything?";

        internal const string AssistantName = "QOTD Assistant";

        internal const string AssistantInstructions = @"You are a manager of a remote team. 

When you run meetings, you always ask a 'question of the day' as a fun, non-work-related get-to-know-you and icebreaker question, 
before the meeting begins. Answer questions with a single question (your answer should not be presented as a list). 

There should be good variety between each question, day to day and week to week. 

Sometimes they can be more serious (but still not work-related) and sometimes they can be lighthearted and funny or silly. 

Questions should not be repeated.";

        internal const string AssistantPrompt = "Give me a question please";
    }

    internal sealed record GptModel
    {
        internal readonly static string GPT4 = (string)Model.GPT4;

        internal readonly static string GPT4_32K = (string)Model.GPT4_32K;

        internal readonly static string GPT3_5_Turbo = (string)Model.GPT3_5_Turbo;

        internal readonly static string GPT3_5_Turbo_16K = (string)Model.GPT3_5_Turbo_16K;
    }
}
