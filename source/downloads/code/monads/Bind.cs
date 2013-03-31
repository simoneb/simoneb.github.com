public static S<U> Bind<T, U>(S<T> m, Func<T, S<U>> k)
{
    return new S<U> 
    {
        RunWithState = s0 =>
        {
            Tuple<int, T> mResult = m.RunWithState(s0);
            
            return k(mResult.Item2).RunWithState(mResult.Item1);
        }
    };
}