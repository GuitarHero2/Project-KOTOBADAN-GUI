using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using TMPro;

public class KanjiDictManager : MonoBehaviour
{
    public TMP_InputField kanjiInputField;
    public TMP_Text kanjiSearched;
    public TMP_Text kanjiMeaning;
    public bool isThereKunyomi;
    public TMP_Text kunyomi;
    public TMP_Text kunyomi1;
    public TMP_Text kunyomi2;
    public TMP_Text kunyomi3;
    public TMP_Text kunyomi4;
    public TMP_Text kunyomi5;
    public TMP_Text kunyomi6;
    public TMP_Text kunyomi7;
    public TMP_Text goonOnyomi;
    public TMP_Text goon;
    public TMP_Text kanonOnyomi;
    public TMP_Text kanon;
    public TMP_Text touonOnyomi;
    public TMP_Text touon;
    public TMP_Text souonOnyomi;
    public TMP_Text souon;
    public TMP_Text kanyouonOnyomi;
    public TMP_Text kanyouon;
    public TMP_Text kanjiStrokeNumber;
    public bool isTheKanjiAJojoKanji;
    public TMP_Text kanjiNumberInJojoKanji;
    public TMP_Text jlptLevel;

    public TMP_Text pageNumberText;
    public TMP_Text noS;
    public TMP_Text cr;

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


    public KanjiDict kanjiDict = new KanjiDict();

    public void SaveToJson()
    {
        string dictData = JsonUtility.ToJson(kanjiDict);
        string fileLocation = Application.persistentDataPath + "/KTBKanji.json";
        File.WriteAllText(fileLocation, dictData);
    }

    public void LoadToJson()
    {
        string fileLocation = Application.persistentDataPath + "/KTBKanji.json";
        if (File.Exists(fileLocation))
        {
            string dictData = File.ReadAllText(fileLocation);
            kanjiDict = JsonUtility.FromJson<KanjiDict>(dictData);
        }
        else
        {
            SaveToJson();
        }
    }

