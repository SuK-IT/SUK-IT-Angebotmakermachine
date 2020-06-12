using SuKer_Angebot.Bindings;
using SuKer_Angebot.Subwindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SuKer_Angebot
{

    public partial class MainWindow : Window
    {
        public DataContextImplementation Implementation = new DataContextImplementation();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Implementation;
            Positions.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Position", System.ComponentModel.ListSortDirection.Ascending));
        }
        private void deleteItem_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Positions.SelectedIndex < 0 || Positions.SelectedIndex > Implementation.OfferObservable.List.Count)
                return;
            Implementation.OfferObservable.List.RemoveAt(Positions.SelectedIndex);
            for(int i = 0; i < Implementation.Position; i++)
            {
                Implementation.OfferObservable.List[i].Position = i;
            }
            Positions.Items.Refresh();
        }
        private void addItem_Button_Click(object sender, RoutedEventArgs e)
        {
            Implementation.OfferObservable.List.Add(new OfferItem(Implementation.Position, 1, $"newPosition{Implementation.Position}", 0.00, 0.00));
            Positions.Items.Refresh();
        }
        private void editItem_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Positions.SelectedItems.Count <= 0 || Positions.SelectedIndex == -1)
                return;

            var Wnd = new Editor();
            Wnd.Implementation = Implementation;
            Wnd.Position = Positions.SelectedIndex;
            Wnd.ShowDialog();
            Positions.Items.Refresh();
        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://www.sukit.de.tl/");
        }
    }
}
