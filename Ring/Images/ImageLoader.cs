// Ring/Services/Images/ImageLoader.cs
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Ring.Services.Images
{
    public class ImageLoader
    {
        private static readonly string ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public static BitmapImage LoadImage(string imageName)
        {
            try
            {
                // Try pack URI first (for resources embedded in assembly)
                var packUri = $"pack://application:,,,/Images/{imageName}";

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(packUri, UriKind.Absolute);
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception ex)
            {
                // If pack URI fails, try file path
                try
                {
                    var imagePath = Path.Combine(ImagePath, imageName);

                    if (!File.Exists(imagePath))
                    {
                        Console.WriteLine($"Image not found: {imagePath}");
                        return CreateErrorImage();
                    }

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    return bitmap;
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"Error loading image {imageName}: {ex.Message}");
                    Console.WriteLine($"Inner error: {innerEx.Message}");
                    return CreateErrorImage();
                }
            }
        }

        private static BitmapImage CreateErrorImage()
        {
            // Return null for missing images - WPF will handle gracefully
            return null;
        }

        public static BitmapImage LoadStateImage(string baseImageName, string state)
        {
            var stateImageName = $"{baseImageName}_{state}.png";
            return LoadImage(stateImageName);
        }
    }
}