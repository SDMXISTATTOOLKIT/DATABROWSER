using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Models;
using EndPointConnector.Models.Dto;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Manager.Parse;
using Org.Sdmxsource.Sdmx.Api.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model;
using Org.Sdmxsource.Sdmx.Structureparser.Engine.Writing;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.Util.Objects;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Translator;
using Org.Sdmxsource.Util;
using Org.Sdmxsource.Util.Io;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EndPointConnector.ParserSdmx
{
    public class SdmxParser
    {
        private const string nameSpaceV21 = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message";

        private static readonly XNamespace regV21 =
            "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/webservices/registry";

        private static readonly XNamespace mesV21 = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message";
        private static readonly XNamespace strV21 = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/structure";
        private static readonly XNamespace comV21 = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/common";
        public static readonly string AnnotationDiscardDataflowFromCatalogTree = "NonProductionDataflow";
        public static readonly string AnnotationHidddenDataflowFromCatalogTree = "DATAFLOW_HIDDEN";
        private readonly ILogger<SdmxParser> _logger;

        public SdmxParser(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SdmxParser>();
        }

        /// <summary>
        ///     Generete SDMX-JSON from SdmxObjects
        /// </summary>
        /// <param name="sdmxObjects"></param>
        /// <param name="langs">language output</param>
        /// <returns>String with SDMX-JSON generated.</returns>
        public string GetSdmxJsonFromSdmxObjects(ISdmxObjects sdmxObjects, string[] langs = null)
        {
            _logger.LogDebug("START GetSdmxJsonFromSdmxObjects");
            JObject result = null;
            HeaderScope.WriteUrn = true;
            if (langs == null)
            {
                langs = new[] { "it", "en", "fr", "de" };
            }

            foreach (var language in langs)
            {
                var format = new SdmxStructureJsonFormat(
                    new PreferedLanguageTranslator(new List<CultureInfo>(), new List<CultureInfo>(),
                        new CultureInfo(language)),
                    StructureOutputFormatEnumType.JsonV10
                );
                var stream = new MemoryStream();
                using (var swe = new StructureWriterEngineJsonV1(stream, format))
                {
                    swe.WriteStructures(sdmxObjects);
                }

                var strJson = Encoding.UTF8.GetString(stream.ToArray());
                strJson = strJson.Substring(strJson.IndexOf("{", StringComparison.Ordinal));

                if (result == null)
                {
                    result = JObject.Parse(strJson);
                }
                else
                {
                    result.Merge(JsonConvert.DeserializeObject(strJson), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Merge
                    });
                }
            }

            //if (sdmxObjects.HasMetadataStructures)
            //{
            //    result.Merge(JsonConvert.DeserializeObject(SerializeMetadatastructure(sdmxObjects.MetadataStructures)), new JsonMergeSettings
            //    {
            //        MergeArrayHandling = MergeArrayHandling.Merge
            //    });
            //}
            //if (sdmxObjects.HasMetadataflows)
            //{
            //    result.Merge(JsonConvert.DeserializeObject(SerializeMetadataflow(sdmxObjects.Metadataflows)), new JsonMergeSettings
            //    {
            //        MergeArrayHandling = MergeArrayHandling.Merge
            //    });
            //}
            //if (sdmxObjects.HasDataProviderSchemes)
            //{
            //    result.Merge(JsonConvert.DeserializeObject(ArtefactDataModel.BL.Utility.ConvertToJson(sdmxObjects.DataProviderSchemes)), new JsonMergeSettings
            //    {
            //        MergeArrayHandling = MergeArrayHandling.Merge
            //    });
            //}
            //if (sdmxObjects.HasDataConsumerSchemes)
            //{
            //    result.Merge(JsonConvert.DeserializeObject(ArtefactDataModel.BL.Utility.ConvertToJson(sdmxObjects.DataConsumerSchemes)), new JsonMergeSettings
            //    {
            //        MergeArrayHandling = MergeArrayHandling.Merge
            //    });
            //}
            //if (sdmxObjects.HasOrganisationUnitSchemes)
            //{
            //    result.Merge(JsonConvert.DeserializeObject(ArtefactDataModel.BL.Utility.ConvertToJson(sdmxObjects.OrganisationUnitSchemes)), new JsonMergeSettings
            //    {
            //        MergeArrayHandling = MergeArrayHandling.Merge
            //    });
            //}

            var resultStr = result.ToString();
            _logger.LogDebug("END GetSdmxJsonFromSdmxObjects");
            return resultStr;
        }

        /// <summary>
        ///     Generete SdmxObjects from SDMX-JSON
        /// </summary>
        /// <param name="json">json</param>
        /// <returns>SdmxObjects</returns>
        public ISdmxObjects GetSdmxObjectsFromSdmxJson(string json)
        {
            _logger.LogDebug("START GetSdmxObjectsFromSdmxJson");
            var format = new SdmxStructureJsonFormat(
                new PreferedLanguageTranslator(new List<CultureInfo>(), new List<CultureInfo>(), new CultureInfo("en")),
                StructureOutputFormatEnumType.JsonV10
            );

            var encoding = Encoding.UTF8;
            ISdmxObjects bean;
            using (var mrl = new MemoryReadableLocation(encoding.GetBytes(json)))
            {
                IStructureParsingManager pm = new StructureParsingJsonManager(format);
                var sw = pm.ParseStructures(mrl);

                bean = sw.GetStructureObjects(false);
            }

            _logger.LogDebug("END GetSdmxObjectsFromSdmxJson");
            return bean;
        }

        /// <summary>
        ///     Generete SDMX-XML from SdmxObjects
        /// </summary>
        /// <param name="sdmxObjects"></param>
        /// <returns>>String with SDMX-XML generated.</returns>
        public string GetSdmxXmlFromSdmxObjects(ISdmxObjects sdmxObjects)
        {
            _logger.LogDebug("START GetSdmxXmlFromSdmxObjects");
            var xmlOutStream = new MemoryStream();
            using (var xmlWriterEngine = new StructureWriterEngineV21(xmlOutStream))
            {
                xmlWriterEngine.WriteStructures(sdmxObjects);
            }

            var result = Encoding.Default.GetString(xmlOutStream.ToArray());
            _logger.LogDebug("END GetSdmxXmlFromSdmxObjects");
            return result;
        }

        /// <summary>
        ///     Generete SdmxObjects from SDMX-XML
        /// </summary>
        /// <param name="xml">xml</param>
        /// <param name="fixXmlDocument">Fix xml endpoint response for languages and data</param>
        /// <param name="includeCrossReference">Include the cross reference in ISdmxObjects structure parse</param>
        /// <param name="schemaType">Sdmx schema type used only for fixed xml response data</param>
        /// <returns>>SdmxObjects</returns>
        public ISdmxObjects GetSdmxObjectsFromSdmxXml(string xml, bool includeCrossReference = false,
            bool fixXmlDocument = false, SdmxSchemaEnumType schemaType = SdmxSchemaEnumType.Null)
        {
            _logger.LogDebug("START GetSdmxObjectsFromSdmxXml");

            if (fixXmlDocument)
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                return GetSdmxObjectsFromNsiXml(xmlDocument, includeCrossReference: includeCrossReference,
                    fixXmlDocument: fixXmlDocument, schemaType: schemaType);
            }

            var encoding = Encoding.UTF8;
            ISdmxObjects bean;
            using (var mrl = new MemoryReadableLocation(encoding.GetBytes(xml)))
            {
                IStructureParsingManager pm = new StructureParsingManager();
                var sw = pm.ParseStructures(mrl);
                bean = sw.GetStructureObjects(includeCrossReference);
            }

            _logger.LogDebug("END GetSdmxObjectsFromSdmxXml");
            return bean;
        }

        /// <summary>
        ///     Generete SdmxObjects from NSI SDMX-XML-Document response
        /// </summary>
        /// <param name="xmlDocument">xml</param>
        /// <param name="fixStructureDocument">Fix xml endpoint response for languages and data</param>
        /// <param name="includeCrossReference">Include the cross reference in ISdmxObjects structure parse</param>
        /// <param name="schemaType">Sdmx schema type used only for fixed xml response data</param>
        /// <returns>>SdmxObjects</returns>
        public ISdmxObjects GetSdmxObjectsFromNsiResponse(SdmxHttpResponseMessage sdmxHttpResponseMessage, bool fixStructureDocument = false,
                                                        bool includeCrossReference = false,
                                                        SdmxSchemaEnumType schemaType = SdmxSchemaEnumType.Null)
        {
            if (sdmxHttpResponseMessage.ResponseType == SdmxEndPointCostant.RequestType.StructureJson)
            {
                return GetSdmxObjectsFromNsiJson(sdmxHttpResponseMessage.TextResponse, includeCrossReference);
            }
            else
            {
                return GetSdmxObjectsFromNsiXml(sdmxHttpResponseMessage.XmlResponse, fixStructureDocument, includeCrossReference, schemaType);
            }

            return null;
        }

        public ISdmxObjects GetSdmxObjectsFromNsiJson(string json, bool includeCrossReference)
        {
            SdmxStructureJsonFormat format = new SdmxStructureJsonFormat(
               new PreferedLanguageTranslator(new List<CultureInfo>(), new List<CultureInfo>(), new CultureInfo("en")),
               StructureOutputFormatEnumType.JsonV10
            );

            Encoding encoding = Encoding.UTF8;
            MemoryReadableLocation mrl = new MemoryReadableLocation(encoding.GetBytes(json));

            IStructureParsingManager pm = new StructureParsingJsonManager(format);
            IStructureWorkspace sw = pm.ParseStructures(mrl);

            return sw.GetStructureObjects(includeCrossReference);
        }

        public ISdmxObjects GetSdmxObjectsFromNsiXml(XmlDocument xmlDocument, bool fixXmlDocument = false,
            bool includeCrossReference = false, SdmxSchemaEnumType schemaType = SdmxSchemaEnumType.Null)
        {
            _logger.LogDebug("START GetSdmxObjectsFromNsiSdmxXml");

            if (fixXmlDocument)
            {
                _logger.LogDebug("Fix xml");
                xmlDocument = XmlDocumentFixed(xmlDocument, schemaType);
            }

            _logger.LogDebug("begin create SdmxObjectsImpl");
            ISdmxObjects structureObjects = new SdmxObjectsImpl();
            IStructureParsingManager parsingManager = new StructureParsingManager(schemaType);
            using (var dataLocation = new XmlDocReadableDataLocation(xmlDocument))
            {
                var structureWorkspace = parsingManager.ParseStructures(dataLocation);
                structureObjects = structureWorkspace.GetStructureObjects(includeCrossReference);
            }

            _logger.LogDebug("END GetSdmxObjectsFromNsiSdmxXml");
            return structureObjects;
        }

        /// <summary>
        ///     Fix XmlDocument (correct wrong languages and datatype)
        /// </summary>
        public XmlDocument XmlDocumentFixed(XmlDocument xDomSource, SdmxSchemaEnumType schemaType)
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");

            if (xDomSource == null)
            {
                return null;
            }

            var nameSpace = "";
            XNamespace str = null;
            if (schemaType == SdmxSchemaEnumType.VersionTwoPointOne || schemaType == SdmxSchemaEnumType.Null)
            {
                nameSpace = nameSpaceV21;
                str = strV21;
            }

            //TODO Header from paramiters
            var xmlTemplate =
                $@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <mes:Structure xmlns:mes=""{nameSpace}"">
                    <mes:Header>
                        <mes:ID>ISTATRegistryRetrieveTemplate</mes:ID>
                        <mes:Test>false</mes:Test>
                        <mes:Prepared>2014-05-06T21:53:11.874Z</mes:Prepared>
                        <mes:Sender id=""MG""/>
                        <mes:Receiver id=""unknown""/>
                    </mes:Header>
                </mes:Structure>";

            // Il documento template che verrà caricato con gli artefatti da importare
            var xDomTemp = new XmlDocument
            {

                // Creo gli elementi del file template
                InnerXml = xmlTemplate
            };

            // Il nodo root "Structure" del template
            var xTempStructNode = xDomTemp.SelectSingleNode("//*[local-name()='Structure']");

            // Creo il nodo "Structures" che conterrà gli artefatti
            var xSourceStructNode = xDomTemp.CreateNode(XmlNodeType.Element, "Structures", nameSpace);

            // Inserisco nel nodo "Structures" gli artefatti presenti nell' sdmx passato in input
            xSourceStructNode.InnerXml = xDomSource.SelectSingleNode("//*[local-name()='Structures']").InnerXml;

            // Aggiungo al template l'elemento "Structures" con gli artefatti da caricare
            xTempStructNode.AppendChild(xSourceStructNode);

            SDMXUtils.FixDataType(xDomTemp, str);
            SDMXUtils.FixLocale(xDomTemp, "la", "lo");

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");

            return xDomTemp;
        }

        /// <summary>
        ///     Convert an XmlDocument to an array of bytes.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private byte[] ConvertToBytes(XmlDocument doc)
        {
            var encoding = Encoding.UTF8;
            var docAsBytes = encoding.GetBytes(doc.OuterXml);
            return docAsBytes;
        }


        public NodeCatalogDto GetTreeFromSdmx(ISdmxObjects sdmxObjects, EndPointSdmxConfig endPointSdmxConfig)
        {
            var nodeCatalogDto = new NodeCatalogDto
            {
                CategoryGroups = new List<CategoryGroupDto>(),
                DatasetMap = new Dictionary<string, DatasetDto>()
            };
            if (endPointSdmxConfig.ShowDataflowUncategorized)
            {
                nodeCatalogDto.DatasetUncategorized = new List<DatasetDto>();
            }

            /*
             * patch to remove BL_ category scheme
            string initBLCS = ConfigurationManager.AppSettings["InitBuilderLoaderCS"].ToString();
            ICategorySchemeObject csBL = SdmxOBJ.CategorySchemes.FirstOrDefault(x => x.Id.StartsWith(initBLCS));
            if (csBL != null) { SdmxOBJ.RemoveCategoryScheme(csBL); };
            */

            var dataflows = new Dictionary<string, Dataflow>();

            // for each dataflows control if has a categorization
            // if true put it in dataflow list or in uncategorizate list
            foreach (var dataflow in sdmxObjects.Dataflows)
            {
                _logger.LogDebug(
                    $"GetTreeFromSdmx process dataflow {dataflow.AgencyId}+{dataflow.Id}+{dataflow.Version}");

                var dfItemConverted =
                        DataModelParser.ConvertArtefact(dataflow, endPointSdmxConfig.AnnotationConfig);

                if (dfItemConverted.DataflowType == DataflowType.IsVirtual)
                {
                    //TODO enable or disable virtual dataflow for catalog by setting from NodeConfig
                }
                else
                {
                    if (!DataflowIsValidForCatalog(endPointSdmxConfig, dataflow))
                    {
                        continue;
                    }

                    var haveAnnotationHidden = dataflow.Annotations.Any(i => i.Type != null &&
                                                                             i.Type.Equals(
                                                                                 AnnotationHidddenDataflowFromCatalogTree,
                                                                                 StringComparison
                                                                                     .InvariantCultureIgnoreCase));
                    if (haveAnnotationHidden)
                    {
                        _logger.LogDebug("is hidden");
                        continue;
                    }
                }
                

                var isDataflowUnCategorized = !sdmxObjects.Categorisations.Any(cat =>
                                                  !cat.IsExternalReference.IsTrue &&
                                                  cat.StructureReference.TargetReference.EnumType ==
                                                  dataflow.StructureType.EnumType &&
                                                  MaintainableUtil<IMaintainableObject>.Match(dataflow,
                                                      cat.StructureReference));

                if (endPointSdmxConfig.ShowDataflowUncategorized &&
                    isDataflowUnCategorized)
                {
                    _logger.LogDebug("isDataflowUnCategorized");
                    nodeCatalogDto.DatasetUncategorized.Add(convertDataflowToCatalogDatasetDto(dfItemConverted));
                }
                else if (!endPointSdmxConfig.ShowDataflowUncategorized &&
                            isDataflowUnCategorized)
                {
                    continue;
                }
                else
                {
                    dataflows.Add(SDMXUtils.MakeKey(dataflow),
                        DataModelParser.ConvertArtefact(dataflow, endPointSdmxConfig.AnnotationConfig));
                }
            }


            foreach (var categoryScheme in sdmxObjects.CategorySchemes)
            {
                _logger.LogDebug(
                    $"categoryScheme process {categoryScheme.AgencyId}+{categoryScheme.Id}+{categoryScheme.Version}");
                var extras = categoryScheme?.Annotations?.Select(i => new ExtraValueDto
                { Key = i.Type, Values = i.Text.GetAllTranslateItem(), IsPublic = false }).ToList();
                if (extras != null && extras.Count == 0)
                {
                    extras = null;
                }
                //TODO filter only annotations used
                var treeCategorySchema = new CategoryGroupDto
                {
                    Id = SDMXUtils.MakeKey(categoryScheme),
                    Descriptions = categoryScheme.Descriptions?.GetAllTranslateItem(),
                    Labels = categoryScheme.Names?.GetAllTranslateItem(),
                    Categories = new List<CategoryDto>(),
                    Extras = extras
                };

                nodeCatalogDto.CategoryGroups.Add(treeCategorySchema);

                foreach (var category in categoryScheme.Items)
                {
                    _logger.LogDebug($"category process {category.Id}");
                    treeCategorySchema.Categories.Add(GetTreeCategory(category, sdmxObjects.Categorisations, dataflows,
                        nodeCatalogDto.DatasetMap, endPointSdmxConfig));
                }
            }

            return nodeCatalogDto;
        }

        public bool DataflowIsValidForCatalog(EndPointSdmxConfig endPointSdmxConfig, IDataflowObject dataflow)
        {
            if (!endPointSdmxConfig.ShowDataflowNotInProduction)
            {
                var haveAnnotationDiscard = dataflow.Annotations.Any(i => i.Type != null &&
                                                                          i.Type.Equals(
                                                                              AnnotationDiscardDataflowFromCatalogTree,
                                                                              StringComparison
                                                                                  .InvariantCultureIgnoreCase));
                if (haveAnnotationDiscard)
                {
                    _logger.LogDebug($"not valid {dataflow.AgencyId}+{dataflow.Id}+{dataflow.Version}");
                    return false;
                }
            }

            return true;
        }

        private CategoryDto GetTreeCategory(ICategoryObject category,
            IEnumerable<ICategorisationObject> categorisations, Dictionary<string, Dataflow> dataflows,
            Dictionary<string, DatasetDto> datasetMap, EndPointSdmxConfig endPointSdmxConfig)
        {
            var extras = category?.Annotations?.Select(i => new ExtraValueDto
            { Key = i.Type, Values = i.Text.GetAllTranslateItem(), IsPublic = false }).ToList();
            if (extras != null && extras.Count == 0)
            {
                extras = null;
            }
            //TODO filter only annotations used
            var treeCategory = new CategoryDto
            {
                Id = category.Id,
                Descriptions = category?.Descriptions?.GetAllTranslateItem(),
                Labels = category?.Names?.GetAllTranslateItem(),
                ChildrenCategories = new List<CategoryDto>(),
                Extras = extras,
                DatasetIdentifiers = new HashSet<string>()
            };

            treeCategory.DatasetIdentifiers =
                GetDataFlowsFromCategory(category, categorisations, dataflows, datasetMap, endPointSdmxConfig);

            if (category.Items == null)
            {
                return treeCategory;
            }

            foreach (var itemCategory in category.Items)
            {
                treeCategory.ChildrenCategories.Add(GetTreeCategory(itemCategory, categorisations, dataflows,
                    datasetMap, endPointSdmxConfig));
            }

            return treeCategory;
        }

        private bool isVirtualDataflow(IDataflowObject dataflowObject)
        {
            if (dataflowObject.IsExternalReference != null &&
                dataflowObject.IsExternalReference.IsTrue)
            {
                return true;
            }



            return false;
        }

        private HashSet<string> GetDataFlowsFromCategory(ICategoryObject categoryObject,
            IEnumerable<ICategorisationObject> categorisations, Dictionary<string, Dataflow> dataflows,
            Dictionary<string, DatasetDto> datasetMap, EndPointSdmxConfig endPointSdmxConfig)
        {
            var dataflowsHash = new HashSet<string>();

            foreach (var cat in categorisations)
            {
                if (cat.IsExternalReference.IsTrue)
                {
                    continue;
                }

                if (cat.CategoryReference.TargetReference.EnumType == categoryObject.StructureType.EnumType &&
                    cat.StructureReference?.MaintainableStructureEnumType != null &&
                    cat.StructureReference.MaintainableStructureEnumType.EnumType == SdmxStructureEnumType.Dataflow)
                {
                    var refId = cat.CategoryReference.IdentifiableIds.Last();

                    if (refId.Equals(categoryObject.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var dfKey = SDMXUtils.MakeKey(cat.StructureReference.MaintainableReference);
                        if (datasetMap.ContainsKey(dfKey))
                        {
                            continue;
                        }

                        if (dataflows.ContainsKey(dfKey)
                        ) //take only valid dataflow (discarted not present in this dictionary)
                        {
                            dataflowsHash.Add(dfKey);

                            var dfItem = dataflows[dfKey];

                            //var extras = dfItem.Annotations?.Select(i => new ExtraValueDto { Key = i.Type, Values = i.Text.GetAllTranslateItem(), IsPublic = false }).ToList();
                            //if (extras != null && extras.Count == 0)
                            //{
                            //    extras = null;
                            //}

                            datasetMap.Add(dfKey, convertDataflowToCatalogDatasetDto(dfItem));
                        }
                    }
                }
            }

            return dataflowsHash;
        }

        private static DatasetDto convertDataflowToCatalogDatasetDto(Dataflow dataflow)
        {
            return new DatasetDto
            {
                Identifier = dataflow.Id,
                Titles = dataflow.Names,
                Descriptions = dataflow.Descriptions,
                AttachedDataFiles = dataflow.AttachedDataFiles,
                DefaultNote = dataflow.DefaultNote,
                Keywords = dataflow.Keywords,
                MetadataUrl = dataflow.MetadataUrl,
                Source = dataflow.DataflowSource,
                LayoutFilter = dataflow.LayoutFilter,
                DataflowType = dataflow.DataflowType,
                DataflowCatalogType = dataflow.DataflowCatalogType,
                HiddenFromCatalog = dataflow.HiddenFromCatalog,
                NonProductionDataflow = dataflow.NonProductionDataflow,

                VirtualEndPointSoapV20 = dataflow.VirtualEndPointSoapV20,
                VirtualEndPointSoapV21 = dataflow.VirtualEndPointSoapV21,
                VirtualEndPointRest = dataflow.VirtualEndPointRest,
                VirtualType = dataflow.VirtualType,
                VirtualSource = dataflow.VirtualSource
            };
        }

        private class DataflowData
        {
            public IList<ITextTypeWrapper> Names { get; set; }
            public IList<ITextTypeWrapper> Descriptions { get; set; }
            public IList<IAnnotation> Annotations { get; set; }
        }
    }
}