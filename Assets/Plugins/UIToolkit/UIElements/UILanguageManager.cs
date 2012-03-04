using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <author>Frederik Buus Sauer</author>
/// <date>2012-02-13</date>
/// <summary>
/// Static event based language manager based on JSON translation files
/// </summary>
public class UILanguageManager
{
    #region Events
    public delegate void OnLanguageChanged();
    public static event OnLanguageChanged onLanguageChanged;

    private delegate string OnLoadLanguage(string language);
    private static event OnLoadLanguage onLoadLanguage;
    #endregion
	
	
    #region Static Variables
    private static Dictionary<string, UILanguageManager> _languageManagers;
    private static Dictionary<string, string> _languages;
    private static string _currentLanguage = string.Empty;
    #endregion
	
	
    #region Private Variables
    private Dictionary<string, string> _texts;
    private string _filename;
    private string _localLanguage;
    #endregion
	
	
    /// <summary>
    /// Constructs an UILanguageManager with custom filename.
    /// </summary>
    /// <param name="filename">Filename for language files</param>
    private UILanguageManager(string filename)
    {
        _filename = filename;
        _localLanguage = loadLanguageTextsFromJSON(_currentLanguage);
    }
	
	
    #region Static Methods
    /// <summary>
    /// Returns an UILanguageManager which loads translations based on the provided filename.
    /// </summary>
    /// <param name="filename">Filename for language files - Defaults to "guiText"</param>
    /// <returns>UILanguageManager instance</returns>
    public static UILanguageManager GetManager(string filename)
    {
        // Sanity check
        if (string.IsNullOrEmpty(filename))
        {
            filename = "guiText";
        }

        // First time a language manager is added
        if (_languageManagers == null)
        {
            _languageManagers = new Dictionary<string, UILanguageManager>(1);

            CheckLanguages();
        }

        UILanguageManager languageManager;
        if (_languageManagers.ContainsKey(filename))
        {
            // Fetch an existing manager
            languageManager = _languageManagers[filename];
        }
        else
        {
            // Create a new language manager 
            languageManager = new UILanguageManager(filename);
            UILanguageManager.onLoadLanguage += languageManager.LoadLanguage;
            _languageManagers.Add(filename, languageManager);
        }

        return languageManager;
    }

    /// <summary>
    /// Removes a language manager by the specified filename.
    /// </summary>
    /// <param name="filename">Translation filename</param>
    public static void RemoveManager(string filename)
    {
        if (_languageManagers.ContainsKey(filename))
        {
            UILanguageManager languageManager = _languageManagers[filename];
            UILanguageManager.onLoadLanguage -= languageManager.LoadLanguage;
            _languageManagers.Remove(filename);
        }
    }

    /// <summary>
    /// Get/Set Language
    /// </summary>
    public static string Language
    {
        get { return _currentLanguage; }
        set
        {
            // Don't do anything if language is the same
            if (value == null || _currentLanguage.Equals(value))
                return;

            // Check if language exists
            if (!_languages.ContainsKey(value))
            {
                Debug.LogError("Language " + value + " was not found!");
                return;
            }

            // Check that all listener managers are updated
            bool success = true;
            if (onLoadLanguage != null)
            {
                foreach (OnLoadLanguage listener in onLoadLanguage.GetInvocationList())
                {
                    success &= value == listener(value);
                }
            }

            // If everything updated correctly
            if (success)
            {
                _currentLanguage = value;

                // Broadcast event
                if (onLanguageChanged != null)
                    onLanguageChanged();
            }
            else
            {
                // Loading for all manager failed. Fall back to previous language.
                if (onLoadLanguage != null)
                    onLoadLanguage(_currentLanguage);
            }
        }
    }

    /// <summary>
    /// Add language to manager.
    /// </summary>
    /// <param name="language">Language identifier</param>
    /// <param name="extension">File extension</param>
    public static void AddLanguage(string language, string extension)
    {
        CheckLanguages();

        if (_languages.ContainsKey(language))
        {
            _languages[language] = extension;
            Debug.LogWarning("Language extension for " + language + " have been overriden!");
        }
        else
        {
            _languages.Add(language, extension);
        }
    }

    /// <summary>
    /// Check if provided language exists.
    /// </summary>
    /// <param name="language">Language identifier</param>
    /// <returns>True if language exists</returns>
    public static bool HasLanguage(string language)
    {
        CheckLanguages();

        return _languages.ContainsKey(language);
    }

    /// <summary>
    /// Return all languages added.
    /// </summary>
    /// <returns></returns>
    public static string[] GetLanguages()
    {
        CheckLanguages();

        string[] keys = new string[_languages.Keys.Count];
        _languages.Keys.CopyTo(keys, 0);
        return keys;
    }

    /// <summary>
    /// Ensures the language Dictionary is initialized
    /// </summary>
    private static void CheckLanguages()
    {
        if (_languages == null)
        {
            _languages = new Dictionary<string, string>(2);
            _languages.Add("English", "EN");
            _currentLanguage = "English";
        }
    }
    #endregion
	
	
    #region Class Methods
    /// <summary>
    /// String accessor for translations.
    /// </summary>
    /// <param name="token">String token specified in language files</param>
    /// <returns>Translation for token string</returns>
    public string this[string token]
    {
        get
        {
            // Validate token exists
            if (_texts.ContainsKey(token))
            {
                return _texts[token];
            }

            throw new System.ArgumentException("Token (" + token + ") was not found!", "token");
        }
    }

    private string LoadLanguage(string language)
    {
        // Don't update if we already use that language
        if (language != _localLanguage)
        {
            _localLanguage = loadLanguageTextsFromJSON(language);
        }

        return language;
    }

    private string loadLanguageTextsFromJSON(string language)
    {
        // Assembe filename
        string filename = _filename + "_" + _languages[language];

        // Attempt to load language file
        TextAsset textAsset = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
        if (textAsset == null)
        {
            Debug.LogError("Language file for " + language + " could not be found!");
            // Attempt to load English as backup
            if (!language.Equals("English"))
            {
                Debug.LogWarning("Defaulting to English if possible.");
                language = "English";
                filename = _filename + "_" + _languages[language];
                textAsset = Resources.Load(filename, typeof(TextAsset)) as TextAsset;

                if (textAsset == null)
                {
                    // Someone forgot to supply files.
                    Debug.LogError("English language was not found. Aborting.");
                    return null;
                }
            }
        }

        // Language file loaded, grab the text.
        string jsonString = textAsset.text;
        // Get a nice IDictionary for us
        IDictionary decodedDict = jsonString.hashtableFromJson();
        // Get the IDictionary we need
        IDictionary stringsDict = decodedDict["TranslatedStrings"] as IDictionary;
        // Let's ensure that we've got something
        if (stringsDict == null)
        {
            Debug.LogError("Translation file for " + language + " is invalid!");
            return null;
        }
        else
        {
            // Dictionary doesn't exist create a new one, otherwise just clear it
            if (_texts == null)
            {
                _texts = new Dictionary<string, string>();
            }
            else
            {
                _texts.Clear();
            }

            // Loop through Hashtable and add entries
            foreach (DictionaryEntry item in stringsDict)
            {
                string text = (string)((IDictionary)item.Value)["StringToRead"];
                _texts.Add(item.Key.ToString(), text);
            }
        }

        // Unload stuff
        textAsset = null;
        Resources.UnloadUnusedAssets();

        return language;
    }
    #endregion
	
}
