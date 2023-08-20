using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is responsible for managing the win screen.
/// It should be attached to a single object in the win scene.
/// All the functions here are called by the buttons in the win scene.
/// </summary>
public class Win : MonoBehaviour {

    public void Chat() {
        SceneManager.LoadScene("Chat");
    }

    public void Life() {
        Application.Quit();
        Debug.Log("The application has quit.");
    }
    
}
