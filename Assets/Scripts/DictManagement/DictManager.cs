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
    public TMP_Text example1;
    public TMP_Text example1Alt;
    public TMP_Text example2;
    public TMP_Text example2Alt;
    public TMP_Text jlpt;

    public Vector3 movementBetweenPagesLine1;
    public Vector3 movementBetweenPagesLine2;
    public Vector3 movementBetweenPagesLine3;
    public Vector3 movementBetweenPagesLine4;
    public Vector3 movementBetweenPagesLine5;
    public Vector3 movementBetweenPagesLine6;
    public Vector3 movementBetweenPagesLine7;

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
        InfoList foundWord = dict.wordList.FirstOrDefault(word => word.word.ToLower() == query || word.kana.ToLower() == query || word.romaji.ToLower() == query);

        if (foundWord != null)
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
            example1.text = foundWord.example1;
            example1Alt.text = foundWord.example1Alt;
            example2.text = foundWord.example2;
            example2Alt.text = foundWord.example2Alt;
            jlpt.text = foundWord.jlptLevel;

            if (foundWord.isTheWordAGivenName == true)
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
        }
        else
        {
            wordSearched.text = "Word not found";
            kana.text = "";
            pitch.text = "";
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
            givenName.text = "";
            wordType.text = "";
            example1.text = "";
            example1Alt.text = "";
            example2.text = "";
            example2Alt.text = "";
            isTheWordAGivenName = false;
            jlpt.text = "";
            pitch.text = "";
        }
    }

    public void ClearHistory()
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
        givenName.text = "";
        wordType.text = "";
        example1.text = "";
        example1Alt.text = "";
        example2.text = "";
        example2Alt.text = "";
        pitch.text = "";
        isTheWordAGivenName = false;
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

    public void MoveToNextSide()
    {
        movementBetweenPagesLine1 = wordDef1.transform.position;
        movementBetweenPagesLine1.x -= 10;
        wordDef1.transform.position = movementBetweenPagesLine1;
    }

    public void MoveBack()
    {
        movementBetweenPagesLine1 = wordDef1.transform.position;
        movementBetweenPagesLine1.x += 10;
        wordDef1.transform.position = movementBetweenPagesLine1;
        wordDef2.transform.position = movementBetweenPagesLine1;
        wordDef3.transform.position = movementBetweenPagesLine1;
        wordDef4.transform.position = movementBetweenPagesLine1;
        wordDef5.transform.position = movementBetweenPagesLine1;
        wordDef6.transform.position = movementBetweenPagesLine1;
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
    public string example1;
    public string example1Alt;
    public string example2;
    public string example2Alt;
}

