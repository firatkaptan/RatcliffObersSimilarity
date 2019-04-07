using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Searcher
{
    public int count = 0;
    private double Ratio;
    private int MaxDegreeParallelism;

    public Searcher(double Ratio, int MaxDegreeParallelism)
    {
        this.Ratio = Ratio;
        this.MaxDegreeParallelism = MaxDegreeParallelism;
    }

    public void Add(Node root, char[] word)
    {
        Node curNode = root;
        double dist = SimilarityRatio(curNode.word, word);

        while (curNode.ContainsKey(dist))
        {
            if (dist == 0.0) return;
            curNode = curNode[dist];
            dist = SimilarityRatio(curNode.word, word);
        }

        curNode.AddChild(dist, word);
    }

    public List<Match> Search(string str, Dictionary<int, Node> Trees)
    {
        count = 0;
        ConcurrentBag<Match> bag = new ConcurrentBag<Match>();
        char[] word = str.ToCharArray();

        int[] keys = Trees.Keys.Where(key => ((200.0 - Ratio) * key >= Ratio * word.Length) && (Ratio * key <= (200.0 - Ratio) * word.Length)).ToArray();

        Parallel.For(0, keys.Length, new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeParallelism }, index =>
        {
            Search(Trees[keys[index]], word, bag);
        });

        return bag.ToList();
    }

    public void Search(Node node, char[] word, ConcurrentBag<Match> bag)
    {
        double curDist = SimilarityRatio(node.word, word);

        if (curDist >= Ratio)
        {
            bag.Add(new Match(new String(node.word), Math.Round(curDist,2)));
        }

        double[] array = node.Keys.Cast<double>().Where(key => curDist - Ratio <= key && key <= curDist + Ratio).ToArray();

        for (int i = 0; i < array.Length; i++)
        {
            Search(node[array[i]], word, bag);
        }
    }

    public double SimilarityRatio(char[] first,char[] second)
    {
        return SimilarityScore(first, second,0,first.Length,0,second.Length) * 200.0 / (first.Length + second.Length);
    }

    private int SimilarityScore(char[] first, char[] second, int start1, int end1, int start2, int end2)
    {
        int score = 0;
        int newStart1 = start1;
        int newStart2 = start2;
        int length1 = end1 - start1;
        int length2 = end2 - start2;
        int[,] lengths = new int[length1, length2];

        for(int i = 0; i < length1; i++)
        {
            for (int j = 0; j < length2; j++)
            {
                if(first[start1 + i] == second[start2 + j])
                {
                    lengths[i, j] = i == 0 || j == 0 ? 1 : lengths[i - 1, j - 1] + 1;
                    if(lengths[i,j]> score)
                    {
                        score = lengths[i, j];
                        newStart1 = start1 + i;
                        newStart2 = start2 + j;
                    }
                }
                else
                {
                    lengths[i, j] = 0;
                }
            }
        }
        if (score != 0)
        {
            score += SimilarityScore(first, second, start1, newStart1 + 1 - score , start2, newStart2 + 1 - score);
            score += SimilarityScore(first, second, newStart1 + 1, end1, newStart2 + 1, end2);
        }
        return score;
    }
    public void CheckAndRemove(string name,Dictionary<string,List<Match>> dict)
    {
        char[] word = name.ToCharArray();
        if(dict.Count > 0)
        {
            ConcurrentBag<string> removed = new ConcurrentBag<string>();
            Parallel.ForEach(dict.Keys, new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeParallelism }, item =>
               {
                   if (SimilarityRatio(word, item.ToCharArray()) >= Ratio)
                   {
                       removed.Add(item);
                   }
               });

            while (!removed.IsEmpty)
            {
                string item;
                if(removed.TryTake(out item))
                {
                    dict.Remove(item);
                }
            }
        }
    }
}

