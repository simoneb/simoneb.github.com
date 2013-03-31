public static Func<int, Tuple<int, U>> Bind<T, U>(
    Func<int, Tuple<int, T>> m, 
    Func<T, Func<int, Tuple<int, U>>> k)