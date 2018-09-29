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
        [Fact]
        public void CanLaunchLoyalty()
        {
            // Given some customer name
            var name = "James Jameson";

            // And customer phone
            var phone = "088 913-49-84";

            // And loyalty client
            var client = new LoyaltyClient();

            var loyaltyCard = string.Empty;

            // When I launch customer's loyalty program
            loyaltyCard = client.LaunchLoyalty(name, phone);

            // Then I expect that customer's balance is zero
            client.LoyaltyBalance(loyaltyCard).Should().Be(0);
        }


        [Fact]
        public void CanProcessPurchase()
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
        public void CanApplySpecialOfferings()
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
        public void CanRemoveSpecialOfferings()
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
        public void CanDeductLoyaltyPoints()
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
    }
}