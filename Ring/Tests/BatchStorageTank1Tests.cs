//using System;
//using System.Windows;
//using Xunit;
//using Ring;

//namespace Ring.Tests.Windows
//{
//    public class StorageTank1Tests
//    {
//        [Fact]
//        public void Constructor_InitializesComponents()
//        {
//            // Arrange & Act
//            var window = new StorageTank1();

//            // Assert
//            Assert.NotNull(window);
//            // Add assertions for specific components
//        }

//        [Fact]
//        public void Close_Click_ClosesWindow()
//        {
//            // Arrange
//            var window = new StorageTank1();
//            var closeButton = window.FindName("CloseButton") as System.Windows.Controls.Button;

//            // Act
//            closeButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));

//            // Assert
//            Assert.False(window.IsVisible);
//        }
//    }
//}