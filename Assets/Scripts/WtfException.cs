/// <summary>
/// For when something happens that should never happen.
/// If you get this then you have done something very wrong.
/// </summary>
public class WtfException : System.Exception {
    public WtfException(string message) : base(message) { }
}