using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvancedSearchManager : MonoBehaviour
{
    [Header("Search Configuration")]
    [SerializeField] private bool enableFuzzySearch = true;
    [SerializeField] private bool enablePartialMatch = true;
    [SerializeField] private bool searchInDefinitions = false;
    [SerializeField] private bool searchInExamples = false;
    [SerializeField] private float fuzzyThreshold = 0.8f;
    [SerializeField] private int maxResults = 50;

    private ENDictionary dictionary;

    public void Initialize(ENDictionary dict)
    {
        dictionary = dict;
    }

    /// <summary>
    /// Realiza una búsqueda avanzada con múltiples criterios
    /// </summary>
    /// <param name="query">Término de búsqueda</param>
    /// <param name="searchOptions">Opciones de búsqueda</param>
    /// <returns>Lista de palabras que coinciden</returns>
    public List<InfoList> AdvancedSearch(string query, SearchOptions searchOptions = null)
    {
        if (string.IsNullOrWhiteSpace(query) || dictionary?.wordList == null)
        {
            return new List<InfoList>();
        }

        searchOptions ??= new SearchOptions();
        var normalizedQuery = NormalizeString(query);
        
        var results = dictionary.wordList.Where(word => 
            MatchesAdvancedCriteria(word, normalizedQuery, searchOptions)).ToList();

        // Ordenar por relevancia
        results = SortByRelevance(results, normalizedQuery, searchOptions);
        
        // Limitar resultados
        return results.Take(maxResults).ToList();
    }

    /// <summary>
    /// Busca palabras por múltiples criterios
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda</param>
    /// <returns>Lista de palabras que coinciden</returns>
    public List<InfoList> SearchByCriteria(SearchCriteria criteria)
    {
        if (dictionary?.wordList == null)
        {
            return new List<InfoList>();
        }

        return dictionary.wordList.Where(word => MatchesCriteria(word, criteria)).ToList();
    }

    /// <summary>
    /// Búsqueda fuzzy (aproximada) de palabras
    /// </summary>
    /// <param name="query">Término de búsqueda</param>
    /// <param name="threshold">Umbral de similitud (0-1)</param>
    /// <returns>Lista de palabras con similitud</returns>
    public List<SearchResult> FuzzySearch(string query, float threshold = 0.8f)
    {
        if (string.IsNullOrWhiteSpace(query) || dictionary?.wordList == null)
        {
            return new List<SearchResult>();
        }

        var normalizedQuery = NormalizeString(query);
        var results = new List<SearchResult>();

        foreach (var word in dictionary.wordList)
        {
            var similarity = CalculateSimilarity(word, normalizedQuery);
            if (similarity >= threshold)
            {
                results.Add(new SearchResult { Word = word, Similarity = similarity });
            }
        }

        return results.OrderByDescending(r => r.Similarity).Take(maxResults).ToList();
    }

    /// <summary>
    /// Busca palabras por nivel JLPT
    /// </summary>
    /// <param name="jlptLevels">Niveles JLPT a buscar</param>
    /// <returns>Lista de palabras de los niveles especificados</returns>
    public List<InfoList> SearchByJLPTLevels(string[] jlptLevels)
    {
        if (jlptLevels == null || jlptLevels.Length == 0 || dictionary?.wordList == null)
        {
            return new List<InfoList>();
        }

        return dictionary.wordList.Where(word => 
            !string.IsNullOrWhiteSpace(word.jlptLevel) && 
            jlptLevels.Contains(word.jlptLevel)).ToList();
    }

    /// <summary>
    /// Busca palabras por tipo gramatical
    /// </summary>
    /// <param name="wordTypes">Tipos de palabra a buscar</param>
    /// <returns>Lista de palabras de los tipos especificados</returns>
    public List<InfoList> SearchByWordTypes(string[] wordTypes)
    {
        if (wordTypes == null || wordTypes.Length == 0 || dictionary?.wordList == null)
        {
            return new List<InfoList>();
        }

        return dictionary.wordList.Where(word => 
            (!string.IsNullOrWhiteSpace(word.wordType1) && wordTypes.Contains(word.wordType1.ToLower())) ||
            (!string.IsNullOrWhiteSpace(word.wordType2) && wordTypes.Contains(word.wordType2.ToLower()))).ToList();
    }

    private bool MatchesAdvancedCriteria(InfoList word, string normalizedQuery, SearchOptions options)
    {
        // Búsqueda básica en campos principales
        if (MatchesBasicFields(word, normalizedQuery))
            return true;

        // Búsqueda en definiciones
        if (options.SearchInDefinitions && MatchesDefinitions(word, normalizedQuery))
            return true;

        // Búsqueda en ejemplos
        if (options.SearchInExamples && MatchesExamples(word, normalizedQuery))
            return true;

        // Búsqueda en conjugaciones verbales
        if (options.SearchInVerbInflections && word.isTheWordAVerb && MatchesVerbInflections(word, normalizedQuery))
            return true;

        // Búsqueda fuzzy
        if (options.EnableFuzzySearch && CalculateSimilarity(word, normalizedQuery) >= fuzzyThreshold)
            return true;

        return false;
    }

    private bool MatchesBasicFields(InfoList word, string query)
    {
        var fields = new[] { word.word, word.kana, word.romaji, word.hiragana, word.alternativeForm };
        return fields.Any(field => MatchesField(field, query));
    }

    private bool MatchesDefinitions(InfoList word, string query)
    {
        if (word.def == null) return false;
        return word.def.Any(def => MatchesField(def, query));
    }

    private bool MatchesExamples(InfoList word, string query)
    {
        var examples = new[] { word.example1, word.example1Alt, word.example2, word.example2Alt };
        return examples.Any(example => MatchesField(example, query));
    }

    private bool MatchesVerbInflections(InfoList word, string query)
    {
        var inflections = new[]
        {
            word.nonPastInflection, word.nonPastInflectionRomaji,
            word.nonPastNegativeInflection, word.nonPastNegativeInflectionRomaji,
            word.nonPastInflectionPolite, word.nonPastInflectionPoliteRomaji,
            word.nonPastNegativeInflectionPolite, word.nonPastNegativeInflectionPoliteRomaji,
            word.pastInflection, word.pastInflectionRomaji,
            word.pastNegativeInflection, word.pastNegativeInflectionRomaji,
            word.pastInflectionPolite, word.pastInflectionPoliteRomaji,
            word.pastNegativeInflectionPolite, word.pastNegativeInflectionPoliteRomaji,
            word.teFormInflection, word.teFormInflectionRomaji,
            word.teFormInflectionNegative, word.teFormInflectionNegativeRomaji,
            word.potentialInflection, word.potentialInflectionRomaji,
            word.potentialNegativeInflection, word.potentialNegativeInflectionRomaji,
            word.passiveInflection, word.passiveInflectionRomaji,
            word.passiveNegativeInflection, word.passiveNegativeInflectionRomaji,
            word.causativeInflection, word.causativeInflectionRomaji,
            word.causativeNegativeInflection, word.causativeNegativeInflectionRomaji,
            word.causativePassiveInflection, word.causativePassiveInflectionRomaji,
            word.causativePassiveNegativeInflection, word.causativePassiveNegativeInflectionRomaji,
            word.imperativeInflection, word.imperativeInflectionRomaji,
            word.imperativeNegativeInflection, word.imperativeNegativeInflectionRomaji
        };

        return inflections.Any(inflection => MatchesField(inflection, query));
    }

    private bool MatchesField(string field, string query)
    {
        if (string.IsNullOrWhiteSpace(field)) return false;
        
        var normalizedField = NormalizeString(field);
        return normalizedField.Contains(query);
    }

    private bool MatchesCriteria(InfoList word, SearchCriteria criteria)
    {
        // JLPT Level
        if (!string.IsNullOrEmpty(criteria.JLPTLevel) && word.jlptLevel != criteria.JLPTLevel)
            return false;

        // Word Types
        if (criteria.WordTypes != null && criteria.WordTypes.Length > 0)
        {
            var wordType1 = word.wordType1?.ToLower();
            var wordType2 = word.wordType2?.ToLower();
            if (!criteria.WordTypes.Any(wt => wt.ToLower() == wordType1 || wt.ToLower() == wordType2))
                return false;
        }

        // Is Verb
        if (criteria.IsVerb.HasValue && word.isTheWordAVerb != criteria.IsVerb.Value)
            return false;

        // Related Word
        if (!string.IsNullOrEmpty(criteria.RelatedWord) && word.relatedWord != criteria.RelatedWord)
            return false;

        return true;
    }

    private List<InfoList> SortByRelevance(List<InfoList> results, string query, SearchOptions options)
    {
        return results.OrderByDescending(word => CalculateRelevance(word, query, options)).ToList();
    }

    private float CalculateRelevance(InfoList word, string query, SearchOptions options)
    {
        float relevance = 0f;

        // Exact matches get highest priority
        if (NormalizeString(word.word) == query) relevance += 100f;
        if (NormalizeString(word.kana) == query) relevance += 90f;
        if (NormalizeString(word.romaji) == query) relevance += 80f;

        // Partial matches
        if (NormalizeString(word.word).Contains(query)) relevance += 70f;
        if (NormalizeString(word.kana).Contains(query)) relevance += 60f;
        if (NormalizeString(word.romaji).Contains(query)) relevance += 50f;

        // Fuzzy similarity
        if (options.EnableFuzzySearch)
        {
            relevance += CalculateSimilarity(word, query) * 30f;
        }

        return relevance;
    }

    private float CalculateSimilarity(InfoList word, string query)
    {
        var wordText = NormalizeString(word.word);
        var kanaText = NormalizeString(word.kana);
        var romajiText = NormalizeString(word.romaji);

        var similarities = new[]
        {
            CalculateStringSimilarity(wordText, query),
            CalculateStringSimilarity(kanaText, query),
            CalculateStringSimilarity(romajiText, query)
        };

        return similarities.Max();
    }

    private float CalculateStringSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0f;

        // Implementación simple de similitud de Levenshtein
        int distance = LevenshteinDistance(s1, s2);
        int maxLength = Mathf.Max(s1.Length, s2.Length);
        
        return maxLength == 0 ? 1f : 1f - ((float)distance / maxLength);
    }

    private int LevenshteinDistance(string s1, string s2)
    {
        int[,] d = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
            d[i, 0] = i;

        for (int j = 0; j <= s2.Length; j++)
            d[0, j] = j;

        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                d[i, j] = Mathf.Min(Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[s1.Length, s2.Length];
    }

    private string NormalizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return input.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Configura las opciones de búsqueda avanzada
    /// </summary>
    public void ConfigureSearch(bool enableFuzzySearch, bool enablePartialMatch, bool searchInDefinitions, 
                              bool searchInExamples, float fuzzyThreshold, int maxResults)
    {
        this.enableFuzzySearch = enableFuzzySearch;
        this.enablePartialMatch = enablePartialMatch;
        this.searchInDefinitions = searchInDefinitions;
        this.searchInExamples = searchInExamples;
        this.fuzzyThreshold = fuzzyThreshold;
        this.maxResults = maxResults;
    }
}

/// <summary>
/// Opciones de búsqueda avanzada
/// </summary>
public class SearchOptions
{
    public bool EnableFuzzySearch { get; set; } = true;
    public bool EnablePartialMatch { get; set; } = true;
    public bool SearchInDefinitions { get; set; } = false;
    public bool SearchInExamples { get; set; } = false;
    public bool SearchInVerbInflections { get; set; } = true;
}

/// <summary>
/// Criterios de búsqueda específicos
/// </summary>
public class SearchCriteria
{
    public string JLPTLevel { get; set; }
    public string[] WordTypes { get; set; }
    public bool? IsVerb { get; set; }
    public string RelatedWord { get; set; }
}

/// <summary>
/// Resultado de búsqueda con información de similitud
/// </summary>
public class SearchResult
{
    public InfoList Word { get; set; }
    public float Similarity { get; set; }
}
