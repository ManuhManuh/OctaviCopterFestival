using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using System.Linq;

public enum Locale
{
    en = 0, 
    de = 1, 
    es = 2,
    fr = 3
}

public static class Localization 
{

    private static Dictionary<Locale, Dictionary<string, string>> localizationTable;
    private static Dictionary<Locale, AudioClip[]> localizedTutorialVOTable;

    public static Locale currentLocale = Locale.en;
    public static Dictionary<string, string> currentLocalizationTable => localizationTable[currentLocale];
    public static AudioClip[] currentVOTable => localizedTutorialVOTable[currentLocale];

    static Localization()
    {
        Load();
    }

    private static void Load()
    {
        var source = Resources.Load<TextAsset>("Localization");     // tab delimted text file
        var lines = source.text.Split('\n');
        var header = lines[0].Split((char)9);

        var localeOrder = new List<Locale>(header.Length - 1);

        localizationTable = new Dictionary<Locale, Dictionary<string, string>>();
        
        // Generate list of locales that are in the text asset and a dictionary for each
        for (int i = 1; i < header.Length; i++)
        {
            var locale = (Locale)Enum.Parse(typeof(Locale), header[i]);     // cast the text in the file as the enum
            localeOrder.Add(locale);
            localizationTable[locale] = new Dictionary<string, string>(lines.Length - 1);
        }

        for(int index = 1;index < lines.Length - 1; index++)
        {
            var entry = lines[index].Split((char)9);
            var key = entry[0];

            for (int i = 0; i < localeOrder.Count; i++)
            {
                var locale = localeOrder[i];
                localizationTable[locale][key] = entry[i + 1];
            }
        }

        // load the audioclip assets
        localizedTutorialVOTable = new Dictionary<Locale, AudioClip[]>();

        for (int i = 0; i < localizationTable.Count; i++)
        {
            var locale = (Locale)i;
            var folderName = ($"TutorialTracks_{locale}");

            AudioClip[] clipSet = Resources.LoadAll(folderName, typeof(AudioClip)).Cast<AudioClip>().ToArray();
            localizedTutorialVOTable[locale] = clipSet;

        }
    }

    public static Locale FindLocaleFromButtonText(string valueToFindLocaleFor)
    {

        for (int index = 0; index < localizationTable.Count; index++)
        {
            string foundKey = FindKeyFromValue((Locale)index, valueToFindLocaleFor);
            if (foundKey != $"NF_{valueToFindLocaleFor}") return (Locale)index;

        }

        return Locale.en;       // return default language (English)

    }

    public static string FindKeyFromValue(Locale locale, string valueToFindKeyFor)
    {
        foreach(KeyValuePair<string, string> key in localizationTable[locale])
        {
            if (key.Value == valueToFindKeyFor)
            {
                
                //Debug.Log($"Found {valueToFindKeyFor} under {key.Key} in {locale} table");
                return key.Key;
            }

        }
        return $"NF_{valueToFindKeyFor}";
    }

}
