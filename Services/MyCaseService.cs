using Grpc.Core;
using gRPCServer;

public class MyCaseService : CaseOpener.CaseOpenerBase

{
    public override Task<OpenCaseResponse> OpenCase(OpenCaseRequest request, ServerCallContext context)
    {
        // Example logic for determining case result
        var random = new Random();
        int selectedIndex = random.Next(0, 4); // Example with 4 segments
        string[] items = { "Common", "Rare", "Epic", "Legendary"};

        return Task.FromResult(new OpenCaseResponse
        {
            Index = selectedIndex,
            Name = items[selectedIndex],
            ImageUrl = "test"
        });
    }
}
