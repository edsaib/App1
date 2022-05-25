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

        // -- Attributes

        private int contactId;
        private string fullName;
        private string description;
        private string phoneNumbers;
        private string emails;
        private string address;
        private string contactImageSource_string;
        private ImageSource contactImageSource;
        private DateTime dob;
        private DateTime createdDate;
        private DateTime lastEdited;
        private DateTime lastContacted;
        private string dob_string;
        private string createdDate_string;
        private string lastEdited_string;
        private string lastContacted_string;

        // -- Constructors

        /// <summary>
        /// Constructor of ViewModel for viewing details of a given Contact.
        /// Initializes Attributes and specifies Commands.
        /// </summary>
        public ItemDetailViewModel()
        {
            // Set attributes
            contactId = -1;

            // Specify commands
            EditContactCommand = new Command(OnEditSelectedContact);
            RemoveContactCommand = new Command(RemoveContact);
        }

        // -- Attribute Getter/Setter

        /// <summary>
        /// Get/Set fullName string atttribute for fullName Label component
        /// </summary>
        public string FullName
        {
            get => fullName;
            set => SetProperty(ref fullName, value);
        }

        /// <summary>
        /// Get/Set emails string atttribute for emails Label component
        /// </summary>
        public string Emails
        {
            get => emails;
            set => SetProperty(ref emails, value);
        }

        /// <summary>
        /// Get/Set phoneNumbers string atttribute for phoneNumbers Label component
        /// </summary>
        public string PhoneNumbers
        {
            get => phoneNumbers;
            set => SetProperty(ref phoneNumbers, value);
        }

        /// <summary>
        /// Get/Set address string atttribute for address Label component
        /// </summary>
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        /// <summary>
        /// Get/Set (date of birth) dob DateTime atttribute and set DoB_String accordingly
        /// </summary>
        public DateTime DoB
        {
            get => dob;
            set
            {
                SetProperty(ref dob, value);
                DoB_String = value.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// Get/Set createdDate DateTime atttribute and set CreatedDate_String accordingly
        /// </summary>
        public DateTime CreatedDate
        {
            get => createdDate;
            set
            {
                SetProperty(ref dob, value);
                CreatedDate_String = value.ToString("yyyy-MM-dd, HH':'mm");
            }
        }

        /// <summary>
        /// Get/Set lastEdited DateTime atttribute and set LastEdited_String accordingly
        /// </summary>
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

        /// <summary>
        /// Get/Set lastContacted DateTime atttribute and set LastContacted_String accordingly
        /// </summary>
        public DateTime LastContacted
        {
            get => lastContacted;
            set
            {
                SetProperty(ref dob, value);
                LastContacted_String = value.ToString("yyyy-MM-dd, HH':'mm");
            }
        }

        /// <summary>
        /// Get/Set contactImageSource_string string atttribute
        /// </summary>
        public string ContactImgSource_String
        {
            get => contactImageSource_string;
            set
            {
                SetProperty(ref contactImageSource_string, value);
            }
        }

        /// <summary>
        /// Get/Set contactImageSource ImageSource atttribute for Contact Image component
        /// </summary>
        public ImageSource ContactImgSource
        {
            get => contactImageSource;
            set
            {
                SetProperty(ref contactImageSource, value);
            }
        }

        /// <summary>
        /// Get/Set (date of birth) dob_string string atttribute for dob Label component
        /// </summary>
        public string DoB_String
        {
            get => dob_string;
            set => SetProperty(ref dob_string, value);
        }

        /// <summary>
        /// Get/Set createdDate_string string atttribute for createdDate Label component
        /// </summary>
        public string CreatedDate_String
        {
            get => createdDate_string;
            set => SetProperty(ref createdDate_string, value);
        }

        /// <summary>
        /// Get/Set lastEdited_string string atttribute for lastEdited Label component
        /// </summary>
        public string LastEdited_String
        {
            get => lastEdited_string;
            set => SetProperty(ref lastEdited_string, value);
        }

        /// <summary>
        /// Get/Set lastContacted_string string atttribute for lastContacted Label component
        /// </summary>
        public string LastContacted_String
        {
            get => lastContacted_string;
            set => SetProperty(ref lastContacted_string, value);
        }

        /// <summary>
        /// Get/Set description string atttribute for description Label component
        /// </summary>
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        /// <summary>
        /// Get/Set the contactId which has been passed by the previous view in order to View the details of an existing Contact.
        /// After setting the contactId the corresponding Contact is load from the local database (LoadContactId).
        /// </summary>
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

        // -- Command Getter

        /// <summary>
        /// Command to edit the current Contact
        /// </summary>
        public Command EditContactCommand { get; }
        /// <summary>
        /// Command to remove the current Contact
        /// </summary>
        public Command RemoveContactCommand { get; }

        // -- Specific Functions

        /// <summary>
        /// Asynchronous method which loads a Contact from the local database on the basis of a contactId.
        /// All attributes are set according to the loaded Contact.
        /// </summary>
        /// <param name="contactId">Contact Identifier which needs to be loaded from the local database</param>
        public async void LoadContactId(int contactId)
        {
            try
            {
                Contact contact = await _ContactDatabase.GetContactAsync(contactId);
                contactId = contact.Id;
                FullName = contact.FullName;
                Emails = contact.Emails;
                Address = contact.Address;
                PhoneNumbers = contact.PhoneNumbers;
                DoB = contact.DoB;
                CreatedDate = contact.CreatedDate;
                LastEdited = contact.LastEdited;
                LastContacted = contact.LastContacted;
                if(LastContacted == DateTime.MinValue)
                {
                    LastContacted_String = "";
                }
                Description = contact.Description;
                ContactImgSource_String = contact.ContactImageSource_String;
                ContactImgSource = ImageSource.FromFile(ContactImgSource_String);

                Debug.WriteLine("Contact image source : " + ContactImgSource);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        /// <summary>
        /// Asynchronous Method to pass the contactId of the currently viewed Contact to the next View, in order to edit this Contact.
        /// </summary>
        private async void OnEditSelectedContact()
        {
            if (contactId == -1)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(NewItemPage)}?{nameof(NewContactViewModel.ContactId)}={contactId}");
        }

        /// <summary>
        /// Asynchronous Method to remove the currently viewed Contact from the local database.
        /// </summary>
        private async void RemoveContact()
        {
            if (contactId == -1)
                return;

            var resp = await App.Current.MainPage.DisplayAlert("Remove Contact", "Are you sure?", "Remove", "Cancel");

            if(resp)
            {
                await _ContactDatabase.DeleteContactAsync(contactId);
                await Shell.Current.GoToAsync("..");
            } else
            {
                return;
            }
        }

    }
}
