using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Ring.Models;

namespace Ring.Services
{
    /// <summary>
    /// Service class for exporting batch data to various formats (Excel, PDF, Print)
    /// </summary>
    public class ExportService
    {
        private const string CompanyName = "Ringwood RS3000";
        private const string ReportTitle = "Batch Query Report";
        
        /// <summary>
        /// Initializes a new instance of the ExportService
        /// </summary>
        public ExportService()
        {
            // Initialize service
        }

        #region Excel Export (Placeholder)

        /// <summary>
        /// Exports batch data to Excel format (CSV placeholder)
        /// </summary>
        /// <param name="batches">List of batch queries to export</param>
        /// <param name="filePath">File path where to save the Excel file</param>
        /// <returns>Task representing the async operation</returns>
        public async Task ExportToExcelAsync(List<BatchQuery> batches, string filePath)
        {
            try
            {
                LogInfo($"Starting Excel export for {batches.Count} batches to {filePath}");

                if (batches == null || !batches.Any())
                {
                    throw new ArgumentException("Batch list cannot be null or empty", nameof(batches));
                }

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                }

                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await Task.Run(() =>
                {
                    // Create CSV file as placeholder for Excel export
                    CreateCsvFile(batches, filePath.Replace(".xlsx", ".csv"));
                });

                LogInfo($"Excel export completed successfully: {filePath}");
            }
            catch (Exception ex)
            {
                LogError($"Error exporting to Excel: {ex.Message}", ex);
                throw new InvalidOperationException($"Failed to export to Excel: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a CSV file from batch data
        /// </summary>
        /// <param name="batches">Batch data</param>
        /// <param name="filePath">File path</param>
        private void CreateCsvFile(List<BatchQuery> batches, string filePath)
        {
            var csv = new StringBuilder();
            
            // Add header
            csv.AppendLine($"{CompanyName} - {ReportTitle}");
            csv.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            csv.AppendLine($"Total Records: {batches.Count}");
            csv.AppendLine();
            
            // Add column headers
            csv.AppendLine("ID,Formula Name,Formula #,Start Date,Start Time,End Date,End Time," +
                          "Storage Tank,Tank #,Volume,Temperature,Status,Status Text,Start Weight,End Weight");
            
            // Add data rows
            foreach (var batch in batches)
            {
                csv.AppendLine($"{batch.Id},{EscapeCsvValue(batch.FormulaName)},{batch.FormulaNumber}," +
                              $"{batch.StartDate:yyyy-MM-dd},{batch.StartTime:hh\\:mm\\:ss}," +
                              $"{batch.EndDate:yyyy-MM-dd},{batch.EndTime:hh\\:mm\\:ss}," +
                              $"{EscapeCsvValue(batch.StorageTankName)},{batch.StorageTankNumber}," +
                              $"{batch.Volume:F2},{batch.Temperature:F1},{batch.Status}," +
                              $"{EscapeCsvValue(batch.StatusText)},{batch.StartWeight:F2},{batch.EndWeight:F2}");
            }
            
            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Escapes CSV values that contain commas or quotes
        /// </summary>
        /// <param name="value">Value to escape</param>
        /// <returns>Escaped value</returns>
        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
                
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            
            return value;
        }

        #endregion

        #region PDF Export (Placeholder)

        /// <summary>
        /// Exports batch data to PDF format (Text file placeholder)
        /// </summary>
        /// <param name="batches">List of batch queries to export</param>
        /// <param name="filePath">File path where to save the PDF file</param>
        /// <returns>Task representing the async operation</returns>
        public async Task ExportToPdfAsync(List<BatchQuery> batches, string filePath)
        {
            try
            {
                LogInfo($"Starting PDF export for {batches.Count} batches to {filePath}");

                if (batches == null || !batches.Any())
                {
                    throw new ArgumentException("Batch list cannot be null or empty", nameof(batches));
                }

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                }

                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await Task.Run(() =>
                {
                    // Create text file as placeholder for PDF export
                    CreateTextFile(batches, filePath.Replace(".pdf", ".txt"));
                });

                LogInfo($"PDF export completed successfully: {filePath}");
            }
            catch (Exception ex)
            {
                LogError($"Error exporting to PDF: {ex.Message}", ex);
                throw new InvalidOperationException($"Failed to export to PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a formatted text file from batch data
        /// </summary>
        /// <param name="batches">Batch data</param>
        /// <param name="filePath">File path</param>
        private void CreateTextFile(List<BatchQuery> batches, string filePath)
        {
            var text = new StringBuilder();
            
            // Add header
            text.AppendLine($"{CompanyName}");
            text.AppendLine($"{ReportTitle}");
            text.AppendLine(new string('=', 50));
            text.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            text.AppendLine($"Total Records: {batches.Count}");
            text.AppendLine();
            
            // Add data in formatted table
            text.AppendLine("ID  | Formula Name        | Formula # | Start Date  | Start Time | End Date    | End Time | Storage Tank | Tank # | Volume  | Temp | Status | Status Text | Start Wt | End Wt");
            text.AppendLine(new string('-', 150));
            
            foreach (var batch in batches)
            {
                text.AppendLine($"{batch.Id,3} | {batch.FormulaName,-19} | {batch.FormulaNumber,8} | " +
                              $"{batch.StartDate:yyyy-MM-dd} | {batch.StartTime:hh\\:mm\\:ss} | " +
                              $"{batch.EndDate:yyyy-MM-dd} | {batch.EndTime:hh\\:mm\\:ss} | " +
                              $"{batch.StorageTankName,-12} | {batch.StorageTankNumber,6} | " +
                              $"{batch.Volume,7:F2} | {batch.Temperature,4:F1} | {batch.Status,6} | " +
                              $"{batch.StatusText,-12} | {batch.StartWeight,8:F2} | {batch.EndWeight,6:F2}");
            }
            
            File.WriteAllText(filePath, text.ToString(), Encoding.UTF8);
        }

        #endregion

        #region Print Functionality

        /// <summary>
        /// Prints batch data using WPF PrintDialog with print preview
        /// </summary>
        /// <param name="batches">List of batch queries to print</param>
        /// <returns>Task representing the async operation</returns>
        public async Task PrintReportAsync(List<BatchQuery> batches)
        {
            try
            {
                LogInfo($"Starting print operation for {batches.Count} batches");

                if (batches == null || !batches.Any())
                {
                    throw new ArgumentException("Batch list cannot be null or empty", nameof(batches));
                }

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Create and show print preview window first
                        var previewWindow = CreatePrintPreviewWindow(batches);
                        if (previewWindow != null)
                        {
                            previewWindow.ShowDialog();
                        }
                    });
                });

                LogInfo("Print operation completed successfully");
            }
            catch (Exception ex)
            {
                LogError($"Error printing report: {ex.Message}", ex);
                throw new InvalidOperationException($"Failed to print report: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Shows print preview for the batch data
        /// </summary>
        /// <param name="batches">Batch data to preview</param>
        /// <returns>True if user wants to proceed with printing</returns>
        private bool ShowPrintPreview(List<BatchQuery> batches)
        {
            try
            {
                var previewWindow = new Window
                {
                    Title = "Print Preview - Batch Report",
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var scrollViewer = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                };

                var document = CreatePrintDocument(batches);
                scrollViewer.Content = document;

                var stackPanel = new StackPanel();
                stackPanel.Children.Add(scrollViewer);

                var buttonPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10)
                };

                var printButton = new Button
                {
                    Content = "Print",
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5)
                };

                var cancelButton = new Button
                {
                    Content = "Cancel",
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5)
                };

                bool printConfirmed = false;

                printButton.Click += (s, e) =>
                {
                    printConfirmed = true;
                    previewWindow.Close();
                };

                cancelButton.Click += (s, e) =>
                {
                    printConfirmed = false;
                    previewWindow.Close();
                };

                buttonPanel.Children.Add(printButton);
                buttonPanel.Children.Add(cancelButton);
                stackPanel.Children.Add(buttonPanel);

                previewWindow.Content = stackPanel;
                previewWindow.ShowDialog();

                return printConfirmed;
            }
            catch (Exception ex)
            {
                LogError($"Error showing print preview: {ex.Message}", ex);
                // If preview fails, proceed with printing
                return true;
            }
        }

        /// <summary>
        /// Creates a modern print document for the batch data with professional styling
        /// </summary>
        /// <param name="batches">Batch data</param>
        /// <returns>FlowDocument for printing</returns>
        private FlowDocument CreatePrintDocument(List<BatchQuery> batches)
        {
            var document = new FlowDocument
            {
                PagePadding = new Thickness(30), // Reduced padding for more content space
                ColumnGap = 0,
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 10, // Reduced font size for better fit
                PageHeight = double.NaN, // Auto-adjust to paper size
                PageWidth = double.NaN   // Auto-adjust to paper size
            };

            // Modern Header Section
            var headerContainer = new Table
            {
                CellSpacing = 0,
                BorderBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Primary blue
                BorderThickness = new Thickness(0, 0, 0, 3),
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Add single column for header
            headerContainer.Columns.Add(new TableColumn());

            var headerRowGroup = new TableRowGroup();
            var headerRow = new TableRow
            {
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243))
            };

            var headerCell = new TableCell
            {
                BorderThickness = new Thickness(0),
                Padding = new Thickness(20, 16, 20, 16)
            };

            // Create header content with company branding
            var headerStack = new StackPanel();
            
            // Title and RS3000 badge
            var titleRow = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left };
            var titleText = new TextBlock
            {
                Text = "Batch Query Report",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            };
            titleRow.Children.Add(titleText);

            // RS3000 badge
            var rs3000Badge = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 215, 0)), // Gold
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(12, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            var rs3000Text = new TextBlock
            {
                Text = "RS3000",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(33, 150, 243))
            };
            rs3000Badge.Child = rs3000Text;
            titleRow.Children.Add(rs3000Badge);

            headerStack.Children.Add(titleRow);

            // Location
            var locationText = new TextBlock
            {
                Text = "CTC -- Waco, TX",
                FontSize = 16,
                Foreground = Brushes.White,
                Opacity = 0.9,
                Margin = new Thickness(0, 4, 0, 0)
            };
            headerStack.Children.Add(locationText);

            // Right side - Report info
            var rightPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            var printDateText = new TextBlock
            {
                Text = $"Print Date / Time: {DateTime.Now:MM/dd/yyyy h:mm tt}",
                FontSize = 12,
                Foreground = Brushes.White,
                Opacity = 0.9,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            rightPanel.Children.Add(printDateText);

            var pageInfoText = new TextBlock
            {
                Text = "Page: 1 of 1",
                FontSize = 12,
                Foreground = Brushes.White,
                Opacity = 0.9,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 2, 0, 0)
            };
            rightPanel.Children.Add(pageInfoText);

            // 31 Ringwood badge
            var ringwoodBadge = new Border
            {
                Background = Brushes.Black,
                BorderBrush = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 8, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var ringwoodText = new TextBlock
            {
                Text = "31 Ringwood",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            ringwoodBadge.Child = ringwoodText;
            rightPanel.Children.Add(ringwoodBadge);

            // Create header grid
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Grid.SetColumn(headerStack, 0);
            Grid.SetColumn(rightPanel, 1);
            headerGrid.Children.Add(headerStack);
            headerGrid.Children.Add(rightPanel);

            headerCell.Blocks.Add(new BlockUIContainer(headerGrid));
            headerRow.Cells.Add(headerCell);
            headerRowGroup.Rows.Add(headerRow);
            headerContainer.RowGroups.Add(headerRowGroup);
            document.Blocks.Add(headerContainer);

            // Batch Information Header
            var batchInfoHeader = new Table
            {
                CellSpacing = 0,
                BorderBrush = new SolidColorBrush(Color.FromRgb(255, 215, 0)), // Gold
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 0, 16)
            };
            batchInfoHeader.Columns.Add(new TableColumn());

            var batchInfoRowGroup = new TableRowGroup();
            var batchInfoRow = new TableRow
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 215, 0))
            };

            var batchInfoCell = new TableCell
            {
                BorderThickness = new Thickness(0),
                Padding = new Thickness(16, 12, 16, 12)
            };

            var batchInfoGrid = new Grid();
            batchInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            batchInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var batchInfoLeft = new TextBlock
            {
                Text = "Batch Information",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(batchInfoLeft, 0);

            var batchInfoRight = new TextBlock
            {
                Text = $"Batch Report: {batches.Count}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(batchInfoRight, 1);

            batchInfoGrid.Children.Add(batchInfoLeft);
            batchInfoGrid.Children.Add(batchInfoRight);

            batchInfoCell.Blocks.Add(new BlockUIContainer(batchInfoGrid));
            batchInfoRow.Cells.Add(batchInfoCell);
            batchInfoRowGroup.Rows.Add(batchInfoRow);
            batchInfoHeader.RowGroups.Add(batchInfoRowGroup);
            document.Blocks.Add(batchInfoHeader);

            // Create modern data table with responsive columns
            var table = new Table
            {
                CellSpacing = 0,
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)), // Light gray
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Define responsive columns that adapt to paper size
            // Using proportional widths instead of fixed pixels
            var columnWidths = new[] 
            { 
                new GridLength(0.5, GridUnitType.Star), // ID - smaller
                new GridLength(2.0, GridUnitType.Star), // Formula Name - larger
                new GridLength(0.8, GridUnitType.Star), // Formula #
                new GridLength(1.0, GridUnitType.Star), // Start Date
                new GridLength(0.8, GridUnitType.Star), // Start Time
                new GridLength(1.0, GridUnitType.Star), // End Date
                new GridLength(0.8, GridUnitType.Star), // End Time
                new GridLength(1.5, GridUnitType.Star), // Storage Tank
                new GridLength(0.8, GridUnitType.Star), // Volume
                new GridLength(0.8, GridUnitType.Star), // Temperature
                new GridLength(1.2, GridUnitType.Star)  // Status
            };
            
            foreach (var width in columnWidths)
            {
                table.Columns.Add(new TableColumn { Width = width });
            }

            // Add header row with modern styling
            var tableHeaderRowGroup = new TableRowGroup();
            var tableHeaderRow = new TableRow
            {
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)), // Light background
                FontWeight = FontWeights.SemiBold
            };

            var modernHeaders = new[]
            {
                "ID", "Formula Name", "Formula #", "Start Date", "Start Time", "End Date", "End Time",
                "Storage Tank", "Volume (Gal)", "Temp (°C)", "Status"
            };

            foreach (var header in modernHeaders)
            {
                var cell = new TableCell
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(4, 3, 4, 3), // Reduced padding
                    TextAlignment = TextAlignment.Center
                };
                cell.Blocks.Add(new Paragraph(new Run(header)) { FontWeight = FontWeights.SemiBold, FontSize = 9 }); // Smaller font
                tableHeaderRow.Cells.Add(cell);
            }

            tableHeaderRowGroup.Rows.Add(tableHeaderRow);
            table.RowGroups.Add(tableHeaderRowGroup);

            // Add data rows with alternating colors and responsive content
            var dataRowGroup = new TableRowGroup();
            bool isAlternate = false;
            foreach (var batch in batches)
            {
                var dataRow = new TableRow
                {
                    Background = isAlternate ? new SolidColorBrush(Color.FromRgb(250, 250, 250)) : Brushes.White
                };
                isAlternate = !isAlternate;
                
                // Create responsive data with shorter formats for better fit
                var data = new[]
                {
                    batch.Id.ToString(),
                    TruncateText(batch.FormulaName, 20), // Truncate long names
                    batch.FormulaNumber.ToString(),
                    batch.StartDate.ToString("MM/dd/yy"), // Shorter date format
                    batch.StartTime.ToString(@"hh\:mm"), // Remove seconds
                    batch.EndDate.ToString("MM/dd/yy"), // Shorter date format
                    batch.EndTime.ToString(@"hh\:mm"), // Remove seconds
                    TruncateText(batch.StorageTankName, 15), // Truncate long tank names
                    batch.Volume.ToString("F1"), // One decimal place
                    batch.Temperature.ToString("F0"), // No decimal places
                    TruncateText(batch.StatusText, 12) // Truncate long status
                };

                foreach (var dataItem in data)
                {
                    var cell = new TableCell
                    {
                        BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(4, 2, 4, 2) // Reduced padding for more content
                    };
                    cell.Blocks.Add(new Paragraph(new Run(dataItem)) { FontSize = 9 }); // Smaller font for data
                    dataRow.Cells.Add(cell);
                }

                dataRowGroup.Rows.Add(dataRow);
            }

            table.RowGroups.Add(dataRowGroup);
            document.Blocks.Add(table);

            return document;
        }

        /// <summary>
        /// Prints the document using the specified print dialog with responsive sizing
        /// </summary>
        /// <param name="batches">Batch data</param>
        /// <param name="printDialog">Print dialog with settings</param>
        private void PrintDocument(List<BatchQuery> batches, PrintDialog printDialog)
        {
            try
            {
                var document = CreatePrintDocument(batches);
                
                // Set responsive print settings based on paper size
                var printableWidth = printDialog.PrintableAreaWidth;
                var printableHeight = printDialog.PrintableAreaHeight;
                
                // Calculate appropriate margins based on paper size
                var marginSize = Math.Min(printableWidth, printableHeight) * 0.05; // 5% of smallest dimension
                marginSize = Math.Max(20, Math.Min(50, marginSize)); // Between 20 and 50 points
                
                document.PageHeight = printableHeight;
                document.PageWidth = printableWidth;
                document.PagePadding = new Thickness(marginSize);
                
                // Set font size based on paper size for better readability
                var baseFontSize = Math.Max(8, Math.Min(12, printableWidth / 100));
                document.FontSize = baseFontSize;

                // Print the document
                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Batch Report");
            }
            catch (Exception ex)
            {
                LogError($"Error printing document: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Creates a proper print preview window with DocumentViewer
        /// </summary>
        /// <param name="batches">Batch data to preview</param>
        /// <returns>Print preview window</returns>
        private Window CreatePrintPreviewWindow(List<BatchQuery> batches)
        {
            try
            {
                var document = CreatePrintDocument(batches);
                
                // Create a proper print preview window
                var previewWindow = new Window
                {
                    Title = "Print Preview - Batch Report",
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Background = SystemColors.WindowBrush
                };

                // Create DocumentViewer for proper print preview
                var documentViewer = new DocumentViewer
                {
                    Document = document,
                    Zoom = 100,
                    Background = SystemColors.WindowBrush
                };

                // Create toolbar with print and close buttons
                var toolbar = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(10)
                };

                var printButton = new Button
                {
                    Content = "Print",
                    Width = 80,
                    Height = 30,
                    Margin = new Thickness(5),
                    Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                var closeButton = new Button
                {
                    Content = "Close",
                    Width = 80,
                    Height = 30,
                    Margin = new Thickness(5),
                    Background = new SolidColorBrush(Color.FromRgb(158, 158, 158)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                // Add button event handlers
                printButton.Click += (s, e) =>
                {
                    try
                    {
                        var printDialog = new PrintDialog();
                        if (printDialog.ShowDialog() == true)
                        {
                            PrintDocument(batches, printDialog);
                            LogInfo("Print job sent to printer successfully");
                            MessageBox.Show("Print job sent successfully!", "Print Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error printing: {ex.Message}", ex);
                        MessageBox.Show($"Print error: {ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                closeButton.Click += (s, e) => previewWindow.Close();

                toolbar.Children.Add(printButton);
                toolbar.Children.Add(closeButton);

                // Create main layout
                var mainGrid = new Grid();
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                Grid.SetRow(toolbar, 0);
                Grid.SetRow(documentViewer, 1);

                mainGrid.Children.Add(toolbar);
                mainGrid.Children.Add(documentViewer);

                previewWindow.Content = mainGrid;

                return previewWindow;
            }
            catch (Exception ex)
            {
                LogError($"Error creating print preview: {ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// Truncates text to specified length for better print layout
        /// </summary>
        /// <param name="text">Text to truncate</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Truncated text with ellipsis if needed</returns>
        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
            
            return text.Substring(0, maxLength - 3) + "...";
        }

        /// <summary>
        /// Simple test method to verify print dialog functionality
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        public async Task TestPrintDialogAsync()
        {
            try
            {
                LogInfo("Testing print dialog functionality");

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Create a simple print dialog test
                        var printDialog = new PrintDialog();
                        
                        // Show print dialog
                        var result = printDialog.ShowDialog();
                        
                        if (result == true)
                        {
                            LogInfo("Print dialog test: User clicked OK");
                            MessageBox.Show("Print dialog test successful! User clicked OK.", "Print Test", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            LogInfo("Print dialog test: User clicked Cancel");
                            MessageBox.Show("Print dialog test: User clicked Cancel.", "Print Test", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    });
                });

                LogInfo("Print dialog test completed");
            }
            catch (Exception ex)
            {
                LogError($"Error testing print dialog: {ex.Message}", ex);
                MessageBox.Show($"Print dialog test failed: {ex.Message}", "Print Test Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] ExportService: {message}");
        }

        /// <summary>
        /// Logs an error message with exception details
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception that occurred</param>
        private void LogError(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] ExportService: {message}");
            if (ex != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Exception: {ex}");
            }
        }

        #endregion
    }
}