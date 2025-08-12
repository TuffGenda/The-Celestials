using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class settingsManager : MonoBehaviour
{
    // Singleton instance for global access to settings
    public static settingsManager instance;

    [Header("Settings UI")]
    // Button to close settings menu
    public Button backButton;

    [Header("Control Settings")]
    // Buttons for remapping each control key
    public Button forwardKeyButton;
    public Button backKeyButton;
    public Button leftKeyButton;
    public Button rightKeyButton;
    public Button jumpKeyButton;
    public Button sprintKeyButton;

    [Header("Display")]
    // Text displays showing current key assignments
    public TMP_Text forwardKeyText;
    public TMP_Text backKeyText;
    public TMP_Text leftKeyText;
    public TMP_Text rightKeyText;
    public TMP_Text jumpKeyText;
    public TMP_Text sprintKeyText;

    // Dictionary storing action names mapped to their assigned keys
    private Dictionary<string, KeyCode> controls = new Dictionary<string, KeyCode>();
    // Flag indicating if we're currently waiting for a key press to remap
    private bool isWaitingForKey = false;
    // String storing which action is being remapped
    private string keyToChange = "";

    // Initialize singleton pattern and prevent destruction on scene load
    void Awake()
    {
        // Singleton pattern - only allow one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    // Initialize settings and set up button listeners
    void Start()
    {
        // Load default key mappings
        LoadDefaultControls();
        // Load any saved custom key mappings
        LoadControls();
        // Update UI to show current key assignments
        UpdateControlTexts();

        // Takes the player back to the Pause Menu
        backButton.onClick.AddListener(BackToPauseMenu);

        // Set up key remapping button events
        forwardKeyButton.onClick.AddListener(() => StartKeyChange("Forward"));
        backKeyButton.onClick.AddListener(() => StartKeyChange("Back"));
        leftKeyButton.onClick.AddListener(() => StartKeyChange("Left"));
        rightKeyButton.onClick.AddListener(() => StartKeyChange("Right"));
        jumpKeyButton.onClick.AddListener(() => StartKeyChange("Jump"));
        sprintKeyButton.onClick.AddListener(() => StartKeyChange("Sprint"));
    }

    // Handle key detection for remapping
    void Update()
    {
        // Only check for key input when we're waiting for a key to be pressed
        if (isWaitingForKey)
        {
            // Check all possible keys
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                // If a key is pressed (except Escape)
                if (Input.GetKeyDown(key) && key != KeyCode.Escape)
                {
                    // Assign the new key and stop waiting
                    ChangeKey(keyToChange, key);
                    isWaitingForKey = false;
                    keyToChange = "";
                    break;
                }
            }

            // If Escape is pressed, cancel the key remapping
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isWaitingForKey = false;
                keyToChange = "";
            }
        }
    }

    // Set up default key mappings (WASD + Space + Shift)
    void LoadDefaultControls()
    {
        controls["Forward"] = KeyCode.W;
        controls["Back"] = KeyCode.S;
        controls["Left"] = KeyCode.A;
        controls["Right"] = KeyCode.D;
        controls["Jump"] = KeyCode.Space;
        controls["Sprint"] = KeyCode.LeftShift;
    }

    // Load saved control settings from PlayerPrefs
    void LoadControls()
    {
        // Create a list of keys to avoid modifying dictionary during enumeration
        List<string> keys = new List<string>(controls.Keys);

        // Check each control action for saved preferences
        foreach (string action in keys)
        {
            // If there's a saved preference for this control
            if (PlayerPrefs.HasKey("Control_" + action))
            {
                // Load the saved key and update the controls dictionary
                KeyCode savedKey = (KeyCode)PlayerPrefs.GetInt("Control_" + action);
                controls[action] = savedKey;
            }
        }
    }

    // Save current control settings to PlayerPrefs for persistence
    void SaveControls()
    {
        // Save each control mapping
        foreach (var control in controls)
        {
            PlayerPrefs.SetInt("Control_" + control.Key, (int)control.Value);
        }
        // Force save to disk
        PlayerPrefs.Save();
    }

    // Begin the process of changing a key mapping
    public void StartKeyChange(string action)
    {
        // Set flags to indicate we're waiting for input
        isWaitingForKey = true;
        keyToChange = action;
        // Update UI to show we're waiting for input
        GetControlText(action).text = "Press any key...";
    }

    // Apply a new key mapping
    void ChangeKey(string action, KeyCode newKey)
    {
        // Update the controls dictionary
        controls[action] = newKey;
        // Save the change persistently
        SaveControls();
        // Update the UI display
        UpdateControlTexts();
    }

    // Update all control text displays with current key mappings
    void UpdateControlTexts()
    {
        if (forwardKeyText != null) forwardKeyText.text = controls["Forward"].ToString();
        if (backKeyText != null) backKeyText.text = controls["Back"].ToString();
        if (leftKeyText != null) leftKeyText.text = controls["Left"].ToString();
        if (rightKeyText != null) rightKeyText.text = controls["Right"].ToString();
        if (jumpKeyText != null) jumpKeyText.text = controls["Jump"].ToString();
        if (sprintKeyText != null) sprintKeyText.text = controls["Sprint"].ToString();
    }

    // Helper method to get the appropriate text component for a control action
    TMP_Text GetControlText(string action)
    {
        switch (action)
        {
            case "Forward": return forwardKeyText;
            case "Back": return backKeyText;
            case "Left": return leftKeyText;
            case "Right": return rightKeyText;
            case "Jump": return jumpKeyText;
            case "Sprint": return sprintKeyText;
            default: return null;
        }
    }

    // Returns the player back to the pause menu
    public void BackToPauseMenu()
    {
        gamemanager.instance.closeSettings();
    }

    // Check if a custom control key is currently held down
    public bool GetKey(string action)
    {
        return controls.ContainsKey(action) && Input.GetKey(controls[action]);
    }

    // Check if a custom control key was just pressed this frame
    public bool GetKeyDown(string action)
    {
        return controls.ContainsKey(action) && Input.GetKeyDown(controls[action]);
    }

    // Check if a custom control key was just released this frame
    public bool GetKeyUp(string action)
    {
        return controls.ContainsKey(action) && Input.GetKeyUp(controls[action]);
    }

    // Get axis input using custom controls (for movement)
    public float GetAxis(string action)
    {
        // Handle horizontal movement (left/right)
        if (action == "Horizontal")
        {
            float horizontal = 0f;
            if (GetKey("Right")) horizontal += 1f;
            if (GetKey("Left")) horizontal -= 1f;
            return horizontal;
        }
        // Handle vertical movement (forward/back)
        else if (action == "Vertical")
        {
            float vertical = 0f;
            if (GetKey("Forward")) vertical += 1f;
            if (GetKey("Back")) vertical -= 1f;
            return vertical;
        }
        return 0f;
    }
}
