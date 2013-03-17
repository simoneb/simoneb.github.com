public Tree<T> leaf<T>(T value)
{
    return new Leaf<T>(value);
}

public Tree<T> branch<T>(Tree<T> left, Tree<T> right)
{
    return new Branch<T>(left, right);
}