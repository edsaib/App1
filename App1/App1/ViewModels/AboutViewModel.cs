using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App1.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        // -- Constructors

        /// <summary>
        /// Constructor of ViewModel for viewing details about the App.
        /// </summary>
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.betterbits.de/de/"));
        }

        // -- Commands

        /// <summary>
        /// Command to open an url (hardcoded in constructor)
        /// </summary>
        public ICommand OpenWebCommand { get; }
    }
}