using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace App6.GeneratUrl
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _uniqueId;

        public string UniqueId
        {
            get { return _uniqueId; }
            set
            {
                _uniqueId = value;
                OnPropertyChanged(nameof(UniqueId));
            }
        }

        // Implement INotifyPropertyChanged interface

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
