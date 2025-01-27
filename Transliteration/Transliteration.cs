using System.Text.Json;

namespace GuragegnaInfoRetrSys.Transliteration;

public static class AmhEngMap
{
    public static Dictionary<string, string>? AmhEngDict { get; set; }

    public static void Load()
    {
        string filePath = @"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Guragegna-Language-Stemmer--\Transliteration\list_mod.json";

        string jsonString = File.ReadAllText(filePath);

        var amharicMap =
            JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)
            ?? throw new NullReferenceException();

        AmhEngDict = amharicMap;
       
    }
}
