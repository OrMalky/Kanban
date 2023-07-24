using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using FrontEnd.Model;
using FrontEnd.ViewModel;
using MaterialDesignThemes.Wpf;

namespace FrontEnd.View
{
    /// <summary>
    /// Interaction logic for BoardListView.xaml
    /// </summary>
    public partial class BoardListView : Window
    {
        private BoardListViewModel vm;
        private BackendController controller;
        private ViewManager manager;
        private UserModel user;

        public BoardListView(UserModel user,ViewManager manager,BackendController controller)
        {
            InitializeComponent();
            vm = new BoardListViewModel(user, controller);
            DataContext = vm;
            this.controller = controller;
            this.manager = manager;
            this.user = user;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            var selectedItem = dataGrid.SelectedItem as BoardModel;

            if (selectedItem != null)
            {
                manager.BoardSelected(selectedItem);
            }
        }

        private void LogOutButton(object sender, RoutedEventArgs e)
        {
            vm.LogOut();
            manager.Logout();
        }
    }
}
