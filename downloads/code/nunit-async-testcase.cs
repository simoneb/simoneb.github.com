[TestCase(1, 2, Result = 3)]
public async Task<int> TestAddAsync(int a, int b)
{
    return await SumAsync(a, b);
}	

public async Task<int> SumAsync(int a, int b)
{
	return await Task.FromResult(a) + await Task.FromResult(b);
}