using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swilago.Data.Tables
{
    [Table("T_Restaurant")]
    public class TRestaurant
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ResName { get; set; }

        public string? ResInfo { get; set; }
    }
}
