using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swilago.Data.Tables
{
    [Table("T_UserRecord")]
    public class TUserRecord
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime ModifiedDate { get; set; }

        public string? RouletteResult { get; set; }

        [Key]
        public string UserEmail { get; set; }

        public string? ResRecord { get; set; }
    }
}
