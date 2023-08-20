/// <summary>
/// A message sender is either the user or the AI.
/// </summary>
public class Message {
    public MessageSender Sender { get; set; }
    public string Text { get; set; } = null!;
}