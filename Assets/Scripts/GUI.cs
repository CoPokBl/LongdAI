using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// This script should be attached to a single object in the main GUI scene.
/// It is responsible for managing the GUI.
/// </summary>
public class GUI : MonoBehaviour {
    private List<Message> _messages;
    private string _systemPrompt;
    
    // Editor Fields
    [FormerlySerializedAs("Messages")] public TMP_Text messages;
    [FormerlySerializedAs("MessageInput")] public TMP_InputField messageInput;
    public Image avatar;
    
    private void Start() {
        Debug.Log("Hello, world!");
        
        // Load Messages
        _messages = Storage.LoadMessages().ToList();
        _messages.ForEach(RenderMessage);
        
        // Avatar
        string avatarName = Storage.GetPrefs().GetValueOrDefault("avatar") ?? "dog";
        avatar.sprite = State.Instance.avatars.Single(a => a.name == avatarName).avatar;
    }

    private void RenderMessage(Message msg) {
        messages.text += $"\n{msg.Sender}: {msg.Text}";
    }
    
    public void ResetMessages() {
        Storage.SaveMessages(Array.Empty<Message>());
        messages.text = "";
    }

    /// <summary>
    /// Triggered whenever the send message button is clicked. (Including when enter is pressed)
    /// </summary>
    public void SendMessageButtonClicked() {
        string text = messageInput.text;
        messageInput.text = "";

        if (text.ToLower() == "i have friends") {
            // Omg they have friends
            // They win
            Win();
            return;
        }
        
        // Save message
        _messages.Add(new Message {
            Sender = MessageSender.User,
            Text = text
        });
        Storage.SaveMessages(_messages.ToArray());
        
        // Render message
        RenderMessage(new Message {
            Sender = MessageSender.User,
            Text = text
        });
        
        GetResponse();  // Get response from OpenAI
        
        // Focus input box for ease of use
        messageInput.ActivateInputField();
        messageInput.Select();
    }
    
    /// <summary>
    /// Listening for enter being pressed.
    /// </summary>
    private void OnGUI() {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
            SendMessageButtonClicked();
        }
    }

    /// <summary>
    /// Called whenever the user says they have friends.
    /// It's a fun easter egg.
    /// </summary>
    private void Win() {
        Debug.Log("OMG YOU HAVE FRIENDS");
        SceneManager.LoadScene("Win");
    }

    /// <summary>
    /// This is added to the settings button in the inspector.
    /// </summary>
    public void Settings() {
        SceneManager.LoadScene("Settings");
    }
    
    /// <summary>
    /// Starts the process of getting a response from the OpenAI API.
    /// </summary>
    private void GetResponse() {
        StartCoroutine(GetChatCompletion(ResponseObtained));
    }

    /// <summary>
    /// This is the callback that is executed when the response is obtained from the OpenAI API.
    /// </summary>
    /// <param name="result">The message object that we got from OpenAI</param>
    private void ResponseObtained(Message result) {
        _messages.Add(result);
        Storage.SaveMessages(_messages.ToArray());
        RenderMessage(result);
    }
    
    /// <summary>
    /// The coroutine that is responsible for getting the response from the OpenAI API.
    /// </summary>
    /// <param name="callback">The callback to run when the request completes.</param>
    /// <returns>The IEnumerator for the coroutine.</returns>
    /// <exception cref="WtfException">This should never be thrown, if it is then something is really messed up.</exception>
    private IEnumerator GetChatCompletion(Action<Message> callback) {
        // prepare the request
        UnityWebRequest www = new("https://api.openai.com/v1/chat/completions", "POST");
        Debug.Log("Token: " + Storage.GetOpenAiToken());
        www.SetRequestHeader("Authorization", "Bearer " + Storage.GetOpenAiToken());
        www.SetRequestHeader("Content-Type", "application/json");

        ChatCompletionBody body = new(GetSystemPrompt(), _messages, "gpt-4");
        Debug.Log("Message length: " + body.messages.Length);
        
        // convert the body data to json and put it in the request
        string bodyJson = body.ToJson();
        Debug.Log(bodyJson);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(bodyJson);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        // Wait for the response
        yield return www.SendWebRequest();
        if (!www.isDone) {
            throw new WtfException("UnityWebRequest is not done but it should be");
        }

        // If there are network errors
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError(www.error);
            yield break;
        }
        string json = www.downloadHandler.text;
        Debug.Log(json);
        ChatCompletionResponse response = ChatCompletionResponse.FromJson(json);
        callback(response.choices[0]);
    }

    /// <summary>
    /// Get the system prompt that is used to instruct the AI.
    /// </summary>
    /// <returns>The prompt.</returns>
    private string GetSystemPrompt() {
        if (_systemPrompt != null) {
            return _systemPrompt;
        }
        
        Dictionary<string, string> prefs = Storage.GetPrefs();
        string prompt = "You are a chatbot designed to help people feel less lonely and improve their mental health. ";
        prompt +=       "You are speaking in a chat format so don't use newlines or markdown, and try and keep your responses short. ";
        prompt +=       "You should act with the following attributes; ";
        prompt +=       "Humour: " + PersonalityLevelToLabel(int.Parse(prefs.GetValueOrDefault("Humor") ?? "1")) + ", ";
        prompt +=       "Empathy: " + PersonalityLevelToLabel(int.Parse(prefs.GetValueOrDefault("Empathy") ?? "1")) + ", ";
        prompt +=       "Intelligence: " + PersonalityLevelToLabel(int.Parse(prefs.GetValueOrDefault("Smarts") ?? "1")) + ", ";
        prompt +=       "Creativity: " + PersonalityLevelToLabel(int.Parse(prefs.GetValueOrDefault("Creativity") ?? "1")) + ", ";
        prompt +=       "You are a " + prefs.GetValueOrDefault("avatar") + ".";
        _systemPrompt = prompt;
        return prompt;
    }

    /// <summary>
    /// Convert the numbers from the sliders to labels that can be used in the system prompt.
    /// </summary>
    /// <param name="level">The integer value of the slider.</param>
    /// <returns>A label for the value.</returns>
    private static string PersonalityLevelToLabel(int level) {
        return level switch {
            0 => "low",
            1 => "medium",
            2 => "high",
            _ => "medium"
        };
    }
    
}
