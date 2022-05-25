using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App1.ViewModels
{
    [QueryProperty(nameof(ContactId), nameof(ContactId))]
    public class NewContactViewModel : BaseViewModel
    {
        // -- Attributes

        //private bool isValid;
        private bool fullNameIsValid;
        private bool emailIsValid;
        private bool addressIsValid;
        private bool phoneNumberIsValid;

        private int contactId;
        private string fullName;
        private string description;
        private DateTime dob;
        private string phoneNumbers;
        private string emails;
        private string address;
        private ImageSource contactImageSource;
        private string contactImageSource_string;
        private DateTime lastContacted;
        private DateTime createdDate;
        private DateTime lastEdited;
        private DateTime propertyMinimumDate;
        private DateTime propertyMaximumDate;

        // -- Constructors

        /// <summary>
        /// Constructor of ViewModel for creating/editing a Contact.
        /// Initializes Attributes and specifies Commands.
        /// </summary>
        public NewContactViewModel()
        {
            // Initial validation
            PhoneNumberIsValid = false;
            FullNameIsValid = false;

            // Set attributes
            contactId = -1;
            DoB = DateTime.Now;
            PropertyMaximumDate = DateTime.Now;
            PropertyMinimumDate = new DateTime(1900, 1, 1);
            CreatedDate = DateTime.Now;
            LastEdited = DateTime.Now;
            LastContacted = DateTime.MinValue;
            ContactImgSource_String = "take_photo.png";
            ContactImgSource = ImageSource.FromFile(ContactImgSource_String);

            // Specify commands
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            ButtonCapture = new Command(ButtonCapture_ClickedAsync);
            ButtonChoose = new Command(ButtonChoose_ClickedAsync);

            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }


        // -- Attribute Getter/Setter

        /// <summary>
        /// Get/Set fullNameIsValid bool attribute for validation of fullName Entry component
        /// </summary>
        public bool FullNameIsValid
        {
            get => fullNameIsValid;
            set
            {
                SetProperty(ref fullNameIsValid, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Get/Set emailIsValid bool attribute for validation of emails Entry component
        /// </summary>
        public bool EmailIsValid
        {
            get => emailIsValid;

            set
            {
                emailIsValid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Get/Set addressIsValid bool attribute for validation of address Entry component
        /// </summary>
        public bool AddressIsValid
        {
            get => addressIsValid;
            set
            {
                addressIsValid= value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Get/Set phoneNumberIsValid bool attribute for validation of phoneNumbers Entry component
        /// </summary>
        public bool PhoneNumberIsValid
        {
            get => phoneNumberIsValid;
            set
            {
                phoneNumberIsValid = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => fullName;
            set => SetProperty(ref fullName, value);
        }

        /// <summary>
        /// Get/Set emails string attribute for Contact
        /// </summary>
        public string Emails
        {
            get => emails;
            set => SetProperty(ref emails, value);
        }

        /// <summary>
        /// Get/Set phoneNumbers string attribute for Contact
        /// </summary>
        public string PhoneNumbers
        {
            get => phoneNumbers;
            set => SetProperty(ref phoneNumbers, value);
        }

        /// <summary>
        /// Get/Set address string attribute for Contact
        /// </summary>
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        /// <summary>
        /// Get/Set propertyMinimumDate DateTime attribute for (date of birth) dob DatePicker component
        /// </summary>
        public DateTime PropertyMinimumDate
        {
            get => propertyMinimumDate;
            set => SetProperty(ref propertyMinimumDate, value);
        }

        /// <summary>
        /// Get/Set propertyMaximumDate DateTime attribute for (date of birth) dob DatePicker component
        /// </summary>
        public DateTime PropertyMaximumDate
        {
            get => propertyMaximumDate;
            set => SetProperty(ref propertyMaximumDate, value);
        }

        /// <summary>
        /// Get/Set (date of birth) dob DateTime attribute for Contact
        /// </summary>
        public DateTime DoB
        {
            get => dob;
            set => SetProperty(ref dob, value);
        }

        /// <summary>
        /// Get/Set lastContacted Datetime attribute for Contact
        /// </summary>
        public DateTime LastContacted
        {
            get => lastContacted;
            set => SetProperty(ref lastContacted, value);
        }

        /// <summary>
        /// Get/Set createdDate DateTime attribute for Contact
        /// </summary>
        public DateTime CreatedDate
        {
            get => createdDate;
            set => SetProperty(ref createdDate, value);
        }

        /// <summary>
        /// Get/Set lastEdited DateTime attribute for Contact
        /// </summary>
        public DateTime LastEdited
        {
            get => lastEdited;
            set => SetProperty(ref lastEdited, value);
        }

        /// <summary>
        /// Get/Set description string attribute for Contact
        /// </summary>
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        /// <summary>
        /// Get/Set contactImgSource ImageSource attribute for Image component
        /// </summary>
        public ImageSource ContactImgSource
        {
            get => contactImageSource;
            set
            {
                SetProperty(ref contactImageSource, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Get/Set contactImgSource_string string attribute for Contact
        /// </summary>
        public string ContactImgSource_String
        {
            get => contactImageSource_string;
            set => SetProperty(ref contactImageSource_string, value);
        }

        /// <summary>
        /// Get/Set the contactId which has been passed by the previous View in order to edit an existing Contact.
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
        /// Command to capture a new image
        /// </summary>
        public Command ButtonCapture { get; }

        /// <summary>
        /// Command to choose an image from local storage
        /// </summary>
        public Command ButtonChoose { get; }

        /// <summary>
        /// Command to add/update Contact to the local database
        /// </summary>
        public Command SaveCommand { get; }

        /// <summary>
        /// Command to cancel Contact creation/editing
        /// </summary>
        public Command CancelCommand { get; }


        // -- Specific Functions

        /// <summary>
        /// Asynchronous method which loads a Contact from the local database on the basis of a contactId.
        /// All attributes are set according to the loaded Contact.
        /// </summary>
        /// <param name="contactId">Contact Identifier which needs to be loaded from the local database</param>
        private async void LoadContactId(int contactId)
        {
            try
            {
                var contact = await _ContactDatabase.GetContactAsync(contactId);

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
                ContactImgSource = ImageSource.FromFile(ContactImgSource_String);

                PhoneNumberIsValid = true;
                FullNameIsValid = true;

                Debug.WriteLine("Contact successfully loaded");

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Contact");
            }
        }

        /// <summary>
        /// Saves the captured image into local storage if a new photo has been captured (photoCaptured equals true).
        /// Loads the path of the image file and sets the ContactImgSource_String(file path for database entry) attribute accordingly.
        /// </summary>
        /// <param name="result">The resulting FileResult after an image has been captured or chosen from local storage</param>
        /// <param name="photoCaptured">A boolean which is set true if a new image has been captured</param>
        /// <returns></returns>
        private async Task LoadPhotoAsync(FileResult result, bool photoCaptured)
        {
            // canceled
            if (result == null)
            {
                return;
            }

            // save the file into local storage if a new photo has been captured
            var newFile = Path.Combine(FileSystem.CacheDirectory, result.FileName);
            if (photoCaptured) 
            {
                using (Stream stream = await result.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                {
                    await stream.CopyToAsync(newStream);

                }
            }
            ContactImgSource_String = newFile;
        }

        /// <summary>
        /// Choose Image Button event handler.
        /// Opens the mobile local image storage, so the user can choose an image for the Contact.
        /// Sets the source of the image in ContactImgSource (Binding for Image Component in xaml file).
        /// </summary>
        private async void ButtonChoose_ClickedAsync()
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Please pick a photo"
            });

            if (result != null)
            {
                await LoadPhotoAsync(result, false);

                var stream = await result.OpenReadAsync();

                var source = ImageSource.FromStream(() => stream);

                ContactImgSource = source;

            }
            else
            {
                ContactImgSource_String = "take_photo.png";
                ContactImgSource = ImageSource.FromFile(ContactImgSource_String);
            }
        }

        /// <summary>
        /// Capture Image Button event handler.
        /// Opens the mobile photo app, so the user can capture an image for the Contact.
        /// Saves the captured image to the local storage and sets the source of the image in ContactImgSource (Binding for Image Component in xaml file).
        /// </summary>
        private async void ButtonCapture_ClickedAsync()
        {
            var result = await MediaPicker.CapturePhotoAsync();

            if (result != null)
            {
                await LoadPhotoAsync(result, true);

                var stream = await result.OpenReadAsync();

                var source = ImageSource.FromStream(() => stream);

                ContactImgSource = source;

            }
            else
            {
                ContactImgSource_String = "take_photo.png";
                ContactImgSource = ImageSource.FromFile(ContactImgSource_String);
            }
        }


        /// <summary>
        /// Simple validation with minimum requirements before being able to persist a Contact in the local database
        /// </summary>
        /// <returns>A boolean which requires FullName & PhoneNumber to be valid in order to be true</returns>
        private bool ValidateSave()
        {
            return (FullNameIsValid && PhoneNumberIsValid);
        }

        /// <summary>
        /// Cancel Button event handler pops current page off the navigation stack.
        /// All changes are discarded.
        /// </summary>
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// Save Button event handler persists created/edited Contact in the local database:
        /// - If a new Contact was created, a new database entry is added.
        /// - If an existing Contact has been edited, the database entry is updated accordingly.
        /// </summary>
        private async void OnSave()
        {
            Models.Contact newContact = new Models.Contact()
            {
                //Id = Guid.NewGuid().ToString(),
                FullName = FullName,
                PhoneNumbers = PhoneNumbers,
                Emails = Emails,
                Description = Description,
                Address = Address,
                DoB = DoB,
                ContactImageSource_String = ContactImgSource_String,
                CreatedDate = CreatedDate,
                LastEdited = LastEdited,
                LastContacted = LastContacted

            };

            if (contactId != -1)
            {
                newContact.Id = contactId;
                await _ContactDatabase.UpdateContactAsync(newContact);
            }
            else
            {
                await _ContactDatabase.AddContactAsync(newContact);
            }

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
