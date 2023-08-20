using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for managing the settings scene.
/// It should be attached to a single object in the settings scene.
/// </summary>
public class Settings : MonoBehaviour {
    /// <summary>
    /// What options should be supplied to the background colour dropdown.
    /// This should line up with the background colours in Assets/Scripts/BackgroundManager.cs.
    /// </summary>
    private readonly string[] bgColors = {"White", "Green", "Blue", "Red", "Yellow", "Pink"};
    
    public TMP_Dropdown bgColorDropdown;
    public AvatarSetButtonProfile[] avatarSetButtons;
    public Slider[] personalitySliders;
    
    private void Start() {
        // Load settings
        Dictionary<string, string> prefs = Storage.GetPrefs();
        
        // Background Color
        string bgColor = prefs.GetValueOrDefault("bgColour") ?? "white";
        bgColorDropdown.options = bgColors.ToList().ConvertAll(color => new TMP_Dropdown.OptionData(color));
        bgColorDropdown.value = Array.IndexOf(bgColors, bgColor);
        
        // Start avatar button press listener
        foreach (AvatarSetButtonProfile avatarSetButton in avatarSetButtons) {
            Button button = avatarSetButton.button;
            button.onClick.AddListener(() => {
                SaveSettings(avatarSetButton.name);
                Debug.Log("Pressed button: " + avatarSetButton.name);
            });
        }
        
        // Load sliders
        foreach (Slider slider in personalitySliders) {
            slider.value = float.Parse(prefs.GetValueOrDefault(slider.name) ?? "1");
        }
        
        // Start slider change listeners
        foreach (Slider slider in personalitySliders) {
            slider.onValueChanged.AddListener(value => {
                SaveSettings();
                Debug.Log("Slider changed: " + value);
            });
        }
    }
    
    /// <summary>
    /// This exists so that the function can still be used in the inspector.
    /// </summary>
    public void SaveSettings() {
        SaveSettings(null);
    }
    
    /// <summary>
    /// Save all settings on the page to file.
    /// </summary>
    /// <param name="avatarChanged">Only set this if the avatar has been changed, this should be set to the ID of the avatar to change to.</param>
    public void SaveSettings([CanBeNull] string avatarChanged = null) {
        // Background Color
        string bgColor = bgColors[bgColorDropdown.value];
        Dictionary<string, string> prefs = Storage.GetPrefs();
        prefs["bgColour"] = bgColor;
        
        // Avatar
        if (avatarChanged != null) {
            prefs["avatar"] = avatarChanged;
        }
        
        // Personality
        foreach (Slider slider in personalitySliders) {
            prefs[slider.name] = ((int) slider.value).ToString();
        }
        
        // Save settings
        Storage.SetPrefs(prefs);
        Debug.Log("Settings saved.");
        
        // Send update to background manager
        GetComponent<BackgroundManager>().UpdateColour();
    }

    /// <summary>
    /// Used in the inspector for the back button.
    /// </summary>
    public void Back() {
        SceneManager.LoadScene("Chat");
    }
}
