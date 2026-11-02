using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Level Buttons")]
    [Tooltip("Drag your level UI Button components here in order (Tutorial, Level 1, Level 2, etc.)")]
    [SerializeField] private List<Button> levelButtons;
    [Tooltip("Scene names corresponding strictly to each level button index above")]
    [SerializeField] private List<string> levelSceneNames;

    [Header("Navigation Buttons")]
    [SerializeField] private Button backArrowButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;

    [Header("Menu Connections")]
    [SerializeField] private string mainPageSceneName = "MainMenu";
    [SerializeField] private GameObject settingsPanel;

    [Header("Audio Settings")]
    private bool isMusicOn = true;

    private void Start()
    {
        SetupNavigation();
        UpdateLevelLockStates();
    }

    private void SetupNavigation()
    {
        // Bind Level Buttons dynamically
        for (int i = 0; i < levelButtons.Count; i++)
        {
            int levelIndex = i; // Local copy for closure in lambda
            if (levelButtons[i] != null)
            {
                levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
            }
        }

        // Bind Outer Control Buttons
        if (backArrowButton != null)
            backArrowButton.onClick.AddListener(OnBackClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (musicButton != null)
            musicButton.onClick.AddListener(OnMusicToggleClicked);

        if (previousPageButton != null)
            previousPageButton.onClick.AddListener(OnPreviousPageClicked);

        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(OnNextPageClicked);
    }

    /// <summary>
    /// Reads saved progression (via PlayerPrefs) and unlocks/locks levels accordingly.
    /// </summary>
    public void UpdateLevelLockStates()
    {
        // Retrieve unlocked level index (Default is 0: Tutorial unlocked)
        int unlockedLevelIndex = PlayerPrefs.GetInt("UnlockedLevelIndex", 0);

        for (int i = 0; i < levelButtons.Count; i++)
        {
            if (levelButtons[i] == null) continue;

            bool isUnlocked = (i <= unlockedLevelIndex);
            levelButtons[i].interactable = isUnlocked;

            // Optional: Toggle child Lock Icon visuals if present on the button
            Transform lockIcon = levelButtons[i].transform.Find("LockIcon");
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(!isUnlocked);
            }
        }
    }

    private void OnLevelButtonClicked(int index)
    {
        if (index >= 0 && index < levelSceneNames.Count)
        {
            string targetScene = levelSceneNames[index];
            if (!string.IsNullOrEmpty(targetScene))
            {
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                Debug.LogWarning($"[LevelSelect] Scene name for level index {index} is not set.");
            }
        }
    }

    private void OnBackClicked()
    {
        if (!string.IsNullOrEmpty(mainPageSceneName))
        {
            SceneManager.LoadScene(mainPageSceneName);
        }
    }

    private void OnSettingsClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        else
        {
            Debug.Log("[LevelSelect] Settings button pressed.");
        }
    }

    private void OnMusicToggleClicked()
    {
        isMusicOn = !isMusicOn;
        AudioListener.volume = isMusicOn ? 1.0f : 0.0f;

        // Save music preference
        PlayerPrefs.SetInt("MusicMuted", isMusicOn ? 0 : 1);
        PlayerPrefs.Save();

        Debug.Log($"[LevelSelect] Music Toggled. Active: {isMusicOn}");
    }

    private void OnPreviousPageClicked()
    {
        Debug.Log("[LevelSelect] Previous level set/page requested.");
        // Add custom carousel/page shifting logic here if applicable
    }

    private void OnNextPageClicked()
    {
        Debug.Log("[LevelSelect] Next level set/page requested.");
        // Add custom carousel/page shifting logic here if applicable
    }
}