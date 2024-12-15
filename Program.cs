using System.Text.RegularExpressions;
using GuragegnaInfoRetrSys;
using GuragegnaInfoRetrSys.Transliteration;

AmhEngMap.Load();
List<string> paths = new(Directory.GetFiles(@"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Documents"));
string stemmedFile="Stemmed.txt";
Stemmer stemmer = new(paths);
List<List<string>> result = stemmer.Stem();
for (int i=0;i<result[0].Count;i++)
{
    File.AppendAllText(stemmedFile,"Before: "+ result[1][i].ToString()+" After: "+result[0][i].ToString()+"\n");
}
    Console.WriteLine("Written Successfully...");


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
