using System.IO;
using System.Text.Json;
using Remnant2UnlockerApp.Models;

namespace Remnant2UnlockerApp.Services;

public sealed class ItemRepository
{
    private readonly GamePathService _gamePathService;

    public ItemRepository(GamePathService gamePathService)
    {
        _gamePathService = gamePathService;
    }

    public async Task<List<RemnantItem>> LoadItemsAsync()
    {
        var path = _gamePathService.GetItemsPath();

        if (!File.Exists(path))
            return new List<RemnantItem>();

        await using var stream = File.OpenRead(path);

        var items = await JsonSerializer.DeserializeAsync<List<RemnantItem>>(
            stream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return items?
            .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Path))
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .ToList() ?? new List<RemnantItem>();
    }
}