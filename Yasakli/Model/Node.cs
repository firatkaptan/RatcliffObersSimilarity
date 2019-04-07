using System.Collections;
using System.Collections.Generic;
public struct Node
{
    public char[] word;
    public Dictionary<double, Node> Children;

    public Node(char[] word)
    {
        this.word = word;
        this.Children = new Dictionary<double, Node>();
    }

    public Node this[double key]
    {
        get
        {
            return Children[key];
        }
    }

    public ICollection Keys
    {
        get
        {
            return Children.Keys;
        }
    }

    public bool ContainsKey(double key)
    {
        return Children.ContainsKey(key);
    }

    public void AddChild(double key,char[] word)
    {
        this.Children[key] = new Node(word);
    }
}

