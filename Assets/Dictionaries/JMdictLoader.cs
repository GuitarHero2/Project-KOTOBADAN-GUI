using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JMdictLoader : MonoBehaviour
{
    public JMdictData jmdictData; // Variable para almacenar los datos del diccionario

    void Start()
    {
        LoadDictionary();
    }

    void LoadDictionary()
    {
        // Cargar el archivo JSON desde la carpeta Resources
        TextAsset jsonFile = Resources.Load<TextAsset>("jmdictEN"); // Cambia "jmdict" al nombre de tu archivo sin la extensi�n
        if (jsonFile != null)
        {
            // Convertir el JSON en un objeto de tipo JMdictData
            jmdictData = JsonUtility.FromJson<JMdictData>(jsonFile.text);
            Debug.Log("Diccionario cargado con �xito, palabras disponibles: " + jmdictData.words.Count);
        }
        else
        {
            Debug.LogError("No se encontr� el archivo JSON del diccionario.");
        }
    }
}
