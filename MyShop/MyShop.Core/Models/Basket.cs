using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models
{
    public class Basket : BaseEntity
    {
        public virtual ICollection<BasketItem> Basketitems { get; set; } //Lazy Loading for entity framework, it will automatically load basket items when calling basket class
        public Basket()
        {
            this.Basketitems = new List<BasketItem>();
        }
    }
}
