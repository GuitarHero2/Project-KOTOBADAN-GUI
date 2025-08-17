#  KOTOBADAN Project / 言葉暖 Project

### This project is meant to help others to make their way easier through the Japanese language, as a multi-tool app available in both TUI version (Terminal User Interface App) and GUI version (Graphic User Interface App) inspired in the Discord bot KOTOBA.

> [!NOTE]
> Take in consideration that this project is still under development and haven't still been released. Don't try to download this program from any other sources other than this Github.

 
 # TUI Version (Console App Application) (DEPRECATED)

 ### This version is based by the Windows Terminal Application and it's meant to be as light-weighted as possible. Use this version if are working under low-end specs. To download this version, please refer to this [Github Link](https://github.com/GuitarHero2/Project-KOTOBADAN)

# GUI Version (Graphical User Interface)

### This version will be developed under Unity and is meant to me more system demanding than its TUI counterpart. 

# What tools does the program have?

### I intent to add the following tools:

* Quiz Game (Kinda what the [Discord Bot KOTOBA](https://kotobaweb.com/bot) does)
* Dictionaries for both grammar and words.
* Support for varios languages (For now, I will stick with English, Spanish, Swedish and Japanese)
* Content recommendation to (try to) boost your language journey.
* Lyrics for songs (This feature is attached to changes)

# Minijuego de Quiz - Mejoras Implementadas

## 🎮 Descripción del Minijuego

El minijuego de quiz es una herramienta interactiva para practicar vocabulario japonés. Los jugadores deben escribir la pronunciación correcta (hiragana) de palabras mostradas en kanji.

### **Funcionalidades:**
- **Juego de preguntas**: Muestra palabras en kanji, el jugador responde en hiragana
- **Múltiples formatos de respuesta**: Acepta kanji, kana, hiragana y romaji
- **Sistema de puntuación**: Seguimiento de respuestas correctas
- **Control de tiempo**: Límite de tiempo por pregunta
- **Filtros JLPT**: Jugar con palabras de niveles específicos
- **Feedback visual**: Colores para respuestas correctas/incorrectas

## 🔧 Mejoras Técnicas Implementadas

### **1. Refactorización del Código**
- **Separación de responsabilidades**: Lógica de juego, UI y estadísticas separadas
- **Encapsulación**: Campos privados con acceso controlado
- **Organización**: Uso de regiones y métodos específicos
- **Configuración**: Valores ajustables desde el Inspector

### **2. Sistema de Estados**
```csharp
public enum GameState
{
    Waiting,    // Esperando inicio
    Playing,    // Juego activo
    Won,        // Juego completado
    Error       // Error en el juego
}
```

### **3. Gestión de Estadísticas**
- **Seguimiento de sesiones**: Registro de cada partida
- **Análisis por palabra**: Dificultad individual de cada palabra
- **Estadísticas JLPT**: Rendimiento por nivel
- **Persistencia**: Guardado automático en JSON

### **4. Mejoras de UX**
- **Validación robusta**: Manejo de casos edge
- **Feedback mejorado**: Mensajes más claros
- **Configuración flexible**: Ajustes desde Inspector
- **Manejo de errores**: Recuperación graciosa

## 📊 Sistema de Estadísticas

### **Estadísticas Generales:**
- Total de juegos jugados
- Puntuación promedio y mejor puntuación
- Tiempo total jugado
- Precisión general

### **Análisis por Palabra:**
- Palabras más difíciles (menor precisión)
- Palabras más fáciles (mayor precisión)
- Número de intentos por palabra
- Tasa de éxito individual

### **Estadísticas por JLPT:**
- Rendimiento por nivel (N5-N1)
- Puntuación promedio por nivel
- Mejor puntuación por nivel

## 🎯 Beneficios de las Mejoras

1. **Mantenibilidad**: Código más fácil de entender y modificar
2. **Escalabilidad**: Fácil agregar nuevas funcionalidades
3. **Robustez**: Mejor manejo de errores y casos edge
4. **Análisis**: Datos detallados para mejorar el aprendizaje
5. **Configuración**: Ajustes flexibles sin modificar código
6. **Experiencia de usuario**: Interfaz más pulida y responsiva

## 🚀 Próximas Mejoras Sugeridas

1. **Modos de juego**: Diferentes tipos de preguntas
2. **Sistema de logros**: Desbloqueos y recompensas
3. **Multiplayer**: Competencia entre jugadores
4. **Progresión**: Sistema de niveles y experiencia
5. **Personalización**: Temas y configuraciones visuales
6. **Analytics avanzados**: Gráficos y tendencias

---

# Feedback and Questions

### If you have any question or feedback about the program, you can DM me on Discord. You can find me as *_Nozomi2_* or join the [Atatakai Community](https://discord.gg/bj9f359bW9) in Discord where I chat sometimes.

> [!IMPORTANT]
> If the Discord link expires, please DM me to fix it.
