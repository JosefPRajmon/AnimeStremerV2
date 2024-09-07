﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;
using test.Models;

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


}