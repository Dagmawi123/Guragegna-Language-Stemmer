using System.Text.RegularExpressions;
namespace GuragegnaInfoRetrSys;
//Let us build a term weighting scheme for our docs
public class Indexer
{
    public Indexer(List<List<string>> corpus, List<string> docNames)
    {
        Corpus = corpus;
        this.DocNames = docNames;
    }
    public Indexer(List<string> docNames)
    {
        fromFile = true;
        this.DocNames = docNames;

    }
    public List<string> Paths = new(Directory.GetFiles(@"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Guragegna-Language-Stemmer--\Corpus"));
    public string DocVectorsPath = @"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Guragegna-Language-Stemmer--\DocVectors.txt";
    public string InvertedFilePath = @"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Guragegna-Language-Stemmer--\InvertedIndex.txt";
    public string IDFPath = @"C:\Users\user\Documents\HiLCoE\4THYR2NDTM\CS485\GuragegnaInfoRetrSys\GuragegnaInfoRetrSys\Guragegna-Language-Stemmer--\TermsIDF.txt";
    public List<List<string>>? Corpus;
    public List<List<Tuple<string, int>>> CorpusWithTF = new();
    public List<Tuple<string, double>> TermWithIDF = new();
    readonly List<string> DocNames = new();
    private bool fromFile = false;


    //  string readText = File.ReadAllText(Paths[0]);
    //         string tempText = Regex.Replace(readText, @"[^\p{L}]", " ");
    //     var tokenizedList = tempText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    //     Corpus.Add(tokenizedList.ToList());





