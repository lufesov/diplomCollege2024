using System;

namespace Spectr.Database
{
    public partial class Repair
    {
        public string StartDateStr => DateStart.ToString("d");
        public string EndDateStr => DateEnd == null ? "" : ((DateTime)DateEnd).ToString("d");
        public string CustomerStr => $"{Customer.CustomerSecondName} {Customer.CustomerFirstName} {Customer.CustomerPatronymic}";
        public string EmployerStr => $"{Employer.EmSecondName} {Employer.EmFirstName}";
        public string CategoriesStr => GetCategories();
        public decimal Cost => GetCost();

        private string GetCategories()
        {
            var str = "";

            foreach(var c in RepairCategoryJunction)
            {
                str += c.RepairCategory.Category + ", ";
            }

            return str.Substring(0, str.Length - 2);
        }

        private decimal GetCost()
        {
            if (Discount != null)
            {
                return Math.Round((TotalCost - (TotalCost * (decimal)Discount / 100)), 2);
            }

            return Math.Round(TotalCost, 2);
        }
    }
}
