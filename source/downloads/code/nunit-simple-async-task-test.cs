[Test]
public async Task OneSimpleTest()
{
    var eightBall = new EightBall();
    var answer = await eightBall.ShouldIChangeJob();
    
    Assert.That(answer, Is.True);
}