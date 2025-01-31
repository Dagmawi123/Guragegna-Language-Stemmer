using System.Text;
using System.Text.RegularExpressions;
using GuragegnaInfoRetrSys;
using GuragegnaInfoRetrSys.Transliteration;

AmhEngMap.Load();
List<string> paths = new(Directory.GetFiles("./Corpus"));
//load docs here
List<List<string>> Corpus = new();
List<string> docNames = new();
Stemmer stemmer = new();
void FetchDocuments()
{
   foreach (string path in paths)
   {
      //Stemmer stemmer1= new Stemmer(paths);
      string readText = File.ReadAllText(path);
      Corpus.Add(Stemmer.Tokenize(readText));
      docNames.Add(path.Substring(path.LastIndexOf(@"\") + 1));
   }
}



Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Enter your query mate 🤨");
string query = Console.ReadLine()!;
List<string> queryTokens = Stemmer.Tokenize(query);
queryTokens = stemmer.Stem(queryTokens);

UseFilesForSearch();
// UseMemoryForSearch();

void UseMemoryForSearch()
{

   FetchDocuments();
   for (int i = 0; i < Corpus.Count; i++)
   {Console.WriteLine($"Stemming file {i+1}");
      Corpus[i] = stemmer.Stem(Corpus[i]);
   }
   Indexer indexer = new Indexer(Corpus, docNames);

   Dictionary<int, double[]> matchingDocs = indexer.MatchDocs(queryTokens);
   double[] queryVector = indexer.PrepareQueryVector(queryTokens);
   List<Tuple<string, double>> rankedResult = indexer.ComputeSimmilarity(queryVector, matchingDocs);
   PrintResult(rankedResult);
}


void UseFilesForSearch()
{
   foreach (string path in paths)
   {
      docNames.Add(path.Substring(path.LastIndexOf(@"\") + 1));
   }
   Indexer index = new Indexer(docNames);
   Dictionary<int, double[]> matchingDocs = index.MatchDocs(queryTokens);
   double[] queryVector = index.PrepareQueryVector(queryTokens);
   List<Tuple<string, double>> rankedResult = index.ComputeSimmilarity(queryVector, matchingDocs);
   PrintResult(rankedResult);
   }





void PrintResult(List<Tuple<string, double>> rankedResult){
   Console.WriteLine("Search result...");
   Console.ForegroundColor = ConsoleColor.Blue;
   foreach (var tup in rankedResult)
   {
      Console.WriteLine(tup.Item1 + " with " + (tup.Item2 * 100.0).ToString("F2") + "% simmilarity");
   }
   Console.ForegroundColor = ConsoleColor.Black;

   Console.WriteLine("Has come to an end!");
}





