using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swilago.Data.Procedures
{
    public class PAllStatistics
    {
        public string ModifiedDate { get; set; }

        [Key]
        public string ResName { get; set; }

        [NotMapped]
        public int ResRank { get; set; }

        public int Selected { get; set; }

        public decimal ResPercentage { get; set; }
    }
}
