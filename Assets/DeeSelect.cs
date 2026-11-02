using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeeSelect : MonoBehaviour
{
    [Header("Level Stops & UI Elements")]
    [SerializeField] private List<Transform> levelStops;
    [SerializeField] private List<RectTransform> levelElements;
    [Tooltip("Drag the Text UI objects for each level here (Tutorial text, Easy text, etc.)")]
    [SerializeField] private List<RectTransform> levelTexts;
    [SerializeField] private List<string> levelSceneNames;

    [Header("UI Button Controls")]
    [Tooltip("Drag your left arrow UI Button component here")]
    [SerializeField] private Button leftArrowButton;
    [Tooltip("Drag your right arrow UI Button component here")]
    [SerializeField] private Button rightArrowButton;
    [Tooltip("Drag Dee's UI Button component here (clicking Dee enters the level)")]
    [SerializeField] private Button deeButton;

    [Header("Text Position Offset")]
    [Tooltip("How far below Dee's stop position the selected text should sit on the Y axis")]
    [SerializeField] private Vector3 textOffset = new Vector3(0f, -60f, 0f);

    [Header("Animation Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float scaleSpeed = 12f;
    [SerializeField] private float selectedScaleMultiplier = 1.2f;

    [Header("Keyboard Controls (Optional)")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode confirmKey = KeyCode.Return;

    private int currentIndex = 0;
    private RectTransform deeRect;
    private int unlockedLevelIndex;
    private List<Vector3> defaultTextPositions = new List<Vector3>();

    private void Start()
    {
        deeRect = GetComponent<RectTransform>();
        unlockedLevelIndex = PlayerPrefs.GetInt("UnlockedLevelIndex", 0);

        // Store default layout positions for each text label
        for (int i = 0; i < levelTexts.Count; i++)
        {
            if (levelTexts[i] != null)
            {
                defaultTextPositions.Add(levelTexts[i].position);
            }
            else
            {
                defaultTextPositions.Add(Vector3.zero);
            }
        }

        // Bind UI Buttons dynamically
        if (leftArrowButton != null)
            leftArrowButton.onClick.AddListener(MoveLeft);

        if (rightArrowButton != null)
            rightArrowButton.onClick.AddListener(MoveRight);

        if (deeButton != null)
            deeButton.onClick.AddListener(TryEnterSelectedLevel);

        if (levelStops != null && levelStops.Count > 0)
        {
            deeRect.position = levelStops[currentIndex].position;
        }

        UpdateTextPositions();
    }

    private void Update()
    {
        HandleKeyboardInput();
        SmoothMoveDee();
        SmoothScaleLevels();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(moveLeftKey) || Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(moveRightKey) || Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(confirmKey) || Input.GetKeyDown(KeyCode.Space))
        {
            TryEnterSelectedLevel();
        }
    }

    public void MoveLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateTextPositions();
        }
    }

    public void MoveRight()
    {
        if (currentIndex < levelStops.Count - 1)
        {
            currentIndex++;
            UpdateTextPositions();
        }
    }

    private void SmoothMoveDee()
    {
        if (levelStops == null || levelStops.Count == 0) return;
        Vector3 targetPos = levelStops[currentIndex].position;
        deeRect.position = Vector3.Lerp(deeRect.position, targetPos, Time.deltaTime * moveSpeed);
    }

    private void SmoothScaleLevels()
    {
        for (int i = 0; i < levelElements.Count; i++)
        {
            if (levelElements[i] == null) continue;

            float targetScale = (i == currentIndex) ? selectedScaleMultiplier : 1.0f;
            Vector3 targetVector = Vector3.one * targetScale;
            levelElements[i].localScale = Vector3.Lerp(levelElements[i].localScale, targetVector, Time.deltaTime * scaleSpeed);
        }
    }

    private void UpdateTextPositions()
    {
        for (int i = 0; i < levelTexts.Count; i++)
        {
            if (levelTexts[i] == null) continue;

            if (i == currentIndex && levelStops[i] != null)
            {
                Vector3 targetPos = defaultTextPositions[i];
                targetPos.y = levelStops[i].position.y + textOffset.y;
                levelTexts[i].position = targetPos;
            }
            else
            {
                levelTexts[i].position = defaultTextPositions[i];
            }
        }
    }

    public void TryEnterSelectedLevel()
    {
        if (currentIndex <= unlockedLevelIndex)
        {
            if (currentIndex < levelSceneNames.Count && !string.IsNullOrEmpty(levelSceneNames[currentIndex]))
            {
                SceneManager.LoadScene(levelSceneNames[currentIndex]);
            }
        }
    }
}