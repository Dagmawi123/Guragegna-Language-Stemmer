namespace GuragegnaInfoRetrSys.Util
{
    public static class Extension
    {
        public static string ToEnglishSyntax(this string amharic_form)
        {
            string english = "";
            foreach (var amharic_letter in amharic_form)
            {
                foreach (var item in Transliteration.AmhEngMap.AmhEngDict!)
                {
                    if (item.Key == amharic_letter.ToString())
                    {
                        english += item.Value;
                    }
                }
            }
            return english;
        }

        public static string ToGurage(this string english_form)
        {
            string gurage = english_form;
            foreach (var eng_letter in Transliteration.AmhEngMap.AmhEngDict!)
            {
                if (english_form.Contains(eng_letter.Value))
                {
                    gurage = gurage.Replace(eng_letter.Value, eng_letter.Key);
                }
            }
            return gurage;
        }
    }
}
