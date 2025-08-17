using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CustomDict : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField[] verbInflections;
    [SerializeField] private GameObject toggleVerbButton;
    [SerializeField] private TMP_Text toggleVerbButtonText;
    [SerializeField] private TMP_InputField wordIF;
    [SerializeField] private TMP_InputField kanaIF;
    [SerializeField] private TMP_InputField pitchIF;
    [SerializeField] private TMP_InputField jlptLevelIF;
    [SerializeField] private TMP_InputField wordType1IF;
    [SerializeField] private TMP_InputField wordType2IF;
    [SerializeField] private TMP_InputField relatedWordIF;
    [SerializeField] private TMP_InputField[] wordDefIF;
    [SerializeField] private TMP_InputField example1JpIF;
    [SerializeField] private TMP_InputField example1EnIF;
    [SerializeField] private TMP_InputField example2JpIF;
    [SerializeField] private TMP_InputField example2EnIF;
    [SerializeField] private TMP_InputField longDefForWordJpIF;
    [SerializeField] private TMP_InputField longDefForWordEnIF;
    [SerializeField] private TMP_InputField romajiIF;
    [SerializeField] private TMP_InputField hiraganaIF;
    [SerializeField] private TMP_InputField alternativeFormIF;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject searchRelatedWordButton;
    [SerializeField] private TMP_Text searchRelatedWordText;
    [SerializeField] private GameObject seeVerbInflectionMenuButton;
    [SerializeField] private TMP_Dropdown wordOptionsDropdown;

    [Header("Dictionary Settings")]
    [SerializeField] private string fileName = "CustomJisho.json";
    
    [Header("Manager Components")]
    [SerializeField] private DictionarySearchManager searchManager;
    [SerializeField] private DictionaryValidator validator;
    
    // Private fields
    private bool isTheWordAVerb;
    private List<InfoListFCJ> currentResults = new List<InfoListFCJ>();
    private CustomJisho dict = new CustomJisho();
    private int currentDictWordAmount;
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    #region Unity Lifecycle

    private void Start()
    {
        InitializeDictionary();
        SetupUI();
    }

    #endregion

    #region Initialization

    private void InitializeDictionary()
    {
        try
        {
            LoadFromJson();
            Debug.Log($"Dictionary loaded successfully. Total words: {dict.wordList.Count}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to load dictionary: {e.Message}. Creating new dictionary.");
            SaveToJson();
        }
    }

    private void SetupUI()
    {
        // Initialize UI state
        UpdateVerbToggleUI();
        
        // Initialize managers
        if (searchManager != null)
        {
            searchManager.Initialize(dict);
        }
        
        if (validator == null)
        {
            validator = GetComponent<DictionaryValidator>();
        }
    }

    #endregion

    #region Data Persistence

    public void SaveToJson()
    {
        try
        {
            if (dict == null)
            {
                dict = new CustomJisho();
            }

            string dictData = JsonUtility.ToJson(dict, true); // Pretty print for debugging
            File.WriteAllText(FilePath, dictData);
            Debug.Log($"Dictionary saved successfully to: {FilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save dictionary: {e.Message}");
        }
    }

    public void LoadFromJson()
    {
        if (!File.Exists(FilePath))
        {
            Debug.Log("Dictionary file not found. Creating new dictionary.");
            dict = new CustomJisho();
            SaveToJson();
            return;
        }

        try
        {
            string dictData = File.ReadAllText(FilePath);
            dict = JsonUtility.FromJson<CustomJisho>(dictData);
            
            if (dict == null)
            {
                dict = new CustomJisho();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load dictionary: {e.Message}");
            dict = new CustomJisho();
        }
    }

    #endregion

    #region Word Management

    public void AddNewWord()
    {
        var inputFields = GetInputFields();
        
        if (validator != null)
        {
            var validationResult = validator.ValidateInputFields(inputFields);
            if (!validationResult.IsValid)
            {
                Debug.LogWarning($"Cannot add word: {validationResult.GetErrorMessage()}");
                return;
            }
            
            if (validationResult.Warnings.Count > 0)
            {
                Debug.LogWarning($"Warnings: {validationResult.GetWarningMessage()}");
            }
        }

        var newWord = CreateWordFromInput();
        dict.wordList.Insert(currentDictWordAmount, newWord);
        
        SaveToJson();
        ClearInputFields();
        
        Debug.Log($"New word added: {newWord.word}");
    }

    private DictionaryInputFields GetInputFields()
    {
        return new DictionaryInputFields
        {
            Word = wordIF?.text ?? string.Empty,
            Kana = kanaIF?.text ?? string.Empty,
            Pitch = pitchIF?.text ?? string.Empty,
            JLPTLevel = jlptLevelIF?.text ?? string.Empty,
            WordType1 = wordType1IF?.text ?? string.Empty,
            WordType2 = wordType2IF?.text ?? string.Empty,
            RelatedWord = relatedWordIF?.text ?? string.Empty,
            Definitions = wordDefIF?.Select(field => field?.text ?? string.Empty).ToArray() ?? new string[0],
            Example1Jp = example1JpIF?.text ?? string.Empty,
            Example1En = example1EnIF?.text ?? string.Empty,
            Example2Jp = example2JpIF?.text ?? string.Empty,
            Example2En = example2EnIF?.text ?? string.Empty,
            LongDefJp = longDefForWordJpIF?.text ?? string.Empty,
            LongDefEn = longDefForWordEnIF?.text ?? string.Empty,
            Romaji = romajiIF?.text ?? string.Empty,
            Hiragana = hiraganaIF?.text ?? string.Empty,
            AlternativeForm = alternativeFormIF?.text ?? string.Empty,
            IsVerb = isTheWordAVerb,
            VerbInflections = verbInflections?.Select(field => field?.text ?? string.Empty).ToArray() ?? new string[0]
        };
    }

    private InfoListFCJ CreateWordFromInput()
    {
        var word = new InfoListFCJ
        {
            word = wordIF.text.Trim(),
            kana = kanaIF.text.Trim(),
            pitch = pitchIF.text.Trim(),
            jlptLevel = jlptLevelIF.text.Trim(),
            wordType1 = wordType1IF.text.Trim(),
            wordType2 = wordType2IF.text.Trim(),
            relatedWord = relatedWordIF.text.Trim(),
            example1 = example1JpIF.text.Trim(),
            example1Alt = example1EnIF.text.Trim(),
            example2 = example2JpIF.text.Trim(),
            example2Alt = example2EnIF.text.Trim(),
            longDefinitionAboutTheWordEn = longDefForWordEnIF.text.Trim(),
            longDefinitionAboutTheWordJp = longDefForWordJpIF.text.Trim(),
            romaji = romajiIF.text.Trim(),
            hiragana = hiraganaIF.text.Trim(),
            alternativeForm = alternativeFormIF.text.Trim(),
            isTheWordAVerb = isTheWordAVerb
        };

        // Handle verb inflections
        if (isTheWordAVerb && verbInflections != null)
        {
            SetVerbInflections(word);
        }

        // Handle definitions
        if (wordDefIF != null && wordDefIF.Length > 0)
        {
            word.def = wordDefIF.Select(field => field.text.Trim())
                               .Where(text => !string.IsNullOrEmpty(text))
                               .ToArray();
        }

        return word;
    }

    private void SetVerbInflections(InfoListFCJ word)
    {
        if (verbInflections.Length >= 18)
        {
            word.nonPastInflection = GetFieldText(0);
            word.nonPastNegativeInflection = GetFieldText(1);
            word.nonPastInflectionPolite = GetFieldText(2);
            word.nonPastNegativeInflectionPolite = GetFieldText(3);
            word.pastInflection = GetFieldText(4);
            word.pastNegativeInflection = GetFieldText(5);
            word.pastInflectionPolite = GetFieldText(6);
            word.pastNegativeInflectionPolite = GetFieldText(7);
            word.teFormInflection = GetFieldText(8);
            word.teFormInflectionNegative = GetFieldText(9);
            word.potentialInflection = GetFieldText(10);
            word.potentialNegativeInflection = GetFieldText(11);
            word.passiveInflection = GetFieldText(12);
            word.passiveNegativeInflection = GetFieldText(13);
            word.causativeInflection = GetFieldText(14);
            word.causativeNegativeInflection = GetFieldText(15);
            word.imperativeInflection = GetFieldText(16);
            word.imperativeNegativeInflection = GetFieldText(17);
        }
    }

    private string GetFieldText(int index)
    {
        return index < verbInflections.Length ? verbInflections[index].text.Trim() : string.Empty;
    }

    private void ClearInputFields()
    {
        // Clear all input fields
        var inputFields = new[] { wordIF, kanaIF, pitchIF, jlptLevelIF, wordType1IF, wordType2IF, 
                                 relatedWordIF, example1JpIF, example1EnIF, example2JpIF, example2EnIF,
                                 longDefForWordJpIF, longDefForWordEnIF, romajiIF, hiraganaIF, alternativeFormIF };

        foreach (var field in inputFields)
        {
            if (field != null) field.text = string.Empty;
        }

        // Clear verb inflections
        if (verbInflections != null)
        {
            foreach (var field in verbInflections)
            {
                if (field != null) field.text = string.Empty;
            }
        }

        // Clear definitions
        if (wordDefIF != null)
        {
            foreach (var field in wordDefIF)
            {
                if (field != null) field.text = string.Empty;
            }
        }
    }

    #endregion

    #region UI Management

    public void ToggleVerb()
    {
        isTheWordAVerb = !isTheWordAVerb;
        UpdateVerbToggleUI();
    }

    private void UpdateVerbToggleUI()
    {
        if (toggleVerbButtonText != null)
        {
            toggleVerbButtonText.text = $"Toggle Verb: {(isTheWordAVerb ? "ON" : "OFF")}";
        }

        if (seeVerbInflectionMenuButton != null)
        {
            seeVerbInflectionMenuButton.SetActive(isTheWordAVerb);
        }
    }

    #endregion

    #region Public Properties

    public CustomJisho Dictionary => dict;
    public int WordCount => dict?.wordList?.Count ?? 0;
    public bool IsVerb => isTheWordAVerb;

    #endregion

    #region Search Methods

    /// <summary>
    /// Busca palabras en el diccionario
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <returns>Lista de palabras que coinciden</returns>
    public List<InfoListFCJ> SearchWords(string searchTerm)
    {
        if (searchManager != null)
        {
            return searchManager.SearchWords(searchTerm);
        }
        
        // Fallback básico si no hay search manager
        if (string.IsNullOrWhiteSpace(searchTerm) || dict?.wordList == null)
        {
            return new List<InfoListFCJ>();
        }

        return dict.wordList.Where(word => 
            word.word?.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase) == true ||
            word.kana?.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase) == true).ToList();
    }

    /// <summary>
    /// Busca palabras por nivel JLPT
    /// </summary>
    /// <param name="jlptLevel">Nivel JLPT</param>
    /// <returns>Lista de palabras del nivel especificado</returns>
    public List<InfoListFCJ> SearchByJLPTLevel(string jlptLevel)
    {
        if (searchManager != null)
        {
            return searchManager.SearchByJLPTLevel(jlptLevel);
        }
        
        // Fallback básico
        if (string.IsNullOrWhiteSpace(jlptLevel) || dict?.wordList == null)
        {
            return new List<InfoListFCJ>();
        }

        return dict.wordList.Where(word => 
            word.jlptLevel?.Equals(jlptLevel, System.StringComparison.OrdinalIgnoreCase) == true).ToList();
    }

    /// <summary>
    /// Obtiene estadísticas del diccionario
    /// </summary>
    /// <returns>Diccionario con estadísticas</returns>
    public Dictionary<string, int> GetDictionaryStats()
    {
        if (searchManager != null)
        {
            return searchManager.GetDictionaryStats();
        }
        
        // Fallback básico
        if (dict?.wordList == null)
        {
            return new Dictionary<string, int>();
        }

        return new Dictionary<string, int>
        {
            ["Total Words"] = dict.wordList.Count,
            ["Verbs"] = dict.wordList.Count(w => w.isTheWordAVerb)
        };
    }

    #endregion
}


