namespace GuragegnaInfoRetrSys;

public static class Affix
{
    public static List<string> GetSuffixes()
    {
        List<string> suffixes =
        [
            "ሽ", "ዌ", "ነ", "ና", "ማ", "ችም", "ኒ", "ናም", "ም", "ዊም",
            "ንም", "ታ", "ነት", "ኦት", "ኣነ", "ህኖ", "የ", "ኣማ", "አነ",
            "ሁኖ", "ኛ", "ሂኖ", "አሁ", "ሂነማ", "አሂማ", "ቹ", "ኣህ",
            "ኤ", "ሂማ", "ሁ", "ኦ", "አማ", "ናህ", "ናሂማ", "ነማ", "ኖ","ች",
            "ና", "ተና"
        ];
        return suffixes;
    }

    public static List<string> GetPrefixes()
    {
         List<string> prefixes =
        [
            "ያ", "በ", "ቲ", "ብት", "ባነ",
            "ብ", "የ", "አን", "እም", "እ",
            "እን", "አት", "ይ", "ይት", "ት",
            "ተ", "ን", "ኣት", "አስ", "ባን"
        ];
         return prefixes;
    }
}
