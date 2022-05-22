using App1.ViewModels;
using Xamarin.Forms;

namespace App1.Views
{
    public partial class NewItemPage : ContentPage
    {
        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewContactViewModel();
        }
    }
}