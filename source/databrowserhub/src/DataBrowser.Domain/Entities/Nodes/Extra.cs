using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Domain.Entities.TransatableItems;
using System;
using System.Collections.Generic;

namespace DataBrowser.Domain.Entities.Nodes
{
    public class Extra : Entity
    {
        public int ExtraId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public bool IsPublic { get; set; }

        public int? TransatableItemFk { get; protected set; }

        public virtual TransatableItem TransatableItem { get; protected set; }

        protected Extra()
        {

        }

        public static Extra CreateExtra(string key, string value, string valueType = null, bool isPublic = true, Dictionary<string, string> translates = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value==null)
            {
                value = "";
            }

            var extraItem = new Extra
            {
                Key = key,
                Value = value,
                ValueType = valueType,
                IsPublic = isPublic
            };
            if (translates != null && translates.Count > 0)
            {
                extraItem.TransatableItem = TransatableItem.CreateTransatableItem(translates);
            }
            
            return extraItem;
        }

        public bool SetTransaltion(string lang, string value)
        {
            return SetTransaltion(new Dictionary<string, string> { { lang, value } });
        }

        public bool SetTransaltion(Dictionary<string, string> translates)
        {
            if (translates == null || translates.Count <= 0)
            {
                RemoveTransatableItem();
                return true;
            }

            if (TransatableItem == null)
            {
                SetTransatableItem(TransatableItem.CreateTransatableItem(translates));
                return true;
            }

            TransatableItem.AddTransatableItemValue(translates);

            return true;
        }

        public bool SetTransatableItem(TransatableItem transatableItem)
        {
            TransatableItem = transatableItem;

            return true;
        }

        public void RemoveTransatableItem()
        {
            TransatableItem = null;
        }

    }
}
