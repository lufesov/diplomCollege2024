using Spectr.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
    /// Interaction logic for EditOrderWindow.xaml
    /// </summary>
    public partial class EditOrderWindow : Window
    {
        private bool _isNew;
        private Repair _current;
        private List<Employer> _employers;
        private List<Realty> _realties;
        private List<Customer> _customers;

        public EditOrderWindow(Repair repair)
        {
            InitializeComponent();

            if (repair == null)
            {
                _isNew = true;

                _current = new Repair();

                _current.DateStart = DateTime.Now;
                _current.PlainDateEnd= DateTime.Now.AddDays(5);

                tbAddBtn.Content = "Добавить";
            }
            else
            {
                _current = repair;

                tbAddBtn.Content = "Изменить";
            }

            DataContext = _current;

            LoadListData();
        }

        private void AddCategory(object sender, RoutedEventArgs e)
        {
            var win = new AddCategoryWindow(_current);

            if (win.ShowDialog() == true)
            {
                CategoriesDataGrid.ItemsSource = null;
                CategoriesDataGrid.ItemsSource = _current.RepairCategoryJunction;
            }
        }

        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem == null)
            {
                return;
            }

            var category = (RepairCategoryJunction)CategoriesDataGrid.SelectedItem;

            _current.RepairCategoryJunction.Remove(category);

            CategoriesDataGrid.ItemsSource = null;

            CategoriesDataGrid.ItemsSource = _current.RepairCategoryJunction;
        }

        private void LoadListData()
        {
            _customers = App.Connection.Customer.ToList();
            _realties = App.Connection.Realty.ToList();

            _employers = App.Connection.Employer.ToList();
            cbEmployers.ItemsSource = _employers;

            CategoriesDataGrid.ItemsSource = _current.RepairCategoryJunction;

            CustomerFilter();
            RealtyFilter();
        }

        private void CustomerFilter()
        {
            if (string.IsNullOrWhiteSpace(tbSearchCustomer.Text))
            {
                CustomersDataGrid.ItemsSource = _customers;
                return;
            }

            var filtered = _customers.Where(x => x.CustomerStr.ToLower().Contains(tbSearchCustomer.Text.ToLower()) || x.CustomerID.ToString().ToLower().Contains(tbSearchCustomer.Text.ToLower())).ToList();

            CustomersDataGrid.ItemsSource = filtered;
        }

        private void RealtyFilter()
        {
            if (string.IsNullOrWhiteSpace(tbSearchRealty.Text))
            {
                RealtyDataGrid.ItemsSource = _realties;
                return;
            }

            var filtered = _realties.Where(x => x.Type.ToLower().Contains(tbSearchRealty.Text.ToLower()) || x.RealtyID.ToString().ToLower().Contains(tbSearchRealty.Text.ToLower()) || x.Rooms.ToString().ToLower().Contains(tbSearchRealty.Text.ToLower())).ToList();

            RealtyDataGrid.ItemsSource = filtered;
        }

        private void tbSearchCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            CustomerFilter();
        }

        private void tbSearchRealty_TextChanged(object sender, TextChangedEventArgs e)
        {
            RealtyFilter();
        }

        private void AddOrder(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpStartDate.SelectedDate == null ||
                    dpPlainEndDate.SelectedDate == null ||
                    cbEmployers.SelectedItem == null ||
                    string.IsNullOrEmpty(tbTotalCost.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (CustomersDataGrid.SelectedItem == null)
                {
                    MessageBox.Show("Выберите заказчика!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (RealtyDataGrid.SelectedItem == null)
                {
                    MessageBox.Show("Выберите недвижимость!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_current.RepairCategoryJunction.Count == 0)
                {
                    MessageBox.Show("Укажите хотя бы один категорий!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                decimal cost;

                if (!decimal.TryParse(tbTotalCost.Text.Replace('.', ','), out cost))
                {
                    MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (cost < 0)
                {
                    MessageBox.Show("Стоиомость не может быть меньше 0!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(tbDiscount.Text))
                {
                    decimal discount;

                    if (!decimal.TryParse(tbDiscount.Text.Replace('.', ','), out discount))
                    {
                        MessageBox.Show("Введите корректную скидку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (discount < 0 || discount > 100)
                    {
                        MessageBox.Show("Скидка не может быть меньше 0 и больше 100!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _current.Discount = discount;
                }

                    DateTime startDate;

                if (!DateTime.TryParse(dpStartDate.Text, out startDate))
                {
                    MessageBox.Show("Введите корректную дату начала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime plainEndDate;

                if (!DateTime.TryParse(dpPlainEndDate.Text, out plainEndDate))
                {
                    MessageBox.Show("Введите корректную дату планируемого окончания!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (dpDateEnd.SelectedDate != null)
                {
                    DateTime endDate;

                    if (!DateTime.TryParse(dpDateEnd.Text, out endDate))
                    {
                        MessageBox.Show("Введите корректную дату окончания!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _current.DateEnd = endDate;
                }

                _current.DateStart = startDate;
                _current.PlainDateEnd = plainEndDate;
                _current.TotalCost = cost;
                _current.Comment = string.IsNullOrWhiteSpace(tbComment.Text) ? "" : tbComment.Text;
                _current.Employer = (Employer) cbEmployers.SelectedItem;
                _current.Customer = (Customer) CustomersDataGrid.SelectedItem;
                _current.Realty = (Realty) RealtyDataGrid.SelectedItem;

                if (_isNew)
                {
                    App.Connection.Repair.Add(_current);
                }
                else
                {
                    var existCategories = App.Connection.RepairCategoryJunction.Where(x => x.OrderID == _current.OrderID).ToList();

                    foreach(var c in existCategories)
                    {
                        var existCategory = _current.RepairCategoryJunction.FirstOrDefault(x => x.RepairCategory.CategoryID == c.CategoryID);

                        if (existCategory == null)
                        {
                            App.Connection.RepairCategoryJunction.Remove(c);
                        }
                    }

                    App.Connection.Repair.AddOrUpdate(_current);
                }

                App.Connection.SaveChanges();
                MessageBox.Show(_isNew ? "Успешно добавлено!" : "Успешно обновлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetCost()
        {
            if (!string.IsNullOrEmpty(tbDiscount.Text) && !string.IsNullOrEmpty(tbTotalCost.Text))
            {
                decimal discount;

                if (!decimal.TryParse(tbDiscount.Text.Replace('.', ','), out discount))
                {
                    tbCost.Text = "";
                    return;
                }

                if (discount < 0 || discount > 100)
                {
                    tbCost.Text = "";
                    return;
                }

                decimal totalCost;

                if (!decimal.TryParse(tbTotalCost.Text.Replace('.', ','), out totalCost))
                {
                    tbCost.Text = "";
                    return;
                }

                if (totalCost < 0)
                {
                    tbCost.Text = "";
                    return;
                }

                tbCost.Text = Math.Round((totalCost - (totalCost * discount / 100)), 2).ToString();

                return;
            }

            if (string.IsNullOrEmpty(tbDiscount.Text) && !string.IsNullOrEmpty(tbTotalCost.Text))
            {
                decimal totalCost;

                if (!decimal.TryParse(tbTotalCost.Text.Replace('.', ','), out totalCost))
                {
                    tbCost.Text = "";
                    return;
                }

                if (totalCost < 0)
                {
                    tbCost.Text = "";
                    return;
                }

                tbCost.Text = Math.Round(totalCost).ToString();
                return;
            }

            tbCost.Text = "";
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbTotalCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetCost();
        }

        private void tbDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetCost();
        }
    }
}