    public void SearchWord()
    {
        string query = kanjiInputField.text.ToLower();
        KanjiInfo foundKanji = kanjiDict.kanjiList.FirstOrDefault(word => word.kanji.ToLower() == query);
        if (foundKanji != null)
        {
            kanjiSearched.text = foundKanji.kanji;
            kanjiMeaning.text = foundKanji.kanjiMeaning;

            if (foundKanji.thereIsInfoOfStrokes == true)
            {
                kanjiStrokeNumber.text = foundKanji.strokeNumber;
                noS.gameObject.SetActive(true);
            }
            else
            {
                kanjiStrokeNumber.text = "";
                noS.gameObject.SetActive(false);
            }

            if (foundKanji.isThisKanjiAJojoKanji == true)
            {
                kanjiNumberInJojoKanji.text = foundKanji.commonRateWithin2200Kanji.ToString();
                cr.gameObject.SetActive(true);
            }
            else
            {
                kanjiNumberInJojoKanji.text = "";
                cr.gameObject.SetActive(false);
            }

            if (foundKanji.isThereKunyomi == true)
            {
                kunyomi.text = "訓読み: ";
                kunyomi1.text = foundKanji.kunyomi1;
                kunyomi2.text = foundKanji.kunyomi2;
                kunyomi3.text = foundKanji.kunyomi3;
                kunyomi4.text = foundKanji.kunyomi4;
                kunyomi5.text = foundKanji.kunyomi5;
                kunyomi6.text = foundKanji.kunyomi6;
                kunyomi7.text = foundKanji.kunyomi7;
            }
            else
            {
                kunyomi.text = "";
                kunyomi1.text = "";
                kunyomi2.text = "";
                kunyomi3.text = "";
                kunyomi4.text = "";
                kunyomi5.text = "";
                kunyomi6.text = "";
                kunyomi7.text = "";
            }

            jlptLevel.text = foundKanji.jlptLevel;

            if (foundKanji.goonOnyomi != "")
            {
                goon.text = "呉音: ";
                goonOnyomi.text = foundKanji.goonOnyomi;
            }
            else
            {
                goon.text = "";
                goonOnyomi.text = "";
            }

            if (foundKanji.kanonOnyomi != "")
            {
                kanon.text = "漢音: ";
                kanonOnyomi.text = foundKanji.kanonOnyomi;
            }
            else
            {
                kanon.text = "";
                kanonOnyomi.text = "";
            }

            if (foundKanji.touonOnyomi != "")
            {
                touon.text = "唐音: ";
                touonOnyomi.text = foundKanji.touonOnyomi;
            }
            else
            {
                touon.text = "";
                touonOnyomi.text = "";
            }

            if (foundKanji.souonOnyomi != "")
            {
                souon.text = "宋音: ";
                souonOnyomi.text = foundKanji.souonOnyomi;
            }
            else
            {
                souon.text = "";
                souonOnyomi.text = "";
            }

            if (foundKanji.kanyouonOnyomi != "")
            {
                kanyouon.text = "慣用音: ";
                kanyouonOnyomi.text = foundKanji.kanyouonOnyomi;
            }
            else
            {
                kanyouon.text = "";
                kanyouonOnyomi.text = "";
            }

        }
        else
        {
            goon.text = "";
            kanon.text = "";
            touon.text = "";
            souon.text = "";
            kanyouon.text = "";
            cr.gameObject.SetActive(false);
            noS.gameObject.SetActive(false);
            kunyomi.text = "";
            kanjiSearched.text = "Kanji not found";
            kanjiMeaning.text = "";
            kunyomi1.text = "";
            kunyomi2.text = "";
            kunyomi3.text = "";
            kunyomi4.text = "";
            kunyomi5.text = "";
            kunyomi6.text = "";
            kunyomi7.text = "";
            goonOnyomi.text = "";
            kanonOnyomi.text = "";
            touonOnyomi.text = "";
            souonOnyomi.text = "";
            kanyouonOnyomi.text = "";
            kanjiStrokeNumber.text = "";
            kanjiNumberInJojoKanji.text = "";
            jlptLevel.text = "";
        }
    }

    public void ClearHistory()
    {
        kanjiInputField.text = "";
        goon.text = "";
        kanon.text = "";
        touon.text = "";
        souon.text = "";
        kanyouon.text = "";
        cr.gameObject.SetActive(false);
        noS.gameObject.SetActive(false);
        kunyomi.text = "";
        kanjiSearched.text = "";
        kanjiMeaning.text = "";
        kunyomi1.text = "";
        kunyomi2.text = "";
        kunyomi3.text = "";
        kunyomi4.text = "";
        kunyomi5.text = "";
        kunyomi6.text = "";
        kunyomi7.text = "";
        goonOnyomi.text = "";
        kanonOnyomi.text = "";
        touonOnyomi.text = "";
        souonOnyomi.text = "";
        kanyouonOnyomi.text = "";
        kanjiStrokeNumber.text = "";
        kanjiNumberInJojoKanji.text = "";
        jlptLevel.text = "";
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
public class KanjiDict
{
    public List<KanjiInfo> kanjiList = new List<KanjiInfo>();
}

[System.Serializable]
public class KanjiInfo
{
    public string kanji;
    public string kanjiMeaning;
    public string alternativeKanji;
    public bool isThereKunyomi;
    public string kunyomi1;
    public string kunyomi2;
    public string kunyomi3;
    public string kunyomi4;
    public string kunyomi5;
    public string kunyomi6;
    public string kunyomi7;
    public string goonOnyomi;
    public string kanonOnyomi;
    public string touonOnyomi;
    public string souonOnyomi;
    public string kanyouonOnyomi;
    public bool thereIsInfoOfStrokes;
    public string strokeNumber;
    public bool isThisKanjiAJojoKanji;
    public int commonRateWithin2200Kanji;
    public string jlptLevel;
}