    //Compute TF
    // List<double> TFs;
    public List<List<Tuple<string, int>>> CalculateTF()
    {

        // List<List<Tuple<string, int>>> CorpusWithTF = new();
        foreach (var doc in Corpus!)
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
            CorpusWithTF.Add(docWithTF);
            //Compute IDF
        }
        // Console.WriteLine("Multiple TF done");
        // BuildInvertedFile();
        return CorpusWithTF;
    }

    public List<Tuple<string, double>> CalculateIDF()
    {
        if (fromFile) return LoadIDFList();
        int totalDocsLength = CorpusWithTF.Count;

        foreach (var doc in CorpusWithTF)
        {
            foreach (Tuple<string, int> tuple in doc)
            {
                if (TermWithIDF.Find(e => e.Item1 == tuple.Item1) == null)
                {
                    int count = CountDocumentsFrequency(CorpusWithTF, tuple.Item1);
                    TermWithIDF.Add(Tuple.Create(tuple.Item1, Math.Log2((double)totalDocsLength / (double)count)));

                }
            }
        }
        // SaveIDFList(TermWithIDF);
        // Console.WriteLine("IDF done!");
        return TermWithIDF;
    }

    public List<List<Tuple<string, double>>> CalculateTFIDF()
    {
        List<List<Tuple<string, int>>> CorpusWithTF = CalculateTF();
        List<Tuple<string, double>> TermWithIDF = CalculateIDF();
        //tf*idf
        List<List<Tuple<string, double>>> corpusWithTFIDF = new();
        foreach (var doc in CorpusWithTF)
        {
            List<Tuple<string, double>> myDoc = new();
            foreach (Tuple<string, int> tuple in doc)
            {
                double idf = TermWithIDF.Find(t => t.Item1 == tuple.Item1)!.Item2;
                myDoc.Add(Tuple.Create(tuple.Item1, (double)tuple.Item2 * idf));
            }
            corpusWithTFIDF.Add(myDoc);
        }
        // Console.WriteLine("TF*IDF ended");
        // PrepareQueryVector("እግዘር ወማታ እግዘር");
        return corpusWithTFIDF;

    }


    //fix this using the idf list for optimization
    public Dictionary<string, List<int>> BuildInvertedFile()
    {
        if (fromFile)
            return LoadInvertedIndex();
        Dictionary<string, List<int>> file = new();

        for (int i = 0; i < CorpusWithTF.Count; i++)
        {
            foreach (var termTuple in CorpusWithTF[i])
            {
                if (!file.ContainsKey(termTuple.Item1))
                {
                    file[termTuple.Item1] = new();
                }
                if (!file[termTuple.Item1].Contains(i + 1))
                    file[termTuple.Item1].Add(i + 1);
            }
        }
        // SaveInvertedIndex(file);
        return file;
    }

    public Dictionary<int, double[]> PrepareDocumentVectors()
    {
        if (fromFile)
            return LoadDocumentVectors();
        List<List<Tuple<string, double>>> docsWithTFIDF = CalculateTFIDF();
        //TermWithIDF used here cause it is assumed to contain unique terms in the entire corpus 
        //docsWithTFIDF is also unique but per document
        List<Tuple<string, double>> uniqueTerms = TermWithIDF;
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
        // SaveDocumentVectors(docVector);
        // Console.WriteLine("Prepared document vectors!");
        return docVector;
    }

    public double[] PrepareQueryVector(List<string> terms)
    {
        if (fromFile)
            TermWithIDF = LoadIDFList();
        double[] QueryVector = new double[TermWithIDF.Count];
        for (int i = 0; i < terms.Distinct().ToList().Count; i++)
        {
            int TF = terms.Count(t => t == terms[i]);
            double idf = TermWithIDF.Find(t => t.Item1 == terms[i])?.Item2 ?? 0.0;
            int index = TermWithIDF.FindIndex(t => t.Item1 == terms[i]);
            if (index != -1)
                QueryVector[index] = ((double)TF * idf);

        }
        // Console.WriteLine("Query Vector prepared!");
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
            for (int i = 0; i < queryVector.Length; i++)
            {
                if (queryVector[i] != 0.0 && doc.Value[i] != 0.0)
                    dotProduct += queryVector[i] * doc.Value[i];
            }
            double docLength = ComputeLength(doc.Value);
            double cosineSim = dotProduct / (queryLength * docLength);
            rankedDocs.Add(Tuple.Create(DocNames[doc.Key - 1], cosineSim));
        }
        return rankedDocs.OrderByDescending(t => t.Item2).ToList();
    }


    //get matching documents for this query from the inverted index file
    public Dictionary<int, double[]> MatchDocs(List<string> terms)
    {
        Dictionary<int, double[]> matchingDocsVectors = new();
        List<int> matchingDocsIndex = new();
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
            matchingDocsVectors[i] = totalDocs[i];
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



    public void SaveDocumentVectors(Dictionary<int, double[]> docVectors)
    {
        using StreamWriter writer = new StreamWriter(DocVectorsPath);
        foreach (var docVector in docVectors)
        {
            string vectorLine = $"{docVector.Key}: {string.Join(", ", docVector.Value)}";
            writer.WriteLine(vectorLine);
            writer.WriteLine("==="); // Unique line separator
        }
    }

    public Dictionary<int, double[]> LoadDocumentVectors()
    {
        Dictionary<int, double[]> docVectors = new();

        foreach (string line in File.ReadLines(DocVectorsPath))
        {
            if (line == "===") continue; // Skip separator lines
            var parts = line.Split(':');
            int docId = int.Parse(parts[0].Trim());
            double[] vector = Array.ConvertAll(parts[1].Split(','), double.Parse);
            docVectors[docId] = vector;
        }

        return docVectors;
    }


    public Dictionary<string, List<int>> LoadInvertedIndex()
    {
        Dictionary<string, List<int>> invertedIndex = new();

        string currentLine;
        using (StreamReader reader = new StreamReader(InvertedFilePath))
        {
            while ((currentLine = reader.ReadLine()!) != null)
            {
                if (currentLine == "===") continue; // Skip separator lines

                var parts = currentLine.Split('|');
                string term = parts[0].Trim();
                List<int> docIds = parts[1]
                                    .Split(',')
                                    .Select(id => int.Parse(id.Trim()))
                                    .ToList();

                invertedIndex[term] = docIds;
            }
        }

        return invertedIndex;
    }

    public void SaveInvertedIndex(Dictionary<string, List<int>> invertedIndex)
    {
        using StreamWriter writer = new StreamWriter(InvertedFilePath);

        foreach (var entry in invertedIndex)
        {
            string line = $"{entry.Key} | {string.Join(", ", entry.Value)}";
            writer.WriteLine(line);
            writer.WriteLine("==="); // Unique line separator
        }
    }


    public void SaveIDFList(List<Tuple<string, double>> idfList)
    {
        using StreamWriter writer = new StreamWriter(IDFPath);

        foreach (var entry in idfList)
        {
            writer.WriteLine($"{entry.Item1} | {entry.Item2}");
        }
    }

    public List<Tuple<string, double>> LoadIDFList()
    {
        List<Tuple<string, double>> idfList = new();

        foreach (string line in File.ReadLines(IDFPath))
        {
            var parts = line.Split('|');
            string term = parts[0].Trim();
            double idfValue = double.Parse(parts[1].Trim());

            idfList.Add(Tuple.Create(term, idfValue));
        }

        return idfList;
    }












}



