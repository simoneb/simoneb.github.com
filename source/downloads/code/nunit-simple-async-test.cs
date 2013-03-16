[Test]
public async void OneSimpleTest()
{
    var eightBall = new EightBall();
    var answer = await eightBall.ShouldIChangeJob();
    
    Assert.That(answer, Is.True);
    
    // why am I still here?
}