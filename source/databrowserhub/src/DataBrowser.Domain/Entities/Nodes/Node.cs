using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Domain.Entities.TransatableItems;
using DataBrowser.Domain.Events;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBrowser.Domain.Entities.Nodes
{
    public class Node : Entity, IAggregateRoot
    {
        public int NodeId { get; protected set; }
        public bool Active { get; protected set; }
        public bool Default { get; protected set; }
        public string Agency { get; protected set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Logo { get; protected set; }
        public string EndPoint { get; set; }
        public int Order { get; set; }
        public bool EnableHttpAuth { get; set; }
        public string AuthHttpUsername { get; set; }
        public string AuthHttpPassword { get; set; }
        public string AuthHttpDomain { get; set; }
        public bool EnableProxy { get; set; }
        public bool UseProxySystem { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public string BackgroundMediaURL { get; protected set; }
        public string EmptyCellDefaultValue { get; set; }
        public string DefaultView { get; set; }
        public bool ShowDataflowUncategorized { get; set; }
        public bool ShowDataflowNotInProduction { get; set; }
        public string CriteriaSelectionMode { get; set; }
        public string LabelDimensionTerritorial { get; protected set; }
        public string LabelDimensionTemporal { get; protected set; }
        public string CategorySchemaExcludes { get; protected set; }
        public string EndPointFormatSupported { get; set; }
        public int? DecimalNumber { get; set; }
        public string CatalogNavigationMode { get; set; }
        public int ShowCategoryLevels { get; set; }
        public int? TtlDataflow { get; set; }
        public int? TtlCatalog { get; set; }


        //FK
        public int? TitleFk { get; protected set; }
        public int? SloganFk { get; protected set; }
        public int? DescriptionFk { get; protected set; }
        public int? DecimalSeparatorFk { get; protected set; }


        //Navigation Property
        public virtual TransatableItem Title { get; protected set; }
        public virtual TransatableItem Slogan { get; protected set; }
        public virtual TransatableItem Description { get; protected set; }
        public virtual TransatableItem DecimalSeparator { get; protected set; }

        private readonly List<Extra> _extras = new List<Extra>();
        public virtual IReadOnlyCollection<Extra> Extras => _extras?.AsReadOnly();

        protected Node()
        {

        }



        public static Node CreateNode(NodeDto nodeDto)
        {
            if (string.IsNullOrWhiteSpace(nodeDto.Type))
            {
                throw new ArgumentNullException(nameof(nodeDto.Type));
            }
            if (string.IsNullOrWhiteSpace(nodeDto.Code))
            {
                throw new ArgumentNullException(nameof(nodeDto.Code));
            }
            if (string.IsNullOrWhiteSpace(nodeDto.EndPoint))
            {
                throw new ArgumentNullException(nameof(nodeDto.EndPoint));
            }

            var node = new Node
            {
                Active = nodeDto.Active,
                Default = nodeDto.Default,
                Agency = nodeDto.Agency,
                AuthHttpPassword = nodeDto.AuthHttpPassword,
                AuthHttpUsername = nodeDto.AuthHttpUsername,
                AuthHttpDomain = nodeDto.AuthHttpDomain,
                Code = nodeDto.Code,
                Type = nodeDto.Type,
                EnableHttpAuth = nodeDto.EnableHttpAuth,
                EnableProxy = nodeDto.EnableProxy,
                EndPoint = nodeDto.EndPoint,
                Order = nodeDto.Order,
                ProxyAddress = nodeDto.ProxyAddress,
                ProxyPassword = nodeDto.ProxyPassword,
                ProxyPort = nodeDto.ProxyPort,
                ProxyUsername = nodeDto.ProxyUsername,
                UseProxySystem = nodeDto.UseProxySystem,
                EmptyCellDefaultValue = nodeDto.EmptyCellDefaultValue,
                DefaultView = nodeDto.DefaultView,
                ShowDataflowUncategorized = nodeDto.ShowDataflowUncategorized,
                ShowDataflowNotInProduction = nodeDto.ShowDataflowNotInProduction,
                CriteriaSelectionMode = nodeDto.CriteriaSelectionMode,
                LabelDimensionTerritorial = convertDimensionTerritorialFromDto(nodeDto),
                LabelDimensionTemporal = convertDimensionTemporalFromDto(nodeDto),
                CategorySchemaExcludes = convertCategorySchemaExcludesFromDto(nodeDto),
                EndPointFormatSupported = nodeDto.EndPointFormatSupported,
                DecimalNumber = nodeDto.DecimalNumber,
                CatalogNavigationMode = nodeDto.CatalogNavigationMode,
                ShowCategoryLevels = nodeDto.ShowCategoryLevels,
                TtlDataflow = nodeDto.TtlDataflow,
                TtlCatalog = nodeDto.TtlCatalog
            };
            node.ChangeLogoUrl(nodeDto.Logo);
            node.ChangeBackgroundMediaURL(nodeDto.BackgroundMediaURL);

            if (nodeDto.Title != null && nodeDto.Title.Count > 0)
            {
                node.Title = TransatableItem.CreateTransatableItem(nodeDto.Title);
            }
            if (nodeDto.Slogan != null && nodeDto.Slogan.Count > 0)
            {
                node.Slogan = TransatableItem.CreateTransatableItem(nodeDto.Slogan);
            }
            if (nodeDto.Description != null && nodeDto.Description.Count > 0)
            {
                node.Description = TransatableItem.CreateTransatableItem(nodeDto.Description);
            }
            if (nodeDto.DecimalSeparator != null && nodeDto.DecimalSeparator.Count > 0)
            {
                node.DecimalSeparator = TransatableItem.CreateTransatableItem(nodeDto.DecimalSeparator);
            }

            if (nodeDto.LabelDimensionTerritorials != null)
            {
                foreach (var item in nodeDto.LabelDimensionTerritorials)
                {
                    node.AddDimensionTerritorial(item);
                }
            }
            if (nodeDto.LabelDimensionTemporals != null)
            {
                foreach (var item in nodeDto.LabelDimensionTemporals)
                {
                    node.AddDimensionTemporal(item);
                }
            }
            if (nodeDto.CategorySchemaExcludes != null)
            {
                foreach (var item in nodeDto.CategorySchemaExcludes)
                {
                    node.AddCategorySchemaExclude(item);
                }
            }

            if (nodeDto.Extras != null && nodeDto.Extras.Count > 0)
            {
                nodeDto.Extras.ForEach(i => node.AddExtra(i.Key, i.Value, i.IsPublic, i.ValueType, i.Transaltes));
            }

            return node;
        }

        private static string convertCategorySchemaExcludesFromDto(NodeDto nodeDto)
        {
            return nodeDto.CategorySchemaExcludes != null && nodeDto.CategorySchemaExcludes.Count > 0 ? string.Join(";", nodeDto.CategorySchemaExcludes) : null;
        }

        private static string convertDimensionTemporalFromDto(NodeDto nodeDto)
        {
            return nodeDto.LabelDimensionTemporals != null && nodeDto.LabelDimensionTemporals.Count > 0 ? string.Join(";", nodeDto.LabelDimensionTemporals) : null;
        }

        public bool AddExtra(string key, string value, bool isPublic = true, string valueType = null, Dictionary<string, string> translates = null)
        {
            var extraItem = _extras.FirstOrDefault(i => i.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            if (extraItem == null)
            {
                extraItem = Extra.CreateExtra(key, value, valueType: valueType, isPublic: isPublic, translates: translates);
                _extras.Add(extraItem);

                return true;
            }
            extraItem.Value = value;
            extraItem.IsPublic = isPublic;
            extraItem.ValueType = valueType;


            if (extraItem.TransatableItem == null && translates != null)
            {
                extraItem.SetTransatableItem(TransatableItem.CreateTransatableItem(translates));
            }
            else if (extraItem.TransatableItem != null && translates != null)
            {
                extraItem.TransatableItem.AddTransatableItemValue(translates);
            }

            return true;
        }

        public void AddCategorySchemaExclude(string categorySchema)
        {
            if (!string.IsNullOrWhiteSpace(CategorySchemaExcludes) &&
                CategorySchemaExcludes.ToUpperInvariant().Split(';').Contains(categorySchema.ToUpperInvariant()))
            {
                return;
            }

            CategorySchemaExcludes = string.IsNullOrWhiteSpace(CategorySchemaExcludes) ? categorySchema : $"{CategorySchemaExcludes};{categorySchema}";
        }

        public void RemoveCategorySchemaExclude(string categorySchema)
        {
            if (string.IsNullOrWhiteSpace(CategorySchemaExcludes) ||
                !CategorySchemaExcludes.ToUpperInvariant().Split(';').Contains(categorySchema.ToUpperInvariant()))
            {
                return;
            }

            CategorySchemaExcludes = string.IsNullOrWhiteSpace(CategorySchemaExcludes) ? CategorySchemaExcludes.Replace(categorySchema, "") : CategorySchemaExcludes.Replace($";{categorySchema}", "");
        }

        public void ClearCategorySchemaExclude()
        {
            CategorySchemaExcludes = null;
        }

        public void AddDimensionTerritorial(string dimensionTerritorial)
        {
            if (!string.IsNullOrWhiteSpace(LabelDimensionTerritorial) &&
                LabelDimensionTerritorial.ToUpperInvariant().Split(';').Contains(dimensionTerritorial.ToUpperInvariant()))
            {
                return;
            }

            LabelDimensionTerritorial = string.IsNullOrWhiteSpace(LabelDimensionTerritorial) ? dimensionTerritorial : $"{LabelDimensionTerritorial};{dimensionTerritorial}";
        }
        public void RemoveDimensionTerritorial(string dimensionTerritorial)
        {
            if (string.IsNullOrWhiteSpace(LabelDimensionTerritorial) ||
                !LabelDimensionTerritorial.ToUpperInvariant().Split(';').Contains(dimensionTerritorial.ToUpperInvariant()))
            {
                return;
            }

            LabelDimensionTerritorial = string.IsNullOrWhiteSpace(LabelDimensionTerritorial) ? CategorySchemaExcludes.Replace(dimensionTerritorial, "") : LabelDimensionTerritorial.Replace($";{dimensionTerritorial}", "");
        }
        public void ClearDimensionTerritorial()
        {
            LabelDimensionTerritorial = null;
        }

        public void AddDimensionTemporal(string dimensionTemporal)
        {
            if (!string.IsNullOrWhiteSpace(LabelDimensionTemporal) &&
                LabelDimensionTemporal.ToUpperInvariant().Split(';').Contains(dimensionTemporal.ToUpperInvariant()))
            {
                return;
            }

            LabelDimensionTemporal = string.IsNullOrWhiteSpace(LabelDimensionTemporal) ? dimensionTemporal : $"{LabelDimensionTemporal};{dimensionTemporal}";
        }
        public void RemoveDimensionTemporal(string dimensionTemporal)
        {
            if (string.IsNullOrWhiteSpace(LabelDimensionTerritorial) ||
                !LabelDimensionTemporal.ToUpperInvariant().Split(';').Contains(dimensionTemporal.ToUpperInvariant()))
            {
                return;
            }

            LabelDimensionTemporal = string.IsNullOrWhiteSpace(LabelDimensionTemporal) ? CategorySchemaExcludes.Replace(dimensionTemporal, "") : LabelDimensionTemporal.Replace($";{dimensionTemporal}", "");
        }
        public void ClearDimensionTemporal()
        {
            LabelDimensionTemporal = null;
        }

        public bool RemoveExtra(string key)
        {
            var extraItem = _extras.FirstOrDefault(i => i.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (extraItem == null)
            {
                return false;
            }

            if (extraItem.TransatableItem != null)
            {
                AddDomainEvent(new TranslatedItemRemovedEvent(extraItem.TransatableItem.TransatableItemId));
            }

            return _extras.Remove(extraItem);
        }

        public bool SetExtraTransaltion(string key, string lang, string value)
        {
            return SetExtraTransaltion(key, new Dictionary<string, string> { { lang, value } });
        }

        public bool SetExtraTransaltion(string key, Dictionary<string, string> translates)
        {
            var itemExtra = _extras.FirstOrDefault(i => i.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (itemExtra == null)
            {
                throw new ArgumentException($"key {key} not found");
            }

            if (translates == null || translates.Count <= 0)
            {
                itemExtra.RemoveTransatableItem();
                return true;
            }

            if (itemExtra.TransatableItem == null)
            {
                itemExtra.SetTransatableItem(TransatableItem.CreateTransatableItem(translates));
                return true;
            }

            itemExtra.TransatableItem.AddTransatableItemValue(translates);

            return true;
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

        public void EditNode(NodeDto nodeDto)
        {
            var oldNodeCode = !Code.Equals(nodeDto.Code) ? Code : null;
            bool referenceIsChanged = checkForReferenceEndPointChanged(nodeDto);

            Active = nodeDto.Active;
            Default = nodeDto.Default;
            Agency = nodeDto.Agency;
            AuthHttpPassword = nodeDto.AuthHttpPassword;
            AuthHttpUsername = nodeDto.AuthHttpUsername;
            AuthHttpDomain = nodeDto.AuthHttpDomain;
            Code = nodeDto.Code;
            Type = nodeDto.Type;
            EnableHttpAuth = nodeDto.EnableHttpAuth;
            EnableProxy = nodeDto.EnableProxy;
            EndPoint = nodeDto.EndPoint;
            ChangeLogoUrl(nodeDto.Logo);
            Order = nodeDto.Order;
            ProxyAddress = nodeDto.ProxyAddress;
            ProxyPassword = nodeDto.ProxyPassword;
            ProxyPort = nodeDto.ProxyPort;
            ProxyUsername = nodeDto.ProxyUsername;
            UseProxySystem = nodeDto.UseProxySystem;
            EmptyCellDefaultValue = nodeDto.EmptyCellDefaultValue;
            DefaultView = nodeDto.DefaultView;
            ShowDataflowUncategorized = nodeDto.ShowDataflowUncategorized;
            ShowDataflowNotInProduction = nodeDto.ShowDataflowNotInProduction;
            CriteriaSelectionMode = nodeDto.CriteriaSelectionMode;
            LabelDimensionTerritorial = convertDimensionTerritorialFromDto(nodeDto);
            LabelDimensionTemporal = convertDimensionTemporalFromDto(nodeDto);
            CategorySchemaExcludes = convertCategorySchemaExcludesFromDto(nodeDto);
            EndPointFormatSupported = nodeDto.EndPointFormatSupported;
            DecimalNumber = nodeDto.DecimalNumber;
            CatalogNavigationMode = nodeDto.CatalogNavigationMode;
            ChangeBackgroundMediaURL(nodeDto.BackgroundMediaURL);
            ShowCategoryLevels = nodeDto.ShowCategoryLevels;
            TtlDataflow = nodeDto.TtlDataflow;
            TtlCatalog = nodeDto.TtlCatalog;


            //
            // BEGIN EXTRA
            //
            //Take all key to remove
            var removeKey = new List<string>();
            foreach (var i in Extras)
            {
                if (nodeDto.Extras == null || !nodeDto.Extras.Any(k => k.Key.Equals(i.Key, StringComparison.InvariantCultureIgnoreCase)))
                {
                    removeKey.Add(i.Key);
                }
            }
            removeKey.ForEach(i => RemoveExtra(i));
            //Remove all TransaltionItemValue (for reinsert after this step)
            foreach (var itemExtra in Extras)
            {
                if (itemExtra.TransatableItem != null)
                {
                    itemExtra.TransatableItem.ClearTransatableItemValue();
                }
            }
            //Add or Edit key
            if (nodeDto.Extras != null)
            {
                nodeDto.Extras.ForEach(i =>
                {
                    AddExtra(key: i.Key, value: i.Value, valueType: i.ValueType, isPublic: i.IsPublic, translates: i.Transaltes);
                });
            }
            //
            // END EXTRA
            //

            SetTitleTransaltion(nodeDto.Title);
            SetSloganTransaltion(nodeDto.Slogan);
            SetDescriptionTransaltion(nodeDto.Description);
            SetDecimalSeparetorTransaltion(nodeDto.DecimalSeparator);

            if (referenceIsChanged)
            {
                AddDomainEvent(new NodeEndPointReferenceChangedPublicEvent(NodeId, nodeDto.Code, oldNodeCode));
                AddDomainEvent(new NodeEndPointReferenceChangedEvent(NodeId, nodeDto.Code, oldNodeCode));
            }
        }

        private static string convertDimensionTerritorialFromDto(NodeDto nodeDto)
        {
            return nodeDto.LabelDimensionTerritorials != null && nodeDto.LabelDimensionTerritorials.Count > 0 ? string.Join(";", nodeDto.LabelDimensionTerritorials) : null;
        }

        private bool checkForReferenceEndPointChanged(NodeDto nodeDto)
        {
            var referenceIsChanged = !EndPoint.Equals(nodeDto.EndPoint) || 
                                        !Type.Equals(nodeDto.Type);
            if (!referenceIsChanged)
            {
                referenceIsChanged = (LabelDimensionTerritorial != null && !LabelDimensionTerritorial.Equals(convertDimensionTerritorialFromDto(nodeDto))) ||
                                       (LabelDimensionTerritorial == null && convertDimensionTerritorialFromDto(nodeDto) != null);
            }
            if (!referenceIsChanged)
            {
                referenceIsChanged = ShowDataflowUncategorized != nodeDto.ShowDataflowUncategorized;
            }
            if (!referenceIsChanged)
            {
                referenceIsChanged = (CategorySchemaExcludes != null && !CategorySchemaExcludes.Equals(nodeDto.CategorySchemaExcludes)) ||
                                    (CategorySchemaExcludes == null && nodeDto.CategorySchemaExcludes != null);
            }
            if (!referenceIsChanged)
            {
                referenceIsChanged = (Extras != null && !DataBrowserJsonSerializer.SerializeObject(Extras).Equals(nodeDto.Extras)) ||
                                        (Extras == null && Extras != null);
            }
            //if (!referenceIsChanged) //Include in extra
            //{
            //    referenceIsChanged = checkIfExtraRestDataResponseXmlChanged(nodeDto, referenceIsChanged);
            //}

            return referenceIsChanged;
        }

        private bool checkIfExtraRestDataResponseXmlChanged(NodeDto nodeDto, bool referenceIsChanged)
        {
            Extra oldRestDataResponseXml = null;
            ExtraDto newRestDataResponseXml = null;
            if (Extras != null)
            {
                oldRestDataResponseXml = Extras.FirstOrDefault(i => i.Key.Equals("RestDataResponseXml", StringComparison.InvariantCultureIgnoreCase));
            }
            if (nodeDto.Extras != null)
            {
                newRestDataResponseXml = nodeDto.Extras.FirstOrDefault(i => i.Key.Equals("RestDataResponseXml", StringComparison.InvariantCultureIgnoreCase));
            }
            if ((oldRestDataResponseXml != null && newRestDataResponseXml == null) ||
                (oldRestDataResponseXml == null && newRestDataResponseXml != null))
            {
                referenceIsChanged = true;
            }
            else if (oldRestDataResponseXml != null && newRestDataResponseXml != null)
            {
                if (!oldRestDataResponseXml.Value.Equals(newRestDataResponseXml.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    referenceIsChanged = true;
                }
            }

            return referenceIsChanged;
        }

        public bool ChangeLogoUrl(string filePath)
        {
            if (filePath == null)
            {
                filePath = "";
            }
            if (filePath.Equals(this.Logo))
            {
                return false;
            }
            AddDomainEvent(new ImageChangePublicEvent(this.GetType().Name, this.Logo, filePath));

            this.Logo = filePath;

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
    }
}
