[Test]
public void OneSimpleTest()
{
    var eightBall = new EightBall();
    Task<bool> answer = eightBall.ShouldIChangeJob();
    answer.Wait();
    
    Assert.That(answer.Result, Is.True);
}