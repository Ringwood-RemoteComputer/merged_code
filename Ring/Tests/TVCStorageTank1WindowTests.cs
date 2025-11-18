using System;
using System.Windows;
using Xunit;
using Ring;

namespace Ring.Tests.Windows
{
    public class TVCStorageTank1WindowTests
    {
        [Fact]
        public void Constructor_InitializesComponents()
        {
            // Arrange & Act
            var window = new TVCStorageTank1Window();

            // Assert
            Assert.NotNull(window);
            // Add assertions for specific components
        }

        [Fact]
        public void Close_Click_ClosesWindow()
        {
            // Arrange
            var window = new TVCStorageTank1Window();
            var closeButton = window.FindName("CloseButton") as System.Windows.Controls.Button;

            // Act
            closeButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));

            // Assert
            Assert.False(window.IsVisible);
        }
    }
}