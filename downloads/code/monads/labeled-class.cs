public class Labeled<T>
{
    public int Label { get; private set; }
    public T Value { get; private set; }
    
    public Labeled(int label, T value)
    {
        Label = label;
        Value = value;
    }
        
    public override string ToString()
    {
        return new { Label, Value }.ToString();
    }
}

public Labeled<T> labeled<T>(int label, T value)
{
    return new Labeled<T>(label, value);
}