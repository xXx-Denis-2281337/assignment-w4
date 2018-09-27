using System.Collections.Generic;

namespace KmaOoad18.Assignments.Week4
{
    public class LoyaltyClient
    {
        // Part I: Front-office (customer operations)

        // Gets customer's loyalty balance
        // loyaltyId can be either loyalty card number OR customer phone number, method must support both cases
        // Returns amount of loyalty points on customer's account
        public int LoyaltyBalance(string loyaltyId) => -500; // Replace with implementation


        // 1. Calculates total amount for given list of products and quantities
        //    a. If useLoyaltyPoints=true, up to 50% of total amount can be covered with loyalty points
        // 2. Calculates bonus for purchase and adds to customer's account
        //    a. Normal loyalty bonus for product is 10% of total amount (price * qty), rounded up 
        // Returns total amount of purchase
        public decimal ProcessPurchase(List<(string sku, double qty)> order, string loyaltyId = null, bool useLoyaltyPoints = false)
        => -2000M; // Replace with implementation



        // Part II: Back-office (admin operations)

        // Adds product with SKU, name, and price
        // SKU is stock keeping unit - unique identifier of product inventory
        public void AddProduct(string sku, string name, decimal price)
        {
            // Add implementation
        }


        // Sets special offering for a given product
        // Special offering is either adding extra points, or multiplying normal points by some factor
        public void AddSpecialOffering(string sku, Promotion promotion, int extra)
        {
            // Add implementation        
        }


        // Cancels special offering for a given product
        public void CancelSpecialOffering(string sku)
        {
            // Add implementation        
        }


        // Adds customer to loyalty program
        // Returns new loyalty card ID
        public string LaunchLoyalty(string customerName, string customerPhone)
        => string.Empty; // Replace with implementation
    }
}
