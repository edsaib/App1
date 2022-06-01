using App1.Services;
using App1.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App1.ViewModels
{
    public class ContactsViewModel : BaseViewModel
    {
        // -- Attributes

        private HttpClient client;


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
        /// Command to open the mobile Maps app to show the address location of the corresponding Contact
        /// </summary>
        public Command<Models.Contact> MapsTapped { get; }
        /// <summary>
        /// TODO: Command to search specific Contacts from the Observable Collection of Contacts
        /// </summary>
        public Command<string> SearchContacts { get; }

        // -- Constructors

        /// <summary>
        /// Constructor of ViewModel for viewing all persisted Contacts.
        /// Initializes Attributes and specifies Commands.
        /// </summary>
        public ContactsViewModel(INavigation navigation)
        {
            // Initialize attributes
            Title = "Browse Contacts";
            client = new HttpClient();
            this.Navigation = navigation;

            // Specify commands
            Contacts = new ObservableCollection<Models.Contact>();
            // directly execute command
            LoadContactsCommand = new Command(async () => await ExecuteLoadContactsCommand());

            ItemTapped = new Command<Models.Contact>(OnContactSelected);

            CallTapped = new Command<Models.Contact>(OnCallContactSelected);

            MailTapped = new Command<Models.Contact>(OnMailContactSelected);

            SMSTapped = new Command<Models.Contact>(OnSMSContactSelected);

            MapsTapped = new Command<Models.Contact>(OnMapSelected);

            //SearchContacts = new Command<string>(PerformSearch);

            AddContactCommand = new Command(OnAddContact);
        }

        // -- Attribute Getter/Setter

        /// <summary>
        /// Navigation property in order to push/pop pages onto/from the Navigation stack.
        /// This property is passed by the Page (ItemsPage) this ViewModel is bound to.
        /// </summary>
        public INavigation Navigation { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
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
        
        public async Task<Models.NominatimGeoJson> GetAddressDataAsync(string string_uri)
        {
            
            Uri uri = new Uri(string.Format(string_uri, string.Empty));
            
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var resultGeoJson = JsonConvert.DeserializeObject<Models.NominatimGeoJson>(content);
                return resultGeoJson;
            }

            return null;
         
        }
        

        private async void OnMapSelected(Models.Contact contact)
        {

            // API call to get coordinates from specific address

            string city = "";
            string country = "";
            string street = "";
            string postalCode = "";
            if (!String.IsNullOrEmpty(contact.City))
            {
                city = contact.City;
            }

            if (!String.IsNullOrEmpty(contact.Country))
            {
                country = contact.Country;
            }

            if (!String.IsNullOrEmpty(contact.Street))
            {
                street = contact.Street;
            }

            if (!String.IsNullOrEmpty(contact.PostalCode))
            {
                postalCode = contact.PostalCode;
            }

            string gui_uri = "https://nominatim.openstreetmap.org/search?";
            gui_uri += "city=" + city + "&";
            gui_uri += "postalcode=" + postalCode + "&";
            gui_uri += "street=" + street + "&";
            gui_uri += "country=" + country + "&";
            string uri = gui_uri +  "format=geojson";

            // NullReferenceException!!!! check whether attributes are null or not.
            string searchQ = country + "+"
                + postalCode + "+"
                + city + "+"
                + street.Replace(' ', '+');


            var result = await GetAddressDataAsync(uri);

            if(result == null)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "There is no valid address for this Contact!", "OK");
                return;
            } else
            {
                Debug.WriteLine(result);

                if (Device.RuntimePlatform == Device.iOS)
                {
                    // https://developer.apple.com/library/ios/featuredarticles/iPhoneURLScheme_Reference/MapLinks/MapLinks.html
                    //await Launcher.OpenAsync("http://maps.apple.com/?q="+searchQ);

                    List<MapDetails> details = new List<MapDetails>
                    {
                        new MapDetails
                        {
                            PinName = "Name",
                            PinAddress = contact.Street,
                            PinLabel = contact.FullName,
                            Position = new Xamarin.Forms.Maps.Position(result.Features[0].Geometry.Coordinates[1], result.Features[0].Geometry.Coordinates[0])
                        }
                    };

                    var mapPage = new MapPage(details);

                    await Navigation.PushAsync(mapPage);

                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    // open the maps app directly
                    //await Launcher.OpenAsync("geo:0,0?q="+searchQ);
                    //await Shell.Current.GoToAsync($"{nameof(MapPage)}");

                    Xamarin.Forms.Maps.Position addressPosition;
                    List<MapDetails> details;

                    if (result.Features.Length == 0)
                    {
                        addressPosition = new Xamarin.Forms.Maps.Position(49.888180, 8.623688);

                        details = new List<MapDetails>
                        {
                        new MapDetails
                            {
                            PinName = "Betterbits",
                            PinAddress = "Bunsenstraße 22",
                            PinLabel = "Betterbits",
                            Position = addressPosition
                            }
                        };
                    } 
                    else 
                    {
                        addressPosition = new Xamarin.Forms.Maps.Position(result.Features[0].Geometry.Coordinates[1], result.Features[0].Geometry.Coordinates[0]);

                        details = new List<MapDetails>
                        {
                        new MapDetails
                            {
                            PinName = "Name",
                            PinAddress = contact.Street,
                            PinLabel = contact.FullName,
                            Position = addressPosition
                            }
                        };
                    }

                    var mapPage = new MapPage(details);

                    await Navigation.PushAsync(mapPage);
                }
                else if (Device.RuntimePlatform == Device.UWP)
                {
                    await Launcher.OpenAsync("bingmaps:?where="+searchQ);
                }
            }
        }
    }
}