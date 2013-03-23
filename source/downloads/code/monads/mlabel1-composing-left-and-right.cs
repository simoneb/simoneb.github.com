return new S<Tree<Labeled<T>>>
{
    RunWithState = s0 => 
    {
        Tuple<int, Tree<Labeled<T>>> leftResult = partialLeft.RunWithState(s0);
        
        int s1 = leftResult.Item1;
        Tree<Labeled<T>> left = leftResult.Item2;
        
        Tuple<int, Tree<Labeled<T>>> rightResult = partialRight.RunWithState(s1);
        
        int s2 = rightResult.Item1;
        Tree<Labeled<T>> right = rightResult.Item2;
        
        return Tuple.Create(s2, branch(left, right));
    }
};