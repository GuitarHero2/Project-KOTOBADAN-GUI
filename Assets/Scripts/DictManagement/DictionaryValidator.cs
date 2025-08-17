using System.Collections.Generic;
using UnityEngine;

public class DictionaryValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    [SerializeField] private bool requireWord = true;
    [SerializeField] private bool requireKana = true;
    [SerializeField] private bool requireJLPTLevel = false;
    [SerializeField] private bool requireAtLeastOneDefinition = true;
    [SerializeField] private int maxWordLength = 50;
    [SerializeField] private int maxDefinitionLength = 500;

    /// <summary>
    /// Valida una palabra completa del diccionario
    /// </summary>
    /// <param name="word">Palabra a validar</param>
    /// <returns>Resultado de la validación</returns>
    public ValidationResult ValidateWord(InfoListFCJ word)
    {
        var result = new ValidationResult();
        
        if (word == null)
        {
            result.AddError("La palabra no puede ser nula");
            return result;
        }

        // Validar palabra principal
        if (requireWord && string.IsNullOrWhiteSpace(word.word))
        {
            result.AddError("La palabra es obligatoria");
        }
        else if (!string.IsNullOrWhiteSpace(word.word) && word.word.Length > maxWordLength)
        {
            result.AddError($"La palabra no puede tener más de {maxWordLength} caracteres");
        }

        // Validar kana
        if (requireKana && string.IsNullOrWhiteSpace(word.kana))
        {
            result.AddError("El kana es obligatorio");
        }

        // Validar JLPT level
        if (requireJLPTLevel && string.IsNullOrWhiteSpace(word.jlptLevel))
        {
            result.AddError("El nivel JLPT es obligatorio");
        }
        else if (!string.IsNullOrWhiteSpace(word.jlptLevel))
        {
            if (!IsValidJLPTLevel(word.jlptLevel))
            {
                result.AddError("El nivel JLPT debe ser N5, N4, N3, N2 o N1");
            }
        }

        // Validar definiciones
        if (requireAtLeastOneDefinition && (word.def == null || word.def.Length == 0))
        {
            result.AddError("Se requiere al menos una definición");
        }
        else if (word.def != null)
        {
            for (int i = 0; i < word.def.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(word.def[i]) && word.def[i].Length > maxDefinitionLength)
                {
                    result.AddError($"La definición {i + 1} no puede tener más de {maxDefinitionLength} caracteres");
                }
            }
        }

        // Validar verbos
        if (word.isTheWordAVerb)
        {
            ValidateVerbInflections(word, result);
        }

        return result;
    }

    /// <summary>
    /// Valida los campos de entrada antes de crear una palabra
    /// </summary>
    /// <param name="inputFields">Campos de entrada a validar</param>
    /// <returns>Resultado de la validación</returns>
    public ValidationResult ValidateInputFields(DictionaryInputFields inputFields)
    {
        var result = new ValidationResult();

        // Validar palabra
        if (requireWord && string.IsNullOrWhiteSpace(inputFields.Word))
        {
            result.AddError("La palabra es obligatoria");
        }

        // Validar kana
        if (requireKana && string.IsNullOrWhiteSpace(inputFields.Kana))
        {
            result.AddError("El kana es obligatorio");
        }

        // Validar JLPT level
        if (requireJLPTLevel && string.IsNullOrWhiteSpace(inputFields.JLPTLevel))
        {
            result.AddError("El nivel JLPT es obligatorio");
        }
        else if (!string.IsNullOrWhiteSpace(inputFields.JLPTLevel))
        {
            if (!IsValidJLPTLevel(inputFields.JLPTLevel))
            {
                result.AddError("El nivel JLPT debe ser N5, N4, N3, N2 o N1");
            }
        }

        // Validar definiciones
        if (requireAtLeastOneDefinition && (inputFields.Definitions == null || inputFields.Definitions.Length == 0))
        {
            result.AddError("Se requiere al menos una definición");
        }

        return result;
    }

    private void ValidateVerbInflections(InfoListFCJ word, ValidationResult result)
    {
        var requiredInflections = new[]
        {
            ("Presente", word.nonPastInflection),
            ("Presente Negativo", word.nonPastNegativeInflection),
            ("Pasado", word.pastInflection),
            ("Pasado Negativo", word.pastNegativeInflection)
        };

        foreach (var (name, inflection) in requiredInflections)
        {
            if (string.IsNullOrWhiteSpace(inflection))
            {
                result.AddWarning($"La conjugación '{name}' está vacía para el verbo");
            }
        }
    }

    private bool IsValidJLPTLevel(string jlptLevel)
    {
        var validLevels = new[] { "N5", "N4", "N3", "N2", "N1" };
        return System.Array.Exists(validLevels, level => level.Equals(jlptLevel, System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Configura las reglas de validación
    /// </summary>
    public void ConfigureValidation(bool requireWord, bool requireKana, bool requireJLPTLevel, 
                                  bool requireAtLeastOneDefinition, int maxWordLength, int maxDefinitionLength)
    {
        this.requireWord = requireWord;
        this.requireKana = requireKana;
        this.requireJLPTLevel = requireJLPTLevel;
        this.requireAtLeastOneDefinition = requireAtLeastOneDefinition;
        this.maxWordLength = maxWordLength;
        this.maxDefinitionLength = maxDefinitionLength;
    }
}

/// <summary>
    /// Clase para almacenar los campos de entrada del diccionario
    /// </summary>
public class DictionaryInputFields
{
    public string Word { get; set; }
    public string Kana { get; set; }
    public string Pitch { get; set; }
    public string JLPTLevel { get; set; }
    public string WordType1 { get; set; }
    public string WordType2 { get; set; }
    public string RelatedWord { get; set; }
    public string[] Definitions { get; set; }
    public string Example1Jp { get; set; }
    public string Example1En { get; set; }
    public string Example2Jp { get; set; }
    public string Example2En { get; set; }
    public string LongDefJp { get; set; }
    public string LongDefEn { get; set; }
    public string Romaji { get; set; }
    public string Hiragana { get; set; }
    public string AlternativeForm { get; set; }
    public bool IsVerb { get; set; }
    public string[] VerbInflections { get; set; }
}

/// <summary>
    /// Clase para almacenar el resultado de la validación
    /// </summary>
public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; private set; } = new List<string>();
    public List<string> Warnings { get; private set; } = new List<string>();

    public void AddError(string error)
    {
        Errors.Add(error);
    }

    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }

    public string GetErrorMessage()
    {
        return string.Join("\n", Errors);
    }

    public string GetWarningMessage()
    {
        return string.Join("\n", Warnings);
    }
}
