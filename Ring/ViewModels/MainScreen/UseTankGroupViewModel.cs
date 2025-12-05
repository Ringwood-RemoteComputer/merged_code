using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace Ring.ViewModels.MainScreen
{
    // ViewModel for the Use Tank Group view - displays read-only data for three use tanks
    public class UseTankGroupViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        // Use Tank 1 Properties
        private string _resinTypeSelected1;
        private string _tank1;
        private string _formula1;
        private string _status1;
        private decimal _temperature1;
        private decimal _level1;
        private bool _isAgitating1;
        private bool _isHeating1;
        private bool _isAddingAdditive1;
        private bool _isAdhesiveSupply1;
        private bool _isAdhesiveTransfer1;

        // Use Tank 2 Properties
        private string _resinTypeSelected2;
        private string _tank2;
        private string _formula2;
        private string _status2;
        private decimal _temperature2;
        private decimal _level2;
        private bool _isAgitating2;
        private bool _isHeating2;
        private bool _isAddingAdditive2;
        private bool _isAdhesiveSupply2;
        private bool _isAdhesiveTransfer2;

        // Use Tank 3 Properties
        private string _resinTypeSelected3;
        private string _tank3;
        private string _formula3;
        private string _status3;
        private decimal _temperature3;
        private decimal _level3;
        private bool _isAgitating3;
        private bool _isHeating3;
        private bool _isAddingAdditive3;
        private bool _isAdhesiveSupply3;
        private bool _isAdhesiveTransfer3;

        // PLC Status Properties
        private string _plcStatusText;
        private string _plcStatusBrush;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public UseTankGroupViewModel()
        {
            // Initialize with default values
            InitializeDefaultValues();
        }

        #endregion

        #region Use Tank 1 Properties

        // Resin type selected for Use Tank 1
        public string ResinTypeSelected1
        {
            get => _resinTypeSelected1;
            set
            {
                if (_resinTypeSelected1 != value)
                {
                    _resinTypeSelected1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Tank identifier for Use Tank 1
        public string Tank1
        {
            get => _tank1;
            set
            {
                if (_tank1 != value)
                {
                    _tank1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Formula for Use Tank 1
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

        // Status for Use Tank 1
        public string Status1
        {
            get => _status1;
            set
            {
                if (_status1 != value)
                {
                    _status1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Temperature for Use Tank 1 (°F)
        public decimal Temperature1
        {
            get => _temperature1;
            set
            {
                if (_temperature1 != value)
                {
                    _temperature1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Level for Use Tank 1 (0-100%)
        public decimal Level1
        {
            get => _level1;
            set
            {
                if (_level1 != value)
                {
                    _level1 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Agitating state for Use Tank 1 (TODO: Connect to PLC data later)
        public bool IsAgitating1
        {
            get => _isAgitating1;
            set
            {
                if (_isAgitating1 != value)
                {
                    _isAgitating1 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage1));
                }
            }
        }

        // Heating state for Use Tank 1 (TODO: Connect to PLC data later)
        public bool IsHeating1
        {
            get => _isHeating1;
            set
            {
                if (_isHeating1 != value)
                {
                    _isHeating1 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage1));
                }
            }
        }

        // Adding additive state for Use Tank 1 (TODO: Connect to PLC data later)
        public bool IsAddingAdditive1
        {
            get => _isAddingAdditive1;
            set
            {
                if (_isAddingAdditive1 != value)
                {
                    _isAddingAdditive1 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage1));
                }
            }
        }

        // Adhesive supply state for Use Tank 1 (TODO: Connect to PLC data later)
        public bool IsAdhesiveSupply1
        {
            get => _isAdhesiveSupply1;
            set
            {
                if (_isAdhesiveSupply1 != value)
                {
                    _isAdhesiveSupply1 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage1));
                }
            }
        }

        // Adhesive transfer state for Use Tank 1 (TODO: Connect to PLC data later)
        public bool IsAdhesiveTransfer1
        {
            get => _isAdhesiveTransfer1;
            set
            {
                if (_isAdhesiveTransfer1 != value)
                {
                    _isAdhesiveTransfer1 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage1));
                }
            }
        }

        // Use Tank 1 Image property - determines which image to show based on state
        public BitmapImage UseTankImage1
        {
            get
            {
                string imageName = GetUseTankImageName1();
                return Ring.Services.Images.ImageLoader.LoadImage($"Use Tank/{imageName}");
            }
        }

        // Combined temperature display for Use Tank 1
        public string TemperatureDisplay1 => $"{Temperature1:F1}°F";

        #endregion

        #region Use Tank 2 Properties

        // Resin type selected for Use Tank 2
        public string ResinTypeSelected2
        {
            get => _resinTypeSelected2;
            set
            {
                if (_resinTypeSelected2 != value)
                {
                    _resinTypeSelected2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Tank identifier for Use Tank 2
        public string Tank2
        {
            get => _tank2;
            set
            {
                if (_tank2 != value)
                {
                    _tank2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Formula for Use Tank 2
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

        // Status for Use Tank 2
        public string Status2
        {
            get => _status2;
            set
            {
                if (_status2 != value)
                {
                    _status2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Temperature for Use Tank 2 (°F)
        public decimal Temperature2
        {
            get => _temperature2;
            set
            {
                if (_temperature2 != value)
                {
                    _temperature2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Level for Use Tank 2 (0-100%)
        public decimal Level2
        {
            get => _level2;
            set
            {
                if (_level2 != value)
                {
                    _level2 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Agitating state for Use Tank 2 (TODO: Connect to PLC data later)
        public bool IsAgitating2
        {
            get => _isAgitating2;
            set
            {
                if (_isAgitating2 != value)
                {
                    _isAgitating2 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage2));
                }
            }
        }

        // Heating state for Use Tank 2 (TODO: Connect to PLC data later)
        public bool IsHeating2
        {
            get => _isHeating2;
            set
            {
                if (_isHeating2 != value)
                {
                    _isHeating2 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage2));
                }
            }
        }

        // Adding additive state for Use Tank 2 (TODO: Connect to PLC data later)
        public bool IsAddingAdditive2
        {
            get => _isAddingAdditive2;
            set
            {
                if (_isAddingAdditive2 != value)
                {
                    _isAddingAdditive2 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage2));
                }
            }
        }

        // Adhesive supply state for Use Tank 2 (TODO: Connect to PLC data later)
        public bool IsAdhesiveSupply2
        {
            get => _isAdhesiveSupply2;
            set
            {
                if (_isAdhesiveSupply2 != value)
                {
                    _isAdhesiveSupply2 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage2));
                }
            }
        }

        // Adhesive transfer state for Use Tank 2 (TODO: Connect to PLC data later)
        public bool IsAdhesiveTransfer2
        {
            get => _isAdhesiveTransfer2;
            set
            {
                if (_isAdhesiveTransfer2 != value)
                {
                    _isAdhesiveTransfer2 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage2));
                }
            }
        }

        // Use Tank 2 Image property - determines which image to show based on state
        public BitmapImage UseTankImage2
        {
            get
            {
                string imageName = GetUseTankImageName2();
                return Ring.Services.Images.ImageLoader.LoadImage($"Use Tank/{imageName}");
            }
        }

        // Combined temperature display for Use Tank 2
        public string TemperatureDisplay2 => $"{Temperature2:F1}°F";

        #endregion

        #region Use Tank 3 Properties

        // Resin type selected for Use Tank 3
        public string ResinTypeSelected3
        {
            get => _resinTypeSelected3;
            set
            {
                if (_resinTypeSelected3 != value)
                {
                    _resinTypeSelected3 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Tank identifier for Use Tank 3
        public string Tank3
        {
            get => _tank3;
            set
            {
                if (_tank3 != value)
                {
                    _tank3 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Formula for Use Tank 3
        public string Formula3
        {
            get => _formula3;
            set
            {
                if (_formula3 != value)
                {
                    _formula3 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Status for Use Tank 3
        public string Status3
        {
            get => _status3;
            set
            {
                if (_status3 != value)
                {
                    _status3 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Temperature for Use Tank 3 (°F)
        public decimal Temperature3
        {
            get => _temperature3;
            set
            {
                if (_temperature3 != value)
                {
                    _temperature3 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Level for Use Tank 3 (0-100%)
        public decimal Level3
        {
            get => _level3;
            set
            {
                if (_level3 != value)
                {
                    _level3 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Agitating state for Use Tank 3 (TODO: Connect to PLC data later)
        public bool IsAgitating3
        {
            get => _isAgitating3;
            set
            {
                if (_isAgitating3 != value)
                {
                    _isAgitating3 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage3));
                }
            }
        }

        // Heating state for Use Tank 3 (TODO: Connect to PLC data later)
        public bool IsHeating3
        {
            get => _isHeating3;
            set
            {
                if (_isHeating3 != value)
                {
                    _isHeating3 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage3));
                }
            }
        }

        // Adding additive state for Use Tank 3 (TODO: Connect to PLC data later)
        public bool IsAddingAdditive3
        {
            get => _isAddingAdditive3;
            set
            {
                if (_isAddingAdditive3 != value)
                {
                    _isAddingAdditive3 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage3));
                }
            }
        }

        // Adhesive supply state for Use Tank 3 (TODO: Connect to PLC data later)
        public bool IsAdhesiveSupply3
        {
            get => _isAdhesiveSupply3;
            set
            {
                if (_isAdhesiveSupply3 != value)
                {
                    _isAdhesiveSupply3 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage3));
                }
            }
        }

        // Adhesive transfer state for Use Tank 3 (TODO: Connect to PLC data later)
        public bool IsAdhesiveTransfer3
        {
            get => _isAdhesiveTransfer3;
            set
            {
                if (_isAdhesiveTransfer3 != value)
                {
                    _isAdhesiveTransfer3 = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseTankImage3));
                }
            }
        }

        // Use Tank 3 Image property - determines which image to show based on state
        public BitmapImage UseTankImage3
        {
            get
            {
                string imageName = GetUseTankImageName3();
                return Ring.Services.Images.ImageLoader.LoadImage($"Use Tank/{imageName}");
            }
        }

        // Combined temperature display for Use Tank 3
        public string TemperatureDisplay3 => $"{Temperature3:F1}°F";

        #endregion

        #region PLC Status Properties

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

        /// <summary>
        /// Determines which Use Tank 1 image to display based on state (agitator, heat, additive, adhesivesupply, adhesivetransfer)
        /// Priority: Most specific combinations first, then individual states, then base
        /// </summary>
        private string GetUseTankImageName1()
        {
            // Three states: agitator + heat + additive
            if (IsAgitating1 && IsHeating1 && IsAddingAdditive1)
            {
                return "usetank_agitator_heat_additive.bmp";
            }
            // Three states: agitator + heat + adhesivesupply
            if (IsAgitating1 && IsHeating1 && IsAdhesiveSupply1)
            {
                return "usetank_agitator_heat_adhesivesupply.bmp";
            }
            // Three states: agitator + heat + adhesivetransfer
            if (IsAgitating1 && IsHeating1 && IsAdhesiveTransfer1)
            {
                return "usetank_agitator_heat_adhesivetransfer.bmp";
            }
            // Two states: agitator + heat (but not additive/adhesivesupply/adhesivetransfer)
            if (IsAgitating1 && IsHeating1)
            {
                return "usetank_agitator_heat.bmp";
            }
            // Two states: agitator + additive (but not heat)
            if (IsAgitating1 && IsAddingAdditive1)
            {
                return "usetank_agitator_additive.bmp";
            }
            // Two states: agitator + adhesivesupply (but not heat)
            if (IsAgitating1 && IsAdhesiveSupply1)
            {
                return "usetank_agitator_adhesivesupply.bmp";
            }
            // Two states: agitator + adhesivetransfer (but not heat)
            if (IsAgitating1 && IsAdhesiveTransfer1)
            {
                return "usetank_agitator_adhesivetransfer.bmp";
            }
            // Only agitator
            if (IsAgitating1)
            {
                return "usetank_agitator.bmp";
            }
            // Two states: heat + additive (but not agitator)
            if (IsHeating1 && IsAddingAdditive1)
            {
                return "usetank_heat_additive.bmp";
            }
            // Two states: heat + adhesivesupply (but not agitator)
            if (IsHeating1 && IsAdhesiveSupply1)
            {
                return "usetank_heat_adhesivesupply.bmp";
            }
            // Two states: heat + adhesivetransfer (but not agitator)
            if (IsHeating1 && IsAdhesiveTransfer1)
            {
                return "usetank_heat_adhesivetransfer.bmp";
            }
            // Only heat
            if (IsHeating1)
            {
                return "usetank_heat.bmp";
            }
            // Only additive
            if (IsAddingAdditive1)
            {
                return "usetank_additive.bmp";
            }
            // Two states: adhesivesupply + adhesivetransfer (but not agitator/heat/additive)
            if (IsAdhesiveSupply1 && IsAdhesiveTransfer1)
            {
                return "usetank_adhesivesupply_adhesivetransfer.bmp";
            }
            // Only adhesivesupply
            if (IsAdhesiveSupply1)
            {
                return "usetank_adhesivesupply.bmp";
            }
            // Only adhesivetransfer
            if (IsAdhesiveTransfer1)
            {
                return "usetank_adhesivetransfer.bmp";
            }
            // Base image (no states active)
            return "usetank.bmp";
        }

        /// <summary>
        /// Determines which Use Tank 2 image to display based on state (agitator, heat, additive, adhesivesupply, adhesivetransfer)
        /// Priority: Most specific combinations first, then individual states, then base
        /// </summary>
        private string GetUseTankImageName2()
        {
            // Three states: agitator + heat + additive
            if (IsAgitating2 && IsHeating2 && IsAddingAdditive2)
            {
                return "usetank_agitator_heat_additive.bmp";
            }
            // Three states: agitator + heat + adhesivesupply
            if (IsAgitating2 && IsHeating2 && IsAdhesiveSupply2)
            {
                return "usetank_agitator_heat_adhesivesupply.bmp";
            }
            // Three states: agitator + heat + adhesivetransfer
            if (IsAgitating2 && IsHeating2 && IsAdhesiveTransfer2)
            {
                return "usetank_agitator_heat_adhesivetransfer.bmp";
            }
            // Two states: agitator + heat (but not additive/adhesivesupply/adhesivetransfer)
            if (IsAgitating2 && IsHeating2)
            {
                return "usetank_agitator_heat.bmp";
            }
            // Two states: agitator + additive (but not heat)
            if (IsAgitating2 && IsAddingAdditive2)
            {
                return "usetank_agitator_additive.bmp";
            }
            // Two states: agitator + adhesivesupply (but not heat)
            if (IsAgitating2 && IsAdhesiveSupply2)
            {
                return "usetank_agitator_adhesivesupply.bmp";
            }
            // Two states: agitator + adhesivetransfer (but not heat)
            if (IsAgitating2 && IsAdhesiveTransfer2)
            {
                return "usetank_agitator_adhesivetransfer.bmp";
            }
            // Only agitator
            if (IsAgitating2)
            {
                return "usetank_agitator.bmp";
            }
            // Two states: heat + additive (but not agitator)
            if (IsHeating2 && IsAddingAdditive2)
            {
                return "usetank_heat_additive.bmp";
            }
            // Two states: heat + adhesivesupply (but not agitator)
            if (IsHeating2 && IsAdhesiveSupply2)
            {
                return "usetank_heat_adhesivesupply.bmp";
            }
            // Two states: heat + adhesivetransfer (but not agitator)
            if (IsHeating2 && IsAdhesiveTransfer2)
            {
                return "usetank_heat_adhesivetransfer.bmp";
            }
            // Only heat
            if (IsHeating2)
            {
                return "usetank_heat.bmp";
            }
            // Only additive
            if (IsAddingAdditive2)
            {
                return "usetank_additive.bmp";
            }
            // Two states: adhesivesupply + adhesivetransfer (but not agitator/heat/additive)
            if (IsAdhesiveSupply2 && IsAdhesiveTransfer2)
            {
                return "usetank_adhesivesupply_adhesivetransfer.bmp";
            }
            // Only adhesivesupply
            if (IsAdhesiveSupply2)
            {
                return "usetank_adhesivesupply.bmp";
            }
            // Only adhesivetransfer
            if (IsAdhesiveTransfer2)
            {
                return "usetank_adhesivetransfer.bmp";
            }
            // Base image (no states active)
            return "usetank.bmp";
        }

        /// <summary>
        /// Determines which Use Tank 3 image to display based on state (agitator, heat, additive, adhesivesupply, adhesivetransfer)
        /// Priority: Most specific combinations first, then individual states, then base
        /// </summary>
        private string GetUseTankImageName3()
        {
            // Three states: agitator + heat + additive
            if (IsAgitating3 && IsHeating3 && IsAddingAdditive3)
            {
                return "usetank_agitator_heat_additive.bmp";
            }
            // Three states: agitator + heat + adhesivesupply
            if (IsAgitating3 && IsHeating3 && IsAdhesiveSupply3)
            {
                return "usetank_agitator_heat_adhesivesupply.bmp";
            }
            // Three states: agitator + heat + adhesivetransfer
            if (IsAgitating3 && IsHeating3 && IsAdhesiveTransfer3)
            {
                return "usetank_agitator_heat_adhesivetransfer.bmp";
            }
            // Two states: agitator + heat (but not additive/adhesivesupply/adhesivetransfer)
            if (IsAgitating3 && IsHeating3)
            {
                return "usetank_agitator_heat.bmp";
            }
            // Two states: agitator + additive (but not heat)
            if (IsAgitating3 && IsAddingAdditive3)
            {
                return "usetank_agitator_additive.bmp";
            }
            // Two states: agitator + adhesivesupply (but not heat)
            if (IsAgitating3 && IsAdhesiveSupply3)
            {
                return "usetank_agitator_adhesivesupply.bmp";
            }
            // Two states: agitator + adhesivetransfer (but not heat)
            if (IsAgitating3 && IsAdhesiveTransfer3)
            {
                return "usetank_agitator_adhesivetransfer.bmp";
            }
            // Only agitator
            if (IsAgitating3)
            {
                return "usetank_agitator.bmp";
            }
            // Two states: heat + additive (but not agitator)
            if (IsHeating3 && IsAddingAdditive3)
            {
                return "usetank_heat_additive.bmp";
            }
            // Two states: heat + adhesivesupply (but not agitator)
            if (IsHeating3 && IsAdhesiveSupply3)
            {
                return "usetank_heat_adhesivesupply.bmp";
            }
            // Two states: heat + adhesivetransfer (but not agitator)
            if (IsHeating3 && IsAdhesiveTransfer3)
            {
                return "usetank_heat_adhesivetransfer.bmp";
            }
            // Only heat
            if (IsHeating3)
            {
                return "usetank_heat.bmp";
            }
            // Only additive
            if (IsAddingAdditive3)
            {
                return "usetank_additive.bmp";
            }
            // Two states: adhesivesupply + adhesivetransfer (but not agitator/heat/additive)
            if (IsAdhesiveSupply3 && IsAdhesiveTransfer3)
            {
                return "usetank_adhesivesupply_adhesivetransfer.bmp";
            }
            // Only adhesivesupply
            if (IsAdhesiveSupply3)
            {
                return "usetank_adhesivesupply.bmp";
            }
            // Only adhesivetransfer
            if (IsAdhesiveTransfer3)
            {
                return "usetank_adhesivetransfer.bmp";
            }
            // Base image (no states active)
            return "usetank.bmp";
        }

        // Initialize with default values for all three use tanks
        private void InitializeDefaultValues()
        {
            // Use Tank 1 - Module Facer 1 defaults
            ResinTypeSelected1 = "None";
            Tank1 = "Storage Tank 1";
            Formula1 = "Formula 1";
            Status1 = "Tank in Use";
            Temperature1 = 98;
            Level1 = 75;
            IsAgitating1 = false;
            IsHeating1 = false;
            IsAddingAdditive1 = false;
            IsAdhesiveSupply1 = false;
            IsAdhesiveTransfer1 = false;

            // Use Tank 2 - Module Facer 2 defaults
            ResinTypeSelected2 = "None";
            Tank2 = "Storage Tank 2";
            Formula2 = "Formula 2";
            Status2 = "Tank in Use";
            Temperature2 = 98;
            Level2 = 60;
            IsAgitating2 = false;
            IsHeating2 = false;
            IsAddingAdditive2 = false;
            IsAdhesiveSupply2 = false;
            IsAdhesiveTransfer2 = false;

            // Use Tank 3 - Double Backer defaults
            ResinTypeSelected3 = "None";
            Tank3 = "None";
            Formula3 = "None";
            Status3 = "Tank Not in Use";
            Temperature3 = 98;
            Level3 = 0;
            IsAgitating3 = false;
            IsHeating3 = false;
            IsAddingAdditive3 = false;
            IsAdhesiveSupply3 = false;
            IsAdhesiveTransfer3 = false;

            // PLC status defaults
            PlcStatusText = "PLC Connected";
            PlcStatusBrush = "#28A745";  // Green color for connected status
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