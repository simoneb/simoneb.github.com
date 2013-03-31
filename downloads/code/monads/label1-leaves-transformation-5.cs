var bumpLabelS = new S<int> 
{
    RunWithState = s0 => Tuple.Create(s0 + 1, s0)
};

Func<int, S<Tree<Labeled<T>>>> labelLeafS = labelValue => new S<Tree<Labeled<T>>>
{
    RunWithState = s1 => Tuple.Create(s1, leaf(labeled(labelValue, lf.Value)))
};

return Bind(bumpLabelS, labelLeafS).RunWithState(label);