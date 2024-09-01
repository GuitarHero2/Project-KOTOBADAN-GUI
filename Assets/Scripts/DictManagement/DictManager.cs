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
    public TMP_Text wordType;
    public TMP_Text wordDef1;
    public TMP_Text wordDef2;
    public TMP_Text wordDef3;
    public TMP_Text wordDef4;
    public TMP_Text wordDef5;
    public TMP_Text wordDef6;
    public TMP_Text wordDef7;
    public TMP_Text wordDef8;
    public TMP_Text wordDef9;
    public TMP_Text wordDef10;
    public TMP_Text wordDef11;
    public TMP_Text wordDef12;
    public TMP_Text wordDef13;
    public TMP_Text wordDef14;
    public TMP_Text wordDef15;
    public TMP_Text wordDef16;
    public TMP_Text wordDef17;
    public TMP_Text wordDef18;
    public TMP_Text wordDef19;
    public TMP_Text wordDef20;
    public TMP_Text wordDef21;
    public TMP_Text wordDef22;
    public TMP_Text wordDef23;
    public TMP_Text wordDef24;
    public TMP_Text example1;
    public TMP_Text example1Alt;
    public TMP_Text example2;
    public TMP_Text example2Alt;
    public TMP_Text jlpt;
    public TMP_Text longDefinitionAboutTheWordEn;
    public TMP_Text longDefinitionAboutTheWordJp;
    public TMP_Text pageNumberText;

    public int pageNumber = 1;
    public GameObject backButton;
    public GameObject line1;
    public GameObject line2;
    public GameObject line3;
    public GameObject line4;
    public GameObject line5;
    public GameObject line6;

    Vector3 movementBetweenPagesLine1;
    Vector3 movementBetweenPagesLine2;
    Vector3 movementBetweenPagesLine3;
    Vector3 movementBetweenPagesLine4;
    Vector3 movementBetweenPagesLine5;
    Vector3 movementBetweenPagesLine6;
    public float distanceBetweenMovement;

    public TMP_Dropdown wordOptionsDropdown;
    private List<InfoList> currentResults = new List<InfoList>();

    public ENDictionary dict = new ENDictionary();

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
        string query = inputField.text.ToLower();
        currentResults = dict.wordList.Where(word => word.word.ToLower() == query || word.kana.ToLower() == query || word.romaji.ToLower() == query).ToList();

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
        wordType.text = foundWord.wordType;
        wordDef1.text = foundWord.def1;
        wordDef2.text = foundWord.def2;
        wordDef3.text = foundWord.def3;
        wordDef4.text = foundWord.def4;
        wordDef5.text = foundWord.def5;
        wordDef6.text = foundWord.def6;
        wordDef7.text = foundWord.def7;
        wordDef8.text = foundWord.def8;
        wordDef9.text = foundWord.def9;
        wordDef10.text = foundWord.def10;
        wordDef11.text = foundWord.def11;
        wordDef12.text = foundWord.def12;
        wordDef13.text = foundWord.def13;
        wordDef14.text = foundWord.def14;
        wordDef15.text = foundWord.def15;
        wordDef16.text = foundWord.def16;
        wordDef17.text = foundWord.def17;
        wordDef18.text = foundWord.def18;
        wordDef19.text = foundWord.def19;
        wordDef20.text = foundWord.def20;
        wordDef21.text = foundWord.def21;
        wordDef22.text = foundWord.def22;
        wordDef23.text = foundWord.def23;
        wordDef24.text = foundWord.def24;
        example1.text = foundWord.example1;
        example1Alt.text = foundWord.example1Alt;
        example2.text = foundWord.example2;
        example2Alt.text = foundWord.example2Alt;
        jlpt.text = foundWord.jlptLevel;
        longDefinitionAboutTheWordEn.text = foundWord.longDefinitionAboutTheWordEn;
        longDefinitionAboutTheWordJp.text = foundWord.longDefinitionAboutTheWordJp;

        if (foundWord.isTheWordAGivenName)
        {
            givenName.text = foundWord.givenName;
        }
        else
        {
            givenName.text = "";
        }

        if (wordType.text.ToLower() == "verb")
        {
            wordType.color = Color.red;
        }
        else if (wordType.text.ToLower() == "noun")
        {
            wordType.color = Color.cyan;
        }
        else if (wordType.text.ToLower() == "adjective")
        {
            wordType.color = Color.yellow;
        }
        else
        {
            wordType.color = Color.white;
        }
    }

    public void ClearHistory() // Clear all textes once the results are null or you go back in the menu.
    {
        inputField.text = "";
        wordSearched.text = "";
        kana.text = "";
        wordDef1.text = "";
        wordDef2.text = "";
        wordDef3.text = "";
        wordDef4.text = "";
        wordDef5.text = "";
        wordDef6.text = "";
        wordDef7.text = "";
        wordDef8.text = "";
        wordDef9.text = "";
        wordDef10.text = "";
        wordDef11.text = "";
        wordDef12.text = "";
        wordDef13.text = "";
        wordDef14.text = "";
        wordDef15.text = "";
        wordDef16.text = "";
        wordDef17.text = "";
        wordDef18.text = "";
        wordDef19.text = "";
        wordDef20.text = "";
        wordDef21.text = "";
        wordDef22.text = "";
        wordDef23.text = "";
        wordDef24.text = "";
        givenName.text = "";
        wordType.text = "";
        example1.text = "";
        example1Alt.text = "";
        example2.text = "";
        example2Alt.text = "";
        pitch.text = "";
        longDefinitionAboutTheWordEn.text = "";
        longDefinitionAboutTheWordJp.text = "";
        isTheWordAGivenName = false;
        wordOptionsDropdown.gameObject.SetActive(false);
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
        pageNumberText.text = pageNumber.ToString();

        if (pageNumber <= 1)
        {
            backButton.SetActive(false);
        }
        else
        {
            backButton.SetActive(true);
        }
    }


    //Menu controll (Temporal Fix, will change it once I move onto the decoration part of the project).
    public void MoveToNextSide()
    {
        pageNumber += 1;
        movementBetweenPagesLine1 = line1.transform.position;
        movementBetweenPagesLine2 = line2.transform.position;
        movementBetweenPagesLine3 = line3.transform.position;
        movementBetweenPagesLine4 = line4.transform.position;
        movementBetweenPagesLine5 = line5.transform.position;
        movementBetweenPagesLine6 = line6.transform.position;
        movementBetweenPagesLine1.x -= distanceBetweenMovement;
        movementBetweenPagesLine2.x -= distanceBetweenMovement;
        movementBetweenPagesLine3.x -= distanceBetweenMovement;
        movementBetweenPagesLine4.x -= distanceBetweenMovement;
        movementBetweenPagesLine5.x -= distanceBetweenMovement;
        movementBetweenPagesLine6.x -= distanceBetweenMovement;
        line1.transform.position = movementBetweenPagesLine1;
        line2.transform.position = movementBetweenPagesLine2;
        line3.transform.position = movementBetweenPagesLine3;
        line4.transform.position = movementBetweenPagesLine4;
        line5.transform.position = movementBetweenPagesLine5;
        line6.transform.position = movementBetweenPagesLine6;
    }

    public void MoveBack()
    {
        pageNumber -= 1;
        movementBetweenPagesLine1 = line1.transform.position;
        movementBetweenPagesLine2 = line2.transform.position;
        movementBetweenPagesLine3 = line3.transform.position;
        movementBetweenPagesLine4 = line4.transform.position;
        movementBetweenPagesLine5 = line5.transform.position;
        movementBetweenPagesLine6 = line6.transform.position;
        movementBetweenPagesLine1.x += distanceBetweenMovement;
        movementBetweenPagesLine2.x += distanceBetweenMovement;
        movementBetweenPagesLine3.x += distanceBetweenMovement;
        movementBetweenPagesLine4.x += distanceBetweenMovement;
        movementBetweenPagesLine5.x += distanceBetweenMovement;
        movementBetweenPagesLine6.x += distanceBetweenMovement;
        line1.transform.position = movementBetweenPagesLine1;
        line2.transform.position = movementBetweenPagesLine2;
        line3.transform.position = movementBetweenPagesLine3;
        line4.transform.position = movementBetweenPagesLine4;
        line5.transform.position = movementBetweenPagesLine5;
        line6.transform.position = movementBetweenPagesLine6;
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
    public string romaji;
    public string wordType;
    public string pitch;
    public string givenName;
    public bool isTheWordAGivenName;
    public string jlptLevel;
    public string def1;
    public string def2;
    public string def3;
    public string def4;
    public string def5;
    public string def6;
    public string def7;
    public string def8;
    public string def9;
    public string def10;
    public string def11;
    public string def12;
    public string def13;
    public string def14;
    public string def15;
    public string def16;
    public string def17;
    public string def18;
    public string def19;
    public string def20;
    public string def21;
    public string def22;
    public string def23;
    public string def24;
    public string example1;
    public string example1Alt;
    public string example2;
    public string example2Alt;
    public string longDefinitionAboutTheWordEn;
    public string longDefinitionAboutTheWordJp;
}