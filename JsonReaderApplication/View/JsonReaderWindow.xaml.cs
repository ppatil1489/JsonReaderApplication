using JsonReaderApplication.ViewModel;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JsonReaderApplication.View
{
    /// <summary>
    /// Interaction logic for JsonReaderWindow.xaml
    /// </summary>
    public partial class JsonReaderWindow : Window
    {
        public JsonReaderWindow()
        {
            InitializeComponent();
            this.Closed += JsonReaderWindow_Closed;
            this.DataContext = new JSonReaderViewModel();
        }

        private void JsonReaderWindow_Closed(object? sender, EventArgs e)
        {
            if (sender is JsonReaderWindow view)
            {
                var viewModel = (JSonReaderViewModel)view.DataContext;
                viewModel?.CancelJSONMonitorService();
            }
        }

        /// <summary>
        /// Initialize the reading Json file content.
        /// </summary>
        /// <param name="sender">view</param>
        /// <param name="e">routed event args</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is JsonReaderWindow view)
            {
                var viewModel = (JSonReaderViewModel)view.DataContext;
                viewModel?.UpdateJSONContent();
            }
        }
       
    }
}
