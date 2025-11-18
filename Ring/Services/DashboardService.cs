using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Ring.ViewModels.MainScreen;

namespace Ring.Services
{
    /// <summary>
    /// Service that coordinates dashboard data from existing ViewModels
    /// </summary>
    public class DashboardService : INotifyPropertyChanged
    {
        #region Private Fields
        private MakeReadyTankViewModel _makeReadyTankViewModel;
        private StorageTankGroupViewModel _storageTankGroupViewModel;
        private UseTankGroupViewModel _useTankGroupViewModel;
        private bool _isConnected;
        private string _connectionStatus;
        private Brush _connectionStatusBrush;
        #endregion

        #region Constructor
        public DashboardService()
        {
            InitializeViewModels();
            InitializeCommands();
            UpdateConnectionStatus();
        }
        #endregion

        #region Properties

        #region ViewModels
        public MakeReadyTankViewModel MakeReadyTankViewModel
        {
            get => _makeReadyTankViewModel;
            set => SetProperty(ref _makeReadyTankViewModel, value);
        }

        public StorageTankGroupViewModel StorageTankGroupViewModel
        {
            get => _storageTankGroupViewModel;
            set => SetProperty(ref _storageTankGroupViewModel, value);
        }

        public UseTankGroupViewModel UseTankGroupViewModel
        {
            get => _useTankGroupViewModel;
            set => SetProperty(ref _useTankGroupViewModel, value);
        }
        #endregion

        #region Connection Status
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    UpdateConnectionStatus();
                }
            }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        public Brush ConnectionStatusBrush
        {
            get => _connectionStatusBrush;
            set => SetProperty(ref _connectionStatusBrush, value);
        }
        #endregion

        #endregion

        #region Commands
        public ICommand NavigateToMakeReadyTankCommand { get; private set; }
        public ICommand NavigateToStorageTanksCommand { get; private set; }
        public ICommand NavigateToTvcTankCommand { get; private set; }
        public ICommand NavigateToUseTanksCommand { get; private set; }
        #endregion

        #region Events
        public event EventHandler<string> NavigationRequested;
        #endregion

        #region Private Methods
        private void InitializeViewModels()
        {
            // Initialize existing ViewModels - use property setters to trigger OnPropertyChanged
            MakeReadyTankViewModel = new MakeReadyTankViewModel();
            StorageTankGroupViewModel = new StorageTankGroupViewModel();
            UseTankGroupViewModel = new UseTankGroupViewModel();

            // Subscribe to ViewModel property changes to update dashboard
            SubscribeToViewModelChanges();

            _isConnected = true;
        }

        private void SubscribeToViewModelChanges()
        {
            // Subscribe to changes in the ViewModels to keep dashboard updated
            if (_makeReadyTankViewModel != null)
            {
                _makeReadyTankViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            if (_storageTankGroupViewModel != null)
            {
                _storageTankGroupViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            if (_useTankGroupViewModel != null)
            {
                _useTankGroupViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Forward property changes from ViewModels to dashboard
            // This ensures the dashboard stays in sync with the individual tank views
            OnPropertyChanged($"TankDataChanged_{sender.GetType().Name}_{e.PropertyName}");
        }

        private void InitializeCommands()
        {
            NavigateToMakeReadyTankCommand = new RelayCommand(() => OnNavigationRequested("MakeReadyTank"));
            NavigateToStorageTanksCommand = new RelayCommand(() => OnNavigationRequested("StorageTankGroup"));
            NavigateToTvcTankCommand = new RelayCommand(() => OnNavigationRequested("TvcControl"));
            NavigateToUseTanksCommand = new RelayCommand(() => OnNavigationRequested("UseTankGroup"));
        }

        private void UpdateConnectionStatus()
        {
            if (_isConnected)
            {
                ConnectionStatus = "Connected";
                ConnectionStatusBrush = new SolidColorBrush(Colors.Green);
            }
            else
            {
                ConnectionStatus = "Disconnected";
                ConnectionStatusBrush = new SolidColorBrush(Colors.Red);
            }
        }

        private void OnNavigationRequested(string viewName)
        {
            NavigationRequested?.Invoke(this, viewName);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Refresh all dashboard data by updating the underlying ViewModels
        /// </summary>
        public void RefreshDashboardData()
        {
            // Trigger property change notifications to refresh the UI
            OnPropertyChanged(nameof(MakeReadyTankViewModel));
            OnPropertyChanged(nameof(StorageTankGroupViewModel));
            OnPropertyChanged(nameof(UseTankGroupViewModel));
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            // Unsubscribe from ViewModel events
            if (_makeReadyTankViewModel != null)
            {
                _makeReadyTankViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            if (_storageTankGroupViewModel != null)
            {
                _storageTankGroupViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            if (_useTankGroupViewModel != null)
            {
                _useTankGroupViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    /// <summary>
    /// Simple relay command implementation for MVVM pattern
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}

