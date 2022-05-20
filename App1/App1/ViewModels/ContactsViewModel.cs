using App1.Models;
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
        private Models.Contact _selectedContact;

        public ObservableCollection<Models.Contact> Contacts { get; }
        public Command LoadContactsCommand { get; }
        public Command AddContactCommand { get; }
        public Command<Models.Contact> ItemTapped { get; }
        public Command<Models.Contact> CallTapped { get; }
        public Command<Models.Contact> MailTapped { get; }
        public Command<Models.Contact> SMSTapped { get; }

        public ContactsViewModel()
        {
            Title = "Browse Contacts";
            Contacts = new ObservableCollection<Models.Contact>();
            LoadContactsCommand = new Command(async () => await ExecuteLoadContactsCommand());

            ItemTapped = new Command<Models.Contact>(OnContactSelected);

            CallTapped = new Command<Models.Contact>(OnCallContactSelected);

            MailTapped = new Command<Models.Contact>(OnMailContactSelected);

            SMSTapped = new Command<Models.Contact>(OnSMSContactSelected);

            AddContactCommand = new Command(OnAddContact);
        }

        async Task ExecuteLoadContactsCommand()
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

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedContact = null;
        }

        public Models.Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                SetProperty(ref _selectedContact, value);
                OnContactSelected(value);
            }
        }

        private async void OnAddContact(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnContactSelected(Models.Contact contact)
        {
            if (contact == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ContactId)}={contact.Id}");
        }

        void OnCallContactSelected(Models.Contact contact)
        {
            if (contact.PhoneNumbers == null) return;
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

        async void OnMailContactSelected(Models.Contact contact)
        {
            if (contact.Emails == null) return;
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

        async void OnSMSContactSelected(Models.Contact contact)
        {
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