using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DictManager : MonoBehaviour
{
    public Words words = new Words();
    public void SaveToJson()
    {
        string wordsData = JsonUtility.ToJson(words);
        string filePath = Application.persistentDataPath + "/KTBDictEN.json";
        Debug.Log(filePath);
        File.WriteAllText(filePath, wordsData);
    }

    public void LoadFromJsonFile()
    {
        string filePath = Application.persistentDataPath + "/KTBDictEN.json";
        string wordsData = File.ReadAllText(filePath);

        words = JsonUtility.FromJson<Words>(wordsData);
        Debug.Log("Dictionary loaded successfully!");
    }

    void Start()
    {
        try
        {
            LoadFromJsonFile();
        }
        catch
        {
            Debug.Log("The dictionary file could not be found. Creating a new empty file to be replaced...");
            SaveToJson();
            Debug.Log("ENDict was created successfully!");
        }
    }

    [System.Serializable]
    public class Words
    {
        public string kana;
        public string def1;
        public string def2;
        public string def3;
        public string def4;
        public string def5;
        public string def6;
        public string example1;
        public string example2;
        public string example3;
    }

    [System.Serializable]
    public class WordList
    {
        public Words[] words;
    }

    public WordList eNWordList = new WordList();

}

