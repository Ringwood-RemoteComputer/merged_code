using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ring.Controls
{
    /// <summary>
    /// A reusable control for displaying tank fill levels with a visual tank graphic
    /// </summary>
    public partial class TankLevelControl : UserControl
    {
        public TankLevelControl()
        {
            InitializeComponent();
            UpdateTankVisual();
        }

        #region Dependency Properties

        /// <summary>
        /// The fill percentage of the tank (0-100)
        /// </summary>
        public static readonly DependencyProperty FillPercentageProperty =
            DependencyProperty.Register(
                nameof(FillPercentage),
                typeof(double),
                typeof(TankLevelControl),
                new PropertyMetadata(0.0, OnFillPercentageChanged));

        public double FillPercentage
        {
            get => (double)GetValue(FillPercentageProperty);
            set => SetValue(FillPercentageProperty, value);
        }

        /// <summary>
        /// The color of the liquid in the tank (default: DodgerBlue)
        /// </summary>
        public static readonly DependencyProperty LiquidColorProperty =
            DependencyProperty.Register(
                nameof(LiquidColor),
                typeof(Brush),
                typeof(TankLevelControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(30, 144, 255)), OnLiquidColorChanged));

        public Brush LiquidColor
        {
            get => (Brush)GetValue(LiquidColorProperty);
            set => SetValue(LiquidColorProperty, value);
        }

        #endregion

        #region Property Changed Handlers

        private static void OnFillPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TankLevelControl control)
            {
                control.UpdateTankVisual();
            }
        }

        private static void OnLiquidColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TankLevelControl control)
            {
                control.UpdateLiquidColor();
            }
        }

        #endregion

        #region Private Methods

        private void UpdateTankVisual()
        {
            if (LiquidFill == null || PercentageText == null || LiquidTop == null || BottomRim == null)
                return;

            // Clamp percentage between 0 and 100
            double percentage = Math.Max(0, Math.Min(100, FillPercentage));

            // Update percentage text
            PercentageText.Text = $"{percentage:F0}%";

            // Calculate fill height (tank body is 140 units tall)
            double tankHeight = 140;
            double fillHeight = (percentage / 100.0) * tankHeight;
            
            // Update liquid fill height
            LiquidFill.Height = fillHeight;
            
            // Position the liquid top ellipse at the current fill level
            if (fillHeight > 0)
            {
                LiquidTop.Margin = new Thickness(0, 0, 0, fillHeight - 6);
                LiquidTop.Visibility = Visibility.Visible;
            }
            else
            {
                LiquidTop.Visibility = Visibility.Collapsed;
            }

            // Update bottom rim fill based on liquid level
            UpdateBottomRimFill(percentage);
        }

        private void UpdateLiquidColor()
        {
            if (LiquidFill == null || LiquidTop == null)
                return;

            LiquidFill.Fill = LiquidColor;
            
            // Update top surface to a lighter version of the liquid color
            if (LiquidColor is SolidColorBrush liquidBrush)
            {
                Color baseColor = liquidBrush.Color;
                Color lighterColor = Color.FromRgb(
                    (byte)Math.Min(255, baseColor.R + 47),
                    (byte)Math.Min(255, baseColor.G + 22),
                    (byte)Math.Min(255, baseColor.B + 0)
                );
                LiquidTop.Fill = new SolidColorBrush(lighterColor);
            }
            
            // Update bottom rim if there's liquid
            UpdateBottomRimFill(FillPercentage);
        }

        private void UpdateBottomRimFill(double percentage)
        {
            if (BottomRim == null)
                return;

            // If tank has liquid (> 0%), fill the bottom rim with the liquid color
            if (percentage > 0)
            {
                // Use the same color as the liquid fill
                if (LiquidFill.Fill is SolidColorBrush liquidBrush)
                {
                    BottomRim.Fill = liquidBrush;
                }
            }
            else
            {
                // Tank is empty, bottom rim is transparent
                BottomRim.Fill = Brushes.Transparent;
            }
        }

        #endregion
    }
}

