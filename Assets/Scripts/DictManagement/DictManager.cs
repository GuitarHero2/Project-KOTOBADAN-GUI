using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;

public class DictManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text kana;
    public TMP_Text wordSearched;
    public TMP_Text wordDef1;
    public TMP_Text wordDef2;
    public TMP_Text wordDef3;
    public TMP_Text wordDef4;
    public TMP_Text wordDef5;
    public TMP_Text wordDef6;
    public TMP_Text example1;
    public TMP_Text example1Alt;
    public TMP_Text example2;
    public TMP_Text example2Alt;

    public ENDictionary dict = new ENDictionary();

    public void SaveToJson()
    {
        string dictData = JsonUtility.ToJson(dict);
        string fileLocation = Application.persistentDataPath + "/KTBDictEN.json";
        File.WriteAllText(fileLocation, dictData);
    }

    public void LoadToJson()
    {
        string fileLocation = Application.persistentDataPath + "/KTBDictEN.json";
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

    public void SearchWord()
    {
        string query = inputField.text.ToLower();
        InfoList foundWord = dict.wordList.FirstOrDefault(word => word.word.ToLower() == query || word.kana.ToLower() == query);

        if (foundWord != null)
        {
            wordSearched.text = foundWord.word;
            kana.text = foundWord.kana;
            wordDef1.text = foundWord.def1;
            wordDef2.text = foundWord.def2;
            wordDef3.text = foundWord.def3;
            wordDef4.text = foundWord.def4;
            wordDef5.text = foundWord.def5;
            wordDef6.text = foundWord.def6;
            example1.text = foundWord.example1;
            example1Alt.text = foundWord.example1Alt;
            example2.text = foundWord.example2;
            example2Alt.text = foundWord.example2Alt;
        }
        else
        {
            wordSearched.text = "Word not found";
            kana.text = "";
            wordDef1.text = "";
            wordDef2.text = "";
            wordDef3.text = "";
            wordDef4.text = "";
            wordDef5.text = "";
            wordDef6.text = "";
            example1.text = "";
            example1Alt.text = "";
            example2.text = "";
            example2Alt.text = "";
        }
    }

    public void ClearHistory()
    {
        wordSearched.text = "";
        kana.text = "";
        wordDef1.text = "";
        wordDef2.text = "";
        wordDef3.text = "";
        wordDef4.text = "";
        wordDef5.text = "";
        wordDef6.text = "";
        example1.text = "";
        example1Alt.text = "";
        example2.text = "";
        example2Alt.text = "";
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
    public string jlptLevel;
    public string def1;
    public string def2;
    public string def3;
    public string def4;
    public string def5;
    public string def6;
    public string example1;
    public string example1Alt;
    public string example2;
    public string example2Alt;
}