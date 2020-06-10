using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService : IBasketService
    {
        IRepository<Product> productContext;
        IRepository<Basket> basketContext;

        public const string BasketSessionName = "eCommerceBasket";
        public BasketService(IRepository<Product> productContext, IRepository<Basket> basketContext)
        {
            this.productContext = productContext;
            this.basketContext = basketContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if (cookie != null)
            {
                string basketId = cookie.Value;
                if (!String.IsNullOrEmpty(basketId))
                {
                    basket = basketContext.Find(basketId);
                }
                else
                {
                    if (createIfNull)
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }

            return basket;
        }
        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.Basketitems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                basket.Basketitems.Add(item);
            }
            else
            {
                item.Quantity = item.Quantity + 1;
            }

            basketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.Basketitems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                basket.Basketitems.Remove(item);
                basketContext.Commit();
            }
        }
        public List<BasketitemViewModel> GetBasketItems(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            if (basket != null)
            {
                var results = (from b in basket.Basketitems
                               join p in productContext.Collection() on b.ProductId equals p.Id
                               select new BasketitemViewModel()
                               {
                                   Id = b.Id,
                                   Quantity = b.Quantity,
                                   Productname = p.Name,
                                   Image = p.Image,
                                   Price = p.Price
                               }
                              ).ToList();

                return results;
            }
            else
            {
                return new List<BasketitemViewModel>();
            }
        }
        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            BasketSummaryViewModel model = new BasketSummaryViewModel(0, 0);
            if (basket != null)
            {
                //? gives the ability to store null as well
                int? basketCount = (from item in basket.Basketitems
                                    select item.Quantity).Sum();
                decimal? basketTotal = (from item in basket.Basketitems
                                        join p in productContext.Collection() on item.ProductId equals p.Id
                                        select item.Quantity * p.Price).Sum();
                model.BasketCount = basketCount ?? 0;   //?? is a conditional operator and returns the right handvalue if the variable assigned is null
                model.BasketTotal = basketTotal ?? decimal.Zero;

                return model;
            }
            else
            {
                return model;
            }
        }


    }
}
