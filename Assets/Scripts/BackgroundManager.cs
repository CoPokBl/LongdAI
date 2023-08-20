using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class it responsible for managing the background colour. It should be attached to one object in each scene that 
/// needs a custom background colour.
/// </summary>
public class BackgroundManager : MonoBehaviour {
    
    /// <summary>
    /// The colour mappings of the name to the actual colour object.
    /// </summary>
    private static readonly Dictionary<string, Color> Colours = new() {
        {"White", Color.white},
        {"Green", Color.green},
        {"Blue", Color.cyan},
        {"Red", Color.red},
        {"Yellow", Color.yellow},
        {"Pink", Color.magenta},
    };
    
    public Image[] backgrounds;
    public float darkness = 1f;  // This will be multiplied by the colour to make it darker
    
    private void Start() {
        UpdateColour();
    }

    public void UpdateColour() {
        string colour = Storage.GetPrefs().GetValueOrDefault("bgColour") ?? "White";
        foreach (Image background in backgrounds) {
            background.color = Colours[colour] * darkness;
        }
    }
}