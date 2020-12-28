using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using EndPointConnector.Interfaces.Sdmx.Models;
using log4net;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.MetadataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;

namespace EndPointConnector.ParserSdmx
{
    public class SDMXUtils
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SDMXUtils));

        /// <summary>
        ///     This regex object holds the regular expression that validates the time dimension contrains
        /// </summary>
        private static readonly Regex _timePeriodValidate =
            new Regex("^[12][09][0-9]{2}(-((Q[1-4])|(W[1-5][0-9])|(H[12])|([01][0-9])|([01][0-9]-[0-3][0-9])))?$");

        public static string GetQuerySoapAction(SdmxStructureEnumType artefactType)
        {
            _logger.Debug($"START {MethodBase.GetCurrentMethod().Name}");
            switch (artefactType)
            {
                case SdmxStructureEnumType.CodeList:
                    return "GetCodelist";
                case SdmxStructureEnumType.CategoryScheme:
                    return "GetCategoryScheme";
                case SdmxStructureEnumType.ConceptScheme:
                    return "GetConceptScheme";
                case SdmxStructureEnumType.Dataflow:
                    return "GetDataflow";
                case SdmxStructureEnumType.Dsd:
                    return "GetDataStructure";
                case SdmxStructureEnumType.Categorisation:
                    return "GetCategorisation";
                case SdmxStructureEnumType.MetadataFlow:
                    return "GetMetadataflow";

                case SdmxStructureEnumType.Agency:
                    return "GetOrganisationScheme";

                case SdmxStructureEnumType.AgencyScheme:
                    return "GetOrganisationScheme";
                case SdmxStructureEnumType.DataProviderScheme:
                    return "GetOrganisationScheme";
                case SdmxStructureEnumType.DataConsumerScheme:
                    return "GetOrganisationScheme";
                case SdmxStructureEnumType.OrganisationUnitScheme:
                    return "GetOrganisationScheme";
                case SdmxStructureEnumType.StructureSet:
                    return "GetStructureSet";
                case SdmxStructureEnumType.ContentConstraint:
                    return "GetConstraint";
                case SdmxStructureEnumType.HierarchicalCodelist:
                    return "GetHierarchicalCodelist";
                case SdmxStructureEnumType.Msd:
                    return "GetMetadataStructure";

                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetArtefactString(SdmxStructureEnumType artefactType)
        {
            switch (artefactType)
            {
                case SdmxStructureEnumType.CodeList:
                    return "codelist";
                case SdmxStructureEnumType.CategoryScheme:
                    return "categoryscheme";
                case SdmxStructureEnumType.ConceptScheme:
                    return "conceptscheme";
                case SdmxStructureEnumType.Dataflow:
                    return "dataflow";
                case SdmxStructureEnumType.Dsd:
                    return "datastructure";
                case SdmxStructureEnumType.Categorisation:
                    return "categorisation";

                case SdmxStructureEnumType.Agency:
                    return "agencyscheme";

                case SdmxStructureEnumType.AgencyScheme:
                    return "agencyscheme";
                case SdmxStructureEnumType.DataProviderScheme:
                    return "dataproviderscheme";
                case SdmxStructureEnumType.DataConsumerScheme:
                    return "dataconsumerscheme";
                case SdmxStructureEnumType.OrganisationUnitScheme:
                    return "organisationunitscheme";
                case SdmxStructureEnumType.StructureSet:
                    return "structureset";
                case SdmxStructureEnumType.ContentConstraint:
                    return "contentconstraint";
                case SdmxStructureEnumType.HierarchicalCodelist:
                    return "hierarchicalcodelist";
                case SdmxStructureEnumType.Msd:
                    return "metadatastructure";

                default:
                    return artefactType.ToString().ToLowerInvariant();
            }
        }

        public static SdmxStructureEnumType GetArtefactEnum(string artefactType)
        {
            switch (artefactType.ToLower())
            {
                case "codelist":
                    return SdmxStructureEnumType.CodeList;
                case "categoryscheme":
                    return SdmxStructureEnumType.CategoryScheme;
                case "conceptscheme":
                    return SdmxStructureEnumType.ConceptScheme;
                case "dataflow":
                    return SdmxStructureEnumType.Dataflow;
                case "datastructure":
                    return SdmxStructureEnumType.Dsd;
                case "categorisation":
                    return SdmxStructureEnumType.Categorisation;
                default:
                    return SdmxStructureEnumType.CodeList;
            }
        }

        /// <summary>
        ///     The contents from <paramref name="source" /> to <paramref name="destination" />
        /// </summary>
        /// <param name="source">
        ///     The source collection.
        /// </param>
        /// <param name="destination">
        ///     The destination collection.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the <paramref name="source" /> and <paramref name="destination" /> items
        /// </typeparam>
        public static void CopyCollection<T>(ICollection<T> source, ICollection<T> destination)
        {
            foreach (var value in source) destination.Add(value);
        }

        /// <summary>
        ///     Get a concept from the specified map from the specified component
        /// </summary>
        /// <param name="component">
        ///     The component
        /// </param>
        /// <param name="conceptMap">
        ///     The concept id to concept bean map
        /// </param>
        /// <returns>
        ///     The concept of the specified component or null
        /// </returns>
        public static IConceptObject GetCachedConcept(IComponent component,
            Dictionary<string, IConceptObject> conceptMap)
        {
            IConceptObject concept;
            conceptMap.TryGetValue(MakeKeyForConcept(component), out concept);
            return concept;
        }

        /// <summary>
        ///     Get the Dimension or TimeDimension that uses the specified concept id inside a KeyFamily
        /// </summary>
        /// <param name="keyFamily">
        ///     The KeyFamily to search
        /// </param>
        /// <param name="concept">
        ///     The concept id e.g. "FREQ"
        /// </param>
        /// <returns>
        ///     The Dimension or TimeDimension as ComponentBean if found, else null
        /// </returns>
        public static IDimension GetComponentByName(IDataStructureObject keyFamily, string concept)
        {
            IDimension component;
            if (keyFamily.TimeDimension != null && keyFamily.TimeDimension.Id.Equals(concept))
                component = keyFamily.TimeDimension;
            else
                component = keyFamily.GetDimension(concept);

            return component;
        }

        /// <summary>
        ///     This method checks a dsd for compliance with producing Compact Data.
        ///     In detail, it checks that if a TimeDimension is present and at least
        ///     one dimension is frequency dimension. If there is none an error message
        ///     is returned to the caller
        /// </summary>
        /// <param name="dsd">
        ///     The <see cref="Estat.Sdmx.Model.Structure.KeyFamilyBean" />of the DSD to be checked
        /// </param>
        /// <returns>
        ///     The error messages in case of invalid dsd or an empty string in case a valid dsd
        /// </returns>
        public static string ValidateForCompact(IDataStructureObject dsd)
        {
            var text = string.Empty;
            var isFrequency = false;

            foreach (var dimension in dsd.DimensionList.Dimensions)
                if (dimension.FrequencyDimension)
                {
                    isFrequency = true;
                    break;
                }

            if (dsd.TimeDimension == null)
                // text= "Dsd does not have at least one Frequency dimension";
                text = "DSD " + dsd.Id + " v" + dsd.Version
                       + " does not have a Time Dimension. Only Cross-Sectional data may be requested.";
            else if (!isFrequency)
                // normally it should never reach here
                text = "DSD " + dsd.Id + " v" + dsd.Version
                       +
                       " does not have a Frequency dimension. According SDMX v2.0: Any DSD which uses the Time dimension must also declare a frequency dimension.";

            return text;
        }

        /// <summary>
        ///     Check if the specified <paramref name="keyFamily" /> can do Time Series data
        /// </summary>
        /// <param name="keyFamily">
        ///     The <see cref="KeyFamilyBean" />
        /// </param>
        /// <returns>
        ///     True if it is a time series DSD else false
        /// </returns>
        public static bool IsTimeSeries(IDataStructureObject keyFamily)
        {
            return string.IsNullOrEmpty(ValidateForCompact(keyFamily));
        }

        /// <summary>
        ///     This is an utility method that verify if a string text equals "true" constant value
        ///     The check is done in invariant culture and the string case is ignored.
        ///     In case the input is null or empty the result is false
        /// </summary>
        /// <param name="input">
        ///     The string that needs to be check
        /// </param>
        /// <returns>
        ///     The returned values are:
        ///     <list type="bullet">
        ///         <item>
        ///             true in case the string is "true"
        ///         </item>
        ///         <item>
        ///             false in case the string is null or empty or any other string
        ///         </item>
        ///     </list>
        /// </returns>
        public static bool IsTrueString(string input)
        {
            var ret = false;

            if (!string.IsNullOrEmpty(input)) ret = input.Equals("true", StringComparison.OrdinalIgnoreCase);

            return ret;
        }

        /// <summary>
        ///     Generate a key using the following format AgencyID+ID+Version
        /// </summary>
        /// <param name="artefact">
        ///     The artefact to generate the key for
        /// </param>
        /// <returns>
        ///     The key
        /// </returns>
        public static string MakeKey(IMaintainableObject artefact)
        {
            return MakeKey(artefact.AgencyId, artefact.Id, artefact.Version);
        }

        /// <summary>
        ///     Generate a key using the following format AgencyID+ID+Version
        /// </summary>
        /// <param name="reference">
        ///     The artefact reference to generate the key for
        /// </param>
        /// <returns>
        ///     The key
        /// </returns>
        public static string MakeKey(IStructureReference reference)
        {
            return MakeKey(reference.MaintainableReference.AgencyId, reference.MaintainableReference.MaintainableId,
                reference.MaintainableReference.Version);
        }

        /// <summary>
        ///     Generate a key using the following format AgencyID+ID+Version
        /// </summary>
        /// <param name="reference">
        ///     The artefact reference to generate the key for
        /// </param>
        /// <returns>
        ///     The key
        /// </returns>
        public static string MakeKey(ICrossReference reference)
        {
            return MakeKey(reference.MaintainableReference.AgencyId, reference.MaintainableReference.MaintainableId,
                reference.MaintainableReference.Version);
        }

        /// <summary>
        ///     Generate a key using the following format AgencyID+ID+Version
        /// </summary>
        /// <param name="reference">
        ///     The artefact reference to generate the key for
        /// </param>
        /// <returns>
        ///     The key
        /// </returns>
        public static string MakeKey(IMaintainableRefObject reference)
        {
            return MakeKey(reference.AgencyId, reference.MaintainableId, reference.Version);
        }

        /// <summary>
        ///     Create a key for the <paramref name="item" /> that belongs to <paramref name="itemScheme" />
        /// </summary>
        /// <param name="item">
        ///     The <see cref="ItemBean" />
        /// </param>
        /// <param name="itemScheme">
        ///     The <see cref="ItemSchemeBean" />
        /// </param>
        /// <returns>
        ///     A string that uniquely identifies the <paramref name="item" />
        /// </returns>
        public static string MakeKey(IItemObject item, IItemSchemeObject<IConceptObject> itemScheme)
        {
            return MakeKey(item.Id, itemScheme.AgencyId, itemScheme.Id, itemScheme.Version);
        }

        /// <summary>
        ///     Make a key from id, agency and version
        /// </summary>
        /// <param name="id">
        ///     The ID
        /// </param>
        /// <param name="agency">
        ///     The Agency
        /// </param>
        /// <param name="version">
        ///     The version
        /// </param>
        /// <returns>
        ///     A key id
        /// </returns>
        public static string MakeKey(string agency, string id, string version)
        {
            return $"{agency}+{id}+{version}";
        }

        /// <summary>
        ///     Make a key from concept scheme id, agency and version and concept id
        /// </summary>
        /// <param name="conceptId">
        ///     The <see cref="ConceptBean" /> Id
        /// </param>
        /// <param name="conceptSchemeId">
        ///     The <see cref="ConceptSchemeBean" /> id
        /// </param>
        /// <param name="conceptSchemeAgency">
        ///     The <see cref="ConceptSchemeBean" /> Agency
        /// </param>
        /// <param name="conceptSchemeVersion">
        ///     The <see cref="ConceptSchemeBean" /> Version
        /// </param>
        /// <returns>
        ///     The make key.
        /// </returns>
        public static string MakeKey(string conceptId, string conceptSchemeAgency, string conceptSchemeId,
            string conceptSchemeVersion)
        {
            return string.Format(
                CultureInfo.InvariantCulture, "{0}:{1}",
                MakeKey(conceptSchemeAgency, conceptSchemeId, conceptSchemeVersion), conceptId);
        }

        /// <summary>
        ///     Make a key for <see cref="ConceptBean" />
        /// </summary>
        /// <param name="component">
        ///     The <see cref="ComponentBean" /> containing the concept reference
        /// </param>
        /// <returns>
        ///     The key for concept.
        /// </returns>
        public static string MakeKeyForConcept(IComponent component)
        {
            IStructureReference concept = component.ConceptRef;
            return MakeKey(
                concept.ChildReference.Id,
                concept.MaintainableReference.AgencyId,
                concept.MaintainableReference.MaintainableId,
                concept.MaintainableReference.Version);
        }

        /// <summary>
        ///     Populate the specified map from ConceptSchemes contained in the specified structure
        /// </summary>
        /// <param name="structure">
        ///     The structure containing the ConceptSchemes
        /// </param>
        /// <param name="conceptMap">
        ///     The concept id to concept bean map
        /// </param>
        public static void PopulateConceptMap(ISdmxObjects structure, Dictionary<string, IConceptObject> conceptMap)
        {
            conceptMap.Clear();
            foreach (var conceptScheme in structure.ConceptSchemes)
            foreach (var concept in conceptScheme.Items)
            {
                var key = MakeKey(concept, conceptScheme);
                if (!conceptMap.ContainsKey(key)) conceptMap.Add(key, concept);
            }
        }

        /// <summary>
        ///     Validate time period
        /// </summary>
        /// <param name="time">
        ///     The time period to validate
        /// </param>
        /// <returns>
        ///     <c>True</c> if it is valid; otherwise <c>False</c>
        /// </returns>
        public static bool ValidateTimePeriod(string time)
        {
            if (!string.IsNullOrEmpty(time) && !_timePeriodValidate.IsMatch(time)) return false;

            return true;
        }


        public static IDataStructureObject GetDsdFromDataflow(IDataflowObject dataflow,
            ISet<IDataStructureObject> dataStructure)
        {
            foreach (var dsd in dataStructure)
                if (dataflow.DataStructureRef.Equals(dsd.AsReference))
                    return dsd;
            return null;
        }

        /// <summary>
        ///     Checks if a structure is complete according to the requirements of <see cref="GetStructure" />
        /// </summary>
        /// <param name="structure">
        ///     The StructureBean object to check.
        /// </param>
        /// <param name="dataflow">
        ///     The requested dataflow
        /// </param>
        /// <exception cref="NsiClientException">
        ///     Server response error
        /// </exception>
        public static bool CheckifStructureComplete(ISdmxObjects structure, IDataflowObject dataflow)
        {
            if (structure.DataStructures.Count != 1)
                //Logger.Error(Resources.ExceptionKeyFamilyCountNot1);
                //throw new NsiClientException(Resources.ExceptionKeyFamilyCountNot1);
                return false;

            var kf = structure.DataStructures.First();
            var keyFamilyRef = dataflow.DataStructureRef;
            if (kf.Id == null || keyFamilyRef == null ||
                !kf.Id.Equals(keyFamilyRef.MaintainableReference.MaintainableId)
                || !kf.AgencyId.Equals(keyFamilyRef.MaintainableReference.AgencyId) ||
                !kf.Version.Equals(keyFamilyRef.MaintainableReference.Version))
                //Logger.Error(Resources.ExceptionServerResponseInvalidKeyFamily);
                //throw new NsiClientException(Resources.ExceptionServerResponseInvalidKeyFamily);
                return false;

            return true;
        }

        /// <summary>
        ///     Build concept scheme requests from the concept scheme references of the specified KeyFamilyBean object
        /// </summary>
        /// <param name="kf">
        ///     The KeyFamily to look for concept Scheme references
        /// </param>
        /// <returns>
        ///     A list of concept scheme requests
        /// </returns>
        public static IEnumerable<IStructureReference> BuildConceptSchemeRequest(IDataStructureObject kf)
        {
            var conceptSchemeSet = new Dictionary<string, object>();
            var ret = new List<IStructureReference>();
            var crossDsd = kf as ICrossSectionalDataStructureObject;

            var components = new List<IComponent>();

            components.AddRange(kf.GetDimensions());
            components.AddRange(kf.Attributes);
            if (kf.PrimaryMeasure != null) components.Add(kf.PrimaryMeasure);
            if (crossDsd != null) components.AddRange(crossDsd.CrossSectionalMeasures);

            ICollection<IComponent> comps = components;

            foreach (var comp in comps)
            {
                var key = MakeKey(comp.ConceptRef.MaintainableReference.MaintainableId,
                    comp.ConceptRef.MaintainableReference.Version, comp.ConceptRef.MaintainableReference.AgencyId);
                if (!conceptSchemeSet.ContainsKey(key))
                {
                    // create concept ref


                    var conceptSchemeRef =
                        new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.ConceptScheme))
                        {
                            MaintainableId = comp.ConceptRef.MaintainableReference.MaintainableId,
                            AgencyId = comp.ConceptRef.MaintainableReference.AgencyId,
                            Version = comp.ConceptRef.MaintainableReference.Version
                        };

                    // add it to request
                    ret.Add(conceptSchemeRef);

                    // added it to set of visited concept schemes
                    conceptSchemeSet.Add(key, null);
                }
            }

            return ret;
        }

        /// <summary>
        ///     Check if the specified structure has all referenced concept schemes from the first keyfamily
        /// </summary>
        /// <param name="structure">
        ///     The StructureBean to check
        /// </param>
        public static bool CheckConcepts(ISdmxObjects structure)
        {
            var cshtMap = new Dictionary<string, IConceptSchemeObject>();
            var kf = structure.DataStructures.First();
            foreach (var c in structure.ConceptSchemes) cshtMap.Add(MakeKey(c), c);

            var crossDsd = kf as ICrossSectionalDataStructureObject;

            var components = new List<IComponent>();

            components.AddRange(kf.GetDimensions());
            components.AddRange(kf.Attributes);
            if (kf.PrimaryMeasure != null) components.Add(kf.PrimaryMeasure);

            if (crossDsd != null) components.AddRange(crossDsd.CrossSectionalMeasures);

            var comps = components;

            foreach (var comp in comps)
            {
                var conceptKey = MakeKey(comp.ConceptRef.MaintainableReference.AgencyId,
                    comp.ConceptRef.MaintainableReference.MaintainableId,
                    comp.ConceptRef.MaintainableReference.Version);
                if (!cshtMap.ContainsKey(conceptKey))
                {
                    var message = string.Format(CultureInfo.InvariantCulture,
                        "SERVER RESPONSE getStructure ERROR: Missing referenced concept  scheme : {0}", conceptKey);
                    //Logger.Error(message);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Fix the datatype of XmlDocument responsed by NSI.
        /// </summary>
        public static XmlDocument FixDataType(XmlDocument XmlDoc, XNamespace str)
        {
            var XDoc = ToXDocument(XmlDoc);

            var EnumFormats = XDoc.Descendants(str + "EnumerationFormat");

            if (EnumFormats.Any())
                foreach (var EnumFormat in EnumFormats)
                    if (EnumFormat.HasAttributes)
                    {
                        var pattern = EnumFormat.Attribute("pattern");
                        var textType = EnumFormat.Attribute("textType");

                        if (pattern != null && textType != null)
                            textType.Value = "String";
                    }

            return ToXmlDocument(XDoc);
        }

        /// <summary>
        ///     Fix the lang of XmlDocument responsed by NSI.
        /// </summary>
        public static XmlDocument FixLocale(XmlDocument XmlDoc, string oldValue, string newValue)
        {
            var Find = string.Format(@"lang=""{0}""", oldValue);
            var Repl = string.Format(@"lang=""{0}""", newValue);

            XmlDoc.InnerXml = XmlDoc.InnerXml.Replace(Find, Repl);

            return XmlDoc;
        }

        private static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }

            return xmlDocument;
        }

        private static XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        public static bool DataflowDsdIsCrossSectional(IDataStructureObject dsd)
        {
            return dsd is ICrossSectionalDataStructureObject;
        }

        public static ISdmxObjects GetOrderArtefacts(ISdmxObjects sdmxObject,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig, string lang,
            SdmxStructureEnumType orderSpeficType = SdmxStructureEnumType.Null)
        {
            var annotations = true;
            var annotationOrderDefaultValue = endPointCustomAnnotationConfig?.ORDER_CODELIST ?? "ORDER";
            var orderSdmxObject = new SdmxObjectsImpl();
            foreach (var mainTableItem in sdmxObject.GetAllMaintainables())
                switch (mainTableItem.StructureType.EnumType)
                {
                    case SdmxStructureEnumType.CodeList:
                        var Codelist = mainTableItem.MutableInstance as ICodelistMutableObject;
                        if (orderSpeficType != SdmxStructureEnumType.CodeList &&
                            orderSpeficType != SdmxStructureEnumType.Null)
                        {
                            orderSdmxObject.AddCodelist(Codelist.ImmutableInstance);
                            break;
                        }

                        var ListItem = new List<ICodeMutableObject>();

                        foreach (var myCode in Codelist.Items) ListItem.Add(myCode);

                        //All annottion must be number
                        var annotationCompare = new AnnotationCompare(mainTableItem.StructureType.EnumType,
                            endPointCustomAnnotationConfig?.ORDER_CODELIST ?? "ORDER", lang);
                        if (annotations) ListItem.Sort(annotationCompare);

                        //Clean and add item ordered
                        Codelist.Items.Clear();
                        ListItem.ForEach(i => Codelist.AddItem(i));

                        orderSdmxObject.AddCodelist(Codelist.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.ConceptScheme:
                        var ConceptScheme = mainTableItem.MutableInstance as IConceptSchemeMutableObject;

                        if (orderSpeficType != SdmxStructureEnumType.ConceptScheme &&
                            orderSpeficType != SdmxStructureEnumType.Null)
                        {
                            orderSdmxObject.AddConceptScheme(ConceptScheme.ImmutableInstance);
                            break;
                        }

                        var ListItemConcept = new List<IConceptMutableObject>();

                        foreach (var myCode in ConceptScheme.Items) ListItemConcept.Add(myCode);

                        //All annottion must be number
                        var annotationCompareConcept = new AnnotationCompare(mainTableItem.StructureType.EnumType,
                            annotationOrderDefaultValue, lang);
                        if (annotations) ListItemConcept.Sort(annotationCompareConcept);

                        //Clean and add item ordered
                        ConceptScheme.Items.Clear();
                        ListItemConcept.ForEach(i => ConceptScheme.AddItem(i));

                        orderSdmxObject.AddConceptScheme(ConceptScheme.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.Dataflow:
                        var mainMutable = mainTableItem.MutableInstance as IDataflowMutableObject;

                        orderSdmxObject.AddDataflow(mainMutable.ImmutableInstance);

                        break;
                    case SdmxStructureEnumType.CategoryScheme:
                        var CategoryScheme = mainTableItem.MutableInstance as ICategorySchemeMutableObject;

                        if (orderSpeficType != SdmxStructureEnumType.CategoryScheme &&
                            orderSpeficType != SdmxStructureEnumType.Null)
                        {
                            orderSdmxObject.AddCategoryScheme(CategoryScheme.ImmutableInstance);
                            break;
                        }

                        var listItemCategory = new List<ICategoryMutableObject>();
                        foreach (var myCode in CategoryScheme.Items)
                            listItemCategory.Add(orderCategory(myCode, endPointCustomAnnotationConfig.ORDER_CATEGORY,
                                lang));

                        //All annottion must be number
                        var annotationCompareCategory = new AnnotationCompare(mainTableItem.StructureType.EnumType,
                            endPointCustomAnnotationConfig?.ORDER_CATEGORY ?? "ORDER", lang);
                        if (annotations) listItemCategory.Sort(annotationCompareCategory);

                        //Clean and add item ordered
                        CategoryScheme.Items.Clear();
                        listItemCategory.ForEach(i => CategoryScheme.AddItem(i));
                        orderSdmxObject.AddCategoryScheme(CategoryScheme.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.Dsd:
                        var dsdObject = mainTableItem.MutableInstance as IDataStructureMutableObject;

                        orderSdmxObject.AddDataStructure(dsdObject.ImmutableInstance);
                        break;

                    case SdmxStructureEnumType.AgencyScheme:
                        var agencyScheme = mainTableItem.MutableInstance as IAgencySchemeMutableObject;

                        orderSdmxObject.AddAgencyScheme(agencyScheme.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.ContentConstraint:
                        var contentConstraint = mainTableItem.MutableInstance as IContentConstraintMutableObject;

                        var listItemConstraint = new List<IConstraintMutableObject>();
                        orderSdmxObject.AddContentConstraintObject(contentConstraint.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.HierarchicalCodelist:

                        var hierarchical = mainTableItem.MutableInstance as IHierarchicalCodelistMutableObject;

                        orderSdmxObject.AddHierarchicalCodelist(hierarchical.ImmutableInstance);
                        break;

                    case SdmxStructureEnumType.Categorisation:
                        var categorisation = mainTableItem.MutableInstance as ICategorisationMutableObject;
                        orderSdmxObject.AddCategorisation(categorisation.ImmutableInstance);
                        break;

                    case SdmxStructureEnumType.DataProviderScheme:
                        var dataProvider = mainTableItem.MutableInstance as IDataProviderSchemeMutableObject;

                        if (orderSpeficType != SdmxStructureEnumType.DataProviderScheme &&
                            orderSpeficType != SdmxStructureEnumType.Null)
                        {
                            orderSdmxObject.AddDataProviderScheme(dataProvider.ImmutableInstance);
                            break;
                        }

                        var listItemDP = new List<IDataProviderMutableObject>();
                        foreach (var myCode in dataProvider.Items) listItemDP.Add(myCode);

                        foreach (var myCode in dataProvider.Items)
                            if (myCode.Annotations.Count == 0)
                            {
                                annotations = false;
                                break;
                            }

                        var annotationDataProviderScheme = new AnnotationCompare(mainTableItem.StructureType.EnumType,
                            annotationOrderDefaultValue, lang);
                        if (annotations) listItemDP.Sort(annotationDataProviderScheme);

                        for (var i = 0; i <= listItemDP.Count - 1; i++) dataProvider.Items[i] = listItemDP[i];

                        orderSdmxObject.AddDataProviderScheme(dataProvider.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.DataConsumerScheme:
                        var dataConsumer = mainTableItem.MutableInstance as IDataConsumerSchemeMutableObject;

                        if (orderSpeficType != SdmxStructureEnumType.DataConsumerScheme &&
                            orderSpeficType != SdmxStructureEnumType.Null)
                        {
                            orderSdmxObject.AddDataConsumerScheme(dataConsumer.ImmutableInstance);
                            break;
                        }

                        var listItemDC = new List<IDataConsumerMutableObject>();
                        foreach (var myCode in dataConsumer.Items) listItemDC.Add(myCode);

                        foreach (var myCode in dataConsumer.Items)
                            if (myCode.Annotations.Count == 0)
                            {
                                annotations = false;
                                break;
                            }

                        var annotationDataConsumerScheme = new AnnotationCompare(mainTableItem.StructureType.EnumType,
                            annotationOrderDefaultValue, lang);
                        if (annotations) listItemDC.Sort(annotationDataConsumerScheme);

                        for (var i = 0; i <= listItemDC.Count - 1; i++) dataConsumer.Items[i] = listItemDC[i];

                        orderSdmxObject.AddDataConsumerScheme(dataConsumer.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.OrganisationUnitScheme:
                        var organizationUnit = mainTableItem.MutableInstance as IOrganisationUnitSchemeMutableObject;

                        if (orderSpeficType != SdmxStructureEnumType.OrganisationUnitScheme &&
                            orderSpeficType != SdmxStructureEnumType.Null)
                        {
                            orderSdmxObject.AddOrganisationUnitScheme(organizationUnit.ImmutableInstance);
                            break;
                        }

                        var listItemOU = new List<IOrganisationUnitMutableObject>();
                        foreach (var myCode in organizationUnit.Items) listItemOU.Add(myCode);

                        foreach (var myCode in organizationUnit.Items)
                            if (myCode.Annotations.Count == 0)
                            {
                                annotations = false;
                                break;
                            }

                        var annotationCompareOrganisation = new AnnotationCompare(mainTableItem.StructureType.EnumType,
                            annotationOrderDefaultValue, lang);
                        if (annotations) listItemOU.Sort(annotationCompareOrganisation);

                        for (var i = 0; i <= listItemOU.Count - 1; i++) organizationUnit.Items[i] = listItemOU[i];

                        orderSdmxObject.AddOrganisationUnitScheme(organizationUnit.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.StructureSet:
                        var structureSet = mainTableItem.MutableInstance as IStructureSetMutableObject;

                        orderSdmxObject.AddStructureSet(structureSet.ImmutableInstance);

                        break;

                    case SdmxStructureEnumType.ProvisionAgreement:
                        orderSdmxObject.AddProvisionAgreement(
                            (mainTableItem.MutableInstance as IProvisionAgreementMutableObject).ImmutableInstance);
                        break;
                    case SdmxStructureEnumType.Registration:
                        orderSdmxObject.AddRegistration((mainTableItem.MutableInstance as IRegistrationMutableObject)
                            .ImmutableInstance);
                        break;
                    case SdmxStructureEnumType.Msd:
                        orderSdmxObject.AddMetadataStructure(
                            (mainTableItem.MutableInstance as IMetadataStructureDefinitionMutableObject)
                            .ImmutableInstance);
                        break;
                    case SdmxStructureEnumType.MetadataFlow:
                        orderSdmxObject.AddMetadataFlow((mainTableItem.MutableInstance as IMetadataFlowMutableObject)
                            .ImmutableInstance);
                        break;
                }

            return orderSdmxObject;
        }

        public static int? GetMaxObservationFromAnnotations(IDataflowObject dataflow,
            IDataStructureObject dataStructure, string annotationType)
        {
            var textValue = dataflow?.Annotations?.FirstOrDefault(i => i.Type != null && i.Type.Equals(annotationType))
                ?.Title;
            if (int.TryParse(textValue, out var maxObj)) return maxObj;

            textValue = dataStructure?.Annotations?.FirstOrDefault(i => i.Type != null && i.Type.Equals(annotationType))
                ?.Title;
            if (int.TryParse(textValue, out maxObj)) return maxObj;

            return null;
        }

        private static ICategoryMutableObject orderCategory(ICategoryMutableObject categoryMutableObject,
            string annotationOrder, string lang)
        {
            if (categoryMutableObject == null || categoryMutableObject.Items == null ||
                categoryMutableObject.Items.Count <= 0) return categoryMutableObject;

            var listItemCategory = new List<ICategoryMutableObject>();
            foreach (var myCode in categoryMutableObject.Items)
            {
                orderCategory(myCode, annotationOrder, lang);
                listItemCategory.Add(myCode);
            }

            var annotationCompareCategory =
                new AnnotationCompare(SdmxStructureEnumType.Category, annotationOrder, lang);
            listItemCategory.Sort(annotationCompareCategory);
            categoryMutableObject.Items.Clear();
            listItemCategory.ForEach(i => categoryMutableObject.AddItem(i));

            return categoryMutableObject;
        }

        public static string GetTempSdmxFileName()
        {
            var folder = "_StorageTmpDataContainer";
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), $@"{folder}\sdmx\")))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), $@"{folder}\sdmx\"));

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $@"{folder}\sdmx\") + Guid.NewGuid() + ".sdmx";
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.SetLength(0);
            }

            return filePath;
        }

        public class AnnotationCompare : IComparer<IItemMutableObject>
        {
            private readonly string _lang = "en";
            private readonly string _orderAnnotation = "ORDER";

            public AnnotationCompare(SdmxStructureEnumType artifact, string annotationOrder, string lang)
            {
                _lang = lang;
                if (string.IsNullOrWhiteSpace(_lang))
                    //TODO warning log for lang empty and force EN
                    _lang = "en";
                _orderAnnotation = annotationOrder;
            }

            public int Compare(IItemMutableObject x, IItemMutableObject y)
            {
                var first = x.Annotations.FirstOrDefault(a => a.Type == _orderAnnotation);
                var second = y.Annotations.FirstOrDefault(a => a.Type == _orderAnnotation);
                if (first == null && second != null)
                    return 1;
                if (first != null && second == null)
                    return -1;
                if (first == null && second == null) return 0;
                var firstValue = int.MaxValue;
                var secondValue = int.MaxValue;
                if (first.Text != null)
                    foreach (var itemValue in first.Text)
                        if (itemValue.Locale.Equals(_lang, StringComparison.InvariantCultureIgnoreCase))
                            try
                            {
                                firstValue = int.Parse(itemValue.Value);
                                break;
                            }
                            catch (Exception)
                            {
                            }

                if (second.Text != null)
                    foreach (var itemValue in second.Text)
                        if (itemValue.Locale.Equals(_lang, StringComparison.InvariantCultureIgnoreCase))
                            try
                            {
                                secondValue = int.Parse(itemValue.Value);
                                break;
                            }
                            catch (Exception)
                            {
                            }

                return firstValue - secondValue;
            }
        }
    }
}