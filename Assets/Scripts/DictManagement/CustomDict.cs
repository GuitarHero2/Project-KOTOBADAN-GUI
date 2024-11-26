using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CustomDict : MonoBehaviour
{
    
    public bool isTheWordAVerb;
    public TMP_InputField[] verbInflections;
    public GameObject toggleVerbButton;
    public TMP_Text toggleVerbButtonText;

    public TMP_InputField wordIF;
    public TMP_InputField kanaIF;
    public TMP_InputField pitchIF;
    public TMP_InputField jlptLevelIF;
    public TMP_InputField wordType1IF;
    public TMP_InputField wordType2IF;
    public TMP_InputField relatedWordIF;
    public TMP_InputField[] wordDefIF;
    public TMP_InputField example1JpIF;
    public TMP_InputField example1EnIF;
    public TMP_InputField example2JpIF;
    public TMP_InputField example2EnIF;
    public TMP_InputField longDefForWordJpIF;
    public TMP_InputField longDefForWordEnIF;
    public TMP_InputField romajiIF;
    public TMP_InputField hiraganaIF;
    public TMP_InputField alternativeFormIF;

    public GameObject backButton;
    public GameObject searchRelatedWordButton;
    public TMP_Text searchRelatedWordText;

    public TMP_Dropdown wordOptionsDropdown;
    private List<InfoListFCJ> currentResults = new List<InfoListFCJ>();

    public CustomJisho dict = new CustomJisho();
    public int currentDictWordAmount;
    public void SaveToJson()
    {
        string dictData = JsonUtility.ToJson(dict);
        string fileLocation = Application.persistentDataPath + "/CustomJisho.json";
        File.WriteAllText(fileLocation, dictData);
    }

    public void LoadToJson()
    {
        string fileLocation = Application.persistentDataPath + "/CustomJisho.json";
        if (File.Exists(fileLocation))
        {
            string dictData = File.ReadAllText(fileLocation);
            dict = JsonUtility.FromJson<CustomJisho>(dictData);
        }
        else
        {
            SaveToJson();
        }
    }

    public void AddNewWord()
    {
        dict.wordList.Insert(currentDictWordAmount, new InfoListFCJ());
        dict.wordList[currentDictWordAmount].word = wordIF.text;
        dict.wordList[currentDictWordAmount].kana = kanaIF.text;
        dict.wordList[currentDictWordAmount].pitch = pitchIF.text;
        dict.wordList[currentDictWordAmount].jlptLevel = jlptLevelIF.text;
        dict.wordList[currentDictWordAmount].wordType1 = wordType1IF.text;
        dict.wordList[currentDictWordAmount].wordType2 = wordType2IF.text;
        dict.wordList[currentDictWordAmount].relatedWord = relatedWordIF.text;

        dict.wordList[currentDictWordAmount].example1 = example1JpIF.text;
        dict.wordList[currentDictWordAmount].example1Alt = example1EnIF.text;
        dict.wordList[currentDictWordAmount].example2 = example2JpIF.text;
        dict.wordList[currentDictWordAmount].example2Alt = example2EnIF.text;
        dict.wordList[currentDictWordAmount].longDefinitionAboutTheWordEn = longDefForWordEnIF.text;
        dict.wordList[currentDictWordAmount].longDefinitionAboutTheWordJp = longDefForWordJpIF.text;
        dict.wordList[currentDictWordAmount].romaji = romajiIF.text;
        dict.wordList[currentDictWordAmount].hiragana = hiraganaIF.text;
        dict.wordList[currentDictWordAmount].alternativeForm = alternativeFormIF.text;

        dict.wordList[currentDictWordAmount].isTheWordAVerb = isTheWordAVerb;
        dict.wordList[currentDictWordAmount].nonPastInflection = verbInflections[0].text;
        dict.wordList[currentDictWordAmount].nonPastNegativeInflection = verbInflections[1].text;
        dict.wordList[currentDictWordAmount].nonPastInflectionPolite = verbInflections[2].text;
        dict.wordList[currentDictWordAmount].nonPastNegativeInflectionPolite = verbInflections[3].text;
        dict.wordList[currentDictWordAmount].pastInflection = verbInflections[4].text;
        dict.wordList[currentDictWordAmount].pastNegativeInflection = verbInflections[5].text;
        dict.wordList[currentDictWordAmount].pastInflectionPolite = verbInflections[6].text;
        dict.wordList[currentDictWordAmount].pastNegativeInflectionPolite = verbInflections[7].text;
        dict.wordList[currentDictWordAmount].teFormInflection = verbInflections[8].text;
        dict.wordList[currentDictWordAmount].teFormInflectionNegative = verbInflections[9].text;
        dict.wordList[currentDictWordAmount].potentialInflection = verbInflections[10].text;
        dict.wordList[currentDictWordAmount].potentialNegativeInflection = verbInflections[11].text;
        dict.wordList[currentDictWordAmount].passiveInflection = verbInflections[12].text;
        dict.wordList[currentDictWordAmount].passiveNegativeInflection = verbInflections[13].text;
        dict.wordList[currentDictWordAmount].causativeInflection = verbInflections[14].text;
        dict.wordList[currentDictWordAmount].causativeNegativeInflection = verbInflections[15].text;
        dict.wordList[currentDictWordAmount].imperativeInflection = verbInflections[16].text;
        dict.wordList[currentDictWordAmount].imperativeNegativeInflection = verbInflections[17].text;

        dict.wordList[currentDictWordAmount].def = new string[wordDefIF.Length];

        for (int i = 0; i < wordDefIF.Length; i++)
        {
            dict.wordList[currentDictWordAmount].def[i] = wordDefIF[i].text;
        }
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

    }

    public void ToggleVerb()
    {
        if (isTheWordAVerb == true)
        {
            isTheWordAVerb = false;
            toggleVerbButtonText.text = "Toggle Verb: OFF";
        }
        else
        {
            isTheWordAVerb = true;
            toggleVerbButtonText.text = "Toggle Verb: ON";
        }
    }
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
