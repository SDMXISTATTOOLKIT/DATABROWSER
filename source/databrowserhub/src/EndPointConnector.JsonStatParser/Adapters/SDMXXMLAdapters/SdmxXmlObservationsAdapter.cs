using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.Model;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.DataParser.Factory;
using Org.Sdmxsource.Sdmx.DataParser.Manager;
using Org.Sdmxsource.Sdmx.EdiParser.Manager;
using Org.Sdmxsource.Util.Io;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxXmlAdapters
{
    public class SdmxXmlObservationsAdapter : IObservationsAdapter
    {

        public HashSet<string>[] DistinctDimensionsCodes { get; }

        public Dictionary<string[], IndexedObservation> Values { get; }

        public IAttributesAdapter Attributes { get; private set; }


        protected ISet<ICodelistObject> Codelists;

        protected ISet<IConceptSchemeObject> ConceptSchemes;

        protected IDataflowObject Dataflow;

        protected IDataStructureObject DataStructure;

        protected XmlDocument XmlDocument;

        private readonly string _defaultLanguage;

        private readonly Dictionary<string, int> _dimensionIdToPosition;

        public SdmxXmlObservationsAdapter(XmlDocument xmlDocument, IDataflowObject dataflow,
            IDataStructureObject dataStructure,
            ISet<ICodelistObject> codelists, ISet<IConceptSchemeObject> conceptSchemes, string defaultLanguage)
        {
            Dataflow = dataflow;
            DataStructure = dataStructure;
            Codelists = codelists;
            XmlDocument = xmlDocument;
            ConceptSchemes = conceptSchemes;
            _defaultLanguage = defaultLanguage;
            Values = new Dictionary<string[], IndexedObservation>();

            var sortedDimensions = DataStructure.DimensionList.Dimensions.OrderBy(x => x.Position).ToArray();
            var dimensionsIdList = new List<IDimension>();
            IDimension timeDimension = null;
            foreach (var dimension in sortedDimensions)
            {
                if (!dimension.Id.Equals(DataStructure.TimeDimension.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    dimensionsIdList.Add(dimension);
                }
                else
                {
                    timeDimension = dimension;
                }
            }
            if (timeDimension != null)
            {
                dimensionsIdList.Add(timeDimension);
            }
            

            _dimensionIdToPosition = dimensionsIdList.Select((val) => (id: val.Id, index: val.Position))
                .ToDictionary(x => x.id, x => x.index);


            //_dimensionIdToPosition = DataStructure.DimensionList.Dimensions
            //    .OrderBy(x => x.Position)
            //    .Select((val, pos) => (id: val.Id, index: pos))
            //    .ToDictionary(x => x.id, x => x.index);

            DistinctDimensionsCodes = new HashSet<string>[DataStructure.DimensionList.Dimensions.Count];
            for (var i = 0; i < DistinctDimensionsCodes.Length; i++) DistinctDimensionsCodes[i] = new HashSet<string>();

            InitObservationCache();
        }

        private static ObservationValue ReadNextObservationValue(IDataReaderEngine reader)
        {
            if (reader.CurrentObservation?.ObservationValue == null) {
                return new ObservationValue();
            }

            if (double.TryParse(reader.CurrentObservation.ObservationValue, NumberStyles.Float,
                CultureInfo.InvariantCulture, out var actualValue)) {
                return actualValue;
            }

            return reader.CurrentObservation.ObservationValue;
        }

        public IEnumerator<IndexedObservation> GetEnumerator()
        {
            return Values.Select(val => val.Value).GetEnumerator();
        }


        private void InitObservationCache()
        {
            var observationAttributesIndex =
                new Dictionary<string[], Dictionary<string, string>
                >(); // [c1,c2,c3...] -> [attrId1 -> attrValueIdx, attrId2 -> attrValueIdy,...]
            Dictionary<string, string> datasetAttributesIndex = null;
            var seriesAttributesIndex =
                new Dictionary<string[], Dictionary<string, string>
                >(); // [c1,c2,c3...] -> [attrId1 -> attrValueIdx, attrId2 -> attrValueIdy,...]

            var fileXmlTmp = Path.GetTempFileName();
            XmlDocument.Save(fileXmlTmp);

            try {
                using (var dataLocation = new FileReadableDataLocation(fileXmlTmp)) {
                    //CompactDataReaderEngine dataReader = new CompactDataReaderEngine(dataLocation, _dataflow, _dataStructure);
                    //GenericDataReaderEngine dataReader = new GenericDataReaderEngine(dataLocation, _dataflow, _dataStructure);
                    var readerFactory = new SdmxDataReaderFactory(new DataInformationManager(), new EdiParseManager());
                    var dataReader = readerFactory.GetDataReaderEngine(dataLocation, DataStructure, Dataflow);

                    var dimensionCount = DataStructure.DimensionList.Dimensions.Count;
                    var timeDimensionPosition = dimensionCount - 1;
                    var isTimeSeries = DataStructure.TimeDimension != null;

                    while (dataReader.MoveNextDataset()) {
                        // init dataset attribute index
                        if (dataReader.DatasetAttributes?.Count > 0 && datasetAttributesIndex == null) {
                            datasetAttributesIndex =
                                dataReader.DatasetAttributes.ToDictionary(el => el.Concept, el => el.Code);
                        }

                        while (dataReader.MoveNextKeyable()) {
                            TryToReadGroupAttributes(seriesAttributesIndex, dataReader, dimensionCount);

                            try {
                                while (dataReader.MoveNextObservation()) {
                                    var observationCoordinates = new string[dimensionCount];
                                    var observationValue = ReadNextObservationValue(dataReader);

                                    if (observationValue.IsNull) {
                                        continue;
                                    }

                                    observationCoordinates[timeDimensionPosition] =
                                        dataReader.CurrentObservation.ObsTime;
                                    var seriesCoordinates = dataReader.CurrentObservation.SeriesKey.Key
                                        .Select(x => (dimPosition: _dimensionIdToPosition[x.Concept], x.Code))
                                        .OrderBy(x => x.dimPosition)
                                        .Select(x => x.Code)
                                        .ToArray();

                                    if (isTimeSeries) {
                                        AddTimeSeriesObservation(observationAttributesIndex, seriesAttributesIndex,
                                            dataReader, observationCoordinates, observationValue, seriesCoordinates);
                                    }
                                    else {
                                        throw new NotImplementedException("plain data is not supported yet");
                                    }
                                }
                            }
                            catch (Exception) {
                                //TODO: handle exception please
                            }
                        }
                    }
                }

                Attributes = new SdmxXmlAttributesAdapter(DataStructure, Codelists, ConceptSchemes,
                    observationAttributesIndex, datasetAttributesIndex, seriesAttributesIndex, _defaultLanguage);
            }
            finally {
                File.Delete(fileXmlTmp);
            }
        }

        private void TryToReadGroupAttributes(IDictionary<string[], Dictionary<string, string>> seriesAttributesIndex,
            IDataReaderEngine dataReader, int dimensionCount)
        {
            if (seriesAttributesIndex == null) {
                throw new ArgumentNullException(nameof(seriesAttributesIndex));
            }

            //try reading group attributes
            try {
                if (dataReader.CurrentObservation != null || string.IsNullOrEmpty(dataReader.CurrentKey?.GroupName) ||
                    dataReader.CurrentKey?.Attributes == null) {
                    return;
                }

                var attrVals = dataReader.CurrentKey.Attributes.ToDictionary(x => x.Concept, x => x.Code);
                var groupDimensionCoordinates = new string[dimensionCount - 1];

                foreach (var dimReference in dataReader.CurrentKey.Key) {
                    var dimensionId = dimReference.Concept;
                    var dimensionPosition = _dimensionIdToPosition[dimensionId];
                    var dimCode = dimReference.Code;
                    groupDimensionCoordinates[dimensionPosition] = dimCode;
                }

                seriesAttributesIndex[groupDimensionCoordinates] = attrVals;
            }
            catch (Exception) {
                //TODO - Handle 
            }
        }

        private void AddTimeSeriesObservation(
            IDictionary<string[], Dictionary<string, string>> observationAttributesIndex,
            IDictionary<string[], Dictionary<string, string>> seriesAttributesIndex, IDataReaderEngine dataReader,
            string[] observationCoordinates, ObservationValue observationValue, string[] seriesCoordinates)
        {
            var dimensionPosition = 0;
            foreach (var obsEntry in dataReader.CurrentObservation.SeriesKey.Key) {
                var dimensionId = obsEntry.Concept;                                
                // dimensionPosition = _dimensionIdToPosition[dimensionId];  
                var dimCode = obsEntry.Code;
                observationCoordinates[dimensionPosition++] = dimCode;
            }

            var observation = new IndexedObservation(observationCoordinates, observationValue);
            Values[observation.Index] = observation;
            AddDimensionCodesToCache(observation.Index);

            var seriesAttributes =
                dataReader.CurrentObservation.SeriesKey.Attributes.ToDictionary(x => x.Concept, x => x.Code);
            seriesAttributesIndex[seriesCoordinates] = seriesAttributes;

            var obsAttributes = dataReader.CurrentObservation.Attributes?.ToDictionary(x => x.Concept, x => x.Code);
            observationAttributesIndex[observation.Index] = obsAttributes;
        }

        private void AddDimensionCodesToCache(IReadOnlyList<string> codes)
        {
            for (var i = 0; i < codes.Count; i++) DistinctDimensionsCodes[i].Add(codes[i]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

    }
}