using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DictionarySearchManager : MonoBehaviour
{
    [Header("Search Settings")]
    [SerializeField] private bool caseSensitive = false;
    [SerializeField] private bool searchInDefinitions = true;
    [SerializeField] private bool searchInExamples = true;
    
    private CustomJisho dictionary;

    public void Initialize(CustomJisho dict)
    {
        dictionary = dict;
    }

    /// <summary>
    /// Busca palabras que coincidan con el término de búsqueda
    /// </summary>
    /// <param name="searchTerm">Término a buscar</param>
    /// <returns>Lista de palabras que coinciden</returns>
    public List<InfoListFCJ> SearchWords(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || dictionary?.wordList == null)
        {
            return new List<InfoListFCJ>();
        }

        var normalizedSearchTerm = NormalizeString(searchTerm);
        
        return dictionary.wordList.Where(word => 
            MatchesSearchCriteria(word, normalizedSearchTerm)).ToList();
    }

    /// <summary>
    /// Busca palabras por JLPT level
    /// </summary>
    /// <param name="jlptLevel">Nivel JLPT (N5, N4, N3, N2, N1)</param>
    /// <returns>Lista de palabras del nivel especificado</returns>
    public List<InfoListFCJ> SearchByJLPTLevel(string jlptLevel)
    {
        if (string.IsNullOrWhiteSpace(jlptLevel) || dictionary?.wordList == null)
        {
            return new List<InfoListFCJ>();
        }

        return dictionary.wordList.Where(word => 
            !string.IsNullOrWhiteSpace(word.jlptLevel) && 
            word.jlptLevel.Equals(jlptLevel, System.StringComparison.OrdinalIgnoreCase)).ToList();
    }

    /// <summary>
    /// Busca solo verbos
    /// </summary>
    /// <returns>Lista de verbos</returns>
    public List<InfoListFCJ> SearchVerbs()
    {
        if (dictionary?.wordList == null)
        {
            return new List<InfoListFCJ>();
        }

        return dictionary.wordList.Where(word => word.isTheWordAVerb).ToList();
    }

    /// <summary>
    /// Busca palabras relacionadas
    /// </summary>
    /// <param name="relatedWord">Palabra relacionada</param>
    /// <returns>Lista de palabras relacionadas</returns>
    public List<InfoListFCJ> SearchRelatedWords(string relatedWord)
    {
        if (string.IsNullOrWhiteSpace(relatedWord) || dictionary?.wordList == null)
        {
            return new List<InfoListFCJ>();
        }

        var normalizedRelatedWord = NormalizeString(relatedWord);
        
        return dictionary.wordList.Where(word => 
            !string.IsNullOrWhiteSpace(word.relatedWord) && 
            NormalizeString(word.relatedWord).Contains(normalizedRelatedWord)).ToList();
    }

    /// <summary>
    /// Obtiene estadísticas del diccionario
    /// </summary>
    /// <returns>Diccionario con estadísticas</returns>
    public Dictionary<string, int> GetDictionaryStats()
    {
        if (dictionary?.wordList == null)
        {
            return new Dictionary<string, int>();
        }

        var stats = new Dictionary<string, int>
        {
            ["Total Words"] = dictionary.wordList.Count,
            ["Verbs"] = dictionary.wordList.Count(w => w.isTheWordAVerb),
            ["N5"] = dictionary.wordList.Count(w => w.jlptLevel == "N5"),
            ["N4"] = dictionary.wordList.Count(w => w.jlptLevel == "N4"),
            ["N3"] = dictionary.wordList.Count(w => w.jlptLevel == "N3"),
            ["N2"] = dictionary.wordList.Count(w => w.jlptLevel == "N2"),
            ["N1"] = dictionary.wordList.Count(w => w.jlptLevel == "N1"),
            ["No JLPT"] = dictionary.wordList.Count(w => string.IsNullOrWhiteSpace(w.jlptLevel))
        };

        return stats;
    }

    private bool MatchesSearchCriteria(InfoListFCJ word, string normalizedSearchTerm)
    {
        // Buscar en palabra principal
        if (NormalizeString(word.word).Contains(normalizedSearchTerm))
            return true;

        // Buscar en kana
        if (!string.IsNullOrWhiteSpace(word.kana) && 
            NormalizeString(word.kana).Contains(normalizedSearchTerm))
            return true;

        // Buscar en hiragana
        if (!string.IsNullOrWhiteSpace(word.hiragana) && 
            NormalizeString(word.hiragana).Contains(normalizedSearchTerm))
            return true;

        // Buscar en romaji
        if (!string.IsNullOrWhiteSpace(word.romaji) && 
            NormalizeString(word.romaji).Contains(normalizedSearchTerm))
            return true;

        // Buscar en definiciones si está habilitado
        if (searchInDefinitions && word.def != null)
        {
            foreach (var definition in word.def)
            {
                if (!string.IsNullOrWhiteSpace(definition) && 
                    NormalizeString(definition).Contains(normalizedSearchTerm))
                    return true;
            }
        }

        // Buscar en ejemplos si está habilitado
        if (searchInExamples)
        {
            if (!string.IsNullOrWhiteSpace(word.example1) && 
                NormalizeString(word.example1).Contains(normalizedSearchTerm))
                return true;

            if (!string.IsNullOrWhiteSpace(word.example1Alt) && 
                NormalizeString(word.example1Alt).Contains(normalizedSearchTerm))
                return true;

            if (!string.IsNullOrWhiteSpace(word.example2) && 
                NormalizeString(word.example2).Contains(normalizedSearchTerm))
                return true;

            if (!string.IsNullOrWhiteSpace(word.example2Alt) && 
                NormalizeString(word.example2Alt).Contains(normalizedSearchTerm))
                return true;
        }

        return false;
    }

    private string NormalizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var normalized = input.Trim();
        
        if (!caseSensitive)
        {
            normalized = normalized.ToLowerInvariant();
        }

        return normalized;
    }

    /// <summary>
    /// Configura las opciones de búsqueda
    /// </summary>
    /// <param name="caseSensitive">Si la búsqueda debe ser sensible a mayúsculas</param>
    /// <param name="searchInDefinitions">Si debe buscar en definiciones</param>
    /// <param name="searchInExamples">Si debe buscar en ejemplos</param>
    public void ConfigureSearch(bool caseSensitive, bool searchInDefinitions, bool searchInExamples)
    {
        this.caseSensitive = caseSensitive;
        this.searchInDefinitions = searchInDefinitions;
        this.searchInExamples = searchInExamples;
    }
}
