var partialLeft = new S<Tree<Labeled<T>>> 
{ 
    RunWithState = s0 => 
    { 
        S<Tree<Labeled<T>>> partialResult = MLabel1(br.Left);
        Tuple<int, Tree<Labeled<T>>> result = partialResult.RunWithState(s0);
        
        return result;
    }
};