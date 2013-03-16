[Test]
public void OneSimpleTest()
{
    var eightBall = new EightBall();
    var answer = eightBall.ShouldIChangeJob();
    
    Assert.That(answer, Is.True);
}