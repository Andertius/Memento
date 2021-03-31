using System.ComponentModel.DataAnnotations;

namespace Memento.Models.ViewModels
{
    public class SettingsModel
    {
        [Range(0, 24.0, ErrorMessage = "Hours per day should be in range [0, 24]")]
        public float HoursPerDay { get; set; }

        [Range(0, 1000, ErrorMessage = "Cards per day should be in range [0, 1000]")]
        public int CardsPerDay { get; set; }

        public string Theme { get; set; }

        public string CardsOrder { get; set; }

        public bool ShowImages { get; set; }
    }
}
