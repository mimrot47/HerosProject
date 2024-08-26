using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerosProject.Entity
{
    public class ProductImage
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        public string ProductCode { get; set; }

        [Column("productName",TypeName ="image")]
        public byte[]? productName { get; set; }
    }
}
