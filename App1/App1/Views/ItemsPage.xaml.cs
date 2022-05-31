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
            // Pass Navigation property to ViewModel context of this page
            BindingContext = _viewModel = new ContactsViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}