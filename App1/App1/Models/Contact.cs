using System;
using SQLite;

namespace App1.Models
{
    public class Contact
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FullName { get; set; }

        public DateTime DoB { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastEdited { get; set; }

        public string PhoneNumbers { get; set; }

        public string Emails { get; set; }

        public string Address { get; set; }

        public DateTime LastContacted { get; set; }
        public string Description { get; set; }

        public string ContactImageSource_String { get; set; }
    }
}