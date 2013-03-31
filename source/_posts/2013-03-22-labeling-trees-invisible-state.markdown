---
layout: post
title: "Labeling trees - Invisible state"
date: 2013-03-22 01:16
comments: true
categories: 
tags: monads
---

This is the second post of a series about labeling trees. In the [previous post][introduction] we saw how to mark leaves of a binary tree with integer, incrementing labels, first manually and then functionally without relying on mutable state. We achieved it by writing a recursive `Label1` function which threaded the value of the label, the _state_, via function arguments and return value, incrementing it every time it came across a leaf of the tree.

Let's review the signature of the `Label1` function and then move one step forward towards a different, less explicit way to pass the state around.

``` csharp
Tuple<int, Tree<Labeled<T>>> Label1<T>(int label, Tree<T> tree)
```

As the signature clearly shows the state passing is visible both in the arguments and the return value of the function. There is certainly a good reason for that: if we need to increment the label and use the new value whenever we come across the next leaf would it be possible to do otherwise? Let's as well review the part of the function which took care of applying labels to the leaves.

{% include_code Label1 leaf logic lang:csharp monads/label1-leaves.cs %}

This simple two-liner mixes together two concerns: creating a labeled leaf and incrementing the value of the label, squeezing them into a tuple. Let's try to abstract and extract these two responsibilities and then apply a series of simple transformations over them, while keeping the behavior unchanged.

#### First transformation: from integer to tuple

{% include_code Label1 first transformation lang:csharp monads/label1-leaves-transformation-1.cs %}

Here we extracted the increment concern as a standalone operation. `newAndOldLabel` is a tuple whose first item contains the new value of the label and the second item contains the previous, unchanged value. The second statement simply uses it to create the final tuple.

#### Second transformation: from tuple to functions returning tuples

{% include_code Label1 second transformation lang:csharp monads/label1-leaves-transformation-2.cs %}

Here we wrap the last statement from the previous step into a function which accesses `newAndOldLabel` via its arguments (rather than via local variables) and returns the result of invoking the function itself with `newAndOldLabel`.

#### Third transformation: from anything to functions

{% include_code Label1 third transformation lang:csharp monads/label1-leaves-transformation-3.cs %}

Here we apply a transformation to both the label increment and the leaf-labeling logic. In the first case we wrap the creation of the `newAndOldLabel` tuple into a function which rather than grabbing the value of the label from the integer `label` argument of the containing function accesses it via its own argument.  
In the second case we _curry_ the already existing function so that instead of accepting a tuple argument it accepts a single integer argument and returns a function which accepts another integer argument and finally returns our output tuple. The external function takes care of fixing the _current_ value used to apply a label to the leaf, while the inner function fixes the _new_, incremented label value as the first item of the resulting tuple.

The rest of the code _binds_ together these two functions in a way which preserves the overall behavior.

#### Fourth transformation: Bind

As we just mentioned binding, let's finally abstract this concept into its own `Bind` generic function. This function basically encapsulates the logic of composing `bumpLabel` and `labelLeafCurried` from the previous transformation.

{% include_code Label1 fourth transformation lang:csharp monads/label1-leaves-transformation-4.cs %}

Before looking at its definition let's observe some of the characteristics of this function. It takes two functions as its arguments and returns a third function, which is then executed passing `label` as its only argument. Knowing both the signatures of the input functions and the return type of the containing `Label1` function implies that we also know the signature of the `Bind` function. Furthermore, being a replacement for the last few lines of code from the previous step means that the body will encapsulate the same logic in some form.

{% include_code Bind lang:csharp monads/bind-tuples.cs %}

The important thing to note is that we've come here by applying some simple and logical transformations to the initial code, and if you try to follow what this code is doing you can realize that the behavior is unchanged. One final step to abstract this even more is to make it generic over the second item of the tuples involved, thus leading to a new signature (the body is unchanged).

{% include_code Generic Bind lang:csharp monads/bind-tuples-generic.cs %}

#### Fifth transformation: S type

The signature of our `Bind` function is a mouthful, but we can see that there's a recurring pattern in there. Specifically, we can see occurrences of `Func<int, Tuple<int, X>>` again and again. Let's encapsulate this into a generic type `S<T>` which will have a single member of that function type, arbitrarily named `RunWithState`.

{% include_code Empty S<T> class lang:csharp monads/s-class-empty.cs %}

Let's as well rewrite `Bind` in terms of `S`.

{% include_code Bind lang:csharp monads/bind.cs %}

Now we can apply one additional transformation to our `Label1` function by rewriting everything in terms of `S`.

{% include_code Label1 fifth transformation lang:csharp monads/label1-leaves-transformation-5.cs %}

#### Sixth transformation: Return

We can finally observe one more pattern in how the `labelLeafS` function creates an instance of the `S` type. You can see that the only value this instance depends on is the labeled leaf created inside its `RunWithState` function, which is of exactly the same type as the generic argument of the instance `S<Tree<Labeled<T>>>`. Not that we benefit much from it now, but let's extract this pattern into a `Return` method, which given a `T`, returns an instance of `S<T>`.

{% include_code Return lang:csharp monads/return.cs %}

Let's also rewrite the relevant part of the `Label1` function in terms of `Return`, additionally inlining the `labelLeafS` function.

{% include_code Label1 sixth transformation lang:csharp monads/label1-leaves-transformation-6.cs %}

### Hiding the state

Before we move on and formalize the outcome of this long sequence of transformations in a new, named pattern let's observe one important thing in the final version of our code. The final statement returns a value of type `Tuple<int, Tree<Labeled<T>>>` simply because we execute the `RunWithState` function of the `S<Tree<Labeled<T>>>` instance returned by `Bind`.  Executing this function is also the only occasion in which we use the integer `label` argument of the containing function `Label1`.

The observation is therefore: if rather than returning a `Tuple<int, Tree<Labeled<T>>>` we returned `S<Tree<Labeled<T>>>` then we wouldn't need the `label` argument in `Label1` and shift to the caller the responsibility of executing `RunWithState` with the initial label value. In fact `Label`, the caller, is already doing that, just more explicitly via function arguments, but we can see that we have an opportunity to change the pattern of providing the initial state by moving from a function with this signature:

```csharp pseudo old signature
int, Tree<T> => Tuple<int, Tree<Labeled<T>>>
```

To a function with this signature:

```csharp pseudo new signature
Tree<T> => int => Tuple<int, Tree<Labeled<T>>>
```

Those familiar with functional programming will find that this is vaguely similar to currying, where a function of n arguments is transformed into n functions of 1 argument each.

We now have all the building blocks for rewriting our whole labeling functions `Label` and `Label1` from a completely new perspective, where the final outcome is the same but the means by which we get there are completely different. This also justifies creating two new functions called `MLabel` and `MLabel1`, respectively.

{% include_code MLabel and MLabel1 lang:csharp monads/mlabel-and-mlabel1.cs %}

Together with `Bind`, `Return` and the type `S<T>` this is the bulk of the code required to label trees in this new way. 

Now, although we have gone to great lengths talking about leaves, you might wonder why branches did not deserve their own considerations as I've included them in the code above already.  
The reason is that as soon as you understand what we did with leaves it should be easy, even easier, to figure out what is happening with branches, as there is no state change involved. Before we try to understand the similarities I suggest you lookup the [final version of the code][label] from the previous post.

The analogy should be more visible than it is for leaves, let's explain in words. We bind a recursive call to `MLabel1` fed with the left tree of the branch to a function which takes the left labeled tree and binds another recursive call, this time fed with the right tree, to a function which takes the right labeled tree and _returns_ an instance of the _state monad_ created from a branch whose left tree is accessed via a closure and the right is the argument of the function itself.

Besides the mouthful, did I just say _state monad_? Ah right, the state monad is the triplet represented by `S<T>`, `Bind` and `Return`, while `MLabel` and `MLabel1` are just our logic to label a tree in a third way besides manually and functionally: _monadically_.  

As for the state monad, we just invented it, but more on this in the next post.

[introduction]: /blog/2013/03/17/labeling-trees-introduction
[label]: /blog/2013/03/17/labeling-trees-introduction#label