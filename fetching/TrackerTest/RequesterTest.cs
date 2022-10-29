using Common.Requesters;
namespace TrackerTest;

[TestClass]
public class Requestertest
{
    public IRequester CUT { get; private set; } = new BasicRequester();

    [TestMethod]
    public Task TestGet()
    {
        throw new NotImplementedException();
        // var answer = await CUT.GetAsync("https://google.com");
    }

    [TestMethod]
    public Task TestPostAsync()
    {
        throw new NotImplementedException();
    }
}