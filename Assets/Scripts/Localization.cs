using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public enum Locale
{
    english = 0, 
    deutsch = 1, 
    espanol = 2
}

public static class Localization 
{

    private static Dictionary<Locale, Dictionary<string, string>> localizationTable;
    public static Locale currentLocale = Locale.english;
    public static Dictionary<string, string> currentLocalizationTable => localizationTable[currentLocale];

    //Constructor - will happen when class is instantiated
    static Localization()
    {
        Load();
    }

    private static void Load()
    {
        var source = Resources.Load<TextAsset>("LocalizationEntry");
        var lines = source.text.Split('\n');
        var header = lines[0].Split(';');

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
            var entry = lines[index].Split(';');
            var key = entry[0];

            for (int i = 0; i < localeOrder.Count; i++)
            {
                var locale = localeOrder[i];
                localizationTable[locale][key] = entry[i + 1];
            }
        }


    }

}
