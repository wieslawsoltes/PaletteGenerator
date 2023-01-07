using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

namespace PaletteGenerator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Filters = new List<FileDialogFilter>()
                {
                    new FileDialogFilter() {Extensions = new List<string> {"*", "png"}, Name = "All Files (*.*)"}
                },
                AllowMultiple = false
            };
            var results = await dlg.ShowAsync(this);
            if (results is { } && results.Length == 1)
            {
                FilePathTextBox.Text = results[0];
            }
        }

        private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var filePath = FilePathTextBox.Text;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }
            
            var dominantColors = PaletteGenerator.Generate(filePath);

            // Print the dominant colors to the console
            /*
            foreach (var color in dominantColors)
            {
                Console.WriteLine(color.ToString());
            }
            */

            SourceImage.Source = new Bitmap(filePath);

            ColorsItemsControl.Items = dominantColors.Select(x => x.ToString());
        }
    }
}
