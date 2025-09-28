using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class settingsManager : MonoBehaviour
{
    // Singleton instance for global access to settings

    [SerializeField] bool starupMainMenu;
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
    // Button to reset controls to default
    public Button resetButton;

    [Header("Audio Settings")]
    // Audio mixer for volume control
    public AudioMixer audioMixer;
    // Volume sliders
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;


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

    //String to store the original key in case of cancel
    private KeyCode originalKey;

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

        // Reset controls back to default
        if (resetButton != null) resetButton.onClick.AddListener(ResetToDefaults);

        // Set up key remapping button events
        forwardKeyButton.onClick.AddListener(() => StartKeyChange("Forward"));
        backKeyButton.onClick.AddListener(() => StartKeyChange("Back"));
        leftKeyButton.onClick.AddListener(() => StartKeyChange("Left"));
        rightKeyButton.onClick.AddListener(() => StartKeyChange("Right"));
        jumpKeyButton.onClick.AddListener(() => StartKeyChange("Jump"));
        sprintKeyButton.onClick.AddListener(() => StartKeyChange("Sprint"));

        // Set up for audio slider events
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        // Load and apply audio settings
        LoadAudioSettings();

        // This loads in the main menu at the earliest point. - Tuff Genda
        if (starupMainMenu)
        {
            gamemanager.instance.titleMenu();
        }

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
                if (Input.GetKeyDown(key) && key != KeyCode.Escape && key != KeyCode.Return)
                {
                    // Assign the new key and stop waiting
                    if (duplicateKeyDetected(key) || key == KeyCode.R || key == KeyCode.Mouse0 || key == KeyCode.Mouse1 || key == KeyCode.Mouse2 || key == KeyCode.Mouse3 || key == KeyCode.Mouse4 || key == KeyCode.Mouse5 || key == KeyCode.Mouse6)
                    {
                        // If duplicate key detected, revert to original key and update text
                        ChangeKey(keyToChange, originalKey);
                        isWaitingForKey = false;
                        keyToChange = "";
                        break;
                    }
                    else
                    {
                        ChangeKey(keyToChange, key);
                        isWaitingForKey = false;
                        keyToChange = "";
                        break;
                    }


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

    bool duplicateKeyDetected(KeyCode key)
    {
        bool duplicate = false;
        if (controls["Forward"] == key)
        {
            duplicate = true;
        }
        else if (controls["Back"] == key)
        {
            duplicate = true;
        }
        else if (controls["Left"] == key)
        {
            duplicate = true;
        }
        else if (controls["Right"] == key)
        {
            duplicate = true;
        }
        else if (controls["Jump"] == key)
        {
            duplicate = true;
        }
        else if (controls["Sprint"] == key)
        {
            duplicate = true;
        }
        return duplicate;
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
        originalKey = controls[action];
        isWaitingForKey = true;
        keyToChange = action;
        // Update UI to show we're waiting for input
        GetControlText(action).text = "Press any key...";
        gamemanager.instance.menuClick.Play();
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
        EventSystem.current.SetSelectedGameObject(gamemanager.instance.firstButtonSettings);
        gamemanager.instance.menuClick.Play();
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
    //EventSystem.current.SetSelectedGameObject(gamemanager.instance.firstButtonPause);

    // Returns the player back to the pause menu
    public void BackToPauseMenu()
    {
        // I changed this to close menu since it is a more general function for credits and settings now. - Tuff Genda
        gamemanager.instance.closeMenu();
        gamemanager.instance.menuClick.Play();
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

    // Resets all controls back to default
    public void ResetToDefaults()
    {
        LoadDefaultControls();
        SaveControls();
        UpdateControlTexts();
        ResetAudioToDefaults();
        gamemanager.instance.menuClick.Play();
    }

    // Audio volume control methods
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            if (volume == 0)
            {
                audioMixer.SetFloat("Master", -80f);
            }
            else
            {
                audioMixer.SetFloat("Master", Mathf.Log10(volume) * 40);
            }

            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            if (volume == 0)
            {
                audioMixer.SetFloat("SFX", -80f);
            }
            else
            {
                audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 40);
            }
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            if (volume == 0)
            {
                audioMixer.SetFloat("Music", -80f);
            }
            else
            {
                audioMixer.SetFloat("Music", Mathf.Log10(volume) * 40);
            }
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }

    // Load saved audio settings
    void LoadAudioSettings()
    {
        // Load master volume (default: 0.8)
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVolume;
        SetMasterVolume(masterVolume);

        // Load SFX volume (default: 0.8)
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
        SetSFXVolume(sfxVolume);

        // Load music volume (default: 0.6)
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        if (musicVolumeSlider != null) musicVolumeSlider.value = musicVolume;
        SetMusicVolume(musicVolume);
    }

    // Reset audio settings to defaults
    void ResetAudioToDefaults()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = 0.8f;
            SetMasterVolume(0.8f);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 0.8f;
            SetSFXVolume(0.8f);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = 0.6f;
            SetMusicVolume(0.6f);
        }
    }
}
