using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.TransatableItems
{
    public class TransatableItemValue : Entity
    {
        public string Language { get; set; }
        public string Value { get; set; }

        //FK
        public int TransatableItemFk { get; set; }

        //Navigation Property
        public virtual TransatableItem TransatableItem { get; protected set; }

        protected TransatableItemValue()
        {

        }

        public static TransatableItemValue CreateTransatableItemValue(string language, string value)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentNullException(nameof(language));
            }
            //if (string.IsNullOrWhiteSpace(value))
            //{
            //    throw new ArgumentNullException(nameof(value));
            //}

            return new TransatableItemValue
            {
                Language = language,
                Value = value??""
            };

        }
    }
}
