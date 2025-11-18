using System;

namespace Ring.Services.PLC
{
    /// <summary>
    /// Static class to store PLC data for Make Ready Tank
    /// This allows MainWindow to store PLC values that Make Ready Tank can access
    /// </summary>
    public static class MakeReadyTankPlcData
    {
        private static double _weight = 0.0;
        private static double _temperature = 25.0;
        private static double _boraxCausticWeight = 0.0;
        private static DateTime _lastUpdate = DateTime.Now;

        public static double Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                _lastUpdate = DateTime.Now;
            }
        }

        public static double Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                _lastUpdate = DateTime.Now;
            }
        }

        public static double BoraxCausticWeight
        {
            get => _boraxCausticWeight;
            set
            {
                _boraxCausticWeight = value;
                _lastUpdate = DateTime.Now;
            }
        }

        public static DateTime LastUpdate => _lastUpdate;

        public static bool IsDataFresh => (DateTime.Now - _lastUpdate).TotalSeconds < 5; // Data is fresh if less than 5 seconds old
    }
}




