// Ring/Services/Images/ImageStateManager.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ring.Services.Images
{
    public class ImageStateManager : IDisposable
    {
        private readonly Dictionary<string, BitmapImage> _imageCache = new Dictionary<string, BitmapImage>();
        private readonly DispatcherTimer _updateTimer;
        private readonly Action<string, BitmapImage> _onImageChanged;

        public ImageStateManager(Action<string, BitmapImage> onImageChanged)
        {
            _onImageChanged = onImageChanged;
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _updateTimer.Tick += UpdateTimer_Tick;
        }

        public void Start()
        {
            _updateTimer.Start();
        }

        public void Stop()
        {
            _updateTimer.Stop();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // Update image states based on PLC data
            UpdateImageStates();
        }

        private void UpdateImageStates()
        {
            // Example: Update tank images based on PLC states
            UpdateTankImage("Tank1", GetTankState("Tank1"));
            UpdateTankImage("Tank2", GetTankState("Tank2"));
            UpdateTankImage("Tank3", GetTankState("Tank3"));
        }

        private void UpdateTankImage(string tankName, string state)
        {
            var imageKey = $"{tankName}_{state}";

            if (!_imageCache.ContainsKey(imageKey))
            {
                _imageCache[imageKey] = ImageLoader.LoadStateImage(tankName, state);
            }

            _onImageChanged?.Invoke(tankName, _imageCache[imageKey]);
        }

        private string GetTankState(string tankName)
        {
            // This would be replaced with actual PLC communication
            // For now, return a default state
            return "normal";
        }

        public void Dispose()
        {
            _updateTimer?.Stop();
            _imageCache.Clear();
        }
    }
}