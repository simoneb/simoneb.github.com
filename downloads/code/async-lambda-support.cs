[Test]
public async Task AsyncLambaSupport()
{
    // throwing asynchronously
    Assert.That(async () => await ThrowAsync(), Throws.TypeOf<InvalidOperationException>());

    // returning values asynchronously
    Assert.That(async () => await ReturnOneAsync(), Is.EqualTo(1));

    // "After" works with async methods too
    Assert.That(async () => await ResutnOneAsync(), Is.EqualTo(1).After(100));
}