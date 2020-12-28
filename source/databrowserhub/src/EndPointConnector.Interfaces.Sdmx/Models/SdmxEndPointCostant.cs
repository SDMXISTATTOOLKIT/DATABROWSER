namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public static class SdmxEndPointCostant
    {
        public enum ConnectorType
        {
            Rest,
            SoapV21,
            SoapV20,
            FromConfig
        }

        public enum RequestType
        {
            DataStructureSpecificV21Xml,
            DataSdmxJson,
            DataCsvSdmxString,
            StructureXml,
            DataCountString,
            DataGenericV20Xml,
            DataGenericV21Xml,
            StructureJson,
            DataCompactXml,
            DataSectionalCompactXml,
            DataEdiXml,
            Structure
        }

        public enum SDMXWSFunction
        {
            /// <summary>
            ///     Obtain Compact Data: This is an exchange invoked by the Query Message, for
            ///     which the response is data marked up according to the Compact Data Message.
            ///     The function should be called “GetCompactData(Query)”.
            /// </summary>
            GetCompactData,

            /// <summary>
            ///     Obtain Cross-Sectional Data: This is an exchange invoked by the Query
            ///     Message, for which the response is data marked up according to the Cross-
            ///     Sectional Data Message. The function should be called
            ///     “GetCrossSectionalData(Query)”.
            /// </summary>
            GetCrossSectionalData,

            /// <summary>
            ///     Query Structural Metadata in Repository: This is an exchange invoked by the
            ///     QueryStructureRequest message, for which the response is a confirmation in the
            ///     form of a QueryStructureResponse message. The function should be called
            ///     “QueryStructure(QueryStructureRequest).”
            /// </summary>
            QueryStructure,

            /// <summary>
            ///     Usata nel richiedere dsd a ws DOTSTAT”
            /// </summary>
            GetDataStructureDefinition
        }

        /// <summary>
        ///     This enum contains the names of  a subset of the web functions as defined in
        ///     <a href="http://sdmx.org/wp-content/uploads/2013/07/SDMX_2_1-SECTION_07_WebServicesGuidelines_2013-041.pdf">
        ///         SDMX
        ///         v2.1 Section 03 SOAP-Based SDMX WebServices: WSDL Operations and Behaviours
        ///     </a>
        /// </summary>
        public enum SDMXWSFunctionV21
        {
            GetCategorisation,

            GetCategory,

            GetDataStructure,

            GetCategoryScheme,

            GetCodelist,

            GetConceptScheme,

            GetDataflow,

            /// <summary>
            ///     Obtain Compact Data or CrossSectional Data: This is an exchange invoked by the
            ///     Complex Data Query Message, for which the response is data marked up according
            ///     to the Compact Data Message.
            ///     The function should be called "GetStructureSpecificData(ComplexDataQuery)".
            /// </summary>
            GetStructureSpecificData,

            QueryStructure,

            GetCrossSectionalData,

            GetCompactData
        }
    }
}