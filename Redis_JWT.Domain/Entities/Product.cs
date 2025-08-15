using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis_JWT.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; } 
        public string Name { get; set; } = default!;//Bos gecilemez null yok!
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;//Nesne olustururken hangi zamanda olustu onu tutariz!
        public DateTime? UpdatedTime { get; set; }//Update ettigimizde tarihini tutalim diye!
    }
}
