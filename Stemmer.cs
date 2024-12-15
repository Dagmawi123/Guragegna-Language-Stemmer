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
        var stemmed = new List<string>();
        var tokens = Tokenize();
        string candidate = "";
        //removing stop words before stemming
        List<string> Filteredtokens = RemoveStopWords(tokens);
        Console.WriteLine("Stemming.....");
        List<string> twoLetterWords=File.ReadAllText("./Documents/TwoLetterWords.txt")
                .Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();


        foreach (var t in Filteredtokens)
        {
            candidate = t;
            //context handling- keeping exceptions
            if(ContextHandling.Exceptions.Contains(candidate)){
                stemmed.Add(candidate);
                continue; 
            }
            //handling reduplication
                candidate=ContextHandling.RemoveDuplication(candidate);

           
            foreach (var p in Affix.GetPrefixes().OrderBy(o => o.Length))
            {
                if (candidate.ToEnglishSyntax().StartsWith(p.ToEnglishSyntax()))
                {
                    //to handle false prefix identification cases like word ባናም(banam) and suffix ብ(b)
                    if(p.Length==1&&!(p[0]==candidate[0])){
                        continue;
                    }
                    if (candidate.Length <3)
                    {
                        break;
                    }
                    //this serves as a general rule for avoiding overstemming into 2 letters
                    string temp = candidate[p.Length..];
                    if((temp.Length!=2)||(temp.Length==2&&twoLetterWords.Contains(temp))){
                        candidate=temp;
                    }
                }
            }

            foreach (var s in Affix.GetSuffixes().OrderBy(o => o.Length))
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

                string temp =  candidate[..^s.Length];
                    if((temp.Length!=2)||(temp.Length==2&&twoLetterWords.Contains(temp))){
                        candidate=temp;
                    }
                }
            }

            stemmed.Add(candidate.ToGurage());
        }

        return [stemmed,Filteredtokens];
    }

    private void Normalize() { }

    private bool IsLettersOnly(string phrase)
    {
        return phrase.All(char.IsLetter);
    }

    private void RemoveStopWords() { }
}
