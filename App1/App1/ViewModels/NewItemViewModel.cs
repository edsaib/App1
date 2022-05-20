using App1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App1.ViewModels
{
    [QueryProperty(nameof(ContactId), nameof(ContactId))]
    public class NewContactViewModel : BaseViewModel
    {
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

        public NewContactViewModel()
        {
            //isValid = true;
            PhoneNumberIsValid = false;
            FullNameIsValid = false;

            contactId = -1;
            Id = contactId;
            if (contactId == -1) DoB = DateTime.Now;
            PropertyMaximumDate = DateTime.Now;
            PropertyMinimumDate = new DateTime(1990, 1, 1);
            CreatedDate = DateTime.Now;
            LastEdited = DateTime.Now;
            LastContacted = DateTime.MinValue;

            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            ButtonCapture = new Command(ButtonCapture_Clicked);
            ButtonChoose = new Command(ButtonChoose_Clicked);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return (FullNameIsValid && PhoneNumberIsValid);
        }

        public bool FullNameIsValid
        {
            get => fullNameIsValid;
            set
            {
                SetProperty(ref fullNameIsValid, value);
                OnPropertyChanged();
            }
        }
        public bool EmailIsValid
        {
            get => emailIsValid;
            set
            {
                emailIsValid = value;
                OnPropertyChanged();
            }
        }

        public bool AddressIsValid
        {
            get => addressIsValid;
            set
            {
                addressIsValid= value;

                OnPropertyChanged();
            }
        }

        public bool PhoneNumberIsValid
        {
            get => phoneNumberIsValid;
            set
            {
                phoneNumberIsValid = value;
                OnPropertyChanged();
            }
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

        public DateTime PropertyMinimumDate
        {
            get => propertyMinimumDate;
            set => SetProperty(ref propertyMinimumDate, value);
        }

        public DateTime PropertyMaximumDate
        {
            get => propertyMaximumDate;
            set => SetProperty(ref propertyMaximumDate, value);
        }

        public DateTime DoB
        {
            get => dob;
            set => SetProperty(ref dob, value);
        }

        public DateTime LastContacted
        {
            get => lastContacted;
            set => SetProperty(ref lastContacted, value);
        }

        public DateTime CreatedDate
        {
            get => createdDate;
            set => SetProperty(ref createdDate, value);
        }

        public DateTime LastEdited
        {
            get => lastEdited;
            set => SetProperty(ref lastEdited, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public ImageSource ContactImgSource
        {
            get => contactImageSource;
            set
            {
                SetProperty(ref contactImageSource, value);
                OnPropertyChanged();
            }
        }

        public string ContactImgSource_String
        {
            get => contactImageSource_string;
            set => SetProperty(ref contactImageSource_string, value);
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
                //Id = contact.Id;
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

                Debug.WriteLine("Contact successfully loaded");

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Contact");
            }
        }

        public Models.Contact Item { get; set; }

        private byte[] ImageSourceToByteArray(ImageSource source)
        {
            StreamImageSource streamImageSource = (StreamImageSource)source; 
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None; 
            Task<Stream> task = streamImageSource.Stream(cancellationToken); 
            Stream stream = task.Result;

            byte[] b;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                b = ms.ToArray();
            }

            return b;
        }

        async Task LoadPhotoAsync(FileResult result)
        {
            // canceled
            if (result == null)
            {
                ContactImgSource_String = null;
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, result.FileName);
            using (Stream stream = await result.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
            {
                await stream.CopyToAsync(newStream);

            }
            ContactImgSource_String = newFile;
        }
    


    async private void ButtonChoose_Clicked()
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Please pick a photo"
            });

            await LoadPhotoAsync(result);

            if (result != null)
            {
                var stream = await result.OpenReadAsync();

                var source = ImageSource.FromStream(() => stream);

                ContactImgSource = source;



                //string source_string = Convert.ToBase64String(ImageSourceToByteArray(source));

                //ContactImgSource_String = source_string;


            }
            else
            {
                ContactImgSource = ImageSource.FromFile("take_photo.png");
                ContactImgSource_String = "take_photo.png";
            }
        }

        async private void ButtonCapture_Clicked()
        {
            var result = await MediaPicker.CapturePhotoAsync();

            await LoadPhotoAsync(result);

            if (result != null)
            {
                var stream = await result.OpenReadAsync();

                var source = ImageSource.FromStream(() => stream);

                ContactImgSource = source;

                //string source_string = Convert.ToBase64String(ImageSourceToByteArray(source));

                //ContactImgSource_String = source_string;
            }
            else
            {
                ContactImgSource = ImageSource.FromFile("take_photo.png");
                ContactImgSource_String = "take_photo.png";
            }
        }

        public Command ButtonCapture { get; }
        public Command ButtonChoose { get; }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

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

            if(contactId != -1)
            {
                newContact.Id = contactId;
                await _ContactDatabase.UpdateContactAsync(newContact);
            } else
            {
                await _ContactDatabase.AddContactAsync(newContact);
            }

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
