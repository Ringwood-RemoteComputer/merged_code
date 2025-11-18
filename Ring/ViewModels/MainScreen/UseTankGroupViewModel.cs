using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        // Use Tank 2 Properties
        private string _resinTypeSelected2;
        private string _tank2;
        private string _formula2;
        private string _status2;
        private decimal _temperature2;
        private decimal _level2;

        // Use Tank 3 Properties
        private string _resinTypeSelected3;
        private string _tank3;
        private string _formula3;
        private string _status3;
        private decimal _temperature3;
        private decimal _level3;

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

            // Use Tank 2 - Module Facer 2 defaults
            ResinTypeSelected2 = "None";
            Tank2 = "Storage Tank 2";
            Formula2 = "Formula 2";
            Status2 = "Tank in Use";
            Temperature2 = 98;
            Level2 = 60;

            // Use Tank 3 - Double Backer defaults
            ResinTypeSelected3 = "None";
            Tank3 = "None";
            Formula3 = "None";
            Status3 = "Tank Not in Use";
            Temperature3 = 98;
            Level3 = 0;

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