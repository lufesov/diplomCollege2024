using Spectr.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace Spectr.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow Window;
        public List<Customer> Customers { get; set; }
        public List<Realty> Realties { get; set; }
        public List<Repair> Repairs { get; set; }
        public List<Employer> Employers { get; set; }

        public Customer SelectedClient { get; set; }
        public Realty SelectedRealty { get; set; }
        public Repair SelectedRepair { get; set; }
        public Employer SelectedEmployer { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Window = this;

            LoadListData();

            DataContext = this;
        }

        private void LoadListData()
        {
            Customers = App.Connection.Customer.ToList();
            CustomerDataGrid.ItemsSource = Customers;

            Realties = App.Connection.Realty.ToList();
            RealtyDataGrid.ItemsSource = Realties;

            Repairs = App.Connection.Repair.ToList();
            RepairOrderDataGrid.ItemsSource = Repairs;

            Employers = App.Connection.Employer.ToList();
            EmployersDataGrid.ItemsSource = Employers;

            var positions = App.Connection.EmployerPosition.ToList();

            cbAddEmployerPositions.ItemsSource = positions;
            cbUpdateEmployerPositions.ItemsSource = positions;
        }

        #region Клиент

        private void CreateClientEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbClientAddDocNumber.Text) ||
                    string.IsNullOrEmpty(tbClientAddFirstName.Text) ||
                    string.IsNullOrEmpty(tbClientAddSecondName.Text) ||
                    string.IsNullOrEmpty(tbClientAddPhoneNumber.Text) ||
                    string.IsNullOrEmpty(tbClientAddPatronymic.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                long docNumber;

                var number = tbClientAddDocNumber.Text.Replace(" ", "");

                if (!long.TryParse(number, out docNumber))
                {
                    MessageBox.Show("Введите корректный номер документа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if ((!tbClientAddPhoneNumber.Text.All(char.IsDigit) && !tbClientAddPhoneNumber.Text.All(c => char.IsWhiteSpace(c) || c == '(' || c == ')')))
                {
                    MessageBox.Show("Введите корректный номер телефона! Не больше 10 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                App.Connection.Customer.Add(new Customer()
                {
                    DocNumber = tbClientAddDocNumber.Text,
                    PhoneNumber = tbClientAddPhoneNumber.Text,
                    CustomerFirstName = tbClientAddFirstName.Text,
                    CustomerSecondName = tbClientAddSecondName.Text,
                    CustomerPatronymic = tbClientAddPatronymic.Text,
                    EmailAdress = tbClientAddEmail.Text
                });

                App.Connection.SaveChanges();
                MessageBox.Show("Успешно добавлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadListData();
                ClearAddClient();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearAddClientInfo(object sender, RoutedEventArgs e)
        {
            ClearAddClient();
        }

        private void UpdateClientEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedClient == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(tbClientUpdateDocNumber.Text) ||
                    string.IsNullOrEmpty(tbClientUpdateFirstName.Text) ||
                    string.IsNullOrEmpty(tbClientUpdateSecondName.Text) ||
                    string.IsNullOrEmpty(tbClientUpdatePhoneNumber.Text) ||
                    string.IsNullOrEmpty(tbClientUpdatePatronymic.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                long docNumber;

                var number = tbClientUpdateDocNumber.Text.Replace(" ", "");

                if (!long.TryParse(number, out docNumber))
                {
                    MessageBox.Show("Введите корректный номер документа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if ((!tbClientUpdatePhoneNumber.Text.Trim().All(char.IsDigit) && !tbClientUpdatePhoneNumber.Text.All(c => char.IsWhiteSpace(c) || c == '(' || c == ')')))
                {
                    MessageBox.Show("Введите корректный номер телефона! Не больше 11 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var customerDb = App.Connection.Customer.FirstOrDefault(x => x.CustomerID == SelectedClient.CustomerID);

                if (customerDb == null)
                {
                    return;
                }

                customerDb.DocNumber = tbClientUpdateDocNumber.Text.Trim();
                customerDb.CustomerFirstName = tbClientUpdateFirstName.Text;
                customerDb.CustomerSecondName = tbClientUpdateSecondName.Text;
                customerDb.CustomerPatronymic = tbClientUpdatePatronymic.Text;
                customerDb.PhoneNumber = tbClientUpdatePhoneNumber.Text.Trim();
                customerDb.EmailAdress = tbClientUpdateEmail.Text;


                App.Connection.Customer.AddOrUpdate(customerDb);

                App.Connection.SaveChanges();
                MessageBox.Show("Успешно обновлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadListData();
                ClearUpdateClient();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteClient(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedClient == null)
                {
                    return;
                }

                var repair = App.Connection.Repair.FirstOrDefault(x => x.CustomerID == SelectedClient.CustomerID);

                if (repair != null)
                {
                    MessageBox.Show($"Не удалось удалить, т.к. данный клиент указан в заказе №{repair.OrderID}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); ; ;
                    return;
                }

                App.Connection.Customer.Remove(SelectedClient);
                App.Connection.SaveChanges();
                MessageBox.Show("Клиент успешно удален!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadListData();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void  ClearAddClient()
        {
            tbClientAddDocNumber.Text = "";
            tbClientAddPhoneNumber.Text = "";
            tbClientAddFirstName.Text = "";
            tbClientAddSecondName.Text = "";
            tbClientAddPatronymic.Text = "";
            tbClientAddEmail.Text = "";
        }

        private void ClearUpdateClient()
        {
            tbClientUpdateDocNumber.Text = "";
            tbClientUpdatePhoneNumber.Text = "";
            tbClientUpdateFirstName.Text = "";
            tbClientUpdateSecondName.Text = "";
            tbClientUpdatePatronymic.Text = "";
            tbClientUpdateEmail.Text = "";

            CustomerDataGrid.SelectedIndex = -1;
            SelectedClient = null;
        }

        private void ClearUpdateClientInfo(object sender, RoutedEventArgs e)
        {
            ClearUpdateClient();
        }

        private void CustomerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedClient = (Customer) CustomerDataGrid.SelectedItem;
            DataContext = this;
        }

        #endregion

        #region Приложение

        private void WindowMoving(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Window.DragMove();
            }
        }

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Недвижимость

        private void ClearAddRealtyInfo(object sender, RoutedEventArgs e)
        {
            ClearAddRealty();
        }

        private void ClearUpdateRealtyInfo(object sender, RoutedEventArgs e)
        {
            ClearUpdateClient();
        }

        private void AddRealtyEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbRealtyAddType.Text) ||
                    string.IsNullOrEmpty(tbRealtyAddRooms.Text) ||
                    string.IsNullOrEmpty(tbRealtyAddMYear.Text) ||
                    string.IsNullOrEmpty(tbRealtyAddFloor.Text) ||
                    string.IsNullOrEmpty(tbRealtyAddCompany.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int floor;

                if (!int.TryParse(tbRealtyAddFloor.Text, out floor))
                {
                    MessageBox.Show("Введите корректный этаж!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int rooms;

                if (!int.TryParse(tbRealtyAddRooms.Text, out rooms))
                {
                    MessageBox.Show("Введите корректное количество комнат!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int year;

                if (!int.TryParse(tbRealtyAddMYear.Text, out year))
                {
                    MessageBox.Show("Введите корректный год!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newRealty = new Realty()
                {
                    Type = tbRealtyAddType.Text,
                    Floors = int.Parse(tbRealtyAddFloor.Text),
                    Rooms = int.Parse(tbRealtyAddRooms.Text),
                    Company = tbRealtyAddCompany.Text,
                    MYear = int.Parse(tbRealtyAddMYear.Text)
                };

                App.Connection.Realty.Add(newRealty);

                App.Connection.SaveChanges();
                MessageBox.Show("Успешно добавлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadListData();
                ClearAddRealty();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRealtyEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedRealty == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(tbRealtyUpdateType.Text) ||
                    string.IsNullOrEmpty(tbRealtyUpdateRooms.Text) ||
                    string.IsNullOrEmpty(tbRealtyUpdateMYear.Text) ||
                    string.IsNullOrEmpty(tbRealtyUpdateFloor.Text) ||
                    string.IsNullOrEmpty(tbRealtyUpdateCompany.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int floor;

                if (!int.TryParse(tbRealtyUpdateFloor.Text, out floor))
                {
                    MessageBox.Show("Введите корректный этаж!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int rooms;

                if (!int.TryParse(tbRealtyUpdateRooms.Text, out rooms))
                {
                    MessageBox.Show("Введите корректное количество комнат!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int year;

                if (!int.TryParse(tbRealtyUpdateMYear.Text, out year))
                {
                    MessageBox.Show("Введите корректный год!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var realtyDb = App.Connection.Realty.FirstOrDefault(x => x.RealtyID == SelectedRealty.RealtyID);

                if (realtyDb == null)
                {
                    return;
                }

                realtyDb.Type = tbRealtyUpdateType.Text;
                realtyDb.Floors = int.Parse(tbRealtyUpdateFloor.Text);
                realtyDb.Rooms = int.Parse(tbRealtyUpdateRooms.Text);
                realtyDb.Company = tbRealtyUpdateCompany.Text;
                realtyDb.MYear = int.Parse(tbRealtyUpdateMYear.Text);

                App.Connection.Realty.AddOrUpdate(realtyDb);

                App.Connection.SaveChanges();
                MessageBox.Show("Успешно обновлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadListData();
                ClearUpdateRealty();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRealtyEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedRealty == null)
                {
                    return;
                }

                var repair = App.Connection.Repair.FirstOrDefault(x => x.Realty.RealtyID == SelectedRealty.RealtyID);

                if (repair != null)
                {
                    MessageBox.Show($"Не удалось удалить, т.к. данная недвижимость указана в заказе №{repair.OrderID}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); ; ;
                    return;
                }

                App.Connection.Realty.Remove(SelectedRealty);
                App.Connection.SaveChanges();
                MessageBox.Show("Недвижимость успешно удалена!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadListData();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearAddRealty()
        {
            tbRealtyAddType.Text = "";
            tbRealtyAddRooms.Text = "";
            tbRealtyAddMYear.Text = "";
            tbRealtyAddFloor.Text = "";
            tbRealtyAddCompany.Text = "";
        }

        private void ClearUpdateRealty()
        {
            tbRealtyUpdateType.Text = "";
            tbRealtyUpdateRooms.Text = "";
            tbRealtyUpdateMYear.Text = "";
            tbRealtyUpdateFloor.Text = "";
            tbRealtyUpdateCompany.Text = "";

            RealtyDataGrid.SelectedIndex = -1;
            SelectedRealty = null;
        }

        private void RealtyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedRealty = (Realty) RealtyDataGrid.SelectedItem;
        }
        #endregion

        #region Заказы

        private void AddOrderEvent(object sender, RoutedEventArgs e)
        {
            var win = new EditOrderWindow(null);

            if (win.ShowDialog() == true)
            {
                LoadListData();
            }
        }

        private void DeleteOrderEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedRepair == null)
                {
                    return;
                }

                var categories = App.Connection.RepairCategoryJunction.Where(x => x.OrderID == SelectedRepair.OrderID);

                foreach(var c in categories)
                {
                    App.Connection.RepairCategoryJunction.Remove(c);
                }

                App.Connection.Repair.Remove(SelectedRepair);
                App.Connection.SaveChanges();
                MessageBox.Show("Заказ успешно удален!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadListData();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditOrderEvent(object sender, RoutedEventArgs e)
        {
            if (SelectedRepair == null)
            {
                return;
            }

            var win = new EditOrderWindow(SelectedRepair);

            if (win.ShowDialog() == true)
            {
                LoadListData();
            }
        }

        private void RepairOrderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedRepair = (Repair) RepairOrderDataGrid.SelectedItem;
        }

        private void ExportOrdersData(object sender, RoutedEventArgs e)
        {
            Excel.Application exApp = new Excel.Application();
            var workBook = exApp.Workbooks.Add();
            Excel.Worksheet workSheet = (Excel.Worksheet)exApp.ActiveSheet;

            workSheet.Name = "Заказы";

            workSheet.Cells[1, 1] = "ID";
            workSheet.Cells[1, 2] = "Дата начала";
            workSheet.Cells[1, 3] = "Планируемая дата окончания";
            workSheet.Cells[1, 4] = "Дата окончания";
            workSheet.Cells[1, 5] = "Работник";
            workSheet.Cells[1, 6] = "Клиент";
            workSheet.Cells[1, 7] = "ID недвижимости";
            workSheet.Cells[1, 8] = "Скидка";
            workSheet.Cells[1, 9] = "Цена";
            workSheet.Cells[1, 10] = "Итоговая цена (со скидкой)";
            workSheet.Cells[1, 11] = "Комментарий";
            workSheet.Cells[1, 12] = "Статус";
            workSheet.Cells[1, 13] = "Категории";

            int rowExcel = 2;
            foreach (var order in Repairs)
            {
                workSheet.Cells[rowExcel, 1] = order.OrderID;
                workSheet.Cells[rowExcel, 2] = order.StartDateStr;
                workSheet.Cells[rowExcel, 3] = order.PlainDateEnd.ToString("d");
                workSheet.Cells[rowExcel, 4] = order.EndDateStr == null ? "" : order.EndDateStr;
                workSheet.Cells[rowExcel, 5] = order.EmployerStr;
                workSheet.Cells[rowExcel, 6] = order.CustomerStr;
                workSheet.Cells[rowExcel, 7] = order.Realty.RealtyID;
                workSheet.Cells[rowExcel, 8] = order.Discount == null ? 0 : order.Discount;
                workSheet.Cells[rowExcel, 9] = order.TotalCost;
                workSheet.Cells[rowExcel, 10] = order.Cost;
                workSheet.Cells[rowExcel, 11] = order.Comment == null ? "" : order.Comment;
                workSheet.Cells[rowExcel, 12] = order.Status;
                workSheet.Cells[rowExcel, 13] = order.CategoriesStr;
                ++rowExcel;
            }


            exApp.Visible = true;
        }
        #endregion

        #region Сотрудник
        private void AddEmployerEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbEmployerAddFirstName.Text) ||
                    string.IsNullOrEmpty(tbEmployerAddSecondName.Text) ||
                    string.IsNullOrEmpty(tbEmployerAddPhoneNUmber.Text) ||
                    string.IsNullOrEmpty(tbEmployerAddSalary.Text) ||
                    cbAddEmployerPositions.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                decimal salary;

                var sal = tbEmployerAddSalary.Text.Replace(".", ",");

                if (!decimal.TryParse(sal, out salary))
                {
                    MessageBox.Show("Введите корректную зарплату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if ((!tbEmployerAddPhoneNUmber.Text.All(char.IsDigit) && !tbEmployerAddPhoneNUmber.Text.All(c => char.IsWhiteSpace(c) || c == '(' || c == ')')))
                {
                    MessageBox.Show("Введите корректный номер телефона! Не больше 10 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                App.Connection.Employer.Add(new Employer()
                {
                    EmFirstName = tbEmployerAddFirstName.Text,
                    EmSecondName = tbEmployerAddSecondName.Text,
                    PhoneNumber = tbEmployerAddPhoneNUmber.Text,
                    Salary = salary,
                    EmployerPosition = (EmployerPosition) cbAddEmployerPositions.SelectedItem
                });

                App.Connection.SaveChanges();
                MessageBox.Show("Успешно добавлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadListData();
                ClearAddEmployer();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearAddEmployerInfo(object sender, RoutedEventArgs e)
        {
            ClearAddEmployer();
        }

        private void UpdateEmployerEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedEmployer == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(tbEmployerUpdateFirstName.Text) ||
                    string.IsNullOrEmpty(tbEmployerUpdateSecondName.Text) ||
                    string.IsNullOrEmpty(tbEmployerUpdatePhoneNUmber.Text) ||
                    string.IsNullOrEmpty(tbEmployerUpdateSalary.Text) ||
                    cbUpdateEmployerPositions.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                decimal salary;

                var sal = tbEmployerUpdateSalary.Text.Replace(".", ",");

                if (!decimal.TryParse(sal, out salary))
                {
                    MessageBox.Show("Введите корректную зарплату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if ((!tbEmployerUpdatePhoneNUmber.Text.All(char.IsDigit) && !tbEmployerUpdatePhoneNUmber.Text.All(c => char.IsWhiteSpace(c) || c == '(' || c == ')')))
                {
                    MessageBox.Show("Введите корректный номер телефона! Не больше 10 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var employerDb = App.Connection.Employer.FirstOrDefault(x => x.EmployerID == SelectedEmployer.EmployerID);

                if (employerDb == null)
                {
                    return;
                }

                employerDb.EmFirstName = tbEmployerUpdateFirstName.Text;
                employerDb.EmSecondName = tbEmployerUpdateSecondName.Text;
                employerDb.PhoneNumber = tbEmployerUpdatePhoneNUmber.Text;
                employerDb.Salary = salary;
                employerDb.EmployerPosition = (EmployerPosition)cbUpdateEmployerPositions.SelectedItem;

                App.Connection.Employer.AddOrUpdate(employerDb);
                App.Connection.SaveChanges();
                MessageBox.Show("Успешно обновлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadListData();
                ClearUpdateEmployer();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearUpdateEmployerInfo(object sender, RoutedEventArgs e)
        {
            ClearUpdateEmployer();
        }

        private void DeleteEmployerEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedEmployer == null)
                {
                    return;
                }

                var repair = App.Connection.Repair.FirstOrDefault(x => x.EmployerID == SelectedEmployer.EmployerID);

                if (repair != null)
                {
                    MessageBox.Show($"Не удалось удалить, т.к. данный сотрудник указан в заказе №{repair.OrderID}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); ; ;
                    return;
                }

                App.Connection.Employer.Remove(SelectedEmployer);
                App.Connection.SaveChanges();
                MessageBox.Show("Сотрудник успешно удален!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadListData();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearAddEmployer()
        {
            tbEmployerAddFirstName.Text = "";
            tbEmployerAddSecondName.Text = "";
            tbEmployerAddPhoneNUmber.Text = "";
            tbEmployerAddSalary.Text = "";
            cbAddEmployerPositions.SelectedIndex = -1;
        }

        private void ClearUpdateEmployer()
        {
            SelectedEmployer = null;
            EmployersDataGrid.SelectedIndex = -1;
            tbEmployerUpdateFirstName.Text = "";
            tbEmployerUpdateSecondName.Text = "";
            tbEmployerUpdatePhoneNUmber.Text = "";
            tbEmployerUpdateSalary.Text = "";
            cbUpdateEmployerPositions.SelectedIndex = -1;
        }

        private void EmployersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedEmployer = (Employer) EmployersDataGrid.SelectedItem;
        }
        #endregion

    }
}
