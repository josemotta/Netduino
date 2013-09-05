using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SilverlightOnNetduino
{
    public class TemperatureViewModel : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Collection of temperature readings.
        /// </summary>
        private ObservableCollection<TemperatureReading> _readings;
        public ObservableCollection<TemperatureReading> Readings
        {
            get { return (_readings); }
            set
            {
                _readings = value;
                RaisePropertyChanged("Readings", "NumberOfReadings");
            }
        }

        /// <summary>
        /// Number of readings in the collection of temperature readings.
        /// </summary>
        public int NumberOfReadings
        {
            get { return (_readings.Count); }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Default constructor
        /// </summary>
        public TemperatureViewModel()
        {
            Readings = new ObservableCollection<TemperatureReading>();
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event used to notify any subscribers that the data in this class has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Let any subscribers know that some data has changed.
        /// </summary>
        /// <param name="properties">Array of name of the properties which have changed.</param>
        private void RaisePropertyChanged(params string[] properties)
        {
            if ((properties != null) & (PropertyChanged != null))
            {
                foreach (string property in properties)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a new reading to the collection.
        /// </summary>
        /// <param name="reading">Reading to be added.</param>
        public void Add(TemperatureReading reading)
        {
            Readings.Add(reading);
            RaisePropertyChanged("Readings", "NumberOfReadings");
        }

        #endregion
    }
}
