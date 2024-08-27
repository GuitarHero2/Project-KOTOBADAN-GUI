/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;

public class DictManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text wordSearched;
    public TMP_Text wordDef1;
    public TMP_Text wordDef2;
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
        string dictData = File.ReadAllText(fileLocation);

        dict = JsonUtility.FromJson<ENDictionary>(dictData);
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
    public string example2;
    public string example3;
}*/