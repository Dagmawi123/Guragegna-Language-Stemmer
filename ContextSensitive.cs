namespace GuragegnaInfoRetrSys;
public static class ContextHandling
{
    public static readonly List<string> Exceptions = ["እግዘር"];
    public static string RemoveDuplication(string word)
    {
        //Removing frequentative duplication  መጠጠረ ---> መጠረ  
        if (word.Length == 4 && (word[1] == word[2]||word[0] == word[1]))
        {
            word = word.Remove(1, 1);
            return word;
        }
        //Removing total duplication ገፈገፈ ----> ገፈ 
        if(word.Length%2==0&&word.Length>2){
        string leftPart = word.Substring(0, word.Length / 2);
        string rightPart = word.Substring(word.Length / 2, word.Length / 2);
        if (leftPart == rightPart)
        {
            return leftPart;
        }
        }
        //Removing final duplication  ለካካ ----> ለካ
        if(word.Length>2&&word[word.Length-1]==word[word.Length-2]){
            return word.Substring(0, word.Length - 1);
        }

        return word;

        // return leftPart;
    }
}