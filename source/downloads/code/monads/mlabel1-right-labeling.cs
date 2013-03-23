var partialRight = new S<Tree<Labeled<T>>> 
{ 
    RunWithState = s1 => 
    { 
        S<Tree<Labeled<T>>> partialResult = MLabel1(br.Right);
        Tuple<int, Tree<Labeled<T>>> result = partialResult.RunWithState(s1);
        
        return Tuple.Create(result.Item1, result.Item2);
    }
};