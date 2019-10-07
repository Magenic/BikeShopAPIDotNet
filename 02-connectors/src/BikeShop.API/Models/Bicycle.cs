using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShop.API.Models
{
    [Table("bicycle")]
    public class Bicycle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        
        public string ProductName { get; set; }
        
        public double Price { get; set; }
        
        public string Description{ get; set; }
        
        public string Image { get; set; }
    }
}