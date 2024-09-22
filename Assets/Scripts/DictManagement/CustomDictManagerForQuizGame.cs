using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class CustomDictManagerForQuizGame : MonoBehaviour
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


    private List<InfoListFQG> current = new List<InfoListFQG>();

    public DeckFQG deck = new DeckFQG();
    public QuizMinigame quizMinigame;

    public void SaveToJson()
    {
        string dictData = JsonUtility.ToJson(deck);
        string fileLocation = Application.persistentDataPath + "/CustomDeckFQG.json";
        File.WriteAllText(fileLocation, dictData);
    }

    public void LoadToJson()
    {
        string fileLocation = Application.persistentDataPath + "/CustomDeckFQG.json";
        if (File.Exists(fileLocation))
        {
            string dictData = File.ReadAllText(fileLocation);
            deck = JsonUtility.FromJson<DeckFQG>(dictData);
        }
        else
        {
            SaveToJson();
        }
    }

    public void SaveWordIntoDeck()
    {
        // For later
    }

    void Start()
    {
        quizMinigame = gameObject.GetComponent<QuizMinigame>();

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

    }
}

[System.Serializable]
public class DeckFQG
{
    public List<InfoListFQG> wordList = new List<InfoListFQG>();
}

[System.Serializable]
public class InfoListFQG
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
}
