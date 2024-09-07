using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AnimeStreamerV2.Models
{
    public class AnimeModel
    {
        //identification
        public int Id { get; set; }

        //information

        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Description field is required.")]
        public string Description { get; set; }
        public AgeRating Rating { get; set; } = AgeRating.GeneralAudiences;
        public string? IconPath { get; set; }
        public string? CountryOfOrigin { get; set; }

        //episodes
        public List<AnimeEpisodeModel> Episodes { get; set; } = new List<AnimeEpisodeModel>();
    }
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.GetName() ?? enumValue.ToString();
        }
    }
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
    public static class EnumHelpers
    {
        public static IEnumerable<SelectListItem> GetEnumSelectList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.GetDisplayName() // Použití vaší existující metody GetDisplayName
                });
        }
    }
}