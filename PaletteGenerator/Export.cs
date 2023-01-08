using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SkiaSharp;

namespace PaletteGenerator;

// RGB Hex
public class ExportRgb
{
    public static void Export(SKColor[] colors, string filePath)
    {
        // Create a list of strings representing the colors in the palette as RGB hexadecimal values
        var lines = CreateRgbHexLines(colors);

        File.WriteAllLines(filePath, lines);
    }

    private static List<string> CreateRgbHexLines(SKColor[] colors)
    {
        // Create a list of strings representing the colors in the palette as RGB hexadecimal values
        var lines = new List<string>();
        foreach (var color in colors)
        {
            lines.Add($"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}");
        }

        // Return the list
        return lines;
    }
}

// CSV
public class ExportCsv
{
    public static void Export(SKColor[] colors, string filePath)
    {
        // Create a CSV string representing the colors in the palette
        var csv = CreateCsvString(colors);

        // Save the CSV string to a file
        File.WriteAllText(filePath, csv);
    }

    private static string CreateCsvString(SKColor[] colors)
    {
        // Create a CSV string representing the colors in the palette
        var csv = "Red,Green,Blue\n";
        foreach (var color in colors)
        {
            csv += $"{color.Red},{color.Green},{color.Blue}\n";
        }

        // Return the CSV string
        return csv;
    }
}

// Adobe Color Table (.act)
public class ExportAct
{
    public static void Export(SKColor[] colors, string filePath)
    {
        // Create a binary data for the palette
        var data = CreateActData(colors);

        // Save the data to a file
        using (var stream = File.OpenWrite(filePath))
        {
            stream.Write(data);
        }
    }

    private static byte[] CreateActData(SKColor[] colors)
    {
        // The ACT file format consists of a 2-byte header and an array of 3-byte color values
        var data = new byte[2 + 3 * colors.Length];

        // Set the header bytes
        data[0] = 0;
        data[1] = 0;

        // Set the color values
        for (int i = 0; i < colors.Length; i++)
        {
            data[2 + i * 3] = colors[i].Red;
            data[2 + i * 3 + 1] = colors[i].Green;
            data[2 + i * 3 + 2] = colors[i].Blue;
        }

        // Return the data
        return data;
    }
}

// Adobe Swatch Exchange (.ase)
public class ExportAse
{
    public static void Export(SKColor[] colors, string filePath)
    {
        // Create a JSON object representing the palette
        var json = CreateAseJson(colors);

        // Save the JSON object to a file
        File.WriteAllText(filePath, json);
    }

    private static string CreateAseJson(SKColor[] colors)
    {
        // The ASE file format consists of a header and an array of color objects
        var root = new Dictionary<string, object>
        {
            { 
                "header", new Dictionary<string, object>
                {
                    { "appid", "SkiaSharp" },
                    { "appversion", "1.0" },
                    { "createdby", "SkiaSharp" },
                    { "numberofcolors", colors.Length }
                }
            },
            { "color", new List<Dictionary<string, object>>() }
        };

        // Add the color objects to the array
        foreach (var color in colors)
        {
            var colorJson = new Dictionary<string, object>
            {
                { "name", color.ToString() },
                { "model", "RGB" },
                { "mode", "Normal" },
                { "colors", new List<int> { color.Red, color.Green, color.Blue } }
            };
            ((List<Dictionary<string, object>>)root["color"]).Add(colorJson);
        }

        // Serialize the JSON object to a string
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(root, options);
    }
}

// GIMP Palette (.gpl)
public class ExportGpl
{
    public static void Export(SKColor[] colors, string filePath)
    {
        // Create a list of strings representing the colors in the palette
        var lines = CreateGplLines(colors);

        // Save the lines to a file
        File.WriteAllLines(filePath, lines);
    }

    private static List<string> CreateGplLines(SKColor[] colors)
    {
        // The GPL file format consists of a header and a list of color values
        var lines = new List<string>
        {
            "GIMP Palette",
            "Name: SkiaSharp Palette",
            "#",
        };

        // Add the color values to the list
        foreach (var color in colors)
        {
            lines.Add($"{color.Red} {color.Green} {color.Blue} {color.ToString()}");
        }

        // Return the list
        return lines;
    }
}
