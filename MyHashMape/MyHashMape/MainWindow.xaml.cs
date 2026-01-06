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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyHashMape
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HashMapViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            CbOperation.SelectionChanged += CbOperation_SelectionChanged;
        }

        private void Add_Element(object sender, RoutedEventArgs e)
        {
            if (vm == null)
            {
                MessageBox.Show("Please create the Hash Map first by entering a size and clicking Create.", "Warning");
                return;
            }
            if (CbOperation.SelectedIndex == 0)
            {
                if (OperTextbox2.Text == "" || OperTextbox1.Text == "") return;
               
                vm.Add(OperTextbox1.Text, OperTextbox2.Text , ShowSteps.IsChecked);
                return;
            }
            if (OperTextbox1.Text == "" ) return;
            vm.Remove(OperTextbox1.Text);
            
           
        }
        private void CbOperation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CbOperation.SelectedIndex == 0)
            {
                OperTextbox2.Visibility = Visibility.Visible;
                OperLabel2.Visibility = Visibility.Visible;
            }
            else
            {
                OperTextbox2.Visibility = Visibility.Collapsed;
                OperLabel2.Visibility = Visibility.Collapsed;
            }
        }
        private void  Create_Hashmap(object sender, RoutedEventArgs e)
        {
            if (HashmapSizeTextbox.Text == "") return;
            if(!int.TryParse(HashmapSizeTextbox.Text, out int size)) return;
            vm = new HashMapViewModel(size);
            DataContext = vm;
        }

        private void ComboBoxAutoFill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not HashMapViewModel vm)
                return;

            int count = 0;

            switch (((ComboBox)sender).SelectedIndex)
            {
                case 0:
                    count = 5;
                    break;
                case 1:
                    count = 10;
                    break;
                case 2:
                    count = 25;
                    break;
                case 3:
                    count = 50;
                    break;
                case 4:
                    count = 100;
                    break;
                default:
                    return;
            }

            vm.AutoFill(count);
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            if (vm != null)
            {
                vm.NextStep();
            }
        }
      
    }
}
