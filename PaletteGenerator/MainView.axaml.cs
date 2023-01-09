using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using SkiaSharp;

namespace PaletteGenerator;

public partial class MainView : UserControl
{
    private List<SKColor>? _dominantColors;

    public MainView()
    {
        InitializeComponent();
    }

    private Window? GetWindow()
    {
        if (this.GetVisualRoot() is Window window)
        {
            return window;
        }

        return null;
    }
    
    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog()
        {
            Filters = new List<FileDialogFilter>()
            {
                new FileDialogFilter() {Extensions = new List<string> {"png", "jpg", "jpeg"}, Name = "Image Files (*.png,*.jpg,*.jpeg)"},
                new FileDialogFilter() {Extensions = new List<string> {"*"}, Name = "All Files (*.*)"}
            },
            AllowMultiple = false
        };

        var results = await dlg.ShowAsync(GetWindow());
        if (results is { } && results.Length == 1)
        {
            FilePathTextBox.Text = results[0];
        }
    }

    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Generate();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

    private void Generate()
    {
        var filePath = FilePathTextBox.Text;
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        var numClusters = (int) ClustersNumericUpDown.Value;
        
        _dominantColors = PaletteGenerator.Generate(filePath, numClusters);

        // Print the dominant colors to the console
        /*
        foreach (var color in dominantColors)
        {
            Debug.WriteLine(color.ToString());
        }
        */

        SourceImage.Source = new Bitmap(filePath);

        ColorsItemsControl.Items = _dominantColors.Select(x => x.ToString());
    }

    private async void ExportActButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_dominantColors is null)
        {
            return;
        }

        var dlg = new SaveFileDialog()
        {
            Filters = new List<FileDialogFilter>()
            {
                new FileDialogFilter() {Extensions = new List<string> {"act" }, Name = "Adobe Color Table Files (*.act)"},
                new FileDialogFilter() {Extensions = new List<string> {"*" }, Name = "All Files (*.*)"}
            },
            DefaultExtension = "act",
            InitialFileName = "palette"
        };
        var result = await dlg.ShowAsync(GetWindow());
        if (result is { })
        {
            ExportAct.Export(_dominantColors.ToArray(), result);
        }
    }

    private async void ExportAseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_dominantColors is null)
        {
            return;
        }

        var dlg = new SaveFileDialog()
        {
            Filters = new List<FileDialogFilter>()
            {
                new FileDialogFilter() {Extensions = new List<string> {"ase" }, Name = "Adobe Swatch Exchange (*.ase)"},
                new FileDialogFilter() {Extensions = new List<string> {"*" }, Name = "All Files (*.*)"}
            },
            DefaultExtension = "ase",
            InitialFileName = "palette"
        };
        var result = await dlg.ShowAsync(GetWindow());
        if (result is { })
        {
            ExportAse.Export(_dominantColors.ToArray(), result);
        }
    }

    private async void ExportGplButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_dominantColors is null)
        {
            return;
        }

        var dlg = new SaveFileDialog()
        {
            Filters = new List<FileDialogFilter>()
            {
                new FileDialogFilter() {Extensions = new List<string> {"gpl" }, Name = "GIMP Palette Files (*.gpl)"},
                new FileDialogFilter() {Extensions = new List<string> {"*" }, Name = "All Files (*.*)"}
            },
            DefaultExtension = "gpl",
            InitialFileName = "palette"
        };
        var result = await dlg.ShowAsync(GetWindow());
        if (result is { })
        {
            ExportGpl.Export(_dominantColors.ToArray(), result);
        }
    }

    private async void ExportHexButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_dominantColors is null)
        {
            return;
        }

        var dlg = new SaveFileDialog()
        {
            Filters = new List<FileDialogFilter>()
            {
                new FileDialogFilter() {Extensions = new List<string> {"txt" }, Name = "RGB Hex Files (*.txt)"},
                new FileDialogFilter() {Extensions = new List<string> {"*" }, Name = "All Files (*.*)"}
            },
            DefaultExtension = "txt",
            InitialFileName = "palette"
        };
        var result = await dlg.ShowAsync(GetWindow());
        if (result is { })
        {
            ExportRgb.Export(_dominantColors.ToArray(), result);
        }
    }

    private async void ExportCsvButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_dominantColors is null)
        {
            return;
        }

        var dlg = new SaveFileDialog()
        {
            Filters = new List<FileDialogFilter>()
            {
                new FileDialogFilter() {Extensions = new List<string> {"csv" }, Name = "CSV Files (*.csv)"},
                new FileDialogFilter() {Extensions = new List<string> {"*" }, Name = "All Files (*.*)"}
            },
            DefaultExtension = "csv",
            InitialFileName = "palette"
        };
        var result = await dlg.ShowAsync(GetWindow());
        if (result is { })
        {
            ExportCsv.Export(_dominantColors.ToArray(), result);
        }
    }
}

