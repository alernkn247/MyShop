using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Core.Contracts
{
    public interface IBasketService
    {
        /// <summary>
        /// Add items to the basket
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="productId">Id of the product</param>
        void AddToBasket(HttpContextBase httpContext, string productId);
        void RemoveFromBasket(HttpContextBase httpContext, string itemId);
        List<BasketitemViewModel> GetBasketItems(HttpContextBase httpContext);
        BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext);

    }
}
