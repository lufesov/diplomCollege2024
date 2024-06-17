
namespace Spectr.Database
{
    public partial class Customer
    {
        public string CustomerStr => $"{CustomerSecondName} {CustomerFirstName} {CustomerPatronymic}";
    }
}
