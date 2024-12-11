using Grpc.Core;
using gRPCServer;


public class MyCaseService : CaseOpener.CaseOpenerBase

{
    public override Task<OpenCaseResponse> OpenCase(OpenCaseRequest request, ServerCallContext context)
    {
        // Example logic for determining case result
        CaseItem item = new CaseItem();
        CaseItem item2 = new CaseItem();
        CaseItem item3 = new CaseItem();
        var random = new Random();
        int selectedIndex = random.Next(0, 3);
        item.Name = "Scar-20 | Grotto";
        item.ImageUrl = "https://community.fastly.steamstatic.com/economy/image/-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXH5ApeO4YmlhxYQknCRvCo04DEVlxkKgpopbmkOVUw7PLZTi5B7c7kxL-Hkvb_DLfYkWNFpp133LGX9o2mjVHmqUU-N2Ghd4WWcFdqaQnV8lC9ybzuhZ-5tcnAz3d9-n51z0AeyDY/360fx360f";
        item2.Name = "AWP | Dragon Lore";
        item2.ImageUrl = "https://community.fastly.steamstatic.com/economy/image/-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXH5ApeO4YmlhxYQknCRvCo04DEVlxkKgpot621FAR17P7NdTRH-t26q4SZlvD7PYTQgXtu5cB1g_zMu9Wk2ATh_0tkMWrzLY7BIQM2NArQq1O9kL_qgJTt6Ziam3Bh6SR3sHfD30vgriIWFx4/360fx360f";
        item3.Name = "AK-47 | Cartel";
        item3.ImageUrl = "https://community.fastly.steamstatic.com/economy/image/-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXH5ApeO4YmlhxYQknCRvCo04DEVlxkKgpot7HxfDhhwszJemkV09-3hpSOm8j4OrzZgiVT6ZIn3e-RrdSkjAzh80s_YWvzIYbHdwJrZQmG_wO7wenug5Ho78ib1zI97dq5Qskr/360fx360f";
        //to sa 3 itemki do testowania komunikacji bo narazie nie pobieramy z api z bazy itp
        CaseItem[] items = {item,item2,item3};

        return Task.FromResult(new OpenCaseResponse
        {
            Index = selectedIndex,
            Name = items[selectedIndex].Name,
            ImageUrl = items[selectedIndex].ImageUrl
        }) ;
    }

    public override Task<GetCaseItemsResponse> GetCaseItems(GetCaseItemsRequest request, ServerCallContext context)
    {

        CaseItem item = new CaseItem();
        CaseItem item2 = new CaseItem();
        CaseItem item3 = new CaseItem();

        item.Name = "Scar-20 | Grotto";
        item.ImageUrl = "https://community.fastly.steamstatic.com/economy/image/-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXH5ApeO4YmlhxYQknCRvCo04DEVlxkKgpopbmkOVUw7PLZTi5B7c7kxL-Hkvb_DLfYkWNFpp133LGX9o2mjVHmqUU-N2Ghd4WWcFdqaQnV8lC9ybzuhZ-5tcnAz3d9-n51z0AeyDY/360fx360f";
        item2.Name = "AWP | Dragon Lore";
        item2.ImageUrl = "https://community.fastly.steamstatic.com/economy/image/-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXH5ApeO4YmlhxYQknCRvCo04DEVlxkKgpot621FAR17P7NdTRH-t26q4SZlvD7PYTQgXtu5cB1g_zMu9Wk2ATh_0tkMWrzLY7BIQM2NArQq1O9kL_qgJTt6Ziam3Bh6SR3sHfD30vgriIWFx4/360fx360f";
        item3.Name = "AK-47 | Cartel";
        item3.ImageUrl = "https://community.fastly.steamstatic.com/economy/image/-9a81dlWLwJ2UUGcVs_nsVtzdOEdtWwKGZZLQHTxDZ7I56KU0Zwwo4NUX4oFJZEHLbXH5ApeO4YmlhxYQknCRvCo04DEVlxkKgpot7HxfDhhwszJemkV09-3hpSOm8j4OrzZgiVT6ZIn3e-RrdSkjAzh80s_YWvzIYbHdwJrZQmG_wO7wenug5Ho78ib1zI97dq5Qskr/360fx360f";
        //to sa 3 itemki do testowania komunikacji bo narazie nie pobieramy z api z bazy itp
        CaseItem[] items = { item, item2, item3 };

        var response = new GetCaseItemsResponse();
        response.Items.Add(item);
        response.Items.Add(item2);
        response.Items.Add(item3);

        return Task.FromResult(response);
    }
}
