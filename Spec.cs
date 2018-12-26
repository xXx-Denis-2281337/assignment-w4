using FluentAssertions;
using Xunit;
using FsCheck;
using System.Collections.Generic;
using System;
using System.Linq;
using KmaOoad18.Assignments.Week4.Data;
using Microsoft.EntityFrameworkCore;

namespace KmaOoad18.Assignments.Week4
{
    public class Spec : IDisposable
    {
        public Spec()
        {
            LoyaltyClient.Db = "test.db";
        }

        [Fact]
        public void BasicScenario()
        {
            // Given loyalty client
            var client = new LoyaltyClient();

            // And some customer
            var name = "James Jameson";
            var phone = "088 913-49-84";
            var loyaltyCard = string.Empty;

            // And some product 
            var sku = $"sku{DateTime.Now.Ticks}";
            var price = 100m;
            client.AddProduct(sku, sku, price);


            // When I launch customer's loyalty program
            loyaltyCard = client.LaunchLoyalty(name, phone);

            // Then I expect that customer's balance to be zero
            client.LoyaltyBalance(loyaltyCard).Should().Be(0);

            // When I purchase some product with qty=1 and price=100
            var purchase = new List<(string, int)> { (sku, 1) };

            client.ProcessPurchase(purchase, loyaltyCard);

            // Then I expect balance to be 10 (0 + 100 * 1 * 0.1)
            var balance1 = client.LoyaltyBalance(loyaltyCard);
            balance1.Should().Be(10);

            // When I add special offering to make every bonus X10
            client.AddSpecialOffering(sku, Promotion.MultiplyPoints, 10);

            // And make same purchase
            client.ProcessPurchase(purchase, loyaltyCard);

            // Then I expect balance to be 110 (10 + 100 * 1 * 0.1 * 10)
            var balance2 = client.LoyaltyBalance(loyaltyCard);
            balance2.Should().Be(110);

            // When I remove special offering
            client.RemoveSpecialOffering(sku);

            // And make same purchase
            client.ProcessPurchase(purchase, loyaltyCard);

            // Then I expect balance to be 120 (110 + 100 * 1 * 0.1)
            var balance3 = client.LoyaltyBalance(loyaltyCard);
            balance3.Should().Be(120);


            // When I make same purchase and use bonus
            client.ProcessPurchase(purchase, loyaltyCard, useLoyaltyPoints: true);

            // Then I expect balance to be 75, because 50 is spent as discount for half amount, customer pays 100-50=50 and receives 5 points for bonus
            var balance4 = client.LoyaltyBalance(loyaltyCard);
            balance4.Should().Be(75);
        }


        #region Extended Tests
        [Fact]
        public void CanProcessPurchaseExtended()
        {
            var (products, purchased) = SeedProducts();

            // Given loyalty client
            var client = new LoyaltyClient();

            // And loyalty card
            var loyaltyCard = client.LaunchLoyalty("Rick Richardson", "074 454-89-90");

            // And some products
            products.ForEach(p => client.AddProduct(p.Sku, p.Name, p.Price));

            // When I process customer's purchase
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance
            var balance1 = client.LoyaltyBalance(loyaltyCard);

            // And then process same purchase
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance again
            var balance2 = client.LoyaltyBalance(loyaltyCard);

            // Then I expect balance to double
            balance2.Should().BeGreaterThan(0);
            balance2.Should().Be(balance1 * 2);
        }

        [Fact]
        public void CanApplySpecialOfferingsExtended()
        {
            var (products, purchased) = SeedProducts();


            // Given loyalty client
            var client = new LoyaltyClient();

            // And loyalty card
            var loyaltyCard = client.LaunchLoyalty("Rick Richardson", "074 454-89-90");

            // And some products
            products.ForEach(p => client.AddProduct(p.Sku, p.Name, p.Price));

            // When I process customer's purchase
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance
            var balance1 = client.LoyaltyBalance(loyaltyCard);

            // And add X3 special offering
            purchased.ForEach(p => client.AddSpecialOffering(p.Sku, Promotion.MultiplyPoints, 3));

            // And then process same purchase again
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance again
            var balance2 = client.LoyaltyBalance(loyaltyCard);

            // Then I expect balance to quadruple!!!
            balance2.Should().BeGreaterThan(0);
            balance2.Should().Be(balance1 * 4);
        }

        [Fact]
        public void CanRemoveSpecialOfferingsExtended()
        {
            var (products, purchased) = SeedProducts();


            // Given loyalty client
            var client = new LoyaltyClient();

            // And loyalty card
            var loyaltyCard = client.LaunchLoyalty("Rick Richardson", "074 454-89-90");

            // And some products
            products.ForEach(p => client.AddProduct(p.Sku, p.Name, p.Price));

            // When I process customer's purchase
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance
            var balance1 = client.LoyaltyBalance(loyaltyCard);

            // And add X3 special offering
            purchased.ForEach(p => client.AddSpecialOffering(p.Sku, Promotion.MultiplyPoints, 3));

            // And then process same purchase again
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And then remove special offering
            purchased.ForEach(p => client.RemoveSpecialOffering(p.Sku));

            // And process same purchase once more
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);


            // And get balance again
            var balance2 = client.LoyaltyBalance(loyaltyCard);

            // Then I expect balance grows 5 times
            balance2.Should().BeGreaterThan(0);
            balance2.Should().Be(balance1 * 5);
        }

        [Fact]
        public void CanDeductLoyaltyPointsExtended()
        {
            var (products, purchased) = SeedProducts();

            // Given loyalty client
            var client = new LoyaltyClient();

            // And loyalty card
            var loyaltyCard = client.LaunchLoyalty("Rick Richardson", "074 454-89-90");

            // And some products
            products.ForEach(p => client.AddProduct(p.Sku, p.Name, p.Price));

            // When I process customer's purchase
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance
            var balance1 = client.LoyaltyBalance(loyaltyCard);

            // And then process same purchase again with loyalty deduction=ON
            client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard, true);

            // And get balance again
            var balance2 = client.LoyaltyBalance(loyaltyCard);

            // Then I expect balance to be 90% of previous one
            balance2.Should().BeGreaterThan(0);
            balance2.Should().Be(balance1 * 0.9m);
        }


        private (List<Product>, List<Purchase>) SeedProducts()
        {
            var seed = DateTime.Now.Second + 3;
            var factor = (seed % 17) + 4;
            var productSeed = seed * factor;

            var prices = new List<int>();
            for (int i = 0; i < factor; i++)
            {
                prices.Add(productSeed + i);
            }

            var products = prices.Select(price => new Product { Name = $"sku{price}", Sku = $"sku{price}", Price = price * 1.0m }).ToList();

            var purchased = products.Take(products.Count > 5 ? 5 : products.Count).Select(pp => new Purchase { Sku = pp.Sku, Qty = factor }).ToList();

            return (products, purchased);
        }

        public void Dispose()
        {
            using (var db = new LoyaltyContext())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM [SpecialOfferings]");
                db.Database.ExecuteSqlCommand("DELETE FROM [Products]");
                db.Database.ExecuteSqlCommand("DELETE FROM [Customers]");
            }
        }

        private struct Purchase
        {
            public string Sku;
            public int Qty;
        }

        private struct Product
        {
            public string Name;
            public string Sku;
            public decimal Price;
        }

        #endregion
    }
}