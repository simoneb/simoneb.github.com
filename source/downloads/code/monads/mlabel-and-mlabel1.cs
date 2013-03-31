public Tree<Labeled<T>> MLabel<T>(Tree<T> tree)
{
    return MLabel1(tree).RunWithState(0).Item2;
}

public S<Tree<Labeled<T>>> MLabel1<T>(Tree<T> tree)
{
    if(tree is Leaf<T>)
    {
        var lf = tree as Leaf<T>;
        
        var updateState = new S<int> 
        {
            RunWithState = s0 => Tuple.Create(s0 + 1, s0)
        };
        
        return Bind(updateState, 
                    labelValue => Return(leaf(labeled(labelValue, lf.Value))));
    }
    else
    {
        var br = tree as Branch<T>;
        
        return Bind(MLabel1(br.Left), 
                    left => Bind(MLabel1(br.Right), 
                                 right => Return(branch(left, right))));
    }
}