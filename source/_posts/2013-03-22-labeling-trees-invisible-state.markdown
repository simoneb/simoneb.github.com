---
layout: post
title: "Labeling trees - Invisible state"
date: 2013-03-22 01:16
comments: true
categories: 
tags: monads
---

This is the second post of a series about labeling trees. In the [previous post][introduction] we saw how to attach labels to the leaves of a binary tree, first manually and then functionally without any mutable state. We achieved that by writing a recursive `Label1` function which threaded the value of the label, the _state_, via function arguments and return value, incrementing it every time it found a leaf of the tree.

Let's review the signature of the `Label1` function and then move one step forward towards a different, less explicit way, to pass the state around.

``` csharp
Tuple<int, Tree<Labeled<T>>> Label1<T>(int label, Tree<T> tree)
```

As the signature clearly shows the state passing is visible both in the arguments and the return value of the function. There is certainly a good reason for that: if we need to increment the label and use the new value whenever we find another leaf would it be possible to do otherwise? Let's try for one minute to imagine that we don't like this state passing concern to leak into the function arguments and that we would like to get rid of the extraneous `int label`. The question we are left with is how to move it around then, for instance how we would set its initial value. Let's have a look at the `Label` function we used last time:

{% include_code Label function lang:csharp monads/label-function.cs %}

If `Label1` did not accept the initial value of the label via its arguments how can we provide the initial state 0? Let's try this variation:

``` csharp
return Label1(tree).RunWithState(0).Item2;
```

The difference is that instead of calling `Label1` with two arguments and expecting a tuple back, we call it with one argument (the tree) and get something else back, which looks like a type with a method called `RunWithState`. Invoking this method with the initial state 0 returns again our tuple whose second item is the labeled tree. This is interesting, but how would we write such a type?

Well, doing this (yet unjustified) change to the `Label` function we have already defined the return type of `Label1`. It needs to be a type with a function `RunWithState` which given an integer value returns a tuple whose second item is of type `Tree<Labeled<T>>`. We can as well assume that the first item of the tuple remains of type `int`. 

Let's try to define how this type looks using the information we have so far and call it `S`.

{% include_code Empty S<T> class lang:csharp monads/s-class-empty.cs %}

Let's as well write the signature of our new Label1 function, which has now changed enough to deserve a different name `MLabel1`.

{% include_code MLabel1 signature lang:csharp monads/mlabel1-signature.cs %}

Our new function takes a tree and returns a type whose only member is a function which takes a label and returns a labeled tree. The next step is to fill the body of `MLabel1` but let's first reason a bit about what we should expect to see in there. Similarly to the previous `Label1` the body of this function will contain the labeling logic:

1. label the tree and bump the label in case it's a leaf
2. recurse over left and right trees in case it's a branch

Let's start with the latter, which is simpler because it does not involve changes to the state as bumping only happens in case of leaves. We still need to access it though in order to make it available during the recursion, and the tricky aspect is where to get the state from. Previously we had it in the function arguments, now we don't have it anymore.  
The catch is in the return value. While previously we returned immediately the final result of the _labeling computation_ in the form of a `Tuple<int, Tree<Labeled<T>>>` you can see that now we instead return a partial result in the form of a class containing a function. 

In other words, while we previously had all the information to do the labeling in place (the unlabeled tree and the current label), we are now missing a fundamental piece of it (the current label), therefore all we can do is return a partial result, which turns out to be a means to get the complete result as soon as the missing piece of information is supplied. In pseudo code, we moved from something like this:

```csharp pseudo Label1
(label, tree) => [label, labeledTree]
```

to something like this:


```csharp pseudo MLabel1
tree => label => [label, labeledTree]
```

To those with some background of functional programming this might look very similar to currying, where a function of n arguments is transformed into n functions of 1 argument each. This resonates quite well with the signature of our functions, and indeed they match exactly.

This basically suggests that in the case of Label1 we were manipulating values in terms of trees and labels, while in the case of MLabel1 we are now manipulating both values (the tree) and functions of this shape `label => [label, labeledTree]`.

With these new findings in mind let's review how tree branches were handled before by `Label1` and how we could reproduce the same logic with this different information available.

{% include_code Label1 branch logic lang:csharp monads/label1-branches.cs %}

Let's start with the first recursive invocation of the function which labels the left branch (line 3). We'll do the same here, but we're missing the current label value. Where do we get it from? Remember that we modified `Label` so that it calls the `RunWithState` function of the type we return from `MLabel1` with 0? That's kinda cool because it is `MLabel1` itself which decides what this type looks like, and specifically what the `RunWithState` function does. Now, we have the freedom to write that function in a way which grabs that initial state and use it in the rest of the computation.

{% include_code MLabel1 left labeling lang:csharp monads/mlabel1-left-labeling.cs %}

Nice! Or maybe not. We basically replaced one line of code (line 3 in the previous example) with a dozen. Would you believe that in its final form this function will actually be shorter than the original one? Bear with me and try to understand what is going on here. First of all, we did nothing for the sake of doing it, we did all of this because we were missing one fundamental input, the label value. So what we had to do to circumvent this limitation is build a container around the logic which labels the left tree of the branch, and this container is a custom implementation of the `RunWithState` function of a new instance of the `S<Tree<Labeled<T>>>` class we build specifically for this purpose. Let's try to see what this instance does then.

If we call its `RunWithState` function with 0 it will:

1. call the `MLabel1` function recursively to label the entire left tree and assign the result to a variable named `partialResult`. Notice that the type of such variable is obviously `S<Tree<Labeled<T>>>`
2. call `partialResult.RunWithState` supplying `s0`, which is a state we got from outside and is supplied as an argument of the function we're writing by its caller. The return type is a tuple containing the new state and the labeled left tree and is assigned to the variable `result`
3. return `result`

I understand this can be daunting at first, but we're doing something which is functionally equivalent to what we were doing earlier in one line, with the only difference that now we're wrapping everything in a container which has the form of a function to overcome the lack of the missing initial state.

The following step is do to the labeling of the right tree of the branch, but fortunately the code is the same as for the left tree, with the only difference that it's now using the right tree rather than the left.

{% include_code MLabel1 right labeling lang:csharp monads/mlabel1-right-labeling.cs %}

What we have now is two instances of the class `S<Tree<Labeled<T>>>` which are independent from each other. If you look again at the body of `Label1` you will see that labeling the right tree was dependent on labeling of the left tree, as it used the state resulting from the latter as an input for the former. How do we combine the `partialLeft` and `partialRight`  to obtain the same effect. The answer is again to wrap this logic into another function.

{% include_code MLabel1 composing left and right partial results lang:csharp monads/mlabel1-composing-left-and-right.cs %}

Ok although this is probably even worse than before let's not give up and go trough the code to see what this statement does.

1. It executes the left-labeling computation by passing `s0` as the initial state. `s0` is supplied by the caller of the function we're writing
2. It explicitly assigns the contents of the returned tuple to `s1`, the final state after the left tree has been labeled, and the labeled left tree
3. It executes the right-labeling computation by passing `s1` as the initial state. The important thing to notice is that `s1` is the value of the label after labeling the left side of the branch
4. It explicitly assigns the contents of the returned tuple to `s2`, the final state after the right tree has been labeled, and the labeled right tree
5. Returns a tuple containing the overall final state `s2` as the first item and a labeled branch whose left tree is the labeled left tree and whose right tree is the labeled right tree

What we're still missing is the labeling of the leaves, which involves attaching a label to the leaf and then bumping its value by 1.

[introduction]: {{base_url}}/blog/2013/03/17/labeling-trees-introduction/