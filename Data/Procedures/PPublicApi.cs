using System.ComponentModel.DataAnnotations;

namespace Swilago.Data.Procedures
{
    public class PPublicApi
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        public string IsSelected { get; set; }

        public string? Info { get; set; }
    }
}
