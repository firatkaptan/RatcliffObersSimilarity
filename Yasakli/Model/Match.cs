public struct Match
{
    string name;
    double ratio;
    public Match(string name, double ratio)
    {
        this.name = name;
        this.ratio = ratio;
    }
    public override string ToString()
    {
        return "(" + this.name + ":" + this.ratio + ")";
    }
}

