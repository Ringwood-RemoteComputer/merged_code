using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace Ring.ViewModels.MainScreen
{
    // ViewModel for the Storage Tank Group view - displays read-only data for two storage tanks
    // and global system status information
    public class StorageTankGroupViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        // Tank 1 Properties
        private decimal _actualTemperature1;
        private decimal _presetTemperature1;
        private decimal _actualLevel1;
        private decimal _requestLevel1;
        private string _formula1;
        private decimal _fillPercentage1;
        private string _levelStatus1;
        
        // Tank 1 State Properties (TODO: Connect to PLC data later)
        private bool _isAgitating1;
        private bool _isFilling1;
        private bool _isTransferring1;

        // Tank 2 Properties
        private decimal _actualTemperature2;
        private decimal _presetTemperature2;
        private decimal _actualLevel2;
        private decimal _requestLevel2;
        private string _formula2;
        private decimal _fillPercentage2;
        private string _levelStatus2;
        
        // Tank 2 State Properties (TODO: Connect to PLC data later)
        private bool _isAgitating2;
        private bool _isFilling2;
        private bool _isTransferring2;

        // Global System Properties
        private decimal _overallActualTemperature;
        private decimal _overallPresetTemperature;
        private string _heatersStatus;
        private string _overallLevelStatus;
        private string _plcStatusText;
        private string _plcStatusBrush;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public StorageTankGroupViewModel()
        {
            // Initialize with default values matching the industrial application image
            InitializeDefaultValues();
        }

        #endregion

        #region Tank 1 Properties

        // Actual temperature for Storage Tank 1
        public decimal ActualTemperature1
        {
            get => _actualTemperature1;
            set
            {
                if (_actualTemperature1 != value)
                {
                    _actualTemperature1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Preset temperature for Storage Tank 1
        public decimal PresetTemperature1
        {
            get => _presetTemperature1;
            set
            {
                if (_presetTemperature1 != value)
                {
                    _presetTemperature1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Actual level for Storage Tank 1
        public decimal ActualLevel1
        {
            get => _actualLevel1;
            set
            {
                if (_actualLevel1 != value)
                {
                    _actualLevel1 = value;
                    OnPropertyChanged();
                    // Recalculate fill percentage when level changes
                    CalculateFillPercentage1();
                }
            }
        }

        // Request level for Storage Tank 1
        public decimal RequestLevel1
        {
            get => _requestLevel1;
            set
            {
                if (_requestLevel1 != value)
                {
                    _requestLevel1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Formula name for Storage Tank 1
        public string Formula1
        {
            get => _formula1;
            set
            {
                if (_formula1 != value)
                {
                    _formula1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Fill percentage for Storage Tank 1 (0-100)
        public decimal FillPercentage1
        {
            get => _fillPercentage1;
            set
            {
                if (_fillPercentage1 != value)
                {
                    _fillPercentage1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Level status for Storage Tank 1
        public string LevelStatus1
        {
            get => _levelStatus1;
            set
            {
                if (_levelStatus1 != value)
                {
                    _levelStatus1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Agitating state for Storage Tank 1 (TODO: Connect to PLC data later)
        public bool IsAgitating1
        {
            get => _isAgitating1;
            set
            {
                if (_isAgitating1 != value)
                {
                    _isAgitating1 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTankImage1));
                }
            }
        }

        // Filling state for Storage Tank 1 (TODO: Connect to PLC data later)
        public bool IsFilling1
        {
            get => _isFilling1;
            set
            {
                if (_isFilling1 != value)
                {
                    _isFilling1 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTankImage1));
                }
            }
        }

        // Transferring state for Storage Tank 1 (TODO: Connect to PLC data later)
        public bool IsTransferring1
        {
            get => _isTransferring1;
            set
            {
                if (_isTransferring1 != value)
                {
                    _isTransferring1 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTankImage1));
                }
            }
        }

        // Storage Tank 1 Image property - determines which image to show based on state
        public BitmapImage StorageTankImage1
        {
            get
            {
                string imageName = GetStorageTankImageName1();
                return Ring.Services.Images.ImageLoader.LoadImage($"Storage Tank/{imageName}");
            }
        }

        #endregion

        #region Tank 2 Properties

        // Actual temperature for Storage Tank 2
        public decimal ActualTemperature2
        {
            get => _actualTemperature2;
            set
            {
                if (_actualTemperature2 != value)
                {
                    _actualTemperature2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Preset temperature for Storage Tank 2
        public decimal PresetTemperature2
        {
            get => _presetTemperature2;
            set
            {
                if (_presetTemperature2 != value)
                {
                    _presetTemperature2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Actual level for Storage Tank 2
        public decimal ActualLevel2
        {
            get => _actualLevel2;
            set
            {
                if (_actualLevel2 != value)
                {
                    _actualLevel2 = value;
                    OnPropertyChanged();
                    // Recalculate fill percentage when level changes
                    CalculateFillPercentage2();
                }
            }
        }

        // Request level for Storage Tank 2
        public decimal RequestLevel2
        {
            get => _requestLevel2;
            set
            {
                if (_requestLevel2 != value)
                {
                    _requestLevel2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Formula name for Storage Tank 2
        public string Formula2
        {
            get => _formula2;
            set
            {
                if (_formula2 != value)
                {
                    _formula2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Fill percentage for Storage Tank 2 (0-100)
        public decimal FillPercentage2
        {
            get => _fillPercentage2;
            set
            {
                if (_fillPercentage2 != value)
                {
                    _fillPercentage2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Level status for Storage Tank 2
        public string LevelStatus2
        {
            get => _levelStatus2;
            set
            {
                if (_levelStatus2 != value)
                {
                    _levelStatus2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Agitating state for Storage Tank 2 (TODO: Connect to PLC data later)
        public bool IsAgitating2
        {
            get => _isAgitating2;
            set
            {
                if (_isAgitating2 != value)
                {
                    _isAgitating2 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTankImage2));
                }
            }
        }

        // Filling state for Storage Tank 2 (TODO: Connect to PLC data later)
        public bool IsFilling2
        {
            get => _isFilling2;
            set
            {
                if (_isFilling2 != value)
                {
                    _isFilling2 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTankImage2));
                }
            }
        }

        // Transferring state for Storage Tank 2 (TODO: Connect to PLC data later)
        public bool IsTransferring2
        {
            get => _isTransferring2;
            set
            {
                if (_isTransferring2 != value)
                {
                    _isTransferring2 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTankImage2));
                }
            }
        }

        // Storage Tank 2 Image property - determines which image to show based on state
        public BitmapImage StorageTankImage2
        {
            get
            {
                string imageName = GetStorageTankImageName2();
                return Ring.Services.Images.ImageLoader.LoadImage($"Storage Tank/{imageName}");
            }
        }

        #endregion

        #region Global System Properties

        // Overall actual temperature for the system
        public decimal OverallActualTemperature
        {
            get => _overallActualTemperature;
            set
            {
                if (_overallActualTemperature != value)
                {
                    _overallActualTemperature = value;
                    OnPropertyChanged();
                }
            }
        }

        // Overall preset temperature for the system
        public decimal OverallPresetTemperature
        {
            get => _overallPresetTemperature;
            set
            {
                if (_overallPresetTemperature != value)
                {
                    _overallPresetTemperature = value;
                    OnPropertyChanged();
                }
            }
        }

        // Status of the heating system
        public string HeatersStatus
        {
            get => _heatersStatus;
            set
            {
                if (_heatersStatus != value)
                {
                    _heatersStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        // Overall level status for the system
        public string OverallLevelStatus
        {
            get => _overallLevelStatus;
            set
            {
                if (_overallLevelStatus != value)
                {
                    _overallLevelStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        // PLC status text for the footer
        public string PlcStatusText
        {
            get => _plcStatusText;
            set
            {
                if (_plcStatusText != value)
                {
                    _plcStatusText = value;
                    OnPropertyChanged();
                }
            }
        }

        // PLC status brush color for the footer indicator
        public string PlcStatusBrush
        {
            get => _plcStatusBrush;
            set
            {
                if (_plcStatusBrush != value)
                {
                    _plcStatusBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Helper Methods

        // Calculate fill percentage for Tank 1 based on actual level vs request level
        private void CalculateFillPercentage1()
        {
            if (RequestLevel1 > 0)
            {
                FillPercentage1 = Math.Min(100, Math.Max(0, (ActualLevel1 / RequestLevel1) * 100));
            }
            else
            {
                FillPercentage1 = 0;
            }
        }

        // Calculate fill percentage for Tank 2 based on actual level vs request level
        private void CalculateFillPercentage2()
        {
            if (RequestLevel2 > 0)
            {
                FillPercentage2 = Math.Min(100, Math.Max(0, (ActualLevel2 / RequestLevel2) * 100));
            }
            else
            {
                FillPercentage2 = 0;
            }
        }

        // Initialize with default values for demonstration purposes
        private void InitializeDefaultValues()
        {
            // Tank 1 defaults - realistic industrial values
            ActualTemperature1 = 99.2m;   // °F
            PresetTemperature1 = 100.0m;  // °F
            ActualLevel1 = 425.0m;        // gallons
            RequestLevel1 = 500.0m;       // gallons
            Formula1 = "Formula 1";
            LevelStatus1 = "Normal";

            // Tank 2 defaults - realistic industrial values
            ActualTemperature2 = 95.0m;   // °F
            PresetTemperature2 = 100.0m;  // °F
            ActualLevel2 = 80.0m;         // gallons (20% of 400)
            RequestLevel2 = 400.0m;       // gallons
            Formula2 = "Formula 2";
            LevelStatus2 = "Normal";

            // TVC system defaults - realistic industrial values
            OverallActualTemperature = 97.1m;   // °F (average of tanks)
            OverallPresetTemperature = 100.0m;  // °F
            HeatersStatus = "Active";
            OverallLevelStatus = "Normal";

            // PLC status defaults
            PlcStatusText = "PLC Connected";
            PlcStatusBrush = "#28A745";  // Green color for connected status

            // Tank state defaults (TODO: Connect to PLC data later)
            IsAgitating1 = false;
            IsFilling1 = false;
            IsTransferring1 = false;
            IsAgitating2 = false;
            IsFilling2 = false;
            IsTransferring2 = false;

            // Calculate initial fill percentages
            CalculateFillPercentage1();
            CalculateFillPercentage2();
        }

        /// <summary>
        /// Determines which Storage Tank 1 image to display based on state (agitating, filling, transferring)
        /// Priority: agitator_filling_transferring > filling_transferring > agitator_filling > agitator_transferring > filling > transferring > agitator > base
        /// </summary>
        private string GetStorageTankImageName1()
        {
            // All three states active
            if (IsAgitating1 && IsFilling1 && IsTransferring1)
            {
                return "storagetank_agitator_filling_transferring.png";
            }
            // Filling and transferring (no agitator)
            if (IsFilling1 && IsTransferring1)
            {
                return "storagetank_filling_transferring.png";
            }
            // Agitating and filling (no transferring)
            if (IsAgitating1 && IsFilling1)
            {
                return "storagetank_agitator_filling.png";
            }
            // Agitating and transferring (no filling)
            if (IsAgitating1 && IsTransferring1)
            {
                return "storagetank_agitator_transferring.png";
            }
            // Only filling
            if (IsFilling1)
            {
                return "storagetank_filling.png";
            }
            // Only transferring
            if (IsTransferring1)
            {
                return "storagetank_transferring.png";
            }
            // Only agitating
            if (IsAgitating1)
            {
                return "storagetank_agitator.png";
            }
            // Base image (no states active)
            return "storagetank.png";
        }

        /// <summary>
        /// Determines which Storage Tank 2 image to display based on state (agitating, filling, transferring)
        /// Priority: agitator_filling_transferring > filling_transferring > agitator_filling > agitator_transferring > filling > transferring > agitator > base
        /// </summary>
        private string GetStorageTankImageName2()
        {
            // All three states active
            if (IsAgitating2 && IsFilling2 && IsTransferring2)
            {
                return "storagetank_agitator_filling_transferring.png";
            }
            // Filling and transferring (no agitator)
            if (IsFilling2 && IsTransferring2)
            {
                return "storagetank_filling_transferring.png";
            }
            // Agitating and filling (no transferring)
            if (IsAgitating2 && IsFilling2)
            {
                return "storagetank_agitator_filling.png";
            }
            // Agitating and transferring (no filling)
            if (IsAgitating2 && IsTransferring2)
            {
                return "storagetank_agitator_transferring.png";
            }
            // Only filling
            if (IsFilling2)
            {
                return "storagetank_filling.png";
            }
            // Only transferring
            if (IsTransferring2)
            {
                return "storagetank_transferring.png";
            }
            // Only agitating
            if (IsAgitating2)
            {
                return "storagetank_agitator.png";
            }
            // Base image (no states active)
            return "storagetank.png";
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}