using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerosProject.Entity
{
    public class products
    {
        [Key]
        [Column("Code")]
        public string Code { get; set; }

        public string ProductName { get; set; }

        public string price { get; set; }

    }
}
