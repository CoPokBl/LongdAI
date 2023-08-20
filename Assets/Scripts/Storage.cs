/*
Custom File Format:

User~Hello there this is a message(u200b)Bot~I am a bot fdks]sd[f]s[df]p%$^&*&R%E^

Sender and message are split by (u200b) (zero width space) and messages are split by \n (new line)
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Responsible for storing everything.
/// This is a static class and should not be instantiated.
/// </summary>
public static class Storage {
    private const string SplitToken = "​";  // There is a zero-width space here (u200b)
    private static string _openAiToken = null!;  // Cached openai token
    
    /// <summary>
    /// Load messages from the file into an array
    /// </summary>
    /// <returns>An array of Messages that were retrieved from file.</returns>
    public static Message[] LoadMessages() {
        if (!File.Exists("msgs.conf")) {
            File.WriteAllText("msgs.conf", "");
        }

        string[] content = File.ReadAllLines("msgs.conf");
        
        if (content.Length == 0) {
            return Array.Empty<Message>();
        }

        return (from line in content select line.Split(SplitToken) into split where split.Length == 2 select new Message {
            Sender = split[0] == "User" ? MessageSender.User : MessageSender.Bot, 
            Text = split[1].Replace("\\n", "\n")
        }).ToArray();
    }
    
    /// <summary>
    /// Save messages to the file
    /// </summary>
    /// <param name="messages">The messages to save</param>
    public static void SaveMessages(IEnumerable<Message> messages) {
        File.WriteAllLines("msgs.conf", messages
            .Reverse()
            .Take(20)
            .Reverse()
            .Select(message => $"{(message.Sender == MessageSender.User ? "User" : "Bot")}{SplitToken}{message.Text.Replace("\n", "\\n")}"));
    }
    
    /// <summary>
    /// Gets the OpenAI token from the environment variable or from the file.
    /// </summary>
    /// <returns>The provided token.</returns>
    public static string GetOpenAiToken() {
        if (_openAiToken != null) {
            return _openAiToken;
        }
        Debug.Log("CD: " + Directory.GetCurrentDirectory());
        _openAiToken = !File.Exists("openai-token.txt") ? Environment.GetEnvironmentVariable("OPENAI_TOKEN") : File.ReadAllText("openai-token.txt").Replace("\n", "");
        return _openAiToken;
    }
    
    /// <summary>
    /// Get user preferences from the file.
    /// </summary>
    /// <returns>Dictionary of preferences.</returns>
    public static Dictionary<string, string> GetPrefs() {
        if (!File.Exists("prefs.conf")) {
            return new Dictionary<string, string>();
        }

        string[] content = File.ReadAllLines("prefs.conf");
        return content.Where(line => line.Contains(SplitToken)).ToDictionary(line => line.Split(SplitToken)[0], line => line.Split(SplitToken)[1]);
    }
    
    /// <summary>
    /// Write user preferences to the file.
    /// </summary>
    /// <param name="prefs">A dictionary of the preferences.</param>
    public static void SetPrefs(Dictionary<string, string> prefs) {
        File.WriteAllLines("prefs.conf", prefs.Select(pref => $"{pref.Key}{SplitToken}{pref.Value}"));
    }
    
}