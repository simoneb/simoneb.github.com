// previous:    Tuple<int, int>
// current:     int => Tuple<int, int>
Func<int, Tuple<int, int>> bumpLabel = s => Tuple.Create(s + 1, s);

// previous:    Tuple<int, int> => Tuple<int, Tree<Labeled<T>>>
// current:     int => int => Tuple<int, Tree<Labeled<T>>>
Func<int, Func<int, Tuple<int, Tree<Labeled<T>>>>> labelLeafCurried = 
    labelValue => s => Tuple.Create(s, leaf(labeled(labelValue, lf.Value)));

Tuple<int, int> bumpResult = bumpLabel(label);

return labelLeafCurried(bumpResult.Item2)(bumpResult.Item1);