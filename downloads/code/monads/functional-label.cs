public Tree<Labeled<T>> Label<T>(Tree<T> tree)
{
    return Label1(0, tree).Item2;
}

public Tuple<int, Tree<Labeled<T>>> Label1<T>(int label, Tree<T> tree)
{
    if(tree is Leaf<T>)
    {
        var lf = tree as Leaf<T>;
    
        return Tuple.Create(label + 1, leaf(labeled(label, lf.Value)));
    }
    else
    {
        var br = tree as Branch<T>;
    
        var left = Label1(label, br.Left); 
        var right = Label1(left.Item1, br.Right);
    
        return Tuple.Create(right.Item1, branch(left.Item2, right.Item2));
    }
}