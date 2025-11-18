//using System;
//using System.Windows;
//using Xunit;
//using Ring;

//namespace Ring.Tests.Windows
//{
//    public class ShiftControlTests
//    {
//        [Fact]
//        public void Constructor_InitializesComponents()
//        {
//            // Arrange & Act
//            var window = new ShiftControl();

//            // Assert
//            Assert.NotNull(window);
//            // Add assertions for specific components
//        }

//        [Fact]
//        public void Save_Click_SavesShiftData()
//        {
//            // Arrange
//            var window = new ShiftControl();
//            var saveButton = window.FindName("SaveButton") as System.Windows.Controls.Button;

//            // Act
//            saveButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));

//            // Assert
//            // Verify shift data was saved
//        }

//        [Fact]
//        public void Close_Click_ClosesWindow()
//        {
//            // Arrange
//            var window = new ShiftControl();
//            var closeButton = window.FindName("CloseButton") as System.Windows.Controls.Button;

//            // Act
//            closeButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));

//            // Assert
//            Assert.False(window.IsVisible);
//        }
//    }
//}