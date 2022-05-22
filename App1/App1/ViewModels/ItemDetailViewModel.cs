using App1.Models;
using App1.Views;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace App1.ViewModels
{
    [QueryProperty(nameof(ContactId), nameof(ContactId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        public Command EditContactCommand { get; }
        public Command RemoveContactCommand { get; }

        private Contact _contact;
        private int contactId;
        private string fullName;
        private string description;
        private DateTime dob;
        private DateTime createdDate;
        private DateTime lastEdited;
        private DateTime lastContacted;
        private string dob_string;
        private string createdDate_string;
        private string lastEdited_string;
        private string lastContacted_string;
        private string phoneNumbers;
        private string emails;
        private string address;
        private string contactImageSource_string;
        private ImageSource contactImageSource;

        public ItemDetailViewModel()
        {
            EditContactCommand = new Command(OnContactSelected);
            RemoveContactCommand = new Command(RemoveContact);
        }

        public int Id { get; set; }

        public string FullName
        {
            get => fullName;
            set => SetProperty(ref fullName, value);
        }

        public string Emails
        {
            get => emails;
            set => SetProperty(ref emails, value);
        }

        public string PhoneNumbers
        {
            get => phoneNumbers;
            set => SetProperty(ref phoneNumbers, value);
        }

        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        public DateTime DoB
        {
            get => dob;
            set
            {
                SetProperty(ref dob, value);
                DoB_String = value.ToString("yyyy-MM-dd");
            }
        }

        public DateTime CreatedDate
        {
            get => createdDate;
            set
            {
                SetProperty(ref dob, value);
                CreatedDate_String = value.ToString("yyyy-MM-dd, HH':'mm");
            }
        }

        public DateTime LastEdited
        {
            get => lastEdited;
            set
            {
                SetProperty(ref dob, value);
                if (value == DateTime.MinValue)
                {
                    LastEdited_String = "";
                }
                else
                {
                    LastEdited_String = value.ToString("yyyy-MM-dd, HH':'mm");
                }

            }
        }

        public DateTime LastContacted
        {
            get => lastContacted;
            set
            {
                SetProperty(ref dob, value);
                LastContacted_String = value.ToString("yyyy-MM-dd, HH':'mm");
            }
        }

        public string ContactImgSource_String
        {
            get => contactImageSource_string;
            set
            {
                SetProperty(ref contactImageSource_string, value);
                ContactImgSource = ImageSource.FromFile(value);
                //ContactImgSource = new Uri(value, UriKind.Absolute); // convert string to image source
            }
        }

        public ImageSource ContactImgSource
        {
            get => contactImageSource;
            set
            {
                SetProperty(ref contactImageSource, value);
            }
        }

        public string DoB_String
        {
            get => dob_string;
            set => SetProperty(ref dob_string, value);
        }

        public string CreatedDate_String
        {
            get => createdDate_string;
            set => SetProperty(ref createdDate_string, value);
        }

        public string LastEdited_String
        {
            get => lastEdited_string;
            set => SetProperty(ref lastEdited_string, value);
        }

        public string LastContacted_String
        {
            get => lastContacted_string;
            set => SetProperty(ref lastContacted_string, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public int ContactId
        {
            get
            {
                return contactId;
            }
            set
            {
                contactId = value;
                LoadContactId(value);
            }
        }

        public async void LoadContactId(int contactId)
        {
            try
            {
                var contact = await _ContactDatabase.GetContactAsync(contactId);
                _contact = contact;
                Id = contact.Id;
                FullName = contact.FullName;
                Emails = contact.Emails;
                Address = contact.Address;
                PhoneNumbers = contact.PhoneNumbers;
                DoB = contact.DoB;
                CreatedDate = contact.CreatedDate;
                LastEdited = contact.LastEdited;
                LastContacted = contact.LastContacted;
                Description = contact.Description;
                ContactImgSource_String = contact.ContactImageSource_String;
                Debug.WriteLine("Contact image source : " + ContactImgSource);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnContactSelected()
        {
            if (_contact == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(NewItemPage)}?{nameof(NewContactViewModel.ContactId)}={_contact.Id}");
        }


        async void RemoveContact(object obj)
        {
            await _ContactDatabase.DeleteContactAsync(Id);
            await Shell.Current.GoToAsync("..");
        }

    }
}
