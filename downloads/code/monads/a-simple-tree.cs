                                 //  tree.Show() outputs:
var tree = branch(               //  Branch:
            leaf("a"),          //    Leaf: a
            branch(             //    Branch:
              branch(           //      Branch:
                leaf("b"),      //        Leaf: b
                leaf("c")),     //        Leaf: c
              leaf("d")));      //      Leaf: d

tree.Show();