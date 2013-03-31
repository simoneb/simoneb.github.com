return Bind(bumpLabelS, 
            labelValue => Return(leaf(labeled(labelValue, lf.Value))))
       .RunWithState(label);