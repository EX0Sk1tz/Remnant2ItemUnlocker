using System.IO;
using System.Text.Json;
using Remnant2UnlockerApp.Models;

namespace Remnant2UnlockerApp.Services;

public sealed class QueueWriter
{
    private readonly GamePathService _gamePathService;

    public QueueWriter(GamePathService gamePathService)
    {
        _gamePathService = gamePathService;
    }

    public async Task WriteAsync(QueueCommand command)
    {
        command.Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var queuePath = _gamePathService.GetQueuePath();
        var directory = Path.GetDirectoryName(queuePath);

        if (string.IsNullOrWhiteSpace(directory))
            throw new InvalidOperationException("Queue directory could not be resolved.");

        Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(
            command,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await File.WriteAllTextAsync(queuePath, json);
    }

    public Task SpawnAsync(RemnantItem item)
    {
        return WriteAsync(new QueueCommand
        {
            Action = "spawn",
            Path = item.Path,
            Name = item.Name,
            DelayMs = 500
        });
    }

    public Task UnlockTypesAsync(IEnumerable<string> types)
    {
        return WriteAsync(new QueueCommand
        {
            Action = "unlock_types",
            Types = types.ToList(),
            DelayMs = 500
        });
    }

    public Task ReloadItemsAsync()
    {
        return WriteAsync(new QueueCommand
        {
            Action = "reload_items"
        });
    }
}