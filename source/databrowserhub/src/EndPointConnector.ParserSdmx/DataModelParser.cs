using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Models;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.MetadataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EndPointConnector.ParserSdmx
{
    public class DataModelParser
    {
        public static ArtefactType.ArtefactEnumType ArtefactType(SdmxStructureEnumType type)
        {
            //TODO parse from string name, istead of each case
            switch (type)
            {
                case SdmxStructureEnumType.Dataflow:
                    return Models.ArtefactType.ArtefactEnumType.Dataflow;
                case SdmxStructureEnumType.Code:
                    return Models.ArtefactType.ArtefactEnumType.Code;
                case SdmxStructureEnumType.CodeList:
                    return Models.ArtefactType.ArtefactEnumType.CodeList;
                case SdmxStructureEnumType.Dsd:
                    return Models.ArtefactType.ArtefactEnumType.Dsd;
                case SdmxStructureEnumType.Categorisation:
                    return Models.ArtefactType.ArtefactEnumType.Categorisation;
                case SdmxStructureEnumType.Category:
                    return Models.ArtefactType.ArtefactEnumType.Category;
                case SdmxStructureEnumType.CategoryScheme:
                    return Models.ArtefactType.ArtefactEnumType.CategoryScheme;
                case SdmxStructureEnumType.Concept:
                    return Models.ArtefactType.ArtefactEnumType.Concept;
                case SdmxStructureEnumType.ConceptScheme:
                    return Models.ArtefactType.ArtefactEnumType.ConceptScheme;
                case SdmxStructureEnumType.Constraint:
                    return Models.ArtefactType.ArtefactEnumType.Constraint;
            }

            throw new NotImplementedException();
        }

        public static SdmxStructureEnumType ArtefactType(ArtefactType.ArtefactEnumType type)
        {
            //TODO parse from string name, istead of each case
            switch (type)
            {
                case Models.ArtefactType.ArtefactEnumType.Dataflow:
                    return SdmxStructureEnumType.Dataflow;
                case Models.ArtefactType.ArtefactEnumType.Code:
                    return SdmxStructureEnumType.Code;
                case Models.ArtefactType.ArtefactEnumType.CodeList:
                    return SdmxStructureEnumType.CodeList;
                case Models.ArtefactType.ArtefactEnumType.Dsd:
                    return SdmxStructureEnumType.Dsd;
                case Models.ArtefactType.ArtefactEnumType.Categorisation:
                    return SdmxStructureEnumType.Categorisation;
                case Models.ArtefactType.ArtefactEnumType.Category:
                    return SdmxStructureEnumType.Category;
                case Models.ArtefactType.ArtefactEnumType.CategoryScheme:
                    return SdmxStructureEnumType.CategoryScheme;
                case Models.ArtefactType.ArtefactEnumType.Concept:
                    return SdmxStructureEnumType.Concept;
                case Models.ArtefactType.ArtefactEnumType.ConceptScheme:
                    return SdmxStructureEnumType.ConceptScheme;
                case Models.ArtefactType.ArtefactEnumType.Contact:
                    return SdmxStructureEnumType.Contact;
            }

            throw new NotImplementedException();
        }

        public static StructureReferenceDetailEnumType ReferenceType(ArtefactType.ReferenceDetailEnumType type)
        {
            switch (type)
            {
                case Models.ArtefactType.ReferenceDetailEnumType.All:
                    return StructureReferenceDetailEnumType.All;
                case Models.ArtefactType.ReferenceDetailEnumType.Children:
                    return StructureReferenceDetailEnumType.Children;
                case Models.ArtefactType.ReferenceDetailEnumType.Descendants:
                    return StructureReferenceDetailEnumType.Descendants;
                case Models.ArtefactType.ReferenceDetailEnumType.None:
                    return StructureReferenceDetailEnumType.None;
                case Models.ArtefactType.ReferenceDetailEnumType.Null:
                    return StructureReferenceDetailEnumType.Null;
                case Models.ArtefactType.ReferenceDetailEnumType.Parents:
                    return StructureReferenceDetailEnumType.Parents;
                case Models.ArtefactType.ReferenceDetailEnumType.ParentsSiblings:
                    return StructureReferenceDetailEnumType.ParentsSiblings;
                case Models.ArtefactType.ReferenceDetailEnumType.Specific:
                    return StructureReferenceDetailEnumType.Specific;
            }

            throw new NotImplementedException();
        }

        public static ArtefactType.ReferenceDetailEnumType ReferenceType(StructureReferenceDetailEnumType type)
        {
            switch (type)
            {
                case StructureReferenceDetailEnumType.All:
                    return Models.ArtefactType.ReferenceDetailEnumType.All;
                case StructureReferenceDetailEnumType.Children:
                    return Models.ArtefactType.ReferenceDetailEnumType.Children;
                case StructureReferenceDetailEnumType.Descendants:
                    return Models.ArtefactType.ReferenceDetailEnumType.Descendants;
                case StructureReferenceDetailEnumType.None:
                    return Models.ArtefactType.ReferenceDetailEnumType.None;
                case StructureReferenceDetailEnumType.Null:
                    return Models.ArtefactType.ReferenceDetailEnumType.Null;
                case StructureReferenceDetailEnumType.Parents:
                    return Models.ArtefactType.ReferenceDetailEnumType.Parents;
                case StructureReferenceDetailEnumType.ParentsSiblings:
                    return Models.ArtefactType.ReferenceDetailEnumType.ParentsSiblings;
                case StructureReferenceDetailEnumType.Specific:
                    return Models.ArtefactType.ReferenceDetailEnumType.Specific;
            }

            throw new NotImplementedException();
        }

        public static string ResponseType(ArtefactType.ResponseDetailEnumType type)
        {
            switch (type)
            {
                case Models.ArtefactType.ResponseDetailEnumType.CompleteStub:
                    return "CompleteStub";
                case Models.ArtefactType.ResponseDetailEnumType.Full:
                    return "Full";
                case Models.ArtefactType.ResponseDetailEnumType.None:
                    return "None";
                case Models.ArtefactType.ResponseDetailEnumType.Null:
                    return "";
                case Models.ArtefactType.ResponseDetailEnumType.Stub:
                    return "Stub";
                case Models.ArtefactType.ResponseDetailEnumType.ReferencePartial:
                    return "ReferencePartial";
            }

            throw new NotImplementedException();
        }

        public static ArtefactType.ResponseDetailEnumType ResponseType(string type)
        {
            switch (type.ToUpperInvariant())
            {
                case "COMPLETESTUB":
                    return Models.ArtefactType.ResponseDetailEnumType.CompleteStub;
                case "FULL":
                    return Models.ArtefactType.ResponseDetailEnumType.Full;
                case "NONE":
                    return Models.ArtefactType.ResponseDetailEnumType.None;
                case "":
                    return Models.ArtefactType.ResponseDetailEnumType.Null;
                case "STUB":
                    return Models.ArtefactType.ResponseDetailEnumType.Stub;
                case "REFERENCEPARTIAL":
                    return Models.ArtefactType.ResponseDetailEnumType.ReferencePartial;
            }

            throw new NotImplementedException();
        }

        public static ArtefactContainer ConvertArtefact(ISdmxObjects sdmxObjects,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var artefactContainer = new ArtefactContainer
            {
                Codelists = new List<Codelist>()
            };
            foreach (var codelist in sdmxObjects.Codelists)
            {
                artefactContainer.Codelists.Add(ConvertArtefact(codelist, endPointCustomAnnotationConfig));
            }

            artefactContainer.Dsds = new List<Dsd>();
            foreach (var dsd in sdmxObjects.DataStructures)
            {
                artefactContainer.Dsds.Add(ConvertArtefact(dsd, endPointCustomAnnotationConfig));
            }

            artefactContainer.Dataflows = new List<Dataflow>();
            foreach (var dataflow in sdmxObjects.Dataflows)
            {
                artefactContainer.Dataflows.Add(ConvertArtefact(dataflow, endPointCustomAnnotationConfig));
            }

            artefactContainer.Criterias = new List<Criteria>();
            foreach (var contraint in sdmxObjects.ContentConstraintObjects)
            {
                artefactContainer.Criterias.Add(ConvertArtefact(contraint, endPointCustomAnnotationConfig));
            }

            artefactContainer.ConceptSchemes = new List<ConceptScheme>();
            foreach (var conceptScheme in sdmxObjects.ConceptSchemes)
            {
                artefactContainer.ConceptSchemes.Add(ConvertConceptScheme(conceptScheme, endPointCustomAnnotationConfig));
            }

            return artefactContainer;
        }

        public static ISdmxObjects ConvertArtefact(ArtefactContainer artefactContainer)
        {
            if (artefactContainer == null)
            {
                return null;
            }
            ISdmxObjects sdmxObjects = new SdmxObjectsImpl();

            foreach (var codelist in artefactContainer.Codelists)
            {
                sdmxObjects.AddCodelist(ConvertArtefact(codelist));
            }

            foreach (var dsd in artefactContainer.Dsds)
            {
                sdmxObjects.AddDataStructure(ConvertArtefact(dsd));
            }

            foreach (var dataflow in artefactContainer.Dataflows)
            {
                sdmxObjects.AddDataflow(ConvertArtefact(dataflow));
            }

            foreach (var conceptScheme in artefactContainer.ConceptSchemes)
            {
                sdmxObjects.AddConceptScheme(ConvertArtefact(conceptScheme));
            }

            return sdmxObjects;
        }

        public static Codelist ConvertArtefact(ICodelistObject codelist,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            if (codelist == null)
            {
                return null;
            }

            var itemCodelist = new Codelist
            {
                Id = keyFromSdmx(codelist.AgencyId, codelist.Id, codelist.Version),

                Names = codelist?.Names?.GetAllTranslateItem(),
                Descriptions = codelist?.Descriptions?.GetAllTranslateItem(),

                Items = new List<Code>()
            };
            foreach (var itemCode in codelist.Items)
            {
                var itemCodeAdd = new Code
                {
                    Id = itemCode.Id
                };
                if (!string.IsNullOrWhiteSpace(itemCode.ParentCode))
                {
                    itemCodeAdd.ParentId = itemCode.ParentCode;
                }

                itemCodeAdd.Names = itemCode.Names.GetAllTranslateItem();

                readCustomAnnotation(itemCodeAdd, itemCode, endPointCustomAnnotationConfig);

                itemCodelist.Items.Add(itemCodeAdd);
            }

            itemCodelist.Extras = createAnnotation(codelist.Annotations);

            readCustomAnnotation(itemCodelist, codelist, endPointCustomAnnotationConfig);

            return itemCodelist;
        }

        public static Codelist ConvertArtefact(IConceptSchemeObject conceptScheme,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig, string conceptSchemeId)
        {
            if (conceptScheme == null)
            {
                return null;
            }

            var itemCodelist = new Codelist
            {
                Id = conceptSchemeId,

                Names = conceptScheme?.Names?.GetAllTranslateItem(),
                Descriptions = conceptScheme?.Descriptions?.GetAllTranslateItem(),

                Items = new List<Code>()
            };
            foreach (var itemCode in conceptScheme.Items)
            {
                var itemCodeAdd = new Code
                {
                    Id = itemCode.Id
                };

                itemCodeAdd.Names = itemCode.Names.GetAllTranslateItem();

                readCustomAnnotation(itemCodeAdd, itemCode, endPointCustomAnnotationConfig);

                itemCodelist.Items.Add(itemCodeAdd);
            }

            itemCodelist.Extras = createAnnotation(conceptScheme.Annotations);

            readCustomAnnotation(itemCodelist, conceptScheme, endPointCustomAnnotationConfig);

            return itemCodelist;
        }

        public static ConceptScheme ConvertConceptScheme(IConceptSchemeObject conceptScheme,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var converted = ConvertArtefact(conceptScheme, endPointCustomAnnotationConfig, keyFromSdmx(conceptScheme.AgencyId, conceptScheme.Id, conceptScheme.Version));
            return new ConceptScheme { Id = converted.Id, Descriptions = converted.Descriptions, Extras = converted.Extras, Names = converted.Names, Items = converted.Items };
        }

        public static IConceptSchemeObject ConvertArtefact(ConceptScheme conceptScheme)
        {
            var keys = keyFromModel(conceptScheme.Id);
            IConceptSchemeMutableObject mutable = new ConceptSchemeMutableCore
            {
                Id = keys[1],
                AgencyId = keys[0],
                Version = keys[2]
            };

            if (conceptScheme.Names != null)
            {
                foreach (var item in conceptScheme.Names)
                {
                    mutable.AddName(item.Key, item.Value);
                }
            }

            if (conceptScheme.Descriptions != null)
            {
                foreach (var item in conceptScheme.Descriptions)
                {
                    mutable.AddDescription(item.Key, item.Value);
                }
            }

            foreach (var itemCode in conceptScheme.Items)
            {
                var mutableCode = new ConceptMutableCore { Id = itemCode.Id };

                foreach (var item in itemCode.Names)
                {
                    mutableCode.AddName(item.Key, item.Value);
                }

                mutable.AddItem(mutableCode);
            }

            return mutable.ImmutableInstance;
        }

        public static ICodelistObject ConvertArtefact(Codelist codelist)
        {
            var keys = keyFromModel(codelist.Id);
            ICodelistMutableObject mutable = new CodelistMutableCore
            {
                Id = keys[1],
                AgencyId = keys[0],
                Version = keys[2]
            };

            if (codelist.Names != null)
            {
                foreach (var item in codelist.Names)
                {
                    mutable.AddName(item.Key, item.Value);
                }
            }

            if (codelist.Descriptions != null)
            {
                foreach (var item in codelist.Descriptions)
                {
                    mutable.AddDescription(item.Key, item.Value);
                }
            }

            foreach (var itemCode in codelist.Items)
            {
                var mutableCode = new CodeMutableCore { Id = itemCode.Id };
                if (!string.IsNullOrWhiteSpace(itemCode.ParentId))
                {
                    mutableCode.ParentCode = itemCode.ParentId;
                }

                foreach (var item in itemCode.Names)
                {
                    mutableCode.AddName(item.Key, item.Value);
                }

                mutable.AddItem(mutableCode);
            }

            return mutable.ImmutableInstance;
        }

        public static Dataflow ConvertArtefact(IDataflowObject dataflow,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var itemDataflow = new Dataflow
            {
                Id = keyFromSdmx(dataflow.AgencyId, dataflow.Id, dataflow.Version),

                Names = dataflow?.Names?.GetAllTranslateItem(),

                Descriptions = dataflow?.Descriptions?.GetAllTranslateItem(),

                Extras = createAnnotation(dataflow.Annotations)
            };

            if (dataflow.DataStructureRef != null)
            {
                itemDataflow.DataStructureRef = new ArtefactRef
                {
                    Id = keyFromSdmx(dataflow.DataStructureRef.AgencyId,
                    dataflow.DataStructureRef.MaintainableId, dataflow.DataStructureRef.Version)
                };
            }

            readCustomAnnotation(itemDataflow, dataflow, endPointCustomAnnotationConfig);

            popolateVirtualDataflowInformation(itemDataflow, dataflow,
                endPointCustomAnnotationConfig?.VIRTUAL_DATAFLOW_NODE);

            return itemDataflow;
        }

        public static IDataflowObject ConvertArtefact(Dataflow dataflow)
        {
            var keys = keyFromModel(dataflow.Id);
            IDataflowMutableObject mutable = new DataflowMutableCore
            {
                Id = keys[1],
                AgencyId = keys[0],
                Version = keys[2]
            };

            if (dataflow.DataStructureRef != null)
            {
                var dsdKey = keyFromModel(dataflow.DataStructureRef.Id);
                mutable.DataStructureRef =
                    new StructureReferenceImpl(dsdKey[0], dsdKey[1], dsdKey[2], SdmxStructureEnumType.Dsd);
            }

            if (dataflow.Names != null)
            {
                foreach (var item in dataflow.Names)
                {
                    mutable.AddName(item.Key, item.Value);
                }
            }

            if (dataflow.Descriptions != null)
            {
                foreach (var item in dataflow.Descriptions)
                {
                    mutable.AddDescription(item.Key, item.Value);
                }
            }

            if (dataflow.Extras != null)
            {
                foreach (var item in dataflow.Extras)
                {
                    var annotation = AnnotationFromExtra(item);
                    if (annotation != null)
                    {
                        mutable.AddAnnotation(annotation);
                    }
                }
            }

            return mutable.ImmutableInstance;
        }

        private static AnnotationMutableCore AnnotationFromExtra(ExtraValue extra)
        {
            if (!ExtraValueAnnotation.IsAnnotationExtra(extra))
            {
                return null;
            }
            var annotation = new AnnotationMutableCore
            {
                Id = extra.Key,
                Type = ExtraValueAnnotation.ExtractAnnotationType(extra),
                Title = ExtraValueAnnotation.ExtractAnnotationTitle(extra)
            };
            foreach (var txt in ExtraValueAnnotation.ExtractAllText(extra))
            {
                var itemText = new TextTypeWrapperMutableCore
                {
                    Locale = txt.Key,
                    Value = txt.Value
                };
                annotation.AddText(itemText);
            }
            return annotation;
        }


        public static Dsd ConvertArtefact(IDataStructureObject dsd,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var itemDsd = new Dsd
            {
                Id = keyFromSdmx(dsd.AgencyId, dsd.Id, dsd.Version)
            };
            if (dsd?.PrimaryMeasure?.Id != null)
            {
                itemDsd.PrimaryMeasure = new PrimaryMeasure
                {
                    Id = dsd.PrimaryMeasure.Id,
                    ConceptRef = new ArtefactRef
                    {
                        Id = keyFromSdmx(dsd.PrimaryMeasure.ConceptRef.AgencyId,
                            dsd.PrimaryMeasure.ConceptRef.MaintainableId, dsd.PrimaryMeasure.ConceptRef.Version),
                        RefType = Models.ArtefactType.ArtefactEnumType.Concept
                    }
                };
            }

            itemDsd.Names = dsd?.Names?.GetAllTranslateItem();
            itemDsd.Descriptions = dsd?.Descriptions?.GetAllTranslateItem();

            itemDsd.Extras = createAnnotation(dsd.Annotations);

            var dimensions = dsd.GetDimensions()?.OrderBy(i => i.Position);
            if (dimensions != null)
            {
                itemDsd.Dimensions = new List<Dimension>();
                foreach (var itemDim in dimensions)
                {
                    var itemDimAdd = new Dimension
                    {
                        Id = itemDim.Id,
                        ConceptRef = new ArtefactRef
                        {
                            Id = keyFromSdmx(itemDim.ConceptRef.AgencyId, itemDim.ConceptRef.MaintainableId,
                            itemDim.ConceptRef.Version),
                            RefType = Models.ArtefactType.ArtefactEnumType.Concept
                        }
                    };
                    if (itemDim?.Representation?.Representation != null)
                    {
                        itemDimAdd.Type = DimensionType.Dimension;
                        itemDimAdd.Representation = new ArtefactRef
                        {
                            Id = keyFromSdmx(itemDim.Representation.Representation.AgencyId,
                                itemDim.Representation.Representation.MaintainableId,
                                itemDim.Representation.Representation.Version),
                            RefType = Models.ArtefactType.ArtefactEnumType.CodeList
                        };
                        if (itemDim.MeasureDimension)
                        {
                            itemDimAdd.Type = DimensionType.MeasureDimension;
                        }
                    }
                    else if (itemDimAdd.Id.Equals("TIME_PERIOD", StringComparison.InvariantCultureIgnoreCase))
                    {
                        itemDimAdd.Type = DimensionType.TimeDimension;
                        itemDimAdd.Representation = new ArtefactRef
                        {
                            Id = keyFromSdmx(CustomCodelistConstants.Agency, CustomCodelistConstants.TimePeriodCodeList,
                                CustomCodelistConstants.Version),
                            RefType = Models.ArtefactType.ArtefactEnumType.CodeList
                        };
                    }
                    itemDsd.Dimensions.Add(itemDimAdd);
                }
            }

            readCustomAnnotation(itemDsd, dsd, endPointCustomAnnotationConfig);

            return itemDsd;
        }

        public static IDataStructureObject ConvertArtefact(Dsd dsd)
        {
            /*
            IDataStructureMutableObject dsd = new DataStructureMutableCore() { Id = "TEST", AgencyId = "TEST_AGENCY", Version = "1.0" };
            dsd.AddName("en", "Test name");
            IDimensionMutableObject dimension = new DimensionMutableCore();
            dimension.ConceptRef = new StructureReferenceImpl("TEST_AGENCY", "TEST_CONCEPTS", "1.0", SdmxStructureEnumType.Concept, "TEST_DIM");
            dimension.Representation = new RepresentationMutableCore { Representation = new StructureReferenceImpl("TEST_AGENCY", "CL_TEST", "2.0", SdmxStructureEnumType.CodeList) };

            dsd.AddDimension(dimension);
            dsd.AddPrimaryMeasure(new StructureReferenceImpl("TEST_AGENCY", "TEST_CONCEPTS", "1.0", SdmxStructureEnumType.Concept, "OBS_VALUE"));
            */

            var dsdKeys = keyFromModel(dsd.Id);

            IDataStructureMutableObject mutable = new DataStructureMutableCore
            {
                Id = dsdKeys[1],
                AgencyId = dsdKeys[0],
                Version = dsdKeys[2]
            };

            if (dsd.Names != null)
            {
                foreach (var item in dsd.Names)
                {
                    mutable.AddName(item.Key, item.Value);
                }
            }

            if (dsd.Descriptions != null)
            {
                foreach (var item in dsd.Descriptions)
                {
                    mutable.AddDescription(item.Key, item.Value);
                }
            }

            var primaryKeys = keyFromModel(dsd.PrimaryMeasure.ConceptRef.Id);
            mutable.AddPrimaryMeasure(new StructureReferenceImpl(primaryKeys[0], primaryKeys[1], primaryKeys[2],
                SdmxStructureEnumType.Concept, dsd.PrimaryMeasure.Id));

            foreach (var dim in dsd.Dimensions)
            {
                IDimensionMutableObject dimension = new DimensionMutableCore
                {
                    Id = dim.Id
                };

                if (dim.Id.Equals("TIME_PERIOD", StringComparison.InvariantCulture))
                {
                    dimension.TimeDimension = true;
                }
                else if (dim.Type == DimensionType.MeasureDimension)
                {
                    dimension.MeasureDimension = true;
                }

                if (dim?.ConceptRef?.Id != null)
                {
                    var conceptKeys = keyFromModel(dim.ConceptRef.Id);
                    dimension.ConceptRef = new StructureReferenceImpl(conceptKeys[0], conceptKeys[1], conceptKeys[2],
                        SdmxStructureEnumType.Concept, dim.Id);
                }

                if (dim?.Representation?.Id != null)
                {
                    var codelistKeys = keyFromModel(dim.Representation.Id);
                    dimension.Representation = new RepresentationMutableCore
                    {
                        Representation = new StructureReferenceImpl(codelistKeys[0], codelistKeys[1], codelistKeys[2],
                            dimension.MeasureDimension ? SdmxStructureEnumType.ConceptScheme : ArtefactType(dim.Representation.RefType))
                    };
                }



                mutable.AddDimension(dimension);
            }

            if (dsd.Extras != null)
            {
                foreach (var item in dsd.Extras)
                {
                    var annotation = AnnotationFromExtra(item);
                    if (annotation != null)
                    {
                        mutable.AddAnnotation(annotation);
                    }
                }
            }

            return mutable.ImmutableInstance;
        }

        public static Criteria ConvertArtefact(IContentConstraintObject contentConstraint,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            if (contentConstraint == null)
            {
                return null;
            }

            var itemCriteria = new Criteria
            {
                Id = keyFromSdmx(contentConstraint.AgencyId, contentConstraint.Id, contentConstraint.Version)
            };

            var items = contentConstraint?.IncludedCubeRegion?.KeyValues?.FirstOrDefault()?.Values;
            if (items != null)
            {
                itemCriteria.Values = new List<Code>();
                foreach (var item in items)
                {
                    itemCriteria.Values.Add(new Code { Id = item });
                }
            }

            return itemCriteria;
        }

        private static List<ExtraValue> createAnnotation(IList<IAnnotation> annotations)
        {
            if (annotations == null)
            {
                return null;
            }

            var annotationExtras = new List<ExtraValue>();
            foreach (var annotation in annotations)
            {
                var annotationId = annotation.Id ?? annotation.Type;
                var annotationExtra = new ExtraValueAnnotation(annotationId);
                annotationExtra.AddTextList(annotation.Text.GetAllTranslateItem());
                annotationExtra.SetAnnotationType(annotation.Type ?? annotation.Id);
                annotationExtra.SetAnnotationTitle(annotation.Title ?? annotation.Id);
                annotationExtra.IsPublic = false;
                annotationExtras.Add(annotationExtra);
            }
            return annotationExtras;
        }

        public static string keyFromSdmx(string agency, string id, string version)
        {
            return $"{agency}+{id}+{version}";
        }

        public static string[] keyFromModel(string keyModel)
        {
            return keyModel.Split('+');
        }

        private static void readCustomAnnotation(Dataflow dataflow, IDataflowObject sdmxDataflow,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            if (endPointCustomAnnotationConfig == null)
            {
                return;
            }

            if (endPointCustomAnnotationConfig?.CRITERIA_SELECTION != null)
            {
                dataflow.CriteriaSelectionMode = generateValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.CRITERIA_SELECTION);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_ROW != null)
            {
                dataflow.LayoutRows = generateListValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_ROW);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_COLUMN != null)
            {
                dataflow.LayoutColumns = generateListValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_COLUMN);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_ROW_SECTION != null)
            {
                dataflow.LayoutRowSections = generateListValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_ROW_SECTION);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_FILTER != null)
            {
                dataflow.LayoutFilter = generateListValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_FILTER);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_CHART_PRIMARY_DIM != null)
            {
                dataflow.LayoutChartPrimaryDim = generateListValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_CHART_PRIMARY_DIM);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_CHART_SECONDARY_DIM != null)
            {
                dataflow.LayoutChartSecondaryDim = generateListValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_CHART_SECONDARY_DIM);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_CHART_FILTER != null)
            {
                dataflow.LayoutChartFilter = generateListValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_CHART_FILTER);
            }

            if (endPointCustomAnnotationConfig?.NOT_DISPLAYED != null)
            {
                dataflow.NotDisplay =
                    generateCustomAnnotationNotDisplayedValue(sdmxDataflow.Annotations, endPointCustomAnnotationConfig);
            }

            if (endPointCustomAnnotationConfig?.CRITERIA_SELECTION != null)
            {
                dataflow.CriteriaSelectionMode = generateValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.CRITERIA_SELECTION);
            }

            if (endPointCustomAnnotationConfig?.VIRTUAL_DATAFLOW_NODE != null)
            {
                dataflow.VirtualEndPoint = generateValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.VIRTUAL_DATAFLOW_NODE);
            }

            if (endPointCustomAnnotationConfig?.DEFAULT_VIEW != null)
            {
                dataflow.DefaultView = generateValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.DEFAULT_VIEW);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_NUMBER_OF_DECIMALS != null)
            {
                var valueDecimal = sdmxDataflow?.Annotations?.FirstOrDefault(i =>
                    i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.LAYOUT_NUMBER_OF_DECIMALS,
                        StringComparison.InvariantCultureIgnoreCase))?.Title;
                if (valueDecimal != null)
                {
                    try
                    {
                        dataflow.DecimalNumber = new Dictionary<string, int?> { { "EN", Convert.ToInt32(valueDecimal) } };
                    }
                    catch (Exception ex)
                    {
                        //TOOD add log warning for convert 
                    }
                }

                //MUltiLang
                //dataflow.DecimalNumber = sdmxDataflow?.Annotations?.FirstOrDefault(i => i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.LAYOUT_NUMBER_OF_DECIMALS, StringComparison.InvariantCultureIgnoreCase))?.Text?.ToDictionary(i => i.Locale, i =>
                //                {
                //                    Nullable<int> valueConverted = null;
                //                    if (i.Value != null)
                //                    {
                //                        int valueTryConvert;
                //                        Int32.TryParse(i.Value, out valueTryConvert);
                //                        if (valueTryConvert > 0)
                //                        {
                //                            valueConverted = valueTryConvert;
                //                            return valueConverted;
                //                        }
                //                    }
                //                    return valueConverted;
                //                });
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_DECIMAL_SEPARATOR != null)
            {
                dataflow.DecimalSeparator = generateValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_DECIMAL_SEPARATOR);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_EMPTY_CELL_PLACEHOLDER != null)
            {
                dataflow.EmptyCellPlaceHolder = generateValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_EMPTY_CELL_PLACEHOLDER);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_MAX_TABLE_CELLS != null)
            {
                var maxObsValue = sdmxDataflow?.Annotations?.FirstOrDefault(i =>
                    i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.LAYOUT_MAX_TABLE_CELLS,
                        StringComparison.InvariantCultureIgnoreCase))?.Title;
                if (maxObsValue == null)
                {
                    dataflow.MaxCell = null;
                }
                else
                {
                    int? valueConverted = null;
                    int valueTryConvert;
                    int.TryParse(maxObsValue, out valueTryConvert);
                    if (valueTryConvert > 0)
                    {
                        valueConverted = valueTryConvert;
                    }

                    dataflow.MaxCell = new Dictionary<string, int?> { { "EN", valueConverted } };
                }


                //Multilanguages
                //dataflow.MaxObservation = sdmxDataflow?.Annotations?.FirstOrDefault(i => i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.MAX_NUM_CELLS, StringComparison.InvariantCultureIgnoreCase))
                //                ?.Text?.ToDictionary(i => i.Locale, i =>
                //                {
                //                    Nullable<int> valueConverted = null;
                //                    if (i.Value != null)
                //                    {
                //                        int valueTryConvert;
                //                        Int32.TryParse(i.Value, out valueTryConvert);
                //                        if (valueTryConvert > 0)
                //                        {
                //                            valueConverted = valueTryConvert;
                //                            return valueConverted;
                //                        }
                //                    }
                //                    return valueConverted;
                //                });
            }

            if (endPointCustomAnnotationConfig?.GEO_ID != null)
            {
                dataflow.GeoIds = generateListValueFromCustomAnnotation(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.GEO_ID, '+');
            }

            if (endPointCustomAnnotationConfig?.DEFAULT != null)
            {
                dataflow.DefaultCodeSelected =
                    setCustomAnnotationDefaultSelected(sdmxDataflow.Annotations, endPointCustomAnnotationConfig);
            }

            if (endPointCustomAnnotationConfig?.LAST_UPDATE != null)
            {
                dataflow.LastUpdate = generateValueFromCustomAnnotation(sdmxDataflow.Annotations,
                    endPointCustomAnnotationConfig.LAST_UPDATE);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_DEFAULT_PRESENTATION != null)
            {
                dataflow.DefaultPresentation = generateValueFromCustomAnnotation(sdmxDataflow.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_DEFAULT_PRESENTATION);
            }

            if (endPointCustomAnnotationConfig?.DATAFLOW_CATALOG_TYPE != null)
            {
                dataflow.DataflowCatalogType = generateValueFromCustomAnnotation(sdmxDataflow.Annotations,
                    endPointCustomAnnotationConfig.DATAFLOW_CATALOG_TYPE);
            }

            dataflow.NonProductionDataflow = generateValueBooleanFromCustomAnnotation(sdmxDataflow.Annotations, SdmxParser.AnnotationDiscardDataflowFromCatalogTree);
            dataflow.HiddenFromCatalog = generateValueBooleanFromCustomAnnotation(sdmxDataflow.Annotations, SdmxParser.AnnotationHidddenDataflowFromCatalogTree);

            //MUltilanguages
            if (endPointCustomAnnotationConfig?.KEYWORDS != null)
            {
                dataflow.Keywords = generateListValueFromCustomAnnotationMultiLanguages(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.KEYWORDS, "+");
            }

            if (endPointCustomAnnotationConfig?.ATTACHED_DATA_FILES != null)
            {
                dataflow.AttachedDataFiles =
                    generateListValueFromCustomAnnotationMultiLanguages(sdmxDataflow.Annotations,
                        endPointCustomAnnotationConfig.ATTACHED_DATA_FILES, "||");
            }

            if (endPointCustomAnnotationConfig?.METADATA_URL != null)
            {
                dataflow.MetadataUrl = generateValueFromCustomAnnotationMultiLanguages(sdmxDataflow.Annotations,
                    endPointCustomAnnotationConfig.METADATA_URL);
            }

            if (endPointCustomAnnotationConfig?.DATAFLOW_NOTES != null)
            {
                dataflow.DefaultNote = generateValueFromCustomAnnotationMultiLanguages(sdmxDataflow.Annotations,
                    endPointCustomAnnotationConfig.DATAFLOW_NOTES);
            }

            if (endPointCustomAnnotationConfig?.DATAFLOW_SOURCE != null)
            {
                dataflow.DataflowSource = generateValueFromCustomAnnotationMultiLanguages(sdmxDataflow?.Annotations,
                    endPointCustomAnnotationConfig.DATAFLOW_SOURCE);
            }
        }

        private static void readCustomAnnotation(Dsd dsd, IDataStructureObject sdmxDsd,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            if (endPointCustomAnnotationConfig == null)
            {
                return;
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_ROW != null)
            {
                dsd.LayoutRows = generateListValueFromCustomAnnotation(sdmxDsd?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_ROW);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_COLUMN != null)
            {
                dsd.LayoutColumns = generateListValueFromCustomAnnotation(sdmxDsd?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_COLUMN);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_ROW_SECTION != null)
            {
                dsd.LayoutRowSections = generateListValueFromCustomAnnotation(sdmxDsd?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_ROW_SECTION);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_CHART_PRIMARY_DIM != null)
            {
                dsd.LayoutChartPrimaryDim = generateListValueFromCustomAnnotation(sdmxDsd?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_CHART_PRIMARY_DIM);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_CHART_SECONDARY_DIM != null)
            {
                dsd.LayoutChartSecondaryDim = generateListValueFromCustomAnnotation(sdmxDsd?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_CHART_SECONDARY_DIM);
            }

            if (endPointCustomAnnotationConfig?.LAYOUT_CHART_FILTER != null)
            {
                dsd.LayoutChartFilter = generateListValueFromCustomAnnotation(sdmxDsd?.Annotations,
                    endPointCustomAnnotationConfig.LAYOUT_CHART_FILTER);
            }

            if (endPointCustomAnnotationConfig?.GEO_ID != null)
            {
                dsd.GeoIds = generateListValueFromCustomAnnotation(sdmxDsd?.Annotations,
                    endPointCustomAnnotationConfig.GEO_ID, '+');
            }

            if (endPointCustomAnnotationConfig?.NOT_DISPLAYED != null)
            {
                dsd.NotDisplay =
                    generateCustomAnnotationNotDisplayedValue(sdmxDsd.Annotations, endPointCustomAnnotationConfig);
            }

            if (endPointCustomAnnotationConfig?.DEFAULT != null)
            {
                dsd.DefaultCodeSelected =
                    setCustomAnnotationDefaultSelected(sdmxDsd.Annotations, endPointCustomAnnotationConfig);
            }
        }

        private static void readCustomAnnotation(Codelist codelist, ICodelistObject sdmxCodelist,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            if (endPointCustomAnnotationConfig == null)
            {
                return;
            }
        }
        private static void readCustomAnnotation(Codelist codelist, IConceptSchemeObject sdmxConceptScheme,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            if (endPointCustomAnnotationConfig == null)
            {
                return;
            }
        }

        private static void readCustomAnnotation(Code code, IItemObject sdmxCode,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            if (endPointCustomAnnotationConfig == null)
            {
                return;
            }

            if (sdmxCode != null)
            {
                code.IsDefault = sdmxCode.Annotations
                    .FirstOrDefault(i => i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.DEFAULT,
                        StringComparison.InvariantCultureIgnoreCase))?.Text?.ToDictionary(i => i.Locale,
                        i => i.Value != null && (i.Value.Equals("1") ||
                                                 i.Value.Equals("True", StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        private static Dictionary<string, List<FilterCriteria>> setCustomAnnotationDefaultSelected(
            IList<IAnnotation> annotations, EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var annotationDefaultItem = annotations?.FirstOrDefault(i =>
                i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.DEFAULT,
                    StringComparison.InvariantCultureIgnoreCase))?.Title;
            if (annotationDefaultItem == null)
            {
                return null;
            }

            var dims = annotationDefaultItem.Split(',');
            var defaultCodeSelected = new Dictionary<string, List<FilterCriteria>>
                {{"EN", new List<FilterCriteria>()}};
            if (dims != null)
            {
                foreach (var itemDim in dims)
                {
                    var ix = itemDim.IndexOf("=", StringComparison.Ordinal);
                    if (ix == -1)
                    {
                        continue;
                    }

                    var criteriaId = itemDim.Substring(0, ix);
                    if (criteriaId.Equals("LASTNPERIODS", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var convertResult = Int32.TryParse(itemDim.Substring(ix + 1), out int period);
                        if (!convertResult)
                        {
                            continue;
                        }

                        defaultCodeSelected["EN"].Add(new FilterCriteria
                        {
                            Id = "TIME_PERIOD",
                            Period = period,
                            Type = FilterType.TimePeriod
                        });
                        continue;
                    }

                    defaultCodeSelected["EN"].Add(new FilterCriteria
                    {
                        Id = criteriaId,
                        FilterValues = itemDim.Substring(ix + 1)?.Split('+')?.ToList(),
                        Type = FilterType.CodeValues
                    });
                }
            }

            return defaultCodeSelected;
        }

        private static Dictionary<string, List<Criteria>> generateCustomAnnotationNotDisplayedValue(
            IList<IAnnotation> annotations, EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var annotationDefaultItem = annotations?.FirstOrDefault(i =>
                i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.NOT_DISPLAYED,
                    StringComparison.InvariantCultureIgnoreCase))?.Title;
            if (annotationDefaultItem == null)
            {
                return null;
            }

            var dims = annotationDefaultItem.Split(',');
            var notDisplayValues = new Dictionary<string, List<Criteria>>();
            if (dims != null)
            {
                var listCriteria = new List<Criteria>();
                foreach (var itemDim in dims)
                {
                    var ix = itemDim.IndexOf("=", StringComparison.Ordinal);
                    if (ix != -1)
                    {
                        listCriteria.Add(new Criteria
                        {
                            Id = itemDim.Substring(0, ix),
                            Values = itemDim.Substring(ix + 1)?.Split('+')?.Select(i => new Code { Id = i }).ToList()
                        });
                    }
                    else
                    {
                        listCriteria.Add(new Criteria { Id = itemDim, Values = new List<Code>() });
                    }
                }

                notDisplayValues.Add("EN", listCriteria);
            }

            return notDisplayValues;
        }

        private static void setCustomAnnotationNotDisplayedDimension(
            Dictionary<string, List<Dimension>> notDisplayDimensions, IList<IAnnotation> annotations,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var annotationDefaultItem = annotations?.FirstOrDefault(i =>
                i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.NOT_DISPLAYED,
                    StringComparison.InvariantCultureIgnoreCase))?.Title;

            if (annotationDefaultItem == null)
            {
                notDisplayDimensions = null;
                return;
            }

            notDisplayDimensions = new Dictionary<string, List<Dimension>>
                {{"EN", annotationDefaultItem.Split(',')?.Select(i => new Dimension {Id = i}).ToList()}};
        }

        private static Dictionary<string, string> generateValueFromCustomAnnotation(IList<IAnnotation> annotations,
            string annotationType)
        {
            if (annotations == null)
            {
                return null;
            }

            var title = annotations?.FirstOrDefault(i =>
                i.Type != null && i.Type.Equals(annotationType, StringComparison.InvariantCultureIgnoreCase))?.Title;
            if (title == null)
            {
                return null;
            }

            return new Dictionary<string, string> { { "EN", title } };
        }

        private static Dictionary<string, bool> generateValueBooleanFromCustomAnnotation(IList<IAnnotation> annotations,
            string annotationType)
        {
            if (annotations == null)
            {
                return null;
            }

            var title = annotations?.FirstOrDefault(i =>
                i.Type != null && i.Type.Equals(annotationType, StringComparison.InvariantCultureIgnoreCase))?.Title;
            if (title == null)
            {
                return null;
            }

            var resultBool = false;
            try
            {
                resultBool = Convert.ToBoolean(title);
            }
            catch (Exception)
            {

            }

            return new Dictionary<string, bool> { { "EN", resultBool } };
        }

        private static Dictionary<string, List<string>> generateListValueFromCustomAnnotation(
            IList<IAnnotation> annotations, string annotationType, char separator = ',')
        {
            if (annotations == null)
            {
                return null;
            }

            var title = annotations?.FirstOrDefault(i =>
                i.Type != null && i.Type.Equals(annotationType, StringComparison.InvariantCultureIgnoreCase))?.Title;

            if (title == null)
            {
                return null;
            }

            return new Dictionary<string, List<string>> { { "EN", title.Split(separator).ToList() } };
        }


        private static void popolateVirtualDataflowInformation(Dataflow dataflow, IDataflowObject sdmxDataflow,
            string annotationVirtualDataflow)
        {
            var virtualDataflow = sdmxDataflow?.Annotations?.FirstOrDefault(i =>
                i.Id != null && i.Id.Equals(annotationVirtualDataflow, StringComparison.InvariantCultureIgnoreCase) ||
                i.Type != null &&
                i.Type.Equals(annotationVirtualDataflow, StringComparison.InvariantCultureIgnoreCase));

            if (virtualDataflow != null)
            {
                dataflow.DataflowType = DataflowType.IsVirtual;
                dataflow.VirtualEndPointSoapV21 = null;
                dataflow.VirtualEndPointSoapV20 = null;
                dataflow.VirtualEndPointRest = null;
                dataflow.VirtualSource = virtualDataflow.Title;
                dataflow.VirtualType = VirtualType.Node;
                return;
            }


            virtualDataflow = sdmxDataflow?.Annotations?.FirstOrDefault(i =>
                i.Id != null && i.Id.Equals("ENDPOINT", StringComparison.InvariantCultureIgnoreCase) &&
                i.Type != null && i.Type.Equals("EP", StringComparison.InvariantCultureIgnoreCase));
            if (virtualDataflow == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(virtualDataflow?.Text.FirstOrDefault()?.Value))
            {
                return;
            }

            var values = virtualDataflow.Text.FirstOrDefault().Value
                .Split(new[] { "@;" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var itemValue in values)
            {
                var itemSplit = itemValue.Split('=');
                if (itemSplit.Length <= 1 || string.IsNullOrWhiteSpace(itemSplit[1]))
                {
                    continue;
                }

                dataflow.DataflowType = DataflowType.IsVirtual;
                if (itemSplit[0].Equals("@EP1", StringComparison.InvariantCultureIgnoreCase))
                {
                    dataflow.VirtualEndPointSoapV21 = itemSplit[1].Trim();
                }
                else if (itemSplit[0].Equals("@EP2", StringComparison.InvariantCultureIgnoreCase))
                {
                    dataflow.VirtualEndPointSoapV20 = itemSplit[1].Trim();
                }
                else if (itemSplit[0].Equals("@EP3", StringComparison.InvariantCultureIgnoreCase))
                {
                    dataflow.VirtualEndPointRest = itemSplit[1].Trim();
                }
                else if (itemSplit[0].Equals("@EPT", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (itemSplit[1].Trim().Equals("SDMX V2.1", StringComparison.InvariantCultureIgnoreCase))
                    {
                        dataflow.VirtualType = VirtualType.SoapV21;
                    }
                    else if (itemSplit[1].Trim().Equals("SDMX V2.0", StringComparison.InvariantCultureIgnoreCase))
                    {
                        dataflow.VirtualType = VirtualType.SoapV20;
                    }
                    else if (itemSplit[1].Trim().Equals("SDMX VREST", StringComparison.InvariantCultureIgnoreCase))
                    {
                        dataflow.VirtualType = VirtualType.Rest;
                    }
                }
                else if (itemSplit[0].Equals("@SOURCE", StringComparison.InvariantCultureIgnoreCase))
                {
                    dataflow.VirtualSource = itemSplit[1].Trim();
                }
            }
        }

        //Multilanguages implementation
        private static void setCustomAnnotationNotDisplayedValueMultiLanguages(
            Dictionary<string, List<Criteria>> notDisplayValues, IList<IAnnotation> annotations,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var annotationDefaultItem = annotations?.FirstOrDefault(i =>
                i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.NOT_DISPLAYED,
                    StringComparison.InvariantCultureIgnoreCase))?.Text;
            if (annotationDefaultItem != null)
            {
                foreach (var itemLang in annotationDefaultItem)
                {
                    var dims = itemLang?.Value?.Split(',');
                    if (dims != null)
                    {
                        var listCriteria = new List<Criteria>();
                        foreach (var itemDim in dims)
                        {
                            var ix = itemDim.IndexOf("=", StringComparison.Ordinal);
                            if (ix != -1)
                            {
                                listCriteria.Add(new Criteria
                                {
                                    Id = itemDim.Substring(0, ix),
                                    Values = itemDim.Substring(ix + 1)?.Split('+')?.Select(i => new Code { Id = i })
                                        .ToList()
                                });
                            }
                        }

                        notDisplayValues.Add(itemLang.Locale, listCriteria);
                    }
                }
            }
        }

        private static void setCustomAnnotationNotDisplayedDimensionMultiLanguages(
            Dictionary<string, List<Dimension>> notDisplayDimensions, IList<IAnnotation> annotations,
            EndPointCustomAnnotationConfig endPointCustomAnnotationConfig)
        {
            var annotationDefaultItem = annotations?.FirstOrDefault(i =>
                i.Type != null && i.Type.Equals(endPointCustomAnnotationConfig.NOT_DISPLAYED,
                    StringComparison.InvariantCultureIgnoreCase))?.Text;
            if (annotationDefaultItem != null)
            {
                foreach (var itemLang in annotationDefaultItem)
                {
                    var dimensions = itemLang?.Value?.Split(',')?.Select(i => new Dimension { Id = i }).ToList();
                    notDisplayDimensions.Add(itemLang.Locale, dimensions);
                }
            }
        }

        private static Dictionary<string, string> generateValueFromCustomAnnotationMultiLanguages(
            IList<IAnnotation> annotations, string annotationType)
        {
            if (annotations == null)
            {
                return null;
            }

            return annotations
                ?.FirstOrDefault(i =>
                    i.Type != null && i.Type.Equals(annotationType, StringComparison.InvariantCultureIgnoreCase))?.Text
                ?.ToDictionary(i => i.Locale, i => i.Value);
        }

        private static Dictionary<string, List<string>> generateListValueFromCustomAnnotationMultiLanguages(
            IList<IAnnotation> annotations, string annotationType, string separator = ",")
        {
            if (annotations == null)
            {
                return null;
            }

            return annotations
                ?.FirstOrDefault(i =>
                    i.Type != null && i.Type.Equals(annotationType, StringComparison.InvariantCultureIgnoreCase))?.Text
                ?.ToDictionary(i => i.Locale,
                    i => !string.IsNullOrEmpty(i.Value)
                        ? i.Value.Split(new[] { separator }, StringSplitOptions.None).ToList()
                        : null);
        }
    }
}