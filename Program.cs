using System.Text;
using System.Text.RegularExpressions;
using GuragegnaInfoRetrSys;
using GuragegnaInfoRetrSys.Transliteration;

AmhEngMap.Load();
List<string> paths = new(Directory.GetFiles(@"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Guragegna-Language-Stemmer--\Documents"));
// string stemmedFile="Stemmed.txt";
Stemmer stemmer = new(paths);
// List<List<string>> result = stemmer.Stem();
// for (int i=0;i<result[0].Count;i++)
// {
//     File.AppendAllText(stemmedFile,"Before: "+ result[1][i].ToString()+"   After:  "+result[0][i].ToString()+"\n");
// }
//     Console.WriteLine("Written Successfully...");
 Indexer indexer= new Indexer();
// double [] query=[0.2,2.7,1.4];
// Dictionary<int,double[]> docs=new(){
//     {1,[0.2,0.5,2.6]},
//     {2,[0.7,0.5,2.6]},
//     {3,[0.4,0.1,0.6]},
//     {4,[3.2,2.5,0.0]}
// };
Console.InputEncoding=Encoding.UTF8;
Console.OutputEncoding=Encoding.UTF8;

Console.WriteLine("Enter your query mate 🤨");
string query=Console.ReadLine()!;
Dictionary<int,double[]> matchingDocs=indexer.MatchDocs(query);
double []queryVector=indexer.PrepareQueryVector(query);
 List<Tuple<string,double>> rankedResult=indexer.ComputeSimmilarity(queryVector,matchingDocs);
 Console.WriteLine("Search result...");
 Console.ForegroundColor=ConsoleColor.Blue;
 foreach(var tup in rankedResult){
    Console.WriteLine(tup.Item1+" with "+(tup.Item2*100.0).ToString("F2")+"% simmilarity");
 }
  Console.ForegroundColor=ConsoleColor.Black;

Console.WriteLine("Has come to an end!");




















// string txt=File.ReadAllText(@"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Documents\temp.txt");
// string FileName="TwoLetterWords.txt";
//   string tempText = Regex.Replace(txt, @"[^\p{L}]", " ");
//  var list = tempText.Split(" ", StringSplitOptions.RemoveEmptyEntries);
//  foreach(var item in list)
// {
//     if(item.Length==2){
//         File.AppendAllText(FileName,item+" ");
//             }
// }
// Console.WriteLine("Success!");











// Console.WriteLine("የገምበናም"[..^2]);
// Console.WriteLine("አትያነችም".EndsWith("ችም"));
// Console.WriteLine("አትያነችም"[..^2]);
