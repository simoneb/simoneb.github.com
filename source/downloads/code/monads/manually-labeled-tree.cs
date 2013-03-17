                                                 // labeledTree.Show() outputs:
var labeledTree = branch(                        // Branch:
                    leaf(labeled(0, "a")),       //   Leaf: { Label = 0, Value = a }
                    branch(                      //   Branch:
                      branch(                    //     Branch:
                        leaf(labeled(1, "b")),   //       Leaf: { Label = 1, Value = b }
                        leaf(labeled(2, "c"))),  //       Leaf: { Label = 2, Value = c }
                      leaf(labeled(3, "d"))));   //     Leaf: { Label = 3, Value = d }
                    
labeledTree.Show();