using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSWebAPI.Models
{
    public class Person
    {
        public int personID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public string addressline1 { get; set; }
        public string addressline2 { get; set; }
        public string addressline3 { get; set; }
        public string postCode { get; set; }

    }
}