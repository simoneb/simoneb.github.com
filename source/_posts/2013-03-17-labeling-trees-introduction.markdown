---
layout: post
title: "Labeling Trees - Introduction"
comments: true
published: true
categories: 
tags: monads
---

A few years ago I came across a video interview on Channel 9 starring Brian Beckman on a topic which I had never heard of before. The interview is still available online ([part 1][bbp1] and [part 2][bbp2]) but you don't need to watch it to follow me as I'll start from the beginning and guide you gently through the whole topic.

Although I then considered myself an already proficient C# developer I was quite confused, I just couldn't wrap my head around the crazy concept of passing state around _invisibly_, although it was done in my favorite language. The interview comes with full source code and an assignment containing 10 exercises, which I was quite disappointed I could not even tackle. The main topic of the interview is labeling a binary tree, and Brian showed how to do it in several ways, which we'll see and try to understand before moving on to the exercises.

Time has passed and I can finally go back to that interesting topic and hopefully help someone else out there. The topic, needless to say, is monads, and not even the simplest of the monads, the state monad. This post is the first of a series where I'll introduce the concept and then go through the exercises, hopefully solving them correctly.

Without further ado, here's the class which will accompany us throughout the journey, during which it will undergo only minor changes: a binary tree in C#.

{% include_code A binary tree lang:csharp monads/binary-tree.cs %}

As you can see a tree can either be a leaf, containing a `Value`, or a branch, whose left and right sides will be other trees (and thus either branches or leaves). The entire class hierarchy is generic for no special purpose except that it will allow us to give a meaningful representation to the leaves via their `T Value` property. I've also included a `Show(int)` method to easily ask the tree to dump itself to the output stream. A couple of helper methods to create branches and leaves will also come useful:

{% include_code Helper methods to create trees lang:csharp monads/helper-methods.cs %}

Now creating and displaying a tree of type `string` is straightforward:

{% include_code A simple tree lang:csharp monads/a-simple-tree.cs %}

## Manual labeling

First of all we need to know what labeling a tree means. In our case it means to navigate the tree and assign a integer label to every leaf. Tree navigation can usually be done in two ways, depth-first ([DFS][dfs]) or breadth-first ([BFS][bfs]), we'll use the former here. In order to store the label in the leaves we need to enrich its structure. We'll do that by making the generic parameter of the tree a `Labeled<T>`, defined as follows.

{% include_code Label lang:csharp monads/labeled-class.cs %}

In practice, labeling a structure of type `Tree<T>` means transforming it into a `Tree<Labeled<T>>`. With this new knowledge labeling our tree manually is as simple as recreating the same tree structure and replacing the value of the leaves with a `Labeled<string>` instance, incrementing the value of the label manually every time we visit a leaf.  
The value, or _state_, of the current label at each point in time lives in our brains: we'll have to remember the previous value and increment it every time we visit a leaf. Not a big deal as long as the tree is small.

{% include_code Manually labeled tree lang:csharp monads/manually-labeled-tree.cs %}

## Functional labeling

As you can see labeling a tree manually is not very useful, imagine to have a tree with more than a bunch branches and leaves, it becomes quickly inconvenient. What if we could label it programmatically? We indeed can, and we'll have to come up with a function, which given our initial unlabeled `Tree<T>`, gives us back a `Tree<Labeled<T>>`.  
Let's call `Tree<Labeled<T>> Label<T>(Tree<T>)` such a function.

{% include_code Label function lang:csharp monads/label-function-empty.cs %}

<a id="label"></a>
One constraint that we want to have is that we don't want to store the current label in shared memory and update it whenever we visit a leaf. We would like to do it in a purely functional way. This means that the state needs to be passed around explicitly, with a starting value of 0. This suggests that we'll need an additional support function that will thread the state via its arguments and return the updated state via its return value.  
Given the structure of the data we're handling this function will also most likely call itself recursively.

{% include_code Label and Label1 functions lang:csharp monads/functional-label.cs %}

`Label1` accepts an integer value representing the current label and a tree to be labeled. Its return value is a tuple containing the new label value and the labeled tree.

Let's go through the code to see what it does exactly. You can see that it does two different things according to whether the tree to be labeled is a leaf or a branch.  
In the first case it labels the leaf with the current value of the label, bumps it and returns a tuple containing the new label value and a leaf labeled with the value of the label before bumping it.  
In the second case it first calls itself recursively to label the left branch of the tree, which returns again a tuple with the current label value and the labeled sub-tree. Then it does the same with the right branch by passing the just-returned label value as the current label value. Finally it returns a tuple containing the final state of the label and a newly constructed labeled branch containing the previously labeled left and right branches.  
You might notice that it is this way of recursing that enables the depth-first approach.

Finally, `Label` calls `Label1` passing `0` as the initial state and returning only the second item in the tuple, which is the labeled tree.

The interesting thing about this approach is that it does not rely on any shared state to do the labeling. The current value of the label is passed as a function argument and the new value is returned as part of the result, versus storing the label value in a variable accessible from the function and incremented every time.

In the next post we'll see how to make this state-passing less explicit, while still avoiding shared memory to store it.

[bbp1]: http://channel9.msdn.com/Shows/Going+Deep/Brian-Beckman-The-Zen-of-Expressing-State-The-State-Monad
[bbp2]: http://channel9.msdn.com/shows/Going+Deep/Brian-Beckman-The-Zen-of-Stateless-State-The-State-Monad-Part-2
[dfs]: http://en.wikipedia.org/wiki/Depth-first_search
[bfs]: http://en.wikipedia.org/wiki/Breadth-first_search