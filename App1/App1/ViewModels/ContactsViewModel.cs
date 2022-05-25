using App1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App1.ViewModels
{
    public class ContactsViewModel : BaseViewModel
    {
        // -- Attributes

        // -- Commands

        /// <summary>
        /// Observable Collection of all persisted Contacts
        /// </summary>
        public ObservableCollection<Models.Contact> Contacts { get; }
        /// <summary>
        /// Command to load all persisted Contacts
        /// </summary>
        public Command LoadContactsCommand { get; }
        /// <summary>
        /// Command to add a new Contact
        /// </summary>
        public Command AddContactCommand { get; }
        /// <summary>
        /// Command to open a detailed view of tapped Contact
        /// </summary>
        public Command<Models.Contact> ItemTapped { get; }
        /// <summary>
        /// Command to open the mobile phone app with the specified phoneNumber of the corresponding Contact
        /// </summary>
        public Command<Models.Contact> CallTapped { get; }
        /// <summary>
        /// Command to open the mobile mail app with the specified emails of the corresponding Contact
        /// </summary>
        public Command<Models.Contact> MailTapped { get; }
        /// <summary>
        /// Command to open the mobile SMS app with the specified phoneNumber of the corresponding Contact
        /// </summary>
        public Command<Models.Contact> SMSTapped { get; }
        /// <summary>
        /// TODO: Command to search specific Contacts from the Observable Collection of Contacts
        /// </summary>
        public Command<string> SearchContacts { get; }

        // -- Constructors

        /// <summary>
        /// Constructor of ViewModel for viewing all persisted Contacts.
        /// Initializes Attributes and specifies Commands.
        /// </summary>
        public ContactsViewModel()
        {
            // Initialize attributes
            Title = "Browse Contacts";

            // Specify commands
            Contacts = new ObservableCollection<Models.Contact>();
            // directly execute command
            LoadContactsCommand = new Command(async () => await ExecuteLoadContactsCommand());

            ItemTapped = new Command<Models.Contact>(OnContactSelected);

            CallTapped = new Command<Models.Contact>(OnCallContactSelected);

            MailTapped = new Command<Models.Contact>(OnMailContactSelected);

            SMSTapped = new Command<Models.Contact>(OnSMSContactSelected);

            //SearchContacts = new Command<string>(PerformSearch);

            AddContactCommand = new Command(OnAddContact);
        }

        // -- Specific Functions

        /// <summary>
        /// Asynchronous method loading all Contacts from database.
        /// </summary>
        /// <returns>Returns Task for LoadContactsCommand lambda function</returns>
        private async Task ExecuteLoadContactsCommand()
        {
            IsBusy = true;

            try
            {
                Contacts.Clear();
                var contacts = await _ContactDatabase.GetContactsAsync(false);
                foreach (var contact in contacts)
                {
                    Contacts.Add(contact);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// State function called as soon as this ViewModel has been instantiated
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true;
        }

        /// <summary>
        /// Asynchronous method to open a new View to create a new Contact.
        /// </summary>
        /// <param name="obj"></param>
        private async void OnAddContact(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        /// <summary>
        /// Asynchronous method to open a new View with detailed information about the selected (tapped) Contact
        /// </summary>
        /// <param name="contact"></param>
        private async void OnContactSelected(Models.Contact contact)
        {
            if (contact == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ContactId)}={contact.Id}");
        }

        private async void OnCallContactSelected(Models.Contact contact)
        {
            if (contact.PhoneNumbers == null)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Please specify a Phone Number", "OK");
                return;
            };
            try
            {
                PhoneDialer.Open(contact.PhoneNumbers);
            }
            catch (ArgumentNullException anEx)
            {
                Debug.WriteLine(anEx.Message);
                // Number was null or white space
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Debug.WriteLine(ex.Message);
            }
        }

        /*
        async void PerformSearch(string searchText)
        {
 
        }
        */

        private async void OnMailContactSelected(Models.Contact contact)
        {
            if (contact.Emails == null)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Please specify an Email Address", "OK");
                return;
            };
            try
            {
                var message = new EmailMessage
                {
                    Subject = "",
                    Body = "",
                    To = new List<string>() { contact.Emails },
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
                Debug.WriteLine(fbsEx.Message);
            }
            catch (Exception ex)
            {
                // Some other exception occurred
                Debug.WriteLine(ex.Message);
            }
        }

        private async void OnSMSContactSelected(Models.Contact contact)
        {
            if (contact.PhoneNumbers == null)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Please specify a Phone Number", "OK");
                return;
            };
            try
            {
                var message = new SmsMessage("", contact.PhoneNumbers);
                await Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Sms is not supported on this device.
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Debug.WriteLine(ex.Message);
            }
        }

        private async void OnMapSelected(Models.Contact contact)
        {

            // Load contact addresses and check whether they are valid or not
            // Idea for validation: specify "valid" attribute for any given address entry
            if (contact.Address == null)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Please specify a valid Address for this Contact", "OK");
                return;
            };

            // Open map with all (valid) addresses with all the requirements from the ticket
            try
            {
                var message = new SmsMessage("", contact.PhoneNumbers);
                await Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Sms is not supported on this device.
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Debug.WriteLine(ex.Message);
            }
        }
    }
}