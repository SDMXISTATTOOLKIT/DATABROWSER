using System.Collections.Generic;
using System.Linq;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Entities.TransatableItems;
using Xunit;

namespace DataBrowser.UnitTests.Entity
{
    public class ExtraTest
    {
        [Fact]
        public async void Extra_Create_Ok()
        {
            var extrakey = "extra1";
            var extraValue = "value1";
            var extraType = "type1";
            var isPublic = true;
            var transaltion = new Dictionary<string, string> {{"FR", "fr extra1"}, {"IT", "it extra1"}};
            checkCreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);

            extrakey = "ex tra2";
            extraValue = "va lue2";
            extraType = "t yp e2";
            isPublic = false;
            transaltion = null;
            checkCreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);

            extrakey = "extra3";
            extraValue = "vae3";
            extraType = "t yp 3";
            isPublic = false;
            transaltion = new Dictionary<string, string>
                {{"FR", "fr extra3"}, {"IT", "it extra3"}, {"EN", "en extra3"}};
            checkCreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);

            extrakey = "ex tra4";
            extraValue = "va lu4";
            extraType = "t y4";
            isPublic = true;
            transaltion = new Dictionary<string, string> {{"EN", "en extra4"}};
            checkCreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);
        }

        private static Extra checkCreateExtra(string extrakey, string extraValue, string extraType, bool isPublic,
            Dictionary<string, string> transaltion, bool returnWithoutCheck = false)
        {
            var extraEntity = Extra.CreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);
            if (returnWithoutCheck) return extraEntity;
            Assert.Equal(extrakey, extraEntity.Key);
            Assert.Equal(extraValue, extraEntity.Value);
            Assert.Equal(extraType, extraEntity.ValueType);
            Assert.Equal(isPublic, extraEntity.IsPublic);

            if (transaltion == null || transaltion.Count < 0)
            {
                Assert.Null(extraEntity.TransatableItem);
            }
            else
            {
                Assert.NotNull(extraEntity.TransatableItem);
                Assert.Equal(transaltion.Count, extraEntity.TransatableItem.TransatableItemValues.Count);
                foreach (var item in extraEntity.TransatableItem.TransatableItemValues)
                {
                    Assert.True(transaltion.ContainsKey(item.Language));
                    Assert.Equal(transaltion[item.Language], item.Value);
                }
            }

            return extraEntity;
        }

        [Fact]
        public async void Extra_SameKeyTransaltion_Ok()
        {
            var extrakey = "extra1";
            var extraValue = "value1";
            var extraType = "type1";
            var isPublic = true;
            var transaltion = new Dictionary<string, string> {{"FR", "fr extra1"}};
            var extraEntity = checkCreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);

            extraEntity.SetTransaltion(new Dictionary<string, string> {{"FR", "fr extra6"}});
            extraEntity.SetTransaltion(new Dictionary<string, string> {{"FR", "fr extra3"}});

            Assert.NotNull(extraEntity.TransatableItem);
            Assert.Equal(1, extraEntity.TransatableItem.TransatableItemValues.Count);
            foreach (var item in extraEntity.TransatableItem.TransatableItemValues)
            {
                Assert.Equal("FR", item.Language);
                Assert.Equal("fr extra3", item.Value);
            }


            var dicAdd = new Dictionary<string, string> {{"IT", "it extra2"}, {"FR", "fr extra3"}};
            extraEntity.SetTransaltion(dicAdd);

            Assert.NotNull(extraEntity.TransatableItem);
            Assert.Equal(dicAdd.Count, extraEntity.TransatableItem.TransatableItemValues.Count);
            foreach (var item in extraEntity.TransatableItem.TransatableItemValues)
            {
                Assert.True(dicAdd.ContainsKey(item.Language));
                Assert.Equal(dicAdd[item.Language], item.Value);
            }
        }

        [Fact]
        public async void Extra_AddKeyTransaltion_Ok()
        {
            var extrakey = "extra1";
            var extraValue = "value1";
            var extraType = "type1";
            var isPublic = true;
            var transaltion = new Dictionary<string, string> {{"FR", "fr extra1"}};
            var extraEntity = checkCreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);

            var dicAdd = new Dictionary<string, string> {{"IT", "it extra2"}};
            extraEntity.SetTransaltion(dicAdd);
            transaltion = transaltion.Concat(dicAdd)
                .GroupBy(kvp => kvp.Key, kvp => kvp.Value)
                .ToDictionary(g => g.Key, g => g.Last());

            Assert.NotNull(extraEntity.TransatableItem);
            Assert.Equal(transaltion.Count, extraEntity.TransatableItem.TransatableItemValues.Count);
            foreach (var item in extraEntity.TransatableItem.TransatableItemValues)
            {
                Assert.True(transaltion.ContainsKey(item.Language));
                Assert.Equal(transaltion[item.Language], item.Value);
            }
        }

        [Fact]
        public async void Extra_SetTransatableItem_Ok()
        {
            var extrakey = "extra1";
            var extraValue = "value1";
            var extraType = "type1";
            var isPublic = true;
            var transaltion = new Dictionary<string, string>
                {{"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}};
            var extraEntity = checkCreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);

            var newTransaltes = new Dictionary<string, string>
                {{"FR", "fr extra1"}, {"DE", "de extra"}, {"ES", "esp extra"}, {"RU", "ru extra"}};
            extraEntity.SetTransatableItem(TransatableItem.CreateTransatableItem(newTransaltes));

            Assert.NotNull(extraEntity.TransatableItem);
            Assert.Equal(newTransaltes.Count, extraEntity.TransatableItem.TransatableItemValues.Count);
            foreach (var item in extraEntity.TransatableItem.TransatableItemValues)
            {
                Assert.True(newTransaltes.ContainsKey(item.Language));
                Assert.Equal(newTransaltes[item.Language], item.Value);
            }
        }

        [Fact]
        public async void Extra_RemoveTransatableItem_Ok()
        {
            var extrakey = "extra1";
            var extraValue = "value1";
            var extraType = "type1";
            var isPublic = true;
            var transaltion = new Dictionary<string, string>
                {{"FR", "fr extra1"}, {"IT", "it extra2"}, {"EN", "EN extra"}};
            var extraEntity = checkCreateExtra(extrakey, extraValue, extraType, isPublic, transaltion);

            extraEntity.RemoveTransatableItem();

            Assert.Null(extraEntity.TransatableItem);
        }
    }
}