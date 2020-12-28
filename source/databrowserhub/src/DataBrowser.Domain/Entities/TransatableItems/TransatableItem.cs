using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBrowser.Domain.Entities.TransatableItems
{
    public class TransatableItem : Entity, IAggregateRoot
    {
        public int TransatableItemId { get; set; }

        //TransatableItemValue
        private readonly List<TransatableItemValue> _transatableItemValues = new List<TransatableItemValue>();
        public virtual IReadOnlyCollection<TransatableItemValue> TransatableItemValues => _transatableItemValues?.AsReadOnly();


        protected TransatableItem()
        {

        }

        public static TransatableItem CreateTransatableItem(Dictionary<string, string> transaltes)
        {
            if (transaltes == null || transaltes.Count <= 0)
            {
                return null;
            }

            var transatableItem = new TransatableItem();

            transatableItem._transatableItemValues.AddRange(transaltes.Where(i => !string.IsNullOrWhiteSpace(i.Value)).Select(t => TransatableItemValue.CreateTransatableItemValue(t.Key, t.Value)));

            return transatableItem;
        }

        public void AddTransatableItemValue(Dictionary<string, string> translates)
        {
            foreach (var itemTranValue in translates)
            {
                if (string.IsNullOrWhiteSpace(itemTranValue.Value))
                {
                    continue;
                }

                var itemTransalte = _transatableItemValues.FirstOrDefault(i => i.Language.Equals(itemTranValue.Key, StringComparison.InvariantCultureIgnoreCase));
                if (itemTransalte != null)
                {
                    itemTransalte.Value = itemTranValue.Value;
                }
                else
                {
                    _transatableItemValues.Add(TransatableItemValue.CreateTransatableItemValue(itemTranValue.Key, itemTranValue.Value));
                }
            }
        }

        public bool RemoveTransatableItemValue(string language)
        {
            var itemTransalte = _transatableItemValues.FirstOrDefault(i => i.Language.Equals(language, StringComparison.InvariantCultureIgnoreCase));
            if (itemTransalte == null)
            {
                return false;
            }

            return _transatableItemValues.Remove(itemTransalte);
        }

        public void ClearTransatableItemValue()
        {
            _transatableItemValues.Clear();
        }

    }
}
