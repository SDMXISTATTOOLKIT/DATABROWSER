using System.Collections.Generic;
using System.Linq;
using DataBrowser.Domain.Entities.TransatableItems;
using Xunit;

namespace DataBrowser.UnitTests.Entity
{
    public class TransatableItemTest
    {
        [Fact]
        public void Create_TransatableItem_Ok()
        {
            var transaltion = new Dictionary<string, string>
                {{"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}};
            var transatableItem = TransatableItem.CreateTransatableItem(transaltion);

            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count, transatableItem.TransatableItemValues.Count);
            foreach (var item in transatableItem.TransatableItemValues)
            {
                Assert.True(transaltion.ContainsKey(item.Language));
                Assert.Equal(transaltion[item.Language], item.Value);
            }
        }

        [Fact]
        public void TransatableItem_AddSameKey_Ok()
        {
            var transaltion = new Dictionary<string, string>
                {{"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}};
            var transatableItem = TransatableItem.CreateTransatableItem(transaltion);

            //var newTransaltes = new Dictionary<string, string> { { "FR", "fr extra1" }, { "DE", "de extra" }, { "ES", "esp extra" }, { "RU", "ru extra" } };
            transatableItem.AddTransatableItemValue(new Dictionary<string, string>
                {{"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}});

            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count, transatableItem.TransatableItemValues.Count);
            foreach (var item in transatableItem.TransatableItemValues)
            {
                Assert.True(transaltion.ContainsKey(item.Language));
                Assert.Equal(transaltion[item.Language], item.Value);
            }
        }

        [Fact]
        public void TransatableItem_AddNewKey_Ok()
        {
            var transaltion = new Dictionary<string, string>
                {{"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}};
            var transatableItem = TransatableItem.CreateTransatableItem(transaltion);

            var newTransaltes = new Dictionary<string, string>
                {{"DE", "de extra"}, {"ES", "esp extra"}, {"RU", "ru extra"}};
            transatableItem.AddTransatableItemValue(newTransaltes);
            transaltion = transaltion.Concat(newTransaltes)
                .GroupBy(kvp => kvp.Key, kvp => kvp.Value)
                .ToDictionary(g => g.Key, g => g.Last());


            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count, transatableItem.TransatableItemValues.Count);
            foreach (var item in transatableItem.TransatableItemValues)
            {
                Assert.True(transaltion.ContainsKey(item.Language));
                Assert.Equal(transaltion[item.Language], item.Value);
            }
        }

        [Fact]
        public void TransatableItem_AddNewAndSameKey_AddAndOverwriteKeysOk()
        {
            var transaltion = new Dictionary<string, string>
                {{"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}};
            var transatableItem = TransatableItem.CreateTransatableItem(transaltion);

            var newTransaltes = new Dictionary<string, string>
                {{"DE", "de extra"}, {"ES", "esp extra"}, {"RU", "ru extra"}};
            transaltion = transaltion.Concat(newTransaltes)
                .GroupBy(kvp => kvp.Key, kvp => kvp.Value)
                .ToDictionary(g => g.Key, g => g.Last());
            newTransaltes.Add("FR", "fr extra1");
            transatableItem.AddTransatableItemValue(newTransaltes);


            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count, transatableItem.TransatableItemValues.Count);
            foreach (var item in transatableItem.TransatableItemValues)
            {
                Assert.True(transaltion.ContainsKey(item.Language));
                Assert.Equal(transaltion[item.Language], item.Value);
            }
        }


        [Fact]
        public void TransatableItem_RemovePresentKey_Ok()
        {
            var transaltion = new Dictionary<string, string>
            {
                {"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}, {"DE", "de extra"}, {"ES", "esp extra"},
                {"RU", "ru extra"}
            };
            var transatableItem = TransatableItem.CreateTransatableItem(transaltion);

            var result = transatableItem.RemoveTransatableItemValue("RU");

            Assert.True(result);
            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count - 1, transatableItem.TransatableItemValues.Count);
            var langs = transatableItem.TransatableItemValues.Select(i => i.Language).ToList();
            Assert.DoesNotContain("RU", langs);
            Assert.Contains("FR", langs);
            Assert.Contains("IT", langs);
            Assert.Contains("EN", langs);
            Assert.Contains("DE", langs);
            Assert.Contains("ES", langs);

            result = transatableItem.RemoveTransatableItemValue("DE");

            Assert.True(result);
            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count - 2, transatableItem.TransatableItemValues.Count);
            langs = transatableItem.TransatableItemValues.Select(i => i.Language).ToList();
            Assert.DoesNotContain("RU", langs);
            Assert.Contains("FR", langs);
            Assert.Contains("IT", langs);
            Assert.Contains("EN", langs);
            Assert.DoesNotContain("DE", langs);
            Assert.Contains("ES", langs);

            result = transatableItem.RemoveTransatableItemValue("FR");

            Assert.True(result);
            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count - 3, transatableItem.TransatableItemValues.Count);
            langs = transatableItem.TransatableItemValues.Select(i => i.Language).ToList();
            Assert.DoesNotContain("RU", langs);
            Assert.DoesNotContain("FR", langs);
            Assert.Contains("IT", langs);
            Assert.Contains("EN", langs);
            Assert.DoesNotContain("DE", langs);
            Assert.Contains("ES", langs);

            result = transatableItem.RemoveTransatableItemValue("EN");

            Assert.True(result);
            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count - 4, transatableItem.TransatableItemValues.Count);
            langs = transatableItem.TransatableItemValues.Select(i => i.Language).ToList();
            Assert.DoesNotContain("RU", langs);
            Assert.DoesNotContain("FR", langs);
            Assert.Contains("IT", langs);
            Assert.DoesNotContain("EN", langs);
            Assert.DoesNotContain("DE", langs);
            Assert.Contains("ES", langs);

            result = transatableItem.RemoveTransatableItemValue("ES");

            Assert.True(result);
            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count - 5, transatableItem.TransatableItemValues.Count);
            langs = transatableItem.TransatableItemValues.Select(i => i.Language).ToList();
            Assert.DoesNotContain("RU", langs);
            Assert.DoesNotContain("FR", langs);
            Assert.Contains("IT", langs);
            Assert.DoesNotContain("EN", langs);
            Assert.DoesNotContain("DE", langs);
            Assert.DoesNotContain("ES", langs);

            result = transatableItem.RemoveTransatableItemValue("IT");

            Assert.True(result);
            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count - 6, transatableItem.TransatableItemValues.Count);
            langs = transatableItem.TransatableItemValues.Select(i => i.Language).ToList();
            Assert.DoesNotContain("RU", langs);
            Assert.DoesNotContain("FR", langs);
            Assert.DoesNotContain("IT", langs);
            Assert.DoesNotContain("EN", langs);
            Assert.DoesNotContain("DE", langs);
            Assert.DoesNotContain("ES", langs);
        }

        [Fact]
        public void TransatableItem_RemoveNotFoundKey_ReturnFalseOk()
        {
            var transaltion = new Dictionary<string, string>
            {
                {"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}, {"DE", "de extra"}, {"ES", "esp extra"},
                {"RU", "ru extra"}
            };
            var transatableItem = TransatableItem.CreateTransatableItem(transaltion);

            var result = transatableItem.RemoveTransatableItemValue("RU2");

            Assert.False(result);
            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count, transatableItem.TransatableItemValues.Count);
            var langs = transatableItem.TransatableItemValues.Select(i => i.Language).ToList();
            Assert.Contains("RU", langs);
            Assert.Contains("FR", langs);
            Assert.Contains("IT", langs);
            Assert.Contains("EN", langs);
            Assert.Contains("DE", langs);
            Assert.Contains("ES", langs);
        }

        [Fact]
        public void TransatableItem_RemoveCaseInsesitiveKey_Ok()
        {
            var transaltion = new Dictionary<string, string>
            {
                {"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}, {"DE", "de extra"}, {"ES", "esp extra"},
                {"RU", "ru extra"}
            };
            var transatableItem = TransatableItem.CreateTransatableItem(transaltion);

            var result = transatableItem.RemoveTransatableItemValue("ru");

            Assert.True(result);
            Assert.NotNull(transatableItem);
            Assert.Equal(transaltion.Count - 1, transatableItem.TransatableItemValues.Count);
            var langs = transatableItem.TransatableItemValues.Select(i => i.Language).ToList();
            Assert.DoesNotContain("RU", langs);
            Assert.Contains("FR", langs);
            Assert.Contains("IT", langs);
            Assert.Contains("EN", langs);
            Assert.Contains("DE", langs);
            Assert.Contains("ES", langs);
        }
    }
}