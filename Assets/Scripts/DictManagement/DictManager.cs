using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class DictManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text kana;
    public TMP_Text givenName;
    public bool isTheWordAGivenName;
    public TMP_Text pitch;
    public TMP_Text wordSearched;
    public TMP_Text wordType1;
    public TMP_Text wordType2;
    public TMP_Text[] wordDef;
    public TMP_Text example1;
    public TMP_Text example1Alt;
    public TMP_Text example2;
    public TMP_Text example2Alt;
    public TMP_Text jlpt;
    public TMP_Text longDefinitionAboutTheWordEn;
    public TMP_Text longDefinitionAboutTheWordJp;
    public TMP_Text pageNumberText;
    public string currentRelatedWord;

    public bool isTheWordAVerb;
    public TMP_Text[] verbInflections;
    public GameObject showVerbInflectionsButton;

    public int pageNumber = 1;
    public GameObject backButton;
    public GameObject searchRelatedWordButton;
    public TMP_Text searchRelatedWordText;
    public GameObject line1;
    public GameObject line2;
    public GameObject line3;
    public GameObject line4;
    public GameObject line5;
    public GameObject line6;

    public Scrollbar defScrollbar;

    public float distanceBetweenMovement;

    public TMP_Dropdown wordOptionsDropdown;
    private List<InfoList> currentResults = new List<InfoList>();

    public ENDictionary dict = new ENDictionary();

    public bool alternateBetweenCustomDictAndDict = false;

    public void SaveToJson()
    {
        string dictData = JsonUtility.ToJson(dict);
        string fileLocation = Application.persistentDataPath + "/KTBDict.json";
        File.WriteAllText(fileLocation, dictData);
    }

    public void LoadToJson()
    {
        string fileLocation = Application.persistentDataPath + "/KTBDict.json";
        if (File.Exists(fileLocation))
        {
            string dictData = File.ReadAllText(fileLocation);
            dict = JsonUtility.FromJson<ENDictionary>(dictData);
        }
        else
        {
            SaveToJson();
        }
    }

    public void SearchWord() // Method for searching words and updating the dropdown to show multiple search alternatives.
    {
        ResetScrollbar();

        string query = inputField.text.ToLower();
        currentResults = dict.wordList.Where(word => word.word == query || word.kana == query || word.romaji.ToLower() == query || word.hiragana == query || word.alternativeForm.ToLower() == query ||
        word.teFormInflection  == query || word.teFormInflectionRomaji.ToLower() == query ||
        word.nonPastNegativeInflection == query || word.nonPastNegativeInflectionRomaji.ToLower() == query ||
        word.nonPastNegativeInflectionPolite == query || word.nonPastNegativeInflectionPoliteRomaji.ToLower() == query ||
        word.pastInflection == query || word.pastInflectionRomaji.ToLower() == query ||
        word.pastNegativeInflection == query || word.pastNegativeInflectionRomaji == query ||
        word.pastInflectionPolite == query || word.pastInflectionPoliteRomaji.ToLower() == query ||
        word.pastNegativeInflectionPolite == query || word.pastNegativeInflectionPoliteRomaji.ToLower() == query ||
        word.potentialInflection == query || word.potentialInflectionRomaji.ToLower() == query ||
        word.potentialNegativeInflection == query || word.potentialNegativeInflectionRomaji.ToLower() == query ||
        word.passiveInflection == query || word.passiveInflectionRomaji.ToLower() == query ||
        word.passiveNegativeInflection == query || word.passiveNegativeInflectionRomaji.ToLower() == query ||
        word.causativeInflection == query || word.causativeInflectionRomaji.ToLower() == query ||
        word.causativeNegativeInflection == query || word.causativeNegativeInflectionRomaji.ToLower() == query ||
        word.causativePassiveInflection == query || word.causativePassiveInflectionRomaji.ToLower() == query ||
        word.causativePassiveNegativeInflection == query || word.causativePassiveNegativeInflectionRomaji.ToLower() == query ||
        word.imperativeInflection == query || word.imperativeInflectionRomaji.ToLower() == query ||
        word.imperativeNegativeInflection == query || word.imperativeNegativeInflectionRomaji.ToLower() == query).ToList();

        if (currentResults.Count > 0)
        {
            wordOptionsDropdown.gameObject.SetActive(true);
            wordOptionsDropdown.ClearOptions();
            wordOptionsDropdown.AddOptions(currentResults.Select(word => word.word).ToList());
            wordOptionsDropdown.onValueChanged.RemoveAllListeners();
            wordOptionsDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            DisplayWord(currentResults[0]);
        }
        else
        {
            ClearHistory();
            wordSearched.text = "Word not found";
        }
    }
    public void SearchRelatedWord()
    {
        ResetScrollbar();

        string query = currentRelatedWord;
        currentResults = dict.wordList.Where(word => word.word.ToLower() == query || word.kana.ToLower() == query || word.romaji.ToLower() == query || word.hiragana.ToLower() == query || word.alternativeForm.ToLower() == query).ToList();

        if (currentResults.Count > 0)
        {
            wordOptionsDropdown.gameObject.SetActive(true);
            wordOptionsDropdown.ClearOptions();
            wordOptionsDropdown.AddOptions(currentResults.Select(word => word.word).ToList());
            wordOptionsDropdown.onValueChanged.RemoveAllListeners();
            wordOptionsDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            DisplayWord(currentResults[0]);
        }
        else
        {
            ClearHistory();
            wordSearched.text = "Word not found";
        }

        if (query == "")
        {
            isTheWordAVerb = false;
        }
    }

    public void OnDropdownValueChanged(int index) // Dropdown search updater
    {
        if (index >= 0 && index < currentResults.Count)
        {
            DisplayWord(currentResults[index]);
        }
    }

    public void DisplayWord(InfoList foundWord)
    {
        wordSearched.text = foundWord.word;
        kana.text = foundWord.kana;
        pitch.text = foundWord.pitch;
        wordType1.text = foundWord.wordType1;
        wordType2.text = foundWord.wordType2;

        for (int i = 0; i < wordDef.Length; i++)
        {
            if (i < foundWord.def.Length)
            {
                wordDef[i].text = foundWord.def[i];
            }
            else
            {
                wordDef[i].text = "";
            }
        }

        isTheWordAVerb = foundWord.isTheWordAVerb;
        if (isTheWordAVerb == true)
        {
            showVerbInflectionsButton.SetActive(true);
            verbInflections[0].text = foundWord.nonPastInflection;
            verbInflections[1].text = foundWord.nonPastNegativeInflection;
            verbInflections[2].text = foundWord.nonPastInflectionPolite;
            verbInflections[3].text = foundWord.nonPastNegativeInflectionPolite;
            verbInflections[4].text = foundWord.pastInflection;
            verbInflections[5].text = foundWord.pastNegativeInflection;
            verbInflections[6].text = foundWord.pastInflectionPolite;
            verbInflections[7].text = foundWord.pastNegativeInflectionPolite;
            verbInflections[8].text = foundWord.teFormInflection;
            verbInflections[9].text = foundWord.teFormInflectionNegative;
            verbInflections[10].text = foundWord.potentialInflection;
            verbInflections[11].text = foundWord.potentialNegativeInflection;
            verbInflections[12].text = foundWord.passiveInflection;
            verbInflections[13].text = foundWord.passiveNegativeInflection;
            verbInflections[14].text = foundWord.causativeInflection;
            verbInflections[15].text = foundWord.causativeNegativeInflection;
            verbInflections[16].text = foundWord.imperativeInflection;
            verbInflections[17].text = foundWord.imperativeNegativeInflection;
        }
        else
        {
            showVerbInflectionsButton.SetActive(false);
            for (int i = 0; i < 18; i++)
            {
                verbInflections[i].text = "";
            }
        }

        example1.text = foundWord.example1;
        example1Alt.text = foundWord.example1Alt;
        example2.text = foundWord.example2;
        example2Alt.text = foundWord.example2Alt;
        jlpt.text = foundWord.jlptLevel;
        longDefinitionAboutTheWordEn.text = foundWord.longDefinitionAboutTheWordEn;
        longDefinitionAboutTheWordJp.text = foundWord.longDefinitionAboutTheWordJp;
        currentRelatedWord = foundWord.relatedWord;
        searchRelatedWordText.text = foundWord.relatedWord;

        if (foundWord.isTheWordAGivenName)
        {
            givenName.text = foundWord.givenName;
        }
        else
        {
            givenName.text = "";
        }

        //Word Type 1
        if (wordType1.text.ToLower() == "verb")
        {
            wordType1.color = Color.red;
        }
        else if (wordType1.text.ToLower() == "noun")
        {
            wordType1.color = Color.cyan;
        }
        else if (wordType1.text.ToLower() == "na-adjective")
        {
            wordType1.color = Color.yellow;
        }
        else if (wordType1.text.ToLower() == "i-adjective" || wordType1.text.ToLower() == "?-adjective")
        {
            wordType1.color = Color.magenta;
        }
        else if (wordType1.text.ToLower() == "suru verb")
        {
            wordType1.color = Color.gray;
        }
        else
        {
            wordType1.color = Color.white;
        }
        //Word Type 2
        if (wordType2.text.ToLower() == "verb")
        {
            wordType2.color = Color.red;
        }
        else if (wordType2.text.ToLower() == "noun")
        {
            wordType2.color = Color.cyan;
        }
        else if (wordType2.text.ToLower() == "na-adjective")
        {
            wordType2.color = Color.yellow;
        }
        else if (wordType2.text.ToLower() == "i-adjective" || wordType2.text.ToLower() == "?-adjective")
        {
            wordType2.color = Color.magenta;
        }
        else if (wordType2.text.ToLower() == "suru verb")
        {
            wordType2.color = Color.gray;
        }
        else
        {
            wordType2.color = Color.white;
        }
    }

    public void ClearHistory() // Clear all textes once the results are null or you go back in the menu.
    {
        currentRelatedWord = "";
        inputField.text = "";
        wordSearched.text = "";
        kana.text = "";
        jlpt.text = "";
        for (int i = 0; i < wordDef.Length; i++)
        {
            wordDef[i].text = "";
        }

        for (int i = 0; i < 18; i++)
        {
            verbInflections[i].text = "";
        }

        wordType1.text = "";
        wordType2.text = "";
        example1.text = "";
        example1Alt.text = "";
        example2.text = "";
        example2Alt.text = "";
        pitch.text = "";
        longDefinitionAboutTheWordEn.text = "";
        longDefinitionAboutTheWordJp.text = "";
        isTheWordAGivenName = false;
        wordOptionsDropdown.gameObject.SetActive(false);
        showVerbInflectionsButton.SetActive(false);
    }

    void Start()
    {
        try
        {
            LoadToJson();
        }
        catch (FileNotFoundException e)
        {
            SaveToJson();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SearchWord();
            inputField.ActivateInputField();
        }

        pageNumberText.text = pageNumber.ToString();

        if (pageNumber <= 1)
        {
            backButton.SetActive(false);
        }
        else
        {
            backButton.SetActive(true);
        }

        if (currentRelatedWord != "")
        {
            searchRelatedWordButton.SetActive(true);
        }
        else
        {
            searchRelatedWordButton.SetActive(false);
        }

        /*if (isTheWordAVerb == true)
        {
            showVerbInflectionsButton.SetActive(true);
        }
        else
        {
            showVerbInflectionsButton.SetActive(false);
        }*/
    }

    public void ResetScrollbar()
    {
        defScrollbar.value = 100;
    }
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