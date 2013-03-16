void Main()
{
  new NUnitLite.Runner.TextUI().Execute(new[]{"-noheader"});
}

// Define other methods and classes here
[Test]
public void SomeTest()
{
  Assert.Pass();
}