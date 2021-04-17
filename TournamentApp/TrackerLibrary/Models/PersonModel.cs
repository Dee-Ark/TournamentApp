using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PersonModel
    {
        /// <summary>
        /// Unique Identifier the Id person
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represent the person first name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Represent the person last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Represent the person email address
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Represent the person cell phone number
        /// </summary>
        public string CellPhoneNumber { get; set; }

        public string FullName
        {
            get
            {
                return $"{ FirstName } { LastName }";
            }
        }

    }
}
