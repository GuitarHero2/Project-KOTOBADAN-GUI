using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DictManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text kana;
    [SerializeField] private TMP_Text givenName;
    [SerializeField] private TMP_Text pitch;
    [SerializeField] private TMP_Text wordSearched;
    [SerializeField] private TMP_Text wordType1;
    [SerializeField] private TMP_Text wordType2;
    [SerializeField] private TMP_Text[] wordDef;
    [SerializeField] private TMP_Text example1;
    [SerializeField] private TMP_Text example1Alt;
    [SerializeField] private TMP_Text example2;
    [SerializeField] private TMP_Text example2Alt;
    [SerializeField] private TMP_Text jlpt;
    [SerializeField] private TMP_Text longDefinitionAboutTheWordEn;
    [SerializeField] private TMP_Text longDefinitionAboutTheWordJp;
    [SerializeField] private TMP_Text pageNumberText;
    [SerializeField] private TMP_Text[] verbInflections;
    [SerializeField] private TMP_Dropdown wordOptionsDropdown;
    [SerializeField] private TMP_Text searchRelatedWordText;

    [Header("UI Controls")]
    [SerializeField] private GameObject showVerbInflectionsButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject searchRelatedWordButton;
    [SerializeField] private GameObject noDictButton;
    [SerializeField] private Scrollbar defScrollbar;

    [Header("Settings")]
    [SerializeField] private string fileName = "KTBDict.json";
    [SerializeField] private bool enableVerbSearch = true;
    [SerializeField] private bool enableRelatedWordSearch = true;

    // Private fields
    private bool isTheWordAVerb;
    private bool isTheWordAGivenName;
    private int pageNumber = 1;
    private string currentRelatedWord = "";
    private List<InfoList> currentResults = new List<InfoList>();
    private ENDictionary dict = new ENDictionary();
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    #region Unity Lifecycle

    private void Start()
    {
        InitializeDictionary();
        SetupUI();
    }

    private void Update()
    {
        HandleInput();
        UpdateUI();
    }

    #endregion

    #region Initialization

    private void InitializeDictionary()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                noDictButton?.SetActive(true);
                Debug.Log("Dictionary file not found. Creating new dictionary.");
            }
            
            LoadFromJson();
            Debug.Log($"Dictionary loaded successfully. Total words: {dict.wordList.Count}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize dictionary: {e.Message}");
            SaveToJson();
        }
    }

    private void SetupUI()
    {
        UpdatePageNumber();
        UpdateBackButton();
        UpdateSearchRelatedButton();
        ResetScrollbar();
    }

    #endregion

    #region Data Persistence

    public void SaveToJson()
    {
        try
        {
            if (dict == null)
            {
                dict = new ENDictionary();
            }

            string dictData = JsonUtility.ToJson(dict, true);
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
            dict = new ENDictionary();
            SaveToJson();
            return;
        }

        try
        {
            string dictData = File.ReadAllText(FilePath);
            dict = JsonUtility.FromJson<ENDictionary>(dictData);
            
            if (dict == null)
            {
                dict = new ENDictionary();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load dictionary: {e.Message}");
            dict = new ENDictionary();
        }
    }

    #endregion

    #region Search Methods

    public void SearchWord()
    {
        if (string.IsNullOrWhiteSpace(inputField?.text))
        {
            Debug.LogWarning("Search term is empty");
            return;
        }

        ResetScrollbar();
        string query = inputField.text.ToLower().Trim();
        currentResults = PerformSearch(query);

        UpdateSearchResults();
    }

    public void SearchRelatedWord()
    {
        if (string.IsNullOrWhiteSpace(currentRelatedWord))
        {
            Debug.LogWarning("No related word to search");
            return;
        }

        ResetScrollbar();
        string query = currentRelatedWord.ToLower().Trim();
        currentResults = PerformSearch(query);

        UpdateSearchResults();
    }

    private List<InfoList> PerformSearch(string query)
    {
        if (dict?.wordList == null)
        {
            return new List<InfoList>();
        }

        return dict.wordList.Where(word => MatchesSearchCriteria(word, query)).ToList();
    }

    private bool MatchesSearchCriteria(InfoList word, string query)
    {
        // Basic search fields
        if (MatchesField(word.word, query) ||
            MatchesField(word.kana, query) ||
            MatchesField(word.romaji, query) ||
            MatchesField(word.hiragana, query) ||
            MatchesField(word.alternativeForm, query))
        {
            return true;
        }

        // Verb inflections search (if enabled)
        if (enableVerbSearch && word.isTheWordAVerb)
        {
            return MatchesVerbInflections(word, query);
        }

        return false;
    }

    private bool MatchesField(string field, string query)
    {
        return !string.IsNullOrWhiteSpace(field) && 
               field.ToLower().Contains(query);
    }

    private bool MatchesVerbInflections(InfoList word, string query)
    {
        var inflections = new[]
        {
            word.nonPastInflection, word.nonPastInflectionRomaji,
            word.nonPastNegativeInflection, word.nonPastNegativeInflectionRomaji,
            word.nonPastInflectionPolite, word.nonPastInflectionPoliteRomaji,
            word.nonPastNegativeInflectionPolite, word.nonPastNegativeInflectionPoliteRomaji,
            word.pastInflection, word.pastInflectionRomaji,
            word.pastNegativeInflection, word.pastNegativeInflectionRomaji,
            word.pastInflectionPolite, word.pastInflectionPoliteRomaji,
            word.pastNegativeInflectionPolite, word.pastNegativeInflectionPoliteRomaji,
            word.teFormInflection, word.teFormInflectionRomaji,
            word.teFormInflectionNegative, word.teFormInflectionNegativeRomaji,
            word.potentialInflection, word.potentialInflectionRomaji,
            word.potentialNegativeInflection, word.potentialNegativeInflectionRomaji,
            word.passiveInflection, word.passiveInflectionRomaji,
            word.passiveNegativeInflection, word.passiveNegativeInflectionRomaji,
            word.causativeInflection, word.causativeInflectionRomaji,
            word.causativeNegativeInflection, word.causativeNegativeInflectionRomaji,
            word.causativePassiveInflection, word.causativePassiveInflectionRomaji,
            word.causativePassiveNegativeInflection, word.causativePassiveNegativeInflectionRomaji,
            word.imperativeInflection, word.imperativeInflectionRomaji,
            word.imperativeNegativeInflection, word.imperativeNegativeInflectionRomaji
        };

        return inflections.Any(inflection => MatchesField(inflection, query));
    }

    private void UpdateSearchResults()
    {
        if (currentResults.Count > 0)
        {
            ShowDropdown();
            DisplayWord(currentResults[0]);
        }
        else
        {
            ClearHistory();
            wordSearched.text = "Word not found";
        }
    }

    private void ShowDropdown()
    {
        if (wordOptionsDropdown != null)
        {
            wordOptionsDropdown.gameObject.SetActive(true);
            wordOptionsDropdown.ClearOptions();
            wordOptionsDropdown.AddOptions(currentResults.Select(word => word.word).ToList());
            wordOptionsDropdown.onValueChanged.RemoveAllListeners();
            wordOptionsDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    #endregion

    #region UI Management

    public void OnDropdownValueChanged(int index)
    {
        if (index >= 0 && index < currentResults.Count)
        {
            DisplayWord(currentResults[index]);
        }
    }

    public void DisplayWord(InfoList foundWord)
    {
        if (foundWord == null) return;

        // Basic word information
        SetTextSafely(wordSearched, foundWord.word);
        SetTextSafely(kana, foundWord.kana);
        SetTextSafely(pitch, foundWord.pitch);
        SetTextSafely(wordType1, foundWord.wordType1);
        SetTextSafely(wordType2, foundWord.wordType2);
        SetTextSafely(jlpt, foundWord.jlptLevel);

        // Definitions
        UpdateDefinitions(foundWord.def);

        // Examples
        SetTextSafely(example1, foundWord.example1);
        SetTextSafely(example1Alt, foundWord.example1Alt);
        SetTextSafely(example2, foundWord.example2);
        SetTextSafely(example2Alt, foundWord.example2Alt);

        // Long definitions
        SetTextSafely(longDefinitionAboutTheWordEn, foundWord.longDefinitionAboutTheWordEn);
        SetTextSafely(longDefinitionAboutTheWordJp, foundWord.longDefinitionAboutTheWordJp);

        // Related word
        currentRelatedWord = foundWord.relatedWord ?? "";
        SetTextSafely(searchRelatedWordText, foundWord.relatedWord);

        // Given name
        UpdateGivenName(foundWord);

        // Verb inflections
        UpdateVerbInflections(foundWord);

        // Word type colors
        UpdateWordTypeColors();
    }

    private void SetTextSafely(TMP_Text textComponent, string value)
    {
        if (textComponent != null)
        {
            textComponent.text = value ?? "";
        }
    }

    private void SetInputFieldSafely(TMP_InputField inputField, string value)
    {
        if (inputField != null)
        {
            inputField.text = value ?? "";
        }
    }

    private void UpdateDefinitions(string[] definitions)
    {
        if (wordDef == null) return;

        for (int i = 0; i < wordDef.Length; i++)
        {
            if (i < definitions?.Length)
            {
                SetTextSafely(wordDef[i], definitions[i]);
            }
            else
            {
                SetTextSafely(wordDef[i], "");
            }
        }
    }

    private void UpdateGivenName(InfoList word)
    {
        if (word.isTheWordAGivenName)
        {
            SetTextSafely(givenName, word.givenName);
        }
        else
        {
            SetTextSafely(givenName, "");
        }
    }

    private void UpdateVerbInflections(InfoList word)
    {
        isTheWordAVerb = word.isTheWordAVerb;
        
        if (showVerbInflectionsButton != null)
        {
            showVerbInflectionsButton.SetActive(isTheWordAVerb);
        }

        if (verbInflections != null)
        {
            if (isTheWordAVerb)
            {
                SetVerbInflectionText(0, word.nonPastInflection);
                SetVerbInflectionText(1, word.nonPastNegativeInflection);
                SetVerbInflectionText(2, word.nonPastInflectionPolite);
                SetVerbInflectionText(3, word.nonPastNegativeInflectionPolite);
                SetVerbInflectionText(4, word.pastInflection);
                SetVerbInflectionText(5, word.pastNegativeInflection);
                SetVerbInflectionText(6, word.pastInflectionPolite);
                SetVerbInflectionText(7, word.pastNegativeInflectionPolite);
                SetVerbInflectionText(8, word.teFormInflection);
                SetVerbInflectionText(9, word.teFormInflectionNegative);
                SetVerbInflectionText(10, word.potentialInflection);
                SetVerbInflectionText(11, word.potentialNegativeInflection);
                SetVerbInflectionText(12, word.passiveInflection);
                SetVerbInflectionText(13, word.passiveNegativeInflection);
                SetVerbInflectionText(14, word.causativeInflection);
                SetVerbInflectionText(15, word.causativeNegativeInflection);
                SetVerbInflectionText(16, word.imperativeInflection);
                SetVerbInflectionText(17, word.imperativeNegativeInflection);
            }
            else
            {
                for (int i = 0; i < verbInflections.Length; i++)
                {
                    SetVerbInflectionText(i, "");
                }
            }
        }
    }

    private void SetVerbInflectionText(int index, string text)
    {
        if (index >= 0 && index < verbInflections?.Length)
        {
            SetTextSafely(verbInflections[index], text);
        }
    }

    private void UpdateWordTypeColors()
    {
        UpdateWordTypeColor(wordType1);
        UpdateWordTypeColor(wordType2);
    }

    private void UpdateWordTypeColor(TMP_Text wordTypeText)
    {
        if (wordTypeText == null) return;

        string text = wordTypeText.text.ToLower();
        wordTypeText.color = text switch
        {
            "verb" => Color.red,
            "noun" => Color.cyan,
            "na-adjective" => Color.yellow,
            "i-adjective" or "?-adjective" => Color.magenta,
            "suru verb" => Color.gray,
            _ => Color.white
        };
    }

    public void ClearHistory()
    {
        currentRelatedWord = "";
        SetInputFieldSafely(inputField, "");
        SetTextSafely(wordSearched, "");
        SetTextSafely(kana, "");
        SetTextSafely(jlpt, "");
        SetTextSafely(wordType1, "");
        SetTextSafely(wordType2, "");
        SetTextSafely(example1, "");
        SetTextSafely(example1Alt, "");
        SetTextSafely(example2, "");
        SetTextSafely(example2Alt, "");
        SetTextSafely(pitch, "");
        SetTextSafely(longDefinitionAboutTheWordEn, "");
        SetTextSafely(longDefinitionAboutTheWordJp, "");
        SetTextSafely(givenName, "");

        UpdateDefinitions(new string[0]);
        UpdateVerbInflections(new InfoList { isTheWordAVerb = false });

        if (wordOptionsDropdown != null)
        {
            wordOptionsDropdown.gameObject.SetActive(false);
        }

        if (showVerbInflectionsButton != null)
        {
            showVerbInflectionsButton.SetActive(false);
        }
    }

    #endregion

    #region Input Handling

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SearchWord();
            inputField?.ActivateInputField();
        }
    }

    private void UpdateUI()
    {
        UpdatePageNumber();
        UpdateBackButton();
        UpdateSearchRelatedButton();
    }

    private void UpdatePageNumber()
    {
        if (pageNumberText != null)
        {
            pageNumberText.text = pageNumber.ToString();
        }
    }

    private void UpdateBackButton()
    {
        if (backButton != null)
        {
            backButton.SetActive(pageNumber > 1);
        }
    }

    private void UpdateSearchRelatedButton()
    {
        if (searchRelatedWordButton != null)
        {
            searchRelatedWordButton.SetActive(!string.IsNullOrEmpty(currentRelatedWord));
        }
    }

    public void ResetScrollbar()
    {
        if (defScrollbar != null)
        {
            defScrollbar.value = 1f;
        }
    }

    #endregion

    #region Public Properties

    public ENDictionary Dictionary => dict;
    public int WordCount => dict?.wordList?.Count ?? 0;
    public bool IsVerb => isTheWordAVerb;
    public int PageNumber => pageNumber;

    #endregion
}

[System.Serializable]
public class ENDictionary
{
    public List<InfoList> wordList = new List<InfoList>();
}

[System.Serializable]
public class InfoList
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