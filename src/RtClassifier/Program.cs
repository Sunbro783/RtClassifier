using System.Diagnostics;
using System.Text;
using System.Text.Json;
using RtClassifier;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string path = args.Length > 0
            ? args[0]
            : Path.Combine(AppContext.BaseDirectory, "data", "variant_medical_monitor.json");

        int exitCode = 0;

        try
        {
            InputData data = InputLoader.Load(path);
            Report.Print(data);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or JsonException)
        {
            Console.Error.WriteLine($"Ошибка: {ex.Message}");
            exitCode = 1;
        }

        if (Debugger.IsAttached && !Console.IsInputRedirected)
        {
            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey(true);
        }

        return exitCode;
    }
}
