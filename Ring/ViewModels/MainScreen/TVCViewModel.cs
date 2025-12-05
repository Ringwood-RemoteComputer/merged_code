using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace Ring.ViewModels.MainScreen
{
    // ViewModel for the TVC view - displays TVC tank model and temperature control status
    public class TVCViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        // TVC Properties
        private decimal _actualTemperature;
        private decimal _presetTemperature;
        private string _heatersStatus;
        private bool _isHeating;

        // Storage Tank 1 Properties
        private decimal _storageTank1ActualTemperature;
        private decimal _storageTank1PresetTemperature;
        private bool _storageTank1TemperatureControlMode;

        // Storage Tank 2 Properties
        private decimal _storageTank2ActualTemperature;
        private decimal _storageTank2PresetTemperature;
        private bool _storageTank2TemperatureControlMode;

        // Storage Tank 3 Properties
        private decimal _storageTank3ActualTemperature;
        private decimal _storageTank3PresetTemperature;
        private bool _storageTank3TemperatureControlMode;

        // Reference to StorageTankGroupViewModel to pull storage tank data
        private StorageTankGroupViewModel _storageTankGroupViewModel;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public TVCViewModel()
        {
            // Get reference to StorageTankGroupViewModel instance
            _storageTankGroupViewModel = new StorageTankGroupViewModel();
            
            // Subscribe to property changes to update when storage tank data changes
            _storageTankGroupViewModel.PropertyChanged += StorageTankGroupViewModel_PropertyChanged;
            
            // Initialize with default values
            InitializeDefaultValues();
            
            // Pull initial values from StorageTankGroupViewModel
            UpdateStorageTankData();
        }

        private void StorageTankGroupViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update storage tank data when StorageTankGroupViewModel properties change
            if (e.PropertyName == nameof(StorageTankGroupViewModel.ActualTemperature1) ||
                e.PropertyName == nameof(StorageTankGroupViewModel.PresetTemperature1) ||
                e.PropertyName == nameof(StorageTankGroupViewModel.ActualTemperature2) ||
                e.PropertyName == nameof(StorageTankGroupViewModel.PresetTemperature2) ||
                e.PropertyName == nameof(StorageTankGroupViewModel.ActualTemperature3) ||
                e.PropertyName == nameof(StorageTankGroupViewModel.PresetTemperature3) ||
                e.PropertyName == nameof(StorageTankGroupViewModel.OverallActualTemperature) ||
                e.PropertyName == nameof(StorageTankGroupViewModel.OverallPresetTemperature) ||
                e.PropertyName == nameof(StorageTankGroupViewModel.HeatersStatus))
            {
                UpdateStorageTankData();
            }
        }

        private void UpdateStorageTankData()
        {
            // Pull storage tank temperatures from StorageTankGroupViewModel
            StorageTank1ActualTemperature = _storageTankGroupViewModel.ActualTemperature1;
            StorageTank1PresetTemperature = _storageTankGroupViewModel.PresetTemperature1;
            StorageTank2ActualTemperature = _storageTankGroupViewModel.ActualTemperature2;
            StorageTank2PresetTemperature = _storageTankGroupViewModel.PresetTemperature2;
            StorageTank3ActualTemperature = _storageTankGroupViewModel.ActualTemperature3;
            StorageTank3PresetTemperature = _storageTankGroupViewModel.PresetTemperature3;
            
            // Pull TVC temperatures from StorageTankGroupViewModel
            ActualTemperature = _storageTankGroupViewModel.OverallActualTemperature;
            PresetTemperature = _storageTankGroupViewModel.OverallPresetTemperature;
            
            // Pull heaters status
            HeatersStatus = _storageTankGroupViewModel.HeatersStatus;
            IsHeating = HeatersStatus == "On" || HeatersStatus == "Active";
        }

        #endregion

        #region TVC Properties

        // Actual temperature for TVC
        public decimal ActualTemperature
        {
            get => _actualTemperature;
            set
            {
                if (_actualTemperature != value)
                {
                    _actualTemperature = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TemperatureDisplay));
                }
            }
        }

        // Preset temperature for TVC
        public decimal PresetTemperature
        {
            get => _presetTemperature;
            set
            {
                if (_presetTemperature != value)
                {
                    _presetTemperature = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TemperatureDisplay));
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
                    OnPropertyChanged(nameof(HeatersDisplay));
                    OnPropertyChanged(nameof(TVCImage));
                    // Update IsHeating based on status
                    IsHeating = value == "On" || value == "Active";
                }
            }
        }

        // Heating state for TVC (TODO: Connect to PLC data later)
        public bool IsHeating
        {
            get => _isHeating;
            set
            {
                if (_isHeating != value)
                {
                    _isHeating = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TVCImage));
                }
            }
        }

        // TVC Image property - determines which image to show based on state
        public BitmapImage TVCImage
        {
            get
            {
                string imageName = GetTVCImageName();
                return Ring.Services.Images.ImageLoader.LoadImage($"TVC Tank/{imageName}");
            }
        }

        // Combined temperature display for TVC
        public string TemperatureDisplay => $"{ActualTemperature:F1}/{PresetTemperature:F1}°F";

        // Heaters display (On/Off)
        public string HeatersDisplay => HeatersStatus == "On" || HeatersStatus == "Active" ? "On" : "Off";

        #endregion

        #region Storage Tank 1 Properties

        // Actual temperature for Storage Tank 1
        public decimal StorageTank1ActualTemperature
        {
            get => _storageTank1ActualTemperature;
            set
            {
                if (_storageTank1ActualTemperature != value)
                {
                    _storageTank1ActualTemperature = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank1TemperatureDisplay));
                }
            }
        }

        // Preset temperature for Storage Tank 1
        public decimal StorageTank1PresetTemperature
        {
            get => _storageTank1PresetTemperature;
            set
            {
                if (_storageTank1PresetTemperature != value)
                {
                    _storageTank1PresetTemperature = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank1TemperatureDisplay));
                }
            }
        }

        // Temperature Control Mode for Storage Tank 1 (TODO: Connect to PLC data later)
        public bool StorageTank1TemperatureControlMode
        {
            get => _storageTank1TemperatureControlMode;
            set
            {
                if (_storageTank1TemperatureControlMode != value)
                {
                    _storageTank1TemperatureControlMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank1ControlModeDisplay));
                    OnPropertyChanged(nameof(TVCImage));
                }
            }
        }

        // Combined temperature display for Storage Tank 1
        public string StorageTank1TemperatureDisplay => $"{StorageTank1ActualTemperature:F1}/{StorageTank1PresetTemperature:F1}°F";

        // Control Mode display for Storage Tank 1 (On/Off)
        public string StorageTank1ControlModeDisplay => StorageTank1TemperatureControlMode ? "On" : "Off";

        #endregion

        #region Storage Tank 2 Properties

        // Actual temperature for Storage Tank 2
        public decimal StorageTank2ActualTemperature
        {
            get => _storageTank2ActualTemperature;
            set
            {
                if (_storageTank2ActualTemperature != value)
                {
                    _storageTank2ActualTemperature = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank2TemperatureDisplay));
                }
            }
        }

        // Preset temperature for Storage Tank 2
        public decimal StorageTank2PresetTemperature
        {
            get => _storageTank2PresetTemperature;
            set
            {
                if (_storageTank2PresetTemperature != value)
                {
                    _storageTank2PresetTemperature = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank2TemperatureDisplay));
                }
            }
        }

        // Temperature Control Mode for Storage Tank 2 (TODO: Connect to PLC data later)
        public bool StorageTank2TemperatureControlMode
        {
            get => _storageTank2TemperatureControlMode;
            set
            {
                if (_storageTank2TemperatureControlMode != value)
                {
                    _storageTank2TemperatureControlMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank2ControlModeDisplay));
                    OnPropertyChanged(nameof(TVCImage));
                }
            }
        }

        // Combined temperature display for Storage Tank 2
        public string StorageTank2TemperatureDisplay => $"{StorageTank2ActualTemperature:F1}/{StorageTank2PresetTemperature:F1}°F";

        // Control Mode display for Storage Tank 2 (On/Off)
        public string StorageTank2ControlModeDisplay => StorageTank2TemperatureControlMode ? "On" : "Off";

        #endregion

        #region Storage Tank 3 Properties

        // Actual temperature for Storage Tank 3
        public decimal StorageTank3ActualTemperature
        {
            get => _storageTank3ActualTemperature;
            set
            {
                if (_storageTank3ActualTemperature != value)
                {
                    _storageTank3ActualTemperature = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank3TemperatureDisplay));
                }
            }
        }

        // Preset temperature for Storage Tank 3
        public decimal StorageTank3PresetTemperature
        {
            get => _storageTank3PresetTemperature;
            set
            {
                if (_storageTank3PresetTemperature != value)
                {
                    _storageTank3PresetTemperature = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank3TemperatureDisplay));
                }
            }
        }

        // Temperature Control Mode for Storage Tank 3 (TODO: Connect to PLC data later)
        public bool StorageTank3TemperatureControlMode
        {
            get => _storageTank3TemperatureControlMode;
            set
            {
                if (_storageTank3TemperatureControlMode != value)
                {
                    _storageTank3TemperatureControlMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StorageTank3ControlModeDisplay));
                    OnPropertyChanged(nameof(TVCImage));
                }
            }
        }

        // Combined temperature display for Storage Tank 3
        public string StorageTank3TemperatureDisplay => $"{StorageTank3ActualTemperature:F1}/{StorageTank3PresetTemperature:F1}°F";

        // Control Mode display for Storage Tank 3 (On/Off)
        public string StorageTank3ControlModeDisplay => StorageTank3TemperatureControlMode ? "On" : "Off";

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines which TVC image to display based on which storage tanks have Temperature Control Mode On and heating state
        /// All possible combinations are accounted for in the available images
        /// </summary>
        private string GetTVCImageName()
        {
            // Determine which tanks have Temperature Control Mode On
            bool tank1 = StorageTank1TemperatureControlMode;
            bool tank2 = StorageTank2TemperatureControlMode;
            bool tank3 = StorageTank3TemperatureControlMode;
            bool heat = IsHeating;

            // All three tanks + heat
            if (tank1 && tank2 && tank3 && heat)
            {
                return "TVC_123_heat.png";
            }
            // All three tanks, no heat
            if (tank1 && tank2 && tank3)
            {
                return "TVC_123.png";
            }
            // Tanks 1 and 2 + heat
            if (tank1 && tank2 && heat)
            {
                return "TVC_12_heat.png";
            }
            // Tanks 1 and 2, no heat
            if (tank1 && tank2)
            {
                return "TVC_12.png";
            }
            // Tanks 1 and 3 + heat
            if (tank1 && tank3 && heat)
            {
                return "TVC_13_heat.png";
            }
            // Tanks 1 and 3, no heat
            if (tank1 && tank3)
            {
                return "TVC_13.png";
            }
            // Tanks 2 and 3 + heat
            if (tank2 && tank3 && heat)
            {
                return "TVC_23_heat.png";
            }
            // Tanks 2 and 3, no heat
            if (tank2 && tank3)
            {
                return "TVC_23.png";
            }
            // Tank 1 only + heat
            if (tank1 && heat)
            {
                return "TVC_1_heat.png";
            }
            // Tank 1 only, no heat
            if (tank1)
            {
                return "TVC_1.png";
            }
            // Tank 2 only + heat
            if (tank2 && heat)
            {
                return "TVC_2_heat.png";
            }
            // Tank 2 only, no heat
            if (tank2)
            {
                return "TVC_2.png";
            }
            // Tank 3 only + heat
            if (tank3 && heat)
            {
                return "TVC_3_heat.png";
            }
            // Tank 3 only, no heat
            if (tank3)
            {
                return "TVC_3.png";
            }
            // Heat only, no tanks
            if (heat)
            {
                return "TVC_heat.png";
            }
            // Base image (no tanks, no heat)
            return "TVC.png";
        }

        // Initialize with default values
        private void InitializeDefaultValues()
        {
            // TVC defaults
            ActualTemperature = 97.1m;   // °F
            PresetTemperature = 100.0m;  // °F
            HeatersStatus = "Off";
            IsHeating = false;

            // Storage Tank Temperature Control Mode defaults (TODO: Connect to PLC data later)
            StorageTank1TemperatureControlMode = false;
            StorageTank2TemperatureControlMode = false;
            StorageTank3TemperatureControlMode = false;
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

