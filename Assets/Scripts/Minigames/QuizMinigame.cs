using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizMinigame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_InputField answerInputField;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text[] meaningText;

    [Header("Game Settings")]
    [SerializeField] private float feedbackDelay = 2f;
    [SerializeField] private float timeBeforeWordChanges = 30f;
    [SerializeField] private float timeReturner = 30f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private string jlptFilter = "";
    [SerializeField] private int maxMeaningTexts = 3;

    [Header("Visual Effects")]
    [SerializeField] private bool enableShakeEffect = true;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeIntensity = 10f;

    [Header("Custom Deck")]
    [SerializeField] private bool enableCustomDeck = true;
    [SerializeField] private string customDeckFileName = "CustomStudyDeck.json";
    [SerializeField] private Button saveToDeckButton;
    [SerializeField] private TMP_Text saveButtonText;
    [SerializeField] private bool useCustomDeckOnly = false;
    [SerializeField] private TMP_Text deckInfoText;

    [Header("Game State")]
    [SerializeField] private bool isGameActive = true;
    [SerializeField] private bool showMeanings = true;

    // Private fields
    private int currentScore;
    private int currentWordInIndex;
    private int timerToInt;
    private InfoList currentWord;
    private List<InfoList> wordList;
    private List<InfoList> usedWords = new List<InfoList>();
    private GameState gameState = GameState.Waiting;
    
    // Custom deck fields
    private CustomStudyDeck customDeck;
    private string CustomDeckPath => Path.Combine(Application.persistentDataPath, customDeckFileName);

    #region Unity Lifecycle

    private void Start()
    {
        InitializeGame();
    }

    private void FixedUpdate()
    {
        if (!isGameActive) return;
        
        UpdateScore();
        UpdateTimer();
    }

    private void Update()
    {
        if (!isGameActive) return;

        HandleInput();
        CheckGameEnd();
    }

    #endregion

    #region Initialization

    private void InitializeGame()
    {
        try
        {
            LoadWordList();
            InitializeCustomDeck();
            
            if (wordList.Count > 0)
            {
                SelectNewWord();
                Debug.Log($"Quiz initialized with {wordList.Count} words");
            }
            else
            {
                Debug.LogError("The deck is empty!");
                gameState = GameState.Error;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize quiz: {e.Message}");
            gameState = GameState.Error;
        }
    }

    private void LoadWordList()
    {
        if (useCustomDeckOnly)
        {
            LoadCustomDeckWordList();
        }
        else
        {
            LoadMainDictionaryWordList();
        }
    }

    private void LoadMainDictionaryWordList()
    {
        var dictManager = FindObjectOfType<DictManager>();
        if (dictManager == null)
        {
            throw new System.InvalidOperationException("DictManager not found in scene");
        }

        wordList = dictManager.Dictionary.wordList;
        Debug.Log($"Loaded {wordList.Count} words from main dictionary");
    }

    private void LoadCustomDeckWordList()
    {
        if (!enableCustomDeck)
        {
            throw new System.InvalidOperationException("Custom deck is not enabled");
        }

        // Asegurar que el deck personalizado esté cargado
        if (customDeck == null)
        {
            LoadCustomDeck();
        }

        wordList = new List<InfoList>(customDeck.wordList);
        Debug.Log($"Loaded {wordList.Count} words from custom deck");
        
        UpdateDeckInfoText();
    }

    private void InitializeCustomDeck()
    {
        if (!enableCustomDeck) return;

        try
        {
            LoadCustomDeck();
            SetupSaveButton();
            Debug.Log($"Custom deck initialized with {customDeck.wordList.Count} words");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize custom deck: {e.Message}");
            customDeck = new CustomStudyDeck();
        }
    }

    private void LoadCustomDeck()
    {
        if (File.Exists(CustomDeckPath))
        {
            string jsonData = File.ReadAllText(CustomDeckPath);
            customDeck = JsonUtility.FromJson<CustomStudyDeck>(jsonData);
            
            if (customDeck == null)
            {
                customDeck = new CustomStudyDeck();
            }
        }
        else
        {
            customDeck = new CustomStudyDeck();
            SaveCustomDeck();
        }
    }

    private void SaveCustomDeck()
    {
        if (!enableCustomDeck) return;

        try
        {
            string jsonData = JsonUtility.ToJson(customDeck, true);
            File.WriteAllText(CustomDeckPath, jsonData);
            Debug.Log($"Custom deck saved with {customDeck.wordList.Count} words");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save custom deck: {e.Message}");
        }
    }

    private void SetupSaveButton()
    {
        if (saveToDeckButton != null)
        {
            saveToDeckButton.onClick.RemoveAllListeners();
            saveToDeckButton.onClick.AddListener(SaveCurrentWordToDeck);
            UpdateSaveButtonText();
        }
    }

    private void UpdateDeckInfoText()
    {
        if (deckInfoText == null) return;

        if (useCustomDeckOnly)
        {
            int totalWords = wordList?.Count ?? 0;
            int usedWordsCount = usedWords.Count;
            int remainingWords = totalWords - usedWordsCount;
            
            deckInfoText.text = $"Custom Deck: {remainingWords}/{totalWords} words remaining";
        }
        else
        {
            deckInfoText.text = "Main Dictionary";
        }
    }

    #endregion

    #region Game Logic

    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    private void UpdateTimer()
    {
        if (timeBeforeWordChanges >= 0)
        {
            timeBeforeWordChanges -= Time.deltaTime;
        }
        else
        {
            HandleTimeOut();
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        timerToInt = Mathf.Max(0, (int)timeBeforeWordChanges);
        if (timerText != null)
        {
            timerText.text = timerToInt.ToString();
        }
    }

    private void HandleTimeOut()
    {
        timeBeforeWordChanges = timeReturner + feedbackDelay;
        ShowIncorrectFeedback();
        StartCoroutine(ShowFeedbackAndSelectNewWord());
    }

    private void ShowIncorrectFeedback()
    {
        if (currentWord != null)
        {
            feedbackText.text = $"Incorrect. The correct answer was: {currentWord.hiragana}";
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckAnswer();
            answerInputField?.ActivateInputField();
        }
    }

    private void CheckGameEnd()
    {
        if (wordList.Count == 0)
        {
            WinScreen();
        }
    }

    public void CheckAnswer()
    {
        if (currentWord == null || timeBeforeWordChanges >= timeReturner)
        {
            return;
        }

        string playerAnswer = answerInputField.text.ToLower().Trim();
        
        if (IsCorrectAnswer(playerAnswer))
        {
            HandleCorrectAnswer();
        }
        else
        {
            HandleIncorrectAnswer();
        }
    }

    private bool IsCorrectAnswer(string playerAnswer)
    {
        if (string.IsNullOrWhiteSpace(playerAnswer) || currentWord == null)
        {
            return false;
        }

        var correctAnswers = new[]
        {
            currentWord.word?.ToLower(),
            currentWord.kana?.ToLower(),
            currentWord.romaji?.ToLower(),
            currentWord.hiragana?.ToLower()
        };

        return correctAnswers.Any(answer => answer == playerAnswer);
    }

    private void HandleCorrectAnswer()
    {
        StartCoroutine(FadeHintColorToWhite(Color.green, fadeDuration, hintText));
        currentScore++;
        feedbackText.text = "Correct!";
        usedWords.Add(currentWord);
        timeBeforeWordChanges = timeReturner + feedbackDelay;
        StartCoroutine(ShowFeedbackAndSelectNewWord());
        
        if (showMeanings)
        {
            UpdateWordMeanings();
        }
        
        UpdateDeckInfoText();
    }

    private void HandleIncorrectAnswer()
    {
        StartCoroutine(FadeHintColorToWhite(Color.red, fadeDuration, hintText));
        
        if (enableShakeEffect)
        {
            StartCoroutine(ShakeText(hintText, shakeDuration, shakeIntensity));
        }
    }

    #endregion

    #region Word Selection

    private void SelectNewWord()
    {
        if (string.IsNullOrEmpty(jlptFilter))
        {
            SelectWordFromAll();
        }
        else
        {
            SelectWordByJLPT();
        }
    }

    private void SelectWordFromAll()
    {
        if (wordList.Count == 0)
        {
            Debug.LogError("No words available!");
            return;
        }

        var availableWords = wordList.Except(usedWords).ToList();
        
        if (availableWords.Count == 0)
        {
            WinScreen();
            return;
        }

        SelectRandomWord(availableWords);
    }

    private void SelectWordByJLPT()
    {
        var filteredList = wordList.Where(word => word.jlptLevel == jlptFilter).ToList();
        
        if (filteredList.Count == 0)
        {
            Debug.LogError($"No words available for JLPT level: {jlptFilter}");
            SceneManager.LoadScene(1);
            return;
        }

        var availableWords = filteredList.Except(usedWords).ToList();
        
        if (availableWords.Count == 0)
        {
            WinScreen();
            return;
        }

        SelectRandomWord(availableWords);
    }

    private void SelectRandomWord(List<InfoList> availableWords)
    {
        currentWordInIndex = Random.Range(0, availableWords.Count);
        currentWord = availableWords[currentWordInIndex];
        
        UpdateUIForNewWord();
    }

    private void UpdateUIForNewWord()
    {
        if (hintText != null)
        {
            hintText.text = currentWord.word;
        }
        
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
        
        answerInputField?.ActivateInputField();
        UpdateSaveButtonText();
        UpdateDeckInfoText();
    }

    #endregion

    #region UI Management

    public void UpdateWordMeanings()
    {
        if (currentWord?.def == null || meaningText == null)
        {
            return;
        }

        for (int i = 0; i < Mathf.Min(maxMeaningTexts, meaningText.Length); i++)
        {
            if (i < currentWord.def.Length && !string.IsNullOrEmpty(currentWord.def[i]))
            {
                meaningText[i].text = currentWord.def[i];
            }
            else
            {
                meaningText[i].text = "";
            }
        }
    }

    private void ClearMeanings()
    {
        if (meaningText == null) return;

        for (int i = 0; i < meaningText.Length; i++)
        {
            meaningText[i].text = "";
        }
    }

    #endregion

    #region Custom Deck Management

    public void SaveCurrentWordToDeck()
    {
        if (!enableCustomDeck || currentWord == null) return;

        try
        {
            // Verificar si la palabra ya existe en el deck
            if (IsWordInCustomDeck(currentWord.word))
            {
                Debug.Log($"Word '{currentWord.word}' is already in the custom deck");
                ShowSaveFeedback("Already saved!", Color.yellow);
                return;
            }

            // Agregar la palabra al deck
            customDeck.wordList.Add(currentWord);
            SaveCustomDeck();
            
            // Si estamos en modo deck personalizado, actualizar la lista de palabras
            if (useCustomDeckOnly)
            {
                wordList = new List<InfoList>(customDeck.wordList);
            }
            
            Debug.Log($"Word '{currentWord.word}' saved to custom deck");
            ShowSaveFeedback("Saved to deck!", Color.green);
            UpdateSaveButtonText();
            UpdateDeckInfoText();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save word to custom deck: {e.Message}");
            ShowSaveFeedback("Save failed!", Color.red);
        }
    }

    private bool IsWordInCustomDeck(string word)
    {
        if (customDeck?.wordList == null) return false;
        
        return customDeck.wordList.Any(w => w.word == word);
    }

    private void UpdateSaveButtonText()
    {
        if (saveButtonText == null || currentWord == null) return;

        if (IsWordInCustomDeck(currentWord.word))
        {
            saveButtonText.text = "✓ Saved";
            if (saveToDeckButton != null)
            {
                saveToDeckButton.interactable = false;
            }
        }
        else
        {
            saveButtonText.text = "Save to Deck";
            if (saveToDeckButton != null)
            {
                saveToDeckButton.interactable = true;
            }
        }
    }

    private void ShowSaveFeedback(string message, Color color)
    {
        if (saveButtonText != null)
        {
            StartCoroutine(ShowTemporaryMessage(saveButtonText, message, color, 2f));
        }
    }

    private IEnumerator ShowTemporaryMessage(TMP_Text textComponent, string message, Color color, float duration)
    {
        if (textComponent == null) yield break;

        string originalText = textComponent.text;
        Color originalColor = textComponent.color;

        textComponent.text = message;
        textComponent.color = color;

        yield return new WaitForSeconds(duration);

        textComponent.text = originalText;
        textComponent.color = originalColor;
    }

    public void ExportCustomDeck()
    {
        if (!enableCustomDeck || customDeck?.wordList == null) return;

        try
        {
            string exportPath = Path.Combine(Application.persistentDataPath, "CustomDeck_Export.json");
            string jsonData = JsonUtility.ToJson(customDeck, true);
            File.WriteAllText(exportPath, jsonData);
            
            Debug.Log($"Custom deck exported to: {exportPath}");
            Debug.Log($"Total words in custom deck: {customDeck.wordList.Count}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to export custom deck: {e.Message}");
        }
    }

    public void ClearCustomDeck()
    {
        if (!enableCustomDeck) return;

        try
        {
            customDeck.wordList.Clear();
            SaveCustomDeck();
            UpdateSaveButtonText();
            
            // Si estamos en modo deck personalizado, actualizar la lista
            if (useCustomDeckOnly)
            {
                wordList.Clear();
                UpdateDeckInfoText();
            }
            
            Debug.Log("Custom deck cleared");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to clear custom deck: {e.Message}");
        }
    }

    public bool IsCustomDeckEmpty()
    {
        return customDeck?.wordList?.Count == 0;
    }

    public void RefreshCustomDeck()
    {
        if (useCustomDeckOnly && enableCustomDeck)
        {
            LoadCustomDeck();
            wordList = new List<InfoList>(customDeck.wordList);
            UpdateDeckInfoText();
            Debug.Log($"Custom deck refreshed with {wordList.Count} words");
        }
    }

    #endregion

    #region Public Methods

    public void SetJlptFilter(string newJlptFilter)
    {
        jlptFilter = newJlptFilter ?? "";
        SelectNewWord();
    }

    public void WinScreen()
    {
        gameState = GameState.Won;
        Debug.Log($"Quiz completed! Final score: {currentScore}");
        SceneManager.LoadScene(1);
    }

    public void ResetGame()
    {
        currentScore = 0;
        usedWords.Clear();
        timeBeforeWordChanges = timeReturner;
        gameState = GameState.Waiting;
        SelectNewWord();
    }

    public void SwitchToCustomDeckMode()
    {
        useCustomDeckOnly = true;
        ResetGame();
        Debug.Log("Switched to Custom Deck mode");
    }

    public void SwitchToMainDictionaryMode()
    {
        useCustomDeckOnly = false;
        ResetGame();
        Debug.Log("Switched to Main Dictionary mode");
    }

    public void ToggleDeckMode()
    {
        useCustomDeckOnly = !useCustomDeckOnly;
        ResetGame();
        Debug.Log($"Switched to {(useCustomDeckOnly ? "Custom Deck" : "Main Dictionary")} mode");
    }

    #endregion

    #region Coroutines

    private IEnumerator ShowFeedbackAndSelectNewWord()
    {
        yield return new WaitForSeconds(feedbackDelay);
        
        ClearMeanings();
        SelectNewWord();
        
        if (answerInputField != null)
        {
            answerInputField.text = "";
        }
    }

    private IEnumerator FadeHintColorToWhite(Color popupColor, float duration, TMP_Text text)
    {
        if (text == null) yield break;

        Color endColor = Color.white;
        float elapsedTime = 0;

        text.color = popupColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(popupColor, endColor, elapsedTime / duration);
            yield return null;
        }

        text.color = endColor;
    }

    private IEnumerator ShakeText(TMP_Text text, float duration, float intensity)
    {
        if (text == null) yield break;

        Vector3 originalPosition = text.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // Calcular factor de decaimiento (el shake se reduce gradualmente)
            float decayFactor = 1f - (elapsedTime / duration);
            
            // Crear movimiento de shake usando múltiples frecuencias para un efecto más natural
            float xOffset = Mathf.Sin(elapsedTime * 60f) * intensity * decayFactor * 0.5f;
            xOffset += Mathf.Sin(elapsedTime * 30f) * intensity * decayFactor * 0.3f;
            
            float yOffset = Mathf.Cos(elapsedTime * 45f) * intensity * decayFactor * 0.3f;
            yOffset += Mathf.Cos(elapsedTime * 25f) * intensity * decayFactor * 0.2f;
            
            text.transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0f);
            
            yield return null;
        }

        // Restaurar posición original suavemente
        float restoreTime = 0f;
        Vector3 currentPosition = text.transform.localPosition;
        
        while (restoreTime < 0.1f)
        {
            restoreTime += Time.deltaTime;
            text.transform.localPosition = Vector3.Lerp(currentPosition, originalPosition, restoreTime / 0.1f);
            yield return null;
        }
        
        text.transform.localPosition = originalPosition;
    }

    #endregion

    #region Public Properties

    public int CurrentScore => currentScore;
    public int WordCount => wordList?.Count ?? 0;
    public int UsedWordCount => usedWords.Count;
    public GameState State => gameState;
    public bool IsGameActive => isGameActive;
    
    // Custom deck properties
    public CustomStudyDeck CustomDeck => customDeck;
    public int CustomDeckWordCount => customDeck?.wordList?.Count ?? 0;
    public bool IsCurrentWordInDeck => currentWord != null && IsWordInCustomDeck(currentWord.word);
    public bool IsCustomDeckMode => useCustomDeckOnly;
    public int RemainingWords => (wordList?.Count ?? 0) - usedWords.Count;

    #endregion
}

/// <summary>
/// Estados del juego
/// </summary>
public enum GameState
{
    Waiting,
    Playing,
    Won,
    Error
}

/// <summary>
/// Deck personalizado para estudio
/// </summary>
[System.Serializable]
public class CustomStudyDeck
{
    public List<InfoList> wordList = new List<InfoList>();
    public string creationDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public string lastModified = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public int totalWords => wordList?.Count ?? 0;
}