// previous:    int
// current:     Tuple<int, int>
var newAndOldLabel = Tuple.Create(label + 1, label);

return Tuple.Create(newAndOldLabel.Item1, leaf(labeled(newAndOldLabel.Item2, lf.Value)));