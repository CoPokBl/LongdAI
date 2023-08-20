using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// This class is used to store the response from the OpenAI API. It does not have all the returned information
/// just the information that is needed for the chat.
/// </summary>
public class ChatCompletionResponse {
    public ProxiedMessage[] choices;
    
    // Unity's JSON utility is so bad that I'm just going to write my own
    // This is terrible but I'm getting frustrated with Unity's JSON utility
    public static ChatCompletionResponse FromJson(string json) {
        ChatCompletionResponse chatCompletionResponse = new();

        const string choicePattern = "\"choices\":\\s*\\[(.*?)\\]";
        string choiceMatch = Regex.Match(json, choicePattern, RegexOptions.Singleline).Groups[1].Value;

        string[] choicesElements = choiceMatch.Split(new[] { "}{" }, StringSplitOptions.None);

        List<ProxiedMessage> choicesList = new();

        foreach (string choiceElement in choicesElements) {            
            ProxiedMessage proxiedMessage = new();

            const string messagePattern = "\"message\":\\s*\\{(.*?)\\}";
            string messageMatch = Regex.Match(choiceElement, messagePattern, RegexOptions.Singleline).Groups[1].Value;

            const string rolePattern = "\"role\":\\s*\"(.*?)\"";
            string roleMatch = Regex.Match(messageMatch, rolePattern, RegexOptions.Singleline).Groups[1].Value;

            const string contentPattern = "\"content\":\\s*\"(.*?)\"";
            string contentMatch = Regex.Match(messageMatch, contentPattern, RegexOptions.Singleline).Groups[1].Value;

            proxiedMessage.message = new OpenAIMessage() { role = roleMatch, content = contentMatch };
    
            choicesList.Add(proxiedMessage);
        }

        chatCompletionResponse.choices = choicesList.ToArray();

        return chatCompletionResponse;
    }
}

/// <summary>
/// This is the result of me trying to use Unity's JSON utility. It's terrible.
/// I needed it because the API response contains the actual message in a nested object.
/// </summary>
public class ProxiedMessage {
    public OpenAIMessage message;
    
    public static implicit operator OpenAIMessage(ProxiedMessage msg) {
        return msg.message;
    }
    
    public static implicit operator Message(ProxiedMessage msg) {
        return msg.message.ToMessage();
    }
}