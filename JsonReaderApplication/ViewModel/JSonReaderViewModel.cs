using GalaSoft.MvvmLight.Command;
using JsonMonitorService;
using JsonReaderApplication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace JsonReaderApplication.ViewModel
{
    /// <summary>
    /// This class represents a ViewModel for initialize 
    /// reading Json content from file and update the content on UI 
    /// based on timer.
    /// 
    /// </summary>
    public class JSonReaderViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Book> _booksCollection = new ObservableCollection<Book>();
        private bool _isProgressVisible;
        private bool _isServiceStarted;
        private string _buttonServiceName = "Stop Service";
        private object _lock = new object();

        /// <summary>
        /// Books collection
        /// </summary>
        public ObservableCollection<Book> BooksCollection
        {
            get { return _booksCollection; }
            set { _booksCollection = value;OnPropertyChanged("BooksCollection"); }
        }

        /// <summary>
        ///  Set flag to display /Hide progress bar on UI.
        /// </summary>
        public bool IsProgressVisible
        {
            get { return _isProgressVisible; }
            set { _isProgressVisible = value; OnPropertyChanged("IsProgressVisible"); }
        }

        /// <summary>
        /// Set flag to cancel / start the json reading service.
        /// </summary>
        public bool IsServiceStarted
        {
            get { return _isServiceStarted; }
            set { _isServiceStarted = value; OnPropertyChanged("IsServiceStarted"); }
        }

        /// <summary>
        /// Update button content 
        /// </summary>
        public string ButtonServiceName
        {
            get { return _buttonServiceName; }
            set { _buttonServiceName = value; OnPropertyChanged("ButtonServiceName"); }
        }

        /// <summary>
        /// Clear listview content on UI.
        /// </summary>
        public ICommand ClearCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Start/ stop the json reading service.
        /// </summary>
        public ICommand ProcessServiceCommand
        {
            get;
            set;
        }

        public JsonReaderModel _jsonReaderModel;

        public  JSonReaderViewModel()
        {
            _jsonReaderModel = new JsonReaderModel(UpdateBooksCollection, DisplayProgressBar);
            ClearCommand = new RelayCommand(OnClear, CanClear);
            ProcessServiceCommand = new RelayCommand(OnProcessService, CanProcessService);
            IsServiceStarted = true;
        }


        public void UpdateBooksCollection(List<Book> books)
        {
            lock (_lock)
            {
                BooksCollection = new ObservableCollection<Book>(books);
                IsProgressVisible = !IsProgressVisible;
            }
        }

        public void DisplayProgressBar(bool isProgressStarted)
        {
            IsProgressVisible = isProgressStarted;
        }

        /// <summary>
        /// Read JSON content from file in every 5 seconds using periodic timer
        /// Update UI if there is changes in JSON file.
        /// </summary>
        /// <returns></returns>
        public void UpdateJSONContent()
        {
           _jsonReaderModel?.RefreshJSONContent();
           
        }

        private bool CanProcessService()
        {
            return true;
        }

        /// <summary>
        ///  start and stop JSON monitor service.
        /// </summary>
        private void OnProcessService()
        {
            if (IsServiceStarted)
            {
                CancelJSONMonitorService();
                ButtonServiceName = "Start Service";
                IsServiceStarted = false;
            }
            else
            {
                UpdateJSONContent();
                ButtonServiceName = "Stop Service";
                IsServiceStarted = true;
            }
        }

        public void CancelJSONMonitorService()
        {
            _jsonReaderModel?.CancelJSONUpdateService();
        }

        private bool CanClear()
        {
            return true;
        }

        private void OnClear()
        {
            lock (_lock)
            {
                BooksCollection = new ObservableCollection<Book>();
            }
        }

       

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

    }
}
