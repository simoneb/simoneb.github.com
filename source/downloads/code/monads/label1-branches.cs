var br = tree as Branch<T>;

var left = Label1(label, br.Left); 
var right = Label1(left.Item1, br.Right);

return Tuple.Create(right.Item1, branch(left.Item2, right.Item2));