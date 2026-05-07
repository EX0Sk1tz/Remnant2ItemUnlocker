using System.Diagnostics;
using System.Runtime.InteropServices;
using Forms = System.Windows.Forms;
using Remnant2UnlockerApp.Models;

namespace Remnant2UnlockerApp.Services;

public sealed class ConsoleSpawnService
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    public async Task SpawnViaConsoleAsync(RemnantItem item)
    {
        var process = Process
            .GetProcessesByName("Remnant2-Win64-Shipping")
            .FirstOrDefault();

        if (process == null)
            throw new InvalidOperationException("Remnant 2 is not running.");

        var command = item.SummonCommand;

        SetForegroundWindow(process.MainWindowHandle);

        await Task.Delay(200);

        Forms.SendKeys.SendWait("{F10}");
        await Task.Delay(30);

        Forms.Clipboard.SetText(command);
        await Task.Delay(30);

        Forms.SendKeys.SendWait("^v");
        await Task.Delay(30);

        Forms.SendKeys.SendWait("{ENTER}");

    }
}