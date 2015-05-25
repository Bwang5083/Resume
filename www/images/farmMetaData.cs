using BWClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace finalExam.Models
{
    [MetadataType(typeof(farmMetaData))]
    public partial class farm : IValidatableObject
    {
        OECContext db = new OECContext();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (town == null)
                town = "";
            else
                town = town.Trim();

            if (county == null)
                county = "";
            else
                county = county.Trim();
            if (town == "" && county == "")
                yield return new ValidationResult("Must input a town or a county", new[] { "town", "county" });

            if(name != null)
            {
                var farmName = db.farm.Where(a => a.name == name);
                if (farmName.Count() > 0)
                    yield return new ValidationResult("The farm name has already been on file", new[] {"name"});
            }

            if (provinceCode != null)
            {
                provinceCode = provinceCode.ToUpper();
                var province = db.province.Find(provinceCode);
                if (province == null)
                    yield return new ValidationResult("The province code is not on file", new[] { "provinceCode" }); //Bin Wang: provinceCode annotation translation
            }

            if (postalCode != null)
            {
                postalCode = postalCode.ToUpper();
                if (postalCode.Length == 6)
                    postalCode = postalCode.Insert(3, " ");
            }

            if (homePhone != null)
            {
                string newPhone = "";
                foreach (char item in homePhone)
                    if (item >= '0' && item <= '9') newPhone += item;
                if (newPhone.Length != 10)
                    yield return new ValidationResult("Home phone should be ten digitals", new[] { "homePhone" });
                else
                    homePhone = newPhone.Insert(3, "-").Insert(7, "-");
            }

            if (dateJoined != null && (dateJoined > DateTime.Now))
                yield return new ValidationResult("The joined date can not be in future", new[] { "dateJoined" });

            if (lastContactDate != null)
            {
                if (lastContactDate > DateTime.Now)
                    yield return new ValidationResult("Last contacted date can not be in future", new[] { "lastContactDate" }); //Bin Wang: lastContactDate in the future annotation translation
                if (dateJoined == null)
                    yield return new ValidationResult("Joined date doesn't exist", new[] { "dateJoined, lastContactDate" }); //Bin Wang: dateJoined is null annotation translation
                else if (lastContactDate < dateJoined)
                    yield return new ValidationResult("lastContactDate can not prior to dateJoined", new[] { "lastContactDate" }); //Bin Wang: lastContactDate less than dateJoined annotation translation
            }

            yield return ValidationResult.Success;
        }
    }

    public class farmMetaData
    {
        public int farmId { get; set; }

        [Required]
        [Display(Name = "Farm Name")]
        public string name { get; set; }

        [Display(Name = "Farm Address")]
        public string address { get; set; }
        public string town { get; set; }
        public string county { get; set; }

        [Display(Name = "Province Code")]
        [ForeignKey("province")]
        [Remote("ProvinceCodeValidation", "Remote")]
        public string provinceCode { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [PostalCodeValidation]
        public string postalCode { get; set; }

        public string homePhone { get; set; }
        public string cellPhone { get; set; }
        public string directions { get; set; }
        [Display(Name = "Joined Date")]
        [DisplayFormat(DataFormatString = "{0:ddd dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> dateJoined { get; set; }
        [Display(Name = "Last Contact Date")]
        [DisplayFormat(DataFormatString = "{0:ddd dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> lastContactDate { get; set; }

        public virtual province province { get; set; }
        public virtual ICollection<plot> plot { get; set; }
    }
}
