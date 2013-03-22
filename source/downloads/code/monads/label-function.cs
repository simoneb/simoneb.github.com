public Tree<Labeled<T>> Label<T>(Tree<T> tree)
{
    return Label1(0, tree).Item2;
}