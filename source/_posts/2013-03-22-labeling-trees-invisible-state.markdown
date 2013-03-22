---
layout: post
title: "Labeling trees - Invisible state"
date: 2013-03-22 01:16
comments: true
categories: 
tags: monads
---

This is the second post of this series about labeling trees. In the [previous post][introduction] we saw how to attach labels to the leaves of a binary tree manually and then functionally, without mutable state. In our case the state is the current value of the label, which is incremented every time we find a leaf.

Let's move one step forward in the functional implementation. We are already happy that we don't have any mutable state but the signature of the `Label1` function looked like this:

``` csharp
Tuple<int, Tree<Labeled<T>>> Label1<T>(int label, Tree<T> tree)
```

As you can see the state passing is quite visible both in the arguments of the function and in the return value, and there is certainly a good reason for that. If we need to increment the label value and pass it around would it be possible to do otherwise? How about removing the `int label` argument from the method signature?

It is indeed possible and here is the big catch. If we remove the `int label` argument how could we modify its return type in a way that the function is still capable of labeling the tree correctly? 

Let's try to visualize it differently by looking at the other `Label` function we used:

{% include_code Label function lang:csharp monads/label-function.cs %}

The interesting part here is the initial state. If `Label1` did not accept the initial value of the label via its arguments how would we provide the initial state? What about this?

``` csharp
return Label1(tree)(0).Item2;
```
The difference is that instead of calling `Label1` with two arguments and expecting a tuple back, we call it with one argument (the tree) and expect a function back, invoking which with the initial state returns again our tuple whose second item is the labeled tree. This is interesting, but how would we write such a function?

Well, by doing this still unexplained change to the `Label` function we have already defined the return type of the `Label1` function. It needs to be a function which given an integer value returns a tuple whose second item is of type `Tree<Labeled<T>>`. We can as well assume that the first item of the tuple remains of type `int`. 

Let's write the signature of such a function by defining a delegate type called `S`.

{% include_code S delegate lang:csharp monads/s-delegate.cs %}

Let's as well write the signature of our new Label1 function, which now deserves a new name: `MLabel1`.

{% include_code MLabel1 signature lang:csharp monads/mlabel1-signature.cs %}

In words, our function takes a tree and returns a function which takes a label and returns a labeled tree. Let's try to implement it.

[introduction]: {{base_url}}/blog/2013/03/17/labeling-trees-introduction/