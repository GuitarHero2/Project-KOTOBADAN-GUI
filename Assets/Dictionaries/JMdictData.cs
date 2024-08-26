using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kana
{
    public bool common;
    public string text;
    public List<string> tags;
    public List<string> appliesToKanji;
}

[System.Serializable]
public class Gloss
{
    public string lang;
    public string text;
}

[System.Serializable]
public class Sense
{
    public List<string> partOfSpeech;
    public List<Gloss> gloss;
}

[System.Serializable]
public class Word
{
    public string id;
    public List<string> kanji;
    public List<Kana> kana;
    public List<Sense> sense;
}

[System.Serializable]
public class JMdictData
{
    public List<Word> words;
}
