using Common.Requesters;
namespace TrackerTest;

[TestClass]
public class Requestertest
{
    public IRequester CUT { get; private set; } = new BasicRequester();

    [TestMethod]
    public async Task TestGet()
    {
        // var answer = await CUT.GetAsync("https://google.com");
    }

    [TestMethod]
    public async Task TestPostAsync()
    {

    }
}