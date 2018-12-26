using System.Collections.Generic;
using System.Linq;

using KmaOoad18.Assignments.Week4.Data;
using KmaOoad18.Assignments.Week4.Extensions;

namespace KmaOoad18.Assignments.Week4
{
    public class LoyaltyClient
    {
        private const decimal StdBonus = 10m; // %

        // Part I: Front-office (customer operations)

        // Gets customer's loyalty balance
        // loyaltyId can be either loyalty card number OR customer phone number, method must support both cases
        // Returns amount of loyalty points on customer's account
        public decimal LoyaltyBalance(string loyaltyId)
        {
            using (var db = new LoyaltyContext())
                return CustomerExt.GetByLoyaltyId(db, loyaltyId).LoyaltyBalance;
        }

        // Calculates bonus for purchase and adds to customer's account
        //    a. Normal loyalty bonus for product is 10% of paid total amount 
        //    b. If useLoyaltyPoints = true, up to 50% of total amount can be covered with loyalty points;
        //       in this case loyalty bonus is calculated only for actually paid amount
        public void ProcessPurchase(List<(string sku, int qty)> order, string loyaltyId, bool useLoyaltyPoints = false)
        {
            using (var db = new LoyaltyContext())
            {
                var cust = CustomerExt.GetByLoyaltyId(db, loyaltyId);
                var bill = order.Select(o => 
                    new { Sku = o.sku, Total = ProductExt.GetBySku(db, o.sku).Price * o.qty });

                var totalBonus = 0m;

                foreach (var pos in bill)
                {
                    var spending = pos.Total;

                    if (useLoyaltyPoints)
                        spending = cust.UseLoyaltyPoints(spending);

                    var offerings = SpecialOfferingExt.ForProduct(db, pos.Sku);
                    var bonus = StdBonus;

                    foreach (var off in offerings)
                        bonus = off.ApplyPromotion(bonus);

                    bonus /= 100;

                    totalBonus += spending * bonus;
                }

                cust.LoyaltyBalance += totalBonus;

                db.SaveChanges();
            }
        }

        // Part II: Back-office (admin operations)

        // Adds product with SKU, name, and price
        // SKU is stock keeping unit - unique identifier of product inventory
        public void AddProduct(string sku, string name, decimal price)
        {
            using (var db = new LoyaltyContext())
            {
                db.Products.Add(ProductExt.Construct(sku, name, price));

                db.SaveChanges();
            }
        }

        // Sets special offering for a given product
        // Special offering is either adding extra points, or multiplying normal points by some coefficient
        // extra param is amount to add to normal bonus or coeff to multiply normal bonus by
        public void AddSpecialOffering(string sku, Promotion promotion, int extra)
        {
            using (var db = new LoyaltyContext())
            {
                var prod = ProductExt.GetBySku(db, sku);

                db.SpecialOfferings.Add(
                    SpecialOfferingExt.Construct(prod, promotion, extra)
                );

                db.SaveChanges();
            }
        }

        // Removes all special offerings for the given product
        public void RemoveSpecialOffering(string sku)
        {
            using (var db = new LoyaltyContext())
            {
                SpecialOfferingExt.RemoveFrom(db, sku);

                db.SaveChanges();
            }
        }

        // Adds customer to loyalty program
        // Returns new loyalty card ID
        public string LaunchLoyalty(string customerName, string customerPhone)
        {
            using (var db = new LoyaltyContext())
            {
                var cust = db.Customers.Add(CustomerExt.Construct(customerName, customerPhone)).Entity;

                db.SaveChanges();

                return cust.LoyaltyCardId;
            }
        }

        #region Config
        // This is to simplify config for testing purposes in this educational project only.
        // Normally you should avoid such public fields in real life!
        public static string Db = "loyalty.db";
        #endregion
    }

    public enum Promotion
    {
        AddPoints,
        MultiplyPoints
    }
}
