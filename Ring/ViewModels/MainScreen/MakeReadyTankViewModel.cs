using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ring.ViewModels.MainScreen
{
    public class MakeReadyTankViewModel : INotifyPropertyChanged
    {
        private string _weight;
        private string _temperature;
        private string _boraxCausticWeight;
        private double _ingredientAdditionProgress;
        private double _mixTimeProgress;
        private string _mixTimeRemaining;
        private string _plcStatusText;
        private Brush _plcStatusBrush;
        private string _formulaName;
        private string _storageTank;
        private string _currentStep;
        private double _formulaProgress;
        private string _processHistory;
        private ObservableCollection<FormulaStep> _formulaSteps;
        private ObservableCollection<Alarm> _activeAlarms;

        // Industrial component states
        private bool _isMotorOn;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Weight
        {
            get => _weight;
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Temperature
        {
            get => _temperature;
            set
            {
                if (_temperature != value)
                {
                    _temperature = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BoraxCausticWeight
        {
            get => _boraxCausticWeight;
            set
            {
                if (_boraxCausticWeight != value)
                {
                    _boraxCausticWeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public double IngredientAdditionProgress
        {
            get => _ingredientAdditionProgress;
            set
            {
                if (_ingredientAdditionProgress != value)
                {
                    _ingredientAdditionProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MixTimeProgress
        {
            get => _mixTimeProgress;
            set
            {
                if (_mixTimeProgress != value)
                {
                    _mixTimeProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MixTimeRemaining
        {
            get => _mixTimeRemaining;
            set
            {
                if (_mixTimeRemaining != value)
                {
                    _mixTimeRemaining = value;
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
        public Brush PlcStatusBrush
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

        public string FormulaName
        {
            get => _formulaName;
            set
            {
                if (_formulaName != value)
                {
                    _formulaName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StorageTank
        {
            get => _storageTank;
            set
            {
                if (_storageTank != value)
                {
                    _storageTank = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentStep
        {
            get => _currentStep;
            set
            {
                if (_currentStep != value)
                {
                    _currentStep = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MRTImage));
                }
            }
        }

        public double FormulaProgress
        {
            get => _formulaProgress;
            set
            {
                if (_formulaProgress != value)
                {
                    _formulaProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProcessHistory
        {
            get => _processHistory;
            set
            {
                if (_processHistory != value)
                {
                    _processHistory = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<FormulaStep> FormulaSteps
        {
            get => _formulaSteps;
            set
            {
                if (_formulaSteps != value)
                {
                    _formulaSteps = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Alarm> ActiveAlarms
        {
            get => _activeAlarms;
            set
            {
                if (_activeAlarms != value)
                {
                    _activeAlarms = value;
                    OnPropertyChanged();
                }
            }
        }

        // Industrial Component Properties
        public bool IsMotorOn
        {
            get => _isMotorOn;
            set
            {
                if (_isMotorOn != value)
                {
                    _isMotorOn = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MotorStatus));
                    OnPropertyChanged(nameof(MotorImage));
                    OnPropertyChanged(nameof(MotorTooltip));
                    OnPropertyChanged(nameof(MRTImage));
                }
            }
        }

        // Status text properties
        public string MotorStatus => IsMotorOn ? "ON" : "OFF";
        public string MotorTooltip => IsMotorOn ? "Agitator - ON" : "Agitator - OFF";

        // Image properties using existing ImageLoader with TransparentBackgrounds folder
        public BitmapImage MotorImage => Ring.Services.Images.ImageLoader.LoadImage(
            IsMotorOn ? "TransparentBackgrounds/Motorgn01.png" : "TransparentBackgrounds/Motorrd01.png");
        public BitmapImage TankImage => Ring.Services.Images.ImageLoader.LoadImage("TransparentBackgrounds/tank.png");

        // MRT Image property - determines which image to show based on current step and agitator state
        // Defaults to MRT.png when no specific step is set
        public BitmapImage MRTImage
        {
            get
            {
                string imageName = GetMRTImageName();
                return Ring.Services.Images.ImageLoader.LoadImage($"Make Ready Tank/{imageName}");
            }
        }

        /// <summary>
        /// Determines which MRT image to display based on current step and agitator state
        /// Defaults to MRT.png when no specific step is set
        /// </summary>
        private string GetMRTImageName()
        {
            string baseImageName;
            string agitatorSuffix = IsMotorOn ? "_agitator" : "";

            // Check if we have a current step to determine specific image
            if (!string.IsNullOrEmpty(CurrentStep))
            {
                string stepLower = CurrentStep.ToLower();

                // Map step descriptions to image filenames
                if (stepLower.Contains("transfer") || stepLower.Contains("discharge"))
                {
                    baseImageName = "MRT_discharge";
                }
                else if (stepLower.Contains("steam"))
                {
                    baseImageName = "MRT_steam";
                }
                else if (stepLower.Contains("finish water"))
                {
                    baseImageName = "MRT_alt_water";
                }
                else if (stepLower.Contains("water") && !stepLower.Contains("finish"))
                {
                    baseImageName = "MRT_main_water";
                }
                else if (stepLower.Contains("caustic"))
                {
                    baseImageName = "MRT_caustic";
                }
                else if (stepLower.Contains("domestic starch"))
                {
                    baseImageName = "MRT_dom_starch";
                }
                else if (stepLower.Contains("modified starch"))
                {
                    baseImageName = "MRT_mod_starch";
                }
                else if (stepLower.Contains("borax") || stepLower.Contains("borex"))
                {
                    baseImageName = "MRT_borax";
                }
                else if (stepLower.Contains("liquid additive 1") || stepLower.Contains("add1") || stepLower.Contains("additive 1"))
                {
                    baseImageName = "MRT_additive1";
                }
                else if (stepLower.Contains("liquid additive 2") || stepLower.Contains("add2") || stepLower.Contains("additive 2"))
                {
                    baseImageName = "MRT_additive2";
                }
                else if (stepLower.Contains("liquid additive 3") || stepLower.Contains("add3") || stepLower.Contains("additive 3"))
                {
                    baseImageName = "MRT_additive3";
                }
                else
                {
                    // Default: base image (with or without agitator)
                    baseImageName = IsMotorOn ? "MRT_agitator" : "MRT";
                    return $"{baseImageName}.png";
                }
            }
            else
            {
                // Default: base image (with or without agitator) when no step is set
                baseImageName = IsMotorOn ? "MRT_agitator" : "MRT";
                return $"{baseImageName}.png";
            }

            // Append agitator suffix if motor is on
            return $"{baseImageName}{agitatorSuffix}.png";
        }

        public MakeReadyTankViewModel()
        {
            // Initialize default values
            Weight = "1960";
            Temperature = "101";
            BoraxCausticWeight = "0";
            IngredientAdditionProgress = 90;
            MixTimeProgress = 33.3;
            MixTimeRemaining = "0:40";

            // Set requested default values
            FormulaName = "Formula 1";
            StorageTank = "Storage Tank 1";
            CurrentStep = "50% Caustic";
            FormulaProgress = 50.0;
            ProcessHistory = "";

            // Initialize collections
            FormulaSteps = new ObservableCollection<FormulaStep>
            {
                new FormulaStep { Step = "1", Description = "Water", PresetAmount = "677", ActualAmount = "677", PresetTime = "0", Status = "Complete" },
                new FormulaStep { Step = "2", Description = "Steam", PresetAmount = "120", ActualAmount = "120", PresetTime = "0", Status = "Complete" },
                new FormulaStep { Step = "3", Description = "Finish Water", PresetAmount = "85", ActualAmount = "85", PresetTime = "0", Status = "Complete" },
                new FormulaStep { Step = "4", Description = "Domestic Starch", PresetAmount = "82", ActualAmount = "82", PresetTime = "0", Status = "Complete" },
                new FormulaStep { Step = "5", Description = "50% Caustic", PresetAmount = "26.0", ActualAmount = "26.0", PresetTime = "120", Status = "In Progress" },
                new FormulaStep { Step = "6", Description = "Primary Borex", PresetAmount = "1.4", ActualAmount = "0", PresetTime = "180", Status = "Pending" },
                new FormulaStep { Step = "7", Description = "Water", PresetAmount = "1170", ActualAmount = "0", PresetTime = "0", Status = "Pending" },
                new FormulaStep { Step = "8", Description = "Steam", PresetAmount = "95", ActualAmount = "0", PresetTime = "0", Status = "Pending" },
                new FormulaStep { Step = "9", Description = "Finish Water", PresetAmount = "85", ActualAmount = "0", PresetTime = "0", Status = "Pending" }
            };
            ActiveAlarms = new ObservableCollection<Alarm>();

            // PLC status defaults
            PlcStatusText = "PLC Connected";
            PlcStatusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#28A745"));  // Green color for connected status

            // Initialize industrial component states
            IsMotorOn = false;
        }
    }
}

public class FormulaStep
{
    public string Step { get; set; }
    public string Description { get; set; }
    public string PresetAmount { get; set; }
    public string ActualAmount { get; set; }
    public string PresetTime { get; set; }
    public string Status { get; set; }
}