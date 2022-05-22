using App1.ViewModels;
using Xamarin.Forms;

namespace App1.Views
{
    public partial class ContactsPage : ContentPage
    {
        ContactsViewModel _viewModel;

        public ContactsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ContactsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}