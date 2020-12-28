using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Domain.Entities.TransatableItems;
using DataBrowser.Domain.Events;
using DataBrowser.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBrowser.Domain.Entities.Hubs
{
    public class Hub : Entity, IAggregateRoot
    {
        public int HubId { get; set; }

        public string LogoURL { get; protected set; }
        public string BackgroundMediaURL { get; protected set; }
        public string SupportedLanguages { get; set; }
        public string DefaultLanguage { get; set; }
        public int MaxObservationsAfterCriteria { get; set; }
        public string DecimalSeparator { get; set; }
        public int DecimalNumber { get; set; }
        public long MaxCells { get; set; }
        public string EmptyCellDefaultValue { get; set; }
        public string DefaultView { get; set; }
        public string Extras { get; set; }

        //FK
        public int? TitleFk { get; set; }
        public int? SloganFk { get; set; }
        public int? DescriptionFk { get; set; }
        public int? DisclaimerFk { get; set; }

        //Navigation Property
        public virtual TransatableItem Title { get; protected set; }
        public virtual TransatableItem Slogan { get; protected set; }
        public virtual TransatableItem Description { get; protected set; }
        public virtual TransatableItem Disclaimer { get; protected set; }

        protected Hub()
        {

        }

        public static Hub CreateHub(HubDto hubDto)
        {
            var hub = new Hub();
            hub.ChangeLogoUrl(hubDto.LogoURL);
            hub.SupportedLanguages = string.Join(";", hubDto.SupportedLanguages);
            hub.DefaultLanguage = hubDto.DefaultLanguage;
            hub.MaxObservationsAfterCriteria = hubDto.MaxObservationsAfterCriteria;
            hub.DecimalSeparator = hubDto.DecimalSeparator;
            hub.DecimalNumber = hubDto.DecimalNumber;
            hub.EmptyCellDefaultValue = hubDto.EmptyCellDefaultValue;
            hub.DefaultView = hubDto.DefaultView;
            hub.MaxCells = hubDto.MaxCells;
            hub.ChangeBackgroundMediaURL(hubDto.BackgroundMediaURL);
            hub.CreationTime = DateTime.UtcNow;
            hub.Extras = hubDto.Extras;

            if (hubDto.Title != null)
            {
                hub.Title = TransatableItem.CreateTransatableItem(hubDto.Title);
            }
            if (hubDto.Slogan != null)
            {
                hub.Slogan = TransatableItem.CreateTransatableItem(hubDto.Slogan);
            }
            if (hubDto.Description != null)
            {
                hub.Description = TransatableItem.CreateTransatableItem(hubDto.Description);
            }
            if (hubDto.Disclaimer != null)
            {
                hub.Disclaimer = TransatableItem.CreateTransatableItem(hubDto.Disclaimer);
            }

            return hub;
        }

        public Hub EditHub(HubDto hubDto)
        {
            ChangeLogoUrl(hubDto.LogoURL);
            SupportedLanguages = string.Join(";", hubDto.SupportedLanguages);
            DefaultLanguage = hubDto.DefaultLanguage;
            MaxObservationsAfterCriteria = hubDto.MaxObservationsAfterCriteria;
            DecimalSeparator = hubDto.DecimalSeparator;
            DecimalNumber = hubDto.DecimalNumber;
            EmptyCellDefaultValue = hubDto.EmptyCellDefaultValue;
            DefaultView = hubDto.DefaultView;
            ChangeBackgroundMediaURL(hubDto.BackgroundMediaURL);
            MaxCells = hubDto.MaxCells;
            Extras = hubDto.Extras;

            var changeDataflowData = MaxObservationsAfterCriteria != hubDto.MaxObservationsAfterCriteria;
            if (changeDataflowData)
            {
                AddDomainEvent(new HubDataflowDataParamiterChangedPublicEvent());
            }

            SetSupportedLanguage(hubDto.SupportedLanguages);
            SetTitleTransaltion(hubDto.Title);
            SetSloganTransaltion(hubDto.Slogan);
            SetDescriptionTransaltion(hubDto.Description);
            SetDisclaimerTransaltion(hubDto.Disclaimer);

            return this;
        }

        public bool ChangeLogoUrl(string filePath)
        {
            if (filePath == null)
            {
                filePath = "";
            }
            if (filePath.Equals(this.LogoURL))
            {
                return false;
            }
            AddDomainEvent(new ImageChangePublicEvent(this.GetType().Name, this.LogoURL, filePath));

            this.LogoURL = filePath;

            return true;
        }

        public bool ChangeBackgroundMediaURL(string filePath)
        {
            if (filePath == null)
            {
                filePath = "";
            }
            if (filePath.Equals(this.BackgroundMediaURL))
            {
                return false;
            }
            AddDomainEvent(new ImageChangePublicEvent(this.GetType().Name, this.BackgroundMediaURL, filePath));

            this.BackgroundMediaURL = filePath;

            return true;
        }

        public void SetSupportedLanguage(List<string> langs)
        {
            SupportedLanguages = string.Join(";", langs);
        }

        public bool AddSupportedLanguage(string lang)
        {
            if (string.IsNullOrWhiteSpace(SupportedLanguages))
            {
                SupportedLanguages = lang;
                return true;
            }

            var haveLang = SupportedLanguages.Split(';').ToList().Contains(lang);
            if (haveLang)
            {
                return false;
            }

            SupportedLanguages += $";{lang}";
            return true;
        }

        public bool RemoveSupportedLanguage(string lang)
        {
            if (string.IsNullOrWhiteSpace(SupportedLanguages))
            {
                return false;
            }

            return SupportedLanguages.Split(';').ToList().Remove(lang);
        }

        public bool SetTitleTransaltion(Dictionary<string, string> translates)
        {
            if (translates == null || translates.Count <= 0)
            {
                Title = null;
                return true;
            }

            if (Title == null)
            {
                Title = TransatableItem.CreateTransatableItem(translates);
                return true;
            }
            else if (Title.TransatableItemValues != null)
            { //Remove all old transalte
                Title.ClearTransatableItemValue();
            }
            //Reinsert all
            Title.AddTransatableItemValue(translates);

            return true;
        }

        public bool SetSloganTransaltion(Dictionary<string, string> translates)
        {
            if (translates == null || translates.Count <= 0)
            {
                Slogan = null;
                return true;
            }

            if (Slogan == null)
            {
                Slogan = TransatableItem.CreateTransatableItem(translates);
                return true;
            }
            else if (Slogan.TransatableItemValues != null)
            { //Remove all old transalte
                Slogan.ClearTransatableItemValue();
            }
            //Reinsert all
            Slogan.AddTransatableItemValue(translates);

            return true;
        }

        public bool SetDescriptionTransaltion(Dictionary<string, string> translates)
        {
            if (translates == null || translates.Count <= 0)
            {
                Description = null;
                return true;
            }

            if (Description == null)
            {
                Description = TransatableItem.CreateTransatableItem(translates);
                return true;
            }
            else if (Description.TransatableItemValues != null)
            { //Remove all old transalte
                Description.ClearTransatableItemValue();
            }
            //Reinsert all
            Description.AddTransatableItemValue(translates);

            return true;
        }

        public bool SetDisclaimerTransaltion(Dictionary<string, string> translates)
        {
            if (translates == null || translates.Count <= 0)
            {
                Disclaimer = null;
                return true;
            }

            if (Disclaimer == null)
            {
                Disclaimer = TransatableItem.CreateTransatableItem(translates);
                return true;
            }
            else if (Disclaimer.TransatableItemValues != null)
            { //Remove all old transalte
                Disclaimer.ClearTransatableItemValue();
            }
            //Reinsert all
            Disclaimer.AddTransatableItemValue(translates);

            return true;
        }

    }
}
