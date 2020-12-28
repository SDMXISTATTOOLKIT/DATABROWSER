using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Domain.Entities.TransatableItems;
using DataBrowser.Domain.Entities.ViewTemplates.Validators;
using DataBrowser.Domain.Specifications.Rules;
using DataBrowser.Domain.Validators;
using DataBrowser.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataBrowser.Domain.Entities.ViewTemplates
{

    public class ViewTemplate : Entity, IAggregateRoot
    {
        public int ViewTemplateId { get; protected set; }
        public string DatasetId { get; set; }
        public string Type { get; set; }
        public string DefaultView { get; set; }
        public string Criteria { get; set; }
        public string Layouts { get; set; }
        public string HiddenDimensions { get; set; }
        public bool BlockAxes { get; set; }
        public bool EnableLayout { get; set; }
        public bool EnableCriteria { get; set; }
        public bool EnableVariation { get; set; }
        public int DecimalNumber { get; set; }
        public DateTime ViewTemplateCreationDate { get; set; }

        //Foreign keys
        public int? TitleFK { get; protected set; }
        public int NodeFK { get; protected set; }
        public int? UserFK { get; protected set; }
        public int? DecimalSeparatorFk { get; protected set; }


        //Navigation Property
        public virtual TransatableItem Title { get; protected set; }
        public virtual TransatableItem DecimalSeparator { get; protected set; }

        protected ViewTemplate()
        {

        }

        public static async Task<IValidator<ViewTemplateDto, ViewTemplate>> CreateViewTemplateAsync(ViewTemplateDto dto, IEnumerable<IRuleSpecification<ViewTemplateDto>> rules)
        {
            var validator = new Validator<ViewTemplateDto, ViewTemplate>(rules);
            await validator.ExecuteCheckAsync(dto, new ViewTemplate());

            if (!validator.IsValid)
            {
                return validator;
            }

            validator.ValidateObject.BlockAxes = dto.BlockAxes;
            validator.ValidateObject.Criteria = dto.ConvertCriteriaToText();
            validator.ValidateObject.DatasetId = dto.DatasetId;
            validator.ValidateObject.DecimalNumber = dto.DecimalNumber;
            validator.ValidateObject.EnableCriteria = dto.EnableCriteria;
            validator.ValidateObject.DefaultView = dto.DefaultView;
            validator.ValidateObject.EnableVariation = dto.EnableVariation;
            validator.ValidateObject.EnableLayout = dto.EnableLayout;
            validator.ValidateObject.HiddenDimensions = dto.HiddenDimensions;
            validator.ValidateObject.Layouts = dto.Layouts;
            validator.ValidateObject.Type = dto.Type.ToString();
            validator.ValidateObject.ViewTemplateId = dto.ViewTemplateId;
            validator.ValidateObject.ViewTemplateCreationDate = dto.ViewTemplateCreationDate;
            validator.ValidateObject.NodeFK = dto.NodeId;
            validator.ValidateObject.UserFK = dto.UserId;


            if (dto.Title != null && dto.Title.Count > 0)
            {
                validator.ValidateObject.Title = TransatableItem.CreateTransatableItem(dto.Title);

            }
            if (dto.DecimalSeparator != null && dto.DecimalSeparator.Count > 0)
            {
                validator.ValidateObject.DecimalSeparator = TransatableItem.CreateTransatableItem(dto.DecimalSeparator);
            }

            return validator;
        }

        public async Task<IValidator<ViewTemplateDto, ViewTemplate>> EditAsync(ViewTemplateDto dto, IEnumerable<IRuleSpecification<ViewTemplateDto>> rules)
        {
            var validator = new Validator<ViewTemplateDto, ViewTemplate>(rules);
            await validator.ExecuteCheckAsync(dto, this);

            if (!validator.IsValid)
            {
                return validator;
            }

            SetTitleTranslation(dto.Title);
            SetDecimalSeparetorTransaltion(dto.DecimalSeparator);

            BlockAxes = dto.BlockAxes;
            Criteria = dto.ConvertCriteriaToText();
            DatasetId = dto.DatasetId;
            DecimalNumber = dto.DecimalNumber;
            EnableCriteria = dto.EnableCriteria;
            DefaultView = dto.DefaultView;
            EnableVariation = dto.EnableVariation;
            HiddenDimensions = dto.HiddenDimensions;
            Layouts = dto.Layouts;
            Type = dto.Type.ToString();
            ViewTemplateId = dto.ViewTemplateId;
            ViewTemplateCreationDate = dto.ViewTemplateCreationDate;
            NodeFK = dto.NodeId;
            UserFK = dto.UserId;
            EnableLayout = dto.EnableLayout;

            return validator;
        }

        private bool SetTitleTranslation(Dictionary<string, string> translates)
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

        public bool SetDecimalSeparetorTransaltion(Dictionary<string, string> translates)
        {
            if (translates == null || translates.Count <= 0)
            {
                DecimalSeparator = null;
                return true;
            }

            if (DecimalSeparator == null)
            {
                DecimalSeparator = TransatableItem.CreateTransatableItem(translates);
                return true;
            }
            else if (DecimalSeparator.TransatableItemValues != null)
            { //Remove all old transalte
                DecimalSeparator.ClearTransatableItemValue();
            }
            //Reinsert all
            DecimalSeparator.AddTransatableItemValue(translates);

            return true;
        }

    }
}