[System.Serializable]
public class CustomJisho
{
    public List<InfoListFCJ> wordList = new List<InfoListFCJ>();
}

[System.Serializable]
public class InfoListFCJ
{
    public string word;
    public string kana;
    public string hiragana;
    public string alternativeForm;
    public string romaji;
    public string wordType1;
    public string wordType2;
    public string pitch;
    public string givenName;
    public bool isTheWordAGivenName;
    public string jlptLevel;
    public string[] def;
    public string example1;
    public string example1Alt;
    public string example2;
    public string example2Alt;
    public string longDefinitionAboutTheWordEn;
    public string longDefinitionAboutTheWordJp;
    public string relatedWord;
    public bool isTheWordAVerb;
    public string nonPastInflection;
    public string nonPastInflectionRomaji;
    public string nonPastNegativeInflection;
    public string nonPastNegativeInflectionRomaji;
    public string nonPastInflectionPolite;
    public string nonPastInflectionPoliteRomaji;
    public string nonPastNegativeInflectionPolite;
    public string nonPastNegativeInflectionPoliteRomaji;
    public string pastInflection;
    public string pastInflectionRomaji;
    public string pastNegativeInflection;
    public string pastNegativeInflectionRomaji;
    public string pastInflectionPolite;
    public string pastInflectionPoliteRomaji;
    public string pastNegativeInflectionPolite;
    public string pastNegativeInflectionPoliteRomaji;
    public string teFormInflection;
    public string teFormInflectionRomaji;
    public string teFormInflectionNegative;
    public string teFormInflectionNegativeRomaji;
    public string potentialInflection;
    public string potentialInflectionRomaji;
    public string potentialNegativeInflection;
    public string potentialNegativeInflectionRomaji;
    public string passiveInflection;
    public string passiveInflectionRomaji;
    public string passiveNegativeInflection;
    public string passiveNegativeInflectionRomaji;
    public string causativeInflection;
    public string causativeInflectionRomaji;
    public string causativeNegativeInflection;
    public string causativeNegativeInflectionRomaji;
    public string causativePassiveInflection;
    public string causativePassiveInflectionRomaji;
    public string causativePassiveNegativeInflection;
    public string causativePassiveNegativeInflectionRomaji;
    public string imperativeInflection;
    public string imperativeInflectionRomaji;
    public string imperativeNegativeInflection;
    public string imperativeNegativeInflectionRomaji;
}
