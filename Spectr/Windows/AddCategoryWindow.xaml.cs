using Spectr.Database;
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

namespace Spectr.Windows
{
    /// <summary>
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        private Repair _repair;
        public AddCategoryWindow(Repair repair)
        {
            InitializeComponent();

            _repair = repair;
            var categories = App.Connection.RepairCategory.ToList();

            foreach(var c in repair.RepairCategoryJunction)
            {
                var category = categories.FirstOrDefault(x => x.CategoryID == c.RepairCategory.CategoryID);
                if (category != null)
                {
                    categories.Remove(category);
                }
            }

            CategoriesDataGrid.ItemsSource = categories;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem == null)
            {
                return;
            }

            _repair.RepairCategoryJunction.Add(new RepairCategoryJunction()
            {
                RepairCategory = (RepairCategory)CategoriesDataGrid.SelectedItem,
                Repair = _repair
            });

            this.DialogResult = true;
        }
    }
}
