using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonMonitorService
{
    public class Book : INotifyPropertyChanged
    {
        private static int _nextId = 1; 
        public Book() 
        {
            UniqueID= _nextId++;
        }

        public int UniqueID { get; private set; }
        
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }
        
        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        private string _rack = string.Empty;
        public string Rack
        {
            get { return _rack; }
            set { _rack = value; OnPropertyChanged(nameof(Rack)); }
        }

        private bool _isDataUpdated;
        public bool IsDataUpdated
        {
            get { return _isDataUpdated; }
            set { _isDataUpdated = value; OnPropertyChanged(nameof(IsDataUpdated)); }
        }

        public static void ResetID()
        {
            _nextId = 0;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
        
    }
}
