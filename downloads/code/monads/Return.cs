public static S<T> Return<T>(T value)
{
    return new S<T> 
    {
        RunWithState = s => Tuple.Create(s, value)
    };
}