using System.Collections.Generic;
public struct BKTree
{
    Dictionary<int, Node> Nodes;
    Dictionary<string, List<Match>> Cache;
    public Searcher src;

    public BKTree(double ratio,int MaxDegreeParallelism)
    {
        Cache = new Dictionary<string, List<Match>>();
        Nodes = new Dictionary<int, Node>();
        src = new Searcher(ratio, MaxDegreeParallelism);
    }

    public void Add(string str)
    {
        char[] word = str.ToCharArray();
        if (Nodes.ContainsKey(word.Length))
        {
            src.Add(Nodes[word.Length], word);
        }
        else
        {
            Nodes[word.Length] = new Node(word);
        }
    }

    public List<Match> Search(string str)
    {
        List<Match> result;
        if (Cache.ContainsKey(str))
        {
            result = Cache[str];
        }
        else
        {
            result = src.Search(str, Nodes);
            Cache[str] = result;
        }
        return result;
    }

    public void CheckAndRemoveCache(string word)
    {
        src.CheckAndRemove(word, Cache);
    }
}

