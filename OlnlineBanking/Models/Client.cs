using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OlnlineBanking.Models
{
    public enum ClientStatus { Classic, Vip }

    public class Client
    {
        [HiddenInput(DisplayValue=false)]
        public int Id { get; set; }
        [Display(Name="Client Contract Number")]
        [StringLength(8, MinimumLength = 3,ErrorMessage = "Length of contract number must be between 3 and 8")]
        [Required(ErrorMessage = "Please enter a client contract number")]
        public string ContractNumber { get; set; }
        [Display(Name="First Name")]
        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Please select status")]
        public ClientStatus Status { get; set; }
        public bool Deposit { get; set; }
    }
}