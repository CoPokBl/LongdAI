using UnityEngine;

/// <summary>
/// This class is used to store the state of the application between scenes.
/// It will not be destroyed when a new scene is loaded.
/// </summary>
public class State : MonoBehaviour {
    
    // ========================================
    //            Singleton Stuff
    // ========================================
    public static State Instance;
    
    private void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    
    // ========================================
    //            State Variables
    // ========================================
    public AvatarProfile[] avatars;

}
