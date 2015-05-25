using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWClassLibrary
{
    public class PostalCodeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            string postalcode = (string)value;
            if (postalcode.ElementAt(0) >= 'a' && postalcode.ElementAt(0) <= 'z')
            {
                return new ValidationResult("the first character can not include all 26 letters");
            }

            return ValidationResult.Success;
        }
    }
}
