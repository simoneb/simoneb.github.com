public abstract class Tree<T> 
{
    public abstract void Show(int indent = 0);
}

public class Leaf<T> : Tree<T>
{
    public T Value { get; private set; }
    
    public Leaf(T value) 
    {
        Value = value;
    }
    
    public override void Show(int indent)
    {
        Console.Write(new string(' ', indent * 2) + "Leaf: ");
        Console.WriteLine(Value.ToString());
    }
}

public class Branch<T> : Tree<T>
{
    public Tree<T> Left { get; private set; }
    public Tree<T> Right { get; private set; }

    public Branch(Tree<T> left, Tree<T> right)
    {
        Left = left;
        Right = right;
    }
    
    public override void Show(int indent)
    {
        Console.WriteLine(new string(' ', indent * 2) + "Branch:");
        Left.Show(indent + 1);
        Right.Show(indent + 1);
    }
}