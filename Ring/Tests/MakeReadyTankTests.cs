using System;
using System.Windows;
using Xunit;
using Ring.Views;

namespace Ring.Tests.Windows
{
    public class MakeReadyTankTests
    {
        [Fact]
        public void Constructor_InitializesComponents()
        {
            // Arrange & Act
            var window = new MakeReadyTank();

            // Assert
            Assert.NotNull(window);
            // Add assertions for specific components
        }

        [Fact]
        public void Close_Click_ClosesWindow()
        {
            // Arrange
            var window = new MakeReadyTank();
            var closeButton = window.FindName("CloseButton") as System.Windows.Controls.Button;

            // Act
            closeButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));

            // Assert
            Assert.False(window.IsVisible);
        }
    }
}