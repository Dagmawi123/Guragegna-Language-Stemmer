using System.Text.RegularExpressions;
using GuragegnaInfoRetrSys.Util;
namespace GuragegnaInfoRetrSys;

public class Stemmer(List<string> docs)
{
    public List<string> DocPaths { get; set; } = docs;

    public string[] Tokenize()
    {
        string readText = "";
       foreach ( string path in DocPaths )
        readText += File.ReadAllText(path);
        string tempText = Regex.Replace(readText, @"[^\p{L}]", " ");
        var tokenizedList = tempText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return tokenizedList;
    }

    public List<string> RemoveStopWords(string[] tokens)
    {
        List<string> filteredWords = new();
        foreach (var token in tokens)
        {
            filteredWords.Add(token);
            if(StopWordList.StopWords.Contains(token))
            filteredWords.Remove(token);
        }
        return filteredWords;
    }
    public List<List<string>> Stem()
    {
        //tokenization,stop word removal,affix removal
        var stemmed = new List<string>();
        var tokens = Tokenize();
        string candidate = "";
        //removing stop words before stemming
        List<string> filteredtokens = RemoveStopWords(tokens);
        Console.WriteLine("Stemming.....");
        List<string> twoLetterWords=File.ReadAllText(@"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Guragegna-Language-Stemmer--\Documents\TwoLetterWords.txt")
                .Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();


        foreach (var t in filteredtokens)
        {

            candidate = t;
            //context handling- keeping exceptions
            if(ContextHandling.Exceptions.Contains(candidate)){
                stemmed.Add(candidate);
                continue; 
            }
            //handling duplication
                candidate=ContextHandling.RemoveDuplication(candidate);

           
            foreach (var p in Affix.GetPrefixes().OrderBy(o => o.Length))
            {
                if (candidate.ToEnglishSyntax().StartsWith(p.ToEnglishSyntax()))
                {
                    //to handle false prefix identification cases like word ባናም(banam) and prefix ብ(b)
                    if(p.Length==1&&!(p[0]==candidate[0])){
                        continue;
                    }
                    if (candidate.Length <3)
                    {
                        break;
                    }
                    //this serves as a general rule for avoiding overstemming into 2 letters
                    string temp = candidate[p.Length..];
                    if((temp.Length>2)||(temp.Length==2&&twoLetterWords.Contains(temp))){
                        candidate=temp;
                    }
                }
            }

            foreach (var s in Affix.GetSuffixes().OrderByDescending(o => o.Length))
            {
                //Recoding rule- suffix 'ንዳ' should not be clipped 
                if(s=="ንዳ"){
                    continue;
                }
                if (candidate.ToEnglishSyntax().EndsWith(s.ToEnglishSyntax()))
                {
                    if (candidate.Length <3)
                    {
                        break;
                    }

                string temp =  candidate.ToEnglishSyntax()[..^s.ToEnglishSyntax().Length];
                    if((temp.Length>2)||(temp.Length==2&&twoLetterWords.Contains(temp))){
                        candidate=temp.ToGurage();
                         }
                }
            }

            stemmed.Add(candidate);
        }

        return [stemmed,filteredtokens];
    }

    private void Normalize() { }

    private bool IsLettersOnly(string phrase)
    {
        return phrase.All(char.IsLetter);
    }

    private void RemoveStopWords() { }
}
