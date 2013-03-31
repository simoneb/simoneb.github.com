// previous:    Tuple<int, Tree<Labeled<T>>>
// current:     Tuple<int, int> => Tuple<int, Tree<Labeled<T>>>
Func<Tuple<int, int>, Tuple<int, Tree<Labeled<T>>>> labelLeaf = 
    arguments => Tuple.Create(arguments.Item1, leaf(labeled(arguments.Item2, lf.Value)));

return labelLeaf(newAndOldLabel);