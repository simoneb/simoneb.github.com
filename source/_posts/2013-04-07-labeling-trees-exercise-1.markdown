---
layout: post
title: "Labeling Trees - Exercise 1"
date: 2013-04-07 19:41
comments: true
categories: 
tags: monads
---

In the [last post][last] of this series we learned how to label a binary tree monadically, and in doing so we discovered the state monad. I've collected the pieces we built incrementally last time into [this gist][monadV1] containing the whole monadic label implementation as a [LINQPad][linqpad] query that you can simply copy and run. I'll keep making changes to that gist and create links which point to specific revisions from now on.

As I mentioned in the [introduction][introduction] I decided to write this series as a result of watching some years ago Brian Beckman talking about monads and not being able to grasp the topic at that time. Going through it again and explaining what it took me to understand it will hopefully help someone else follow the same path. Up until now we've rediscovered what Brian explained in his interviews, but the accompanying source code (that I've copied in [this gist][beckman monad] for convenience) also gave 10 assignments to the reader - without solutions - which I'm going to tackle in the remainder of this series.

The exercises are of varying difficulties, basically expanding on the topic of the state monad with tasks which require generalizing the pattern and applying it to solve problems other than labeling binary trees. So, without further ado, let's move on to the first exercise.

### Exercise 1

> generalize over the type of the state, from int
> to \<TState\>, say, so that the S type can handle any kind of
> state object. Start with Labeled\<T\> --> StateContent<TState, T>, from
> "label-content pair" to "state-content pair".

I rephrased the original text of the exercise a little bit so it matches our own implementation rather than Brian's. Some type names are different as well as some design choices, but the overall concept is the same. Let's see what this exercise is about.

So far we used a `Labeled<T>` class to represent the contents of a labeled tree - leaves, specifically. This class is a simple container of label and value pairs, where only the latter is generically parameterized via the `T` generic parameter. This exercise requires to parameterize the type of the label/state too, which is currently fixed to be an integer.  It doesn't sound very difficult, does it? We simply come up with a new type `StateContent<TState, T>` which does just that, and then adapt the usages of `Labeled<T>` to match the new type, as shown in this diff.

```diff
@@ -59,10 +59,10 @@ public class Branch<T> : Tree<T>
 
-public class Labeled<T>
+public class StateContent<TState, T>
 {
-    public int Label { get; private set; }
+    public TState State { get; private set; }
     public T Value { get; private set; }
     
-    public Labeled(int label, T value)
+    public StateContent(TState state, T value)
     {
-        Label = label;
+        State = state;
         Value = value;
@@ -72,3 +72,3 @@ public class Labeled<T>
     {
-        return new { Label, Value }.ToString();
+        return new { State, Value }.ToString();
     }
@@ -86,5 +86,5 @@ public Tree<T> branch<T>(Tree<T> left, Tree<T> right)
 
-public Labeled<T> labeled<T>(int label, T value)
+public StateContent<TState, T> stateContent<TState, T>(TState state, T value)
 {
-    return new Labeled<T>(label, value);
+    return new StateContent<TState, T>(state, value);
 }
@@ -117,3 +117,3 @@ public static S<T> Return<T>(T value)
 
-public Tree<Labeled<T>> MLabel<T>(Tree<T> tree)
+public Tree<StateContent<int, T>> MLabel<T>(Tree<T> tree)
 {
@@ -122,3 +122,3 @@ public Tree<Labeled<T>> MLabel<T>(Tree<T> tree)
 
-public S<Tree<Labeled<T>>> MLabel1<T>(Tree<T> tree)
+public S<Tree<StateContent<int, T>>> MLabel1<T>(Tree<T> tree)
 {
@@ -134,3 +134,3 @@ public S<Tree<Labeled<T>>> MLabel1<T>(Tree<T> tree)
         return Bind(updateState, 
-                    label => Return(leaf(labeled(label, lf.Value))));
+                    label => Return(leaf(stateContent(label, lf.Value))));
     }
```

It was an easy one this time, which sets the foundation for the next exercise, but we'll come to it in the next post. The source code after this change is in the same old gist at [this revision][monadV2].

Stay tuned!

[last]: /blog/2013/03/31/labeling-trees-invisible-state
[introduction]: /blog/2013/03/17/labeling-trees-introduction
[linqpad]: http://www.linqpad.net
[monadV1]: https://gist.github.com/simoneb/5332234/7beedb523a5c8ed2c72142000c881b3b3a890703
[beckman monad]: https://gist.github.com/simoneb/627a349a1632307c301b
[monadV2]: https://gist.github.com/simoneb/5332234/632617aa38af3bbce179bd3891c1ffc660881e92