public static Func<int, Tuple<int, Tree<Labeled<T>>>> Bind<T>(
    Func<int, Tuple<int, int>> m, 
    Func<int, Func<int, Tuple<int, Tree<Labeled<T>>>>> k)
{
    return s0 => 
    {
        var mResult = m(s0);
        
        var kResult = k(mResult.Item2);
        
        return kResult(mResult.Item1);
    };
}