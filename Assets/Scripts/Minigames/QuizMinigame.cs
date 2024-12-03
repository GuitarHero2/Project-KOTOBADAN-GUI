using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizMinigame : MonoBehaviour
{
    public int currentScore;
    public TMP_Text scoreText;
    public TMP_Text timerText;

    public TMP_InputField answerInputField;
    public TMP_Text hintText;
    public TMP_Text feedbackText;
    public float feedbackDelay;
    public float timeBeforeWordChanges;
    public float timeReturner;
    public int currentWordInIndex;
    public float fadeDuration;
    public string jlptFilter;
    int timerToInt;

    public InfoList currentWord;
    public List<InfoList> wordList;
    public List<InfoList> usedWords = new List<InfoList>();
    public TMP_Text[] meaningText;

    // public DeckFQG deckFQG = new DeckFQG();


    // DEBUG
    //public float debugFloat;

    /*public void SaveToJson()
    {
        string dictData = JsonUtility.ToJson(deckFQG);
        string fileLocation = Application.persistentDataPath + "/CustomDeckFQG.json";
        File.WriteAllText(fileLocation, dictData);
    }*/

    /*public void LoadToJson()
    {
        string fileLocation = Application.persistentDataPath + "/CustomDeckFQG.json";
        if (File.Exists(fileLocation))
        {
            string dictData = File.ReadAllText(fileLocation);
            deckFQG = JsonUtility.FromJson<DeckFQG>(dictData);
        }
        else
        {
            SaveToJson();
        }
    }*/

    void Start()
    {
        wordList = FindObjectOfType<DictManager>().dict.wordList;
        Debug.Log("Number of words in deck: " + wordList.Count);

        if (wordList.Count > 0)
        {
            SelectNewWord();
        }
        else
        {
            Debug.LogError("The deck is empty!");
        }
    }
    void FixedUpdate()
    {
        scoreText.text = currentScore.ToString();

        if (timeBeforeWordChanges >= 0)
        {
            timeBeforeWordChanges -= 1 * Time.deltaTime;
        }
        else if (timeBeforeWordChanges <= 0)
        {
            timeBeforeWordChanges = timeReturner + feedbackDelay;
            feedbackText.text = "Incorrect. The correct answer was: " + currentWord.hiragana;
            //SaveMissedWordInCustomDeck();
            StartCoroutine(ShowFeedbackAndSelectNewWord());
        }
    }

    void Update()
    {
        // DEBUG
        //debugFloat = timeBeforeWordChanges;

        timerToInt = (int)timeBeforeWordChanges;
        timerText.text = timerToInt.ToString();

        if (wordList.Count == 0)
        {
            WinScreen();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckAnswer();
            answerInputField.ActivateInputField();
        }
    }

    void SelectNewWord()
    {
        if (jlptFilter == "")
        {
            if (wordList.Count > 0)
            {
                // This makes sure that any words repeats.
                var availableWords = wordList.Except(usedWords).ToList();

                if (availableWords.Count == 0)
                {
                    WinScreen();
                }

                currentWordInIndex = Random.Range(0, availableWords.Count);
                currentWord = availableWords[currentWordInIndex];
                hintText.text = currentWord.word;
                feedbackText.text = "";
                answerInputField.ActivateInputField();
            }
            else
            {
                Debug.LogError("No words available for the selected JLPT level!");
            }
        }
        else if (jlptFilter != "")
        {
            // Filter all words with the same desired JLPT Level.
            var filteredList = wordList.Where(word => word.jlptLevel == jlptFilter).ToList();

            if (filteredList.Count > 0)
            {
                // This makes sure that any words repeats.
                var availableWords = filteredList.Except(usedWords).ToList();

                if (availableWords.Count == 0)
                {
                    WinScreen();
                }

                currentWordInIndex = Random.Range(0, availableWords.Count);
                currentWord = availableWords[currentWordInIndex];
                hintText.text = currentWord.word;
                feedbackText.text = "";
                answerInputField.ActivateInputField();
            }
            else
            {
                Debug.LogError("No words available for the selected JLPT level!");
            }
        }
    }

    public void CheckAnswer()
    {
        string playerAnswer = answerInputField.text.ToLower();
        if (timeBeforeWordChanges < timeReturner)
        {
            if (playerAnswer == currentWord.word.ToLower() || playerAnswer == currentWord.kana.ToLower() || playerAnswer == currentWord.romaji.ToLower())
            {
                StartCoroutine(FadeHintColorToWhite(Color.green, fadeDuration, hintText));
                currentScore++;
                feedbackText.text = "Correct!";
                usedWords.Add(currentWord); // Mark any new word as used if answered correctly.
                timeBeforeWordChanges = timeReturner + feedbackDelay;
                StartCoroutine(ShowFeedbackAndSelectNewWord());
                UpdateWordMeanings();
            }
            else
            {
                StartCoroutine(FadeHintColorToWhite(Color.red, fadeDuration, hintText));
            }
        }
    }

    public void SetJlptFilter(string newJlptFilter)
    {
        jlptFilter = newJlptFilter;
        SelectNewWord(); // Update the method with the filter.
    }

    public void WinScreen()
    {
        SceneManager.LoadScene(1);
    }

    public void UpdateWordMeanings()
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentWord.def[i] == null)
            {
                meaningText[i].text = "";
            }
            else
            {
                meaningText[i].text = currentWord.def[i];
            }
        }
    }

    IEnumerator ShowFeedbackAndSelectNewWord()
    {
        yield return new WaitForSeconds(feedbackDelay);
        for (int i = 0; i < 3; i++)
        {
            meaningText[i].text = "";
        }
        SelectNewWord();
        answerInputField.text = "";
    }

    IEnumerator FadeHintColorToWhite(Color popupColor, float duration, TMP_Text text)
    {
        Color endColor = Color.white;
        float elapsedTime = 0;

        text.color = popupColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(popupColor, endColor, elapsedTime / duration);
            yield return null;
        }

        text.color = endColor;
    }

    /*public void SaveMissedWordInCustomDeck()
    {
        SaveToJson();
    }*/

   /* [System.Serializable]
    public class DeckFQG
    {
        public List<InfoList> wordList = new List<InfoList>();
    }*/
}