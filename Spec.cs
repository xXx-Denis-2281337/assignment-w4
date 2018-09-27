using Xbehave;
using FluentAssertions;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using System.Collections.Generic;
using System;
using Xunit.Abstractions;
using System.Linq;

namespace KmaOoad18.Assignments.Week4
{
    public class Spec
    {
        [Scenario]
        public void CanLaunchLoyalty(string name, string phone, LoyaltyClient client)
        {
            "Given some customer name"
            .x(() => name = "James Jameson");

            "And customer phone"
            .x(() => phone = "088 913-49-84");

            "And loyalty client"
            .x(() => client = new LoyaltyClient());

            var loyaltyCard = string.Empty;

            "When I launch customer's loyalty program"
            .x(() => loyaltyCard = client.LaunchLoyalty(name, phone));

            "Then I expect that customer's balance is zero"
            .x(() => client.LoyaltyBalance(loyaltyCard).Should().Be(0));
        }


        [Scenario]
        public void CanProcessPurchase()
        {
            var (products, purchased) = SeedProducts();

            LoyaltyClient client = null;
            string loyaltyCard = string.Empty;

            "Given loyalty client"
            .x(() => client = new LoyaltyClient());

            "And loyalty card"
            .x(() => loyaltyCard = client.LaunchLoyalty("Rick Richardson", "074 454-89-90"));

            "And some products"
            .x(() => products.ForEach(p => client.AddProduct(p.Sku, p.Name, p.Price)));

            decimal total1 = 0m;
            decimal total2 = 0m;

            int balance1 = 0;
            int balance2 = 0;

            "When I process customer's purchase"
            .x(() => total1 = client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList()));

            "And get balance"
            .x(() => balance1 = client.LoyaltyBalance(loyaltyCard));

            "And then process same purchase with doubled qty"
            .x(() => total2 = client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty * 2)).ToList()));

            "And get balance again"
            .x(() => balance2 = client.LoyaltyBalance(loyaltyCard));

            "Then I expect total to double"
            .x(() => total2.Should().Be(total1 * 2));

            "And balance to triple"
            .x(() => balance2.Should().Be(balance1 * 3));
        }

        private (List<Product>, List<Purchase>) SeedProducts()
        {
            var seed = DateTime.Now.Second + 3;
            var factor = seed % 10;
            var productSeed = seed * factor;

            var prices = new List<int>();
            for (int i = 0; i < factor; i++)
            {
                prices.Add(productSeed);
            }

            var products = prices.Select(price => new Product { Name = $"sku{price}", Sku = $"sku{price}", Price = price * 1.0m }).ToList();

            var purchased = products.Take(products.Count > 5 ? 5 : products.Count).Select(pp => new Purchase { Sku = pp.Sku, Qty = factor }).ToList();

            return (products, purchased);
        }

        private struct Purchase
        {
            public string Sku;
            public double Qty;
        }

        private struct Product
        {
            public string Name;
            public string Sku;
            public decimal Price;
        }
    }
}