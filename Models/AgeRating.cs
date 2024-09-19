using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public enum AgeRating
    {
        [Display(Name = "G – General Audiences")]
        GeneralAudiences,

        [Display(Name = "PG – Parental Guidance Suggested")]
        ParentalGuidanceSuggested,

        [Display(Name = "PG-13 – Parents Strongly Cautioned")]
        ParentsStronglyCautioned,

        [Display(Name = "R – Restricted")]
        Restricted,

        [Display(Name = "NC-17 – Adults Only")]
        AdultsOnly
    }
}
