using System.Text.RegularExpressions;
namespace GuragegnaInfoRetrSys;
//Let us build a term weighting scheme for our docs
public class Indexer
{
    public List<string> paths = new(Directory.GetFiles(@"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Guragegna-Language-Stemmer--\Corpus"));
    public List<List<string>> Corpus = new();
    public List<List<Tuple<string, int>>> corpusWithTF = new();
    public List<Tuple<string, double>> termWithIDF = new();
    List<string> docNames=new();
    void FetchDocuments()
    {
        foreach (string path in paths)
        {
            //Stemmer stemmer1= new Stemmer(paths);
            string readText = File.ReadAllText(path);
            string tempText = Regex.Replace(readText, @"[^\p{L}]", " ");
            var tokenizedList = tempText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Corpus.Add(tokenizedList.ToList());
            docNames.Add(path.Substring(path.LastIndexOf(@"\")+1));
        }

        //  string readText = File.ReadAllText(paths[0]);
        //         string tempText = Regex.Replace(readText, @"[^\p{L}]", " ");
        //     var tokenizedList = tempText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //     Corpus.Add(tokenizedList.ToList());


    }


    //Compute TF
    // List<double> TFs;
    public List<List<Tuple<string, int>>> CalculateTF()
    {
        FetchDocuments();
        // List<List<Tuple<string, int>>> corpusWithTF = new();
        foreach (var doc in Corpus)
        {
            //TODO stemming
            List<Tuple<string, int>> docWithTF = new();
            foreach (var str in doc)
            {
                if (docWithTF.Find(e => e.Item1 == str) == null)
                {
                    docWithTF.Add(Tuple.Create(str, 1));
                }
                else
                {
                    int index = docWithTF.FindIndex(e => e.Item1 == str);
                    docWithTF[index] = Tuple.Create(str, docWithTF[index].Item2 + 1);
                }

            }
            corpusWithTF.Add(docWithTF);
            //Compute IDF
        }
        Console.WriteLine("Multiple TF done");
        BuildInvertedFile();
        return corpusWithTF;
    }

    public List<Tuple<string, double>> CalculateIDF()
    {

        int totalDocsLength = corpusWithTF.Count;

        foreach (var doc in corpusWithTF)
        {
            foreach (Tuple<string, int> tuple in doc)
            {
                if (termWithIDF.Find(e => e.Item1 == tuple.Item1) == null)
                {
                    int count = CountDocumentsFrequency(corpusWithTF, tuple.Item1);
                    termWithIDF.Add(Tuple.Create(tuple.Item1, Math.Log2((double)totalDocsLength / (double)count)));

                }
            }
        }

        Console.WriteLine("IDF done!");
        return termWithIDF;
    }

    public List<List<Tuple<string, double>>> CalculateTFIDF()
    {
        List<List<Tuple<string, int>>> corpusWithTF = CalculateTF();
        List<Tuple<string, double>> termWithIDF = CalculateIDF();
        //tf*idf
        List<List<Tuple<string, double>>> corpusWithTFIDF = new();
        foreach (var doc in corpusWithTF)
        {
            List<Tuple<string, double>> myDoc = new();
            foreach (Tuple<string, int> tuple in doc)
            {
                double idf = termWithIDF.Find(t => t.Item1 == tuple.Item1)!.Item2;
                myDoc.Add(Tuple.Create(tuple.Item1, (double)tuple.Item2 * idf));
            }
            corpusWithTFIDF.Add(myDoc);
        }
        Console.WriteLine("TF*IDF ended");
       // PrepareQueryVector("እግዘር ወማታ እግዘር");
        return corpusWithTFIDF;

    }



    //     try{
    //         string path = "TfIdf.txt";
    //     foreach (var doc in corpusWithTFIDF)
    //     {
    //         foreach (var tup in doc)
    //         {
    //             File.AppendAllText(path, tup.Item1 + " " + tup.Item2.ToString() + "\n");
    //         }
    //     }
    // Console.WriteLine("Written to file!");
    //     }
    //     catch (Exception ex){
    //         Console.WriteLine(ex.ToString());
    //     }



    //fix this using the idf list for optimization
    public Dictionary<string, List<int>> BuildInvertedFile()
    {
        Dictionary<string, List<int>> file = new();

        for (int i = 0; i < corpusWithTF.Count; i++)
        {
            foreach (var termTuple in corpusWithTF[i])
            {
                if (!file.ContainsKey(termTuple.Item1))
                {
                    file[termTuple.Item1] = new();
                }
                if (!file[termTuple.Item1].Contains(i+1))
                    file[termTuple.Item1].Add(i + 1);
            }
        }
        return file;
    }

    public Dictionary<int, double[]> PrepareDocumentVectors()
    {
        List<List<Tuple<string, double>>> docsWithTFIDF = CalculateTFIDF();
        List<Tuple<string, double>> uniqueTerms = termWithIDF;
        Dictionary<int, double[]> docVector = new();
        for (int i = 0; i < docsWithTFIDF.Count; i++)
        {
            docVector[i + 1] = new double[uniqueTerms.Count];
            for (int j = 0; j < uniqueTerms.Count; j++)
            {
                if (docsWithTFIDF[i].Find(t => t.Item1 == uniqueTerms[j].Item1) != null)
                {
                    docVector[i + 1][j] = docsWithTFIDF[i].Find(t => t.Item1 == uniqueTerms[j].Item1)!.Item2;
                }
            }
        }
        Console.WriteLine("Prepared document vectors!");
        return docVector;
    }

    public double[] PrepareQueryVector(string query)
    {

        //TODO: go through preparation and stemming process
        List<string> terms = query.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
        double[] QueryVector = new double[termWithIDF.Count];
        for (int i = 0; i < terms.Count; i++)
        {
            int TF = terms.GroupBy(t => t == terms[i]).ToList().Count;
            double idf = termWithIDF.Find(t => t.Item1 == terms[i])?.Item2 ?? 0.0;
            int index = termWithIDF.FindIndex(t => t.Item1 == terms[i]);
            if(index!=-1)
            QueryVector[index] = ((double)TF * idf);

        }
        Console.WriteLine("Query Vector prepared!");
        return QueryVector;

    }


    //Compute Vectors simillarity 

    public List<Tuple<string, double>> ComputeSimmilarity(double[] queryVector, Dictionary<int, double[]> documentVectors)
    {
        List<Tuple<string, double>> rankedDocs = new();
        double queryLength = ComputeLength(queryVector);
        //iterate through each doc vector and compute cosine simmilarity 
        foreach (var doc in documentVectors)
        {
            double dotProduct = 0.0;
            for (int i = 0; i < queryVector.Length ; i++)
            {
                if( queryVector[i] != 0.0 && doc.Value[i] != 0.0) 
                dotProduct += queryVector[i] * doc.Value[i];
            }
            double docLength = ComputeLength(doc.Value);
            double cosineSim = dotProduct / (queryLength * docLength);
            rankedDocs.Add(Tuple.Create(docNames[doc.Key-1], cosineSim));
        }
        return rankedDocs.OrderByDescending(t => t.Item2).ToList();
    }


    public Dictionary<int, double[]> MatchDocs(string query)
    {
        Dictionary<int, double[]> matchingDocsVectors = new();
        List<int> matchingDocsIndex = new();
        List<string> terms = query.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
        Dictionary<int, double[]> totalDocs = PrepareDocumentVectors();
        Dictionary<string, List<int>> invertedFile = BuildInvertedFile();
        foreach (var term in terms)
        {
            if (invertedFile.ContainsKey(term))
            {
                matchingDocsIndex.AddRange(invertedFile[term]);
            }
        }
        foreach (int i in matchingDocsIndex.Distinct().ToList())
        {
            matchingDocsVectors[i]=totalDocs[i];
        }

        return matchingDocsVectors;
    }





    private double ComputeLength(double[] vector)
    {
        double squaredSum = 0.0;
        foreach (double dim in vector)
        {
            squaredSum += dim * dim;
        }
        return Math.Sqrt(squaredSum);
    }






    //Ranking






    private int CountDocumentsFrequency(List<List<Tuple<string, int>>> corpus, string str)
    {
        int count = 0;
        foreach (var doc in corpus)
        {
            if (doc.Find(t => t.Item1 == str) != null)
            {
                count += 1;
            }
        }
        return count;
    }

}



