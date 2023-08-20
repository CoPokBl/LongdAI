using System.Collections.Generic;
using System.Linq;

public class ChatCompletionBody {
    public string model;
    public OpenAIMessage[] messages;

    public ChatCompletionBody(string systemPrompt, IEnumerable<Message> msgs, string model = "gpt-3.5-turbo") {
        this.model = model;
        OpenAIMessage systemPromptMessage = new() {
            role = "system",
            content = systemPrompt
        };
        messages = msgs.Reverse().Take(10).Select(m => new OpenAIMessage(m)).Reverse().Append(systemPromptMessage).ToArray();
    }
    
    // Unity's JSON utility is so bad that I'm just going to write my own
    public string ToJson() {
        string json = "{";
        json += $"\"model\": \"{model}\",";
        json += "\"messages\": [";
        json += string.Join(",", messages.Select(m => $"{{\"role\": \"{m.role}\", \"content\": \"{m.content}\"}}"));
        json += "]}";
        return json;
    }
}

public class OpenAIMessage {
    public string role;
    public string content;

    public OpenAIMessage(Message msg) {
        role = msg.Sender == MessageSender.User ? "user" : "assistant";
        content = msg.Text;
        JsonSanitise();
    }
    
    public OpenAIMessage() { }
    
    public Message ToMessage() {
        return new Message {
            Sender = role == "user" ? MessageSender.User : MessageSender.Bot,
            Text = content
        };
    }
    
    private void JsonSanitise() {
        content = content.Replace("\"", "").Replace("\\", "");
    }
}