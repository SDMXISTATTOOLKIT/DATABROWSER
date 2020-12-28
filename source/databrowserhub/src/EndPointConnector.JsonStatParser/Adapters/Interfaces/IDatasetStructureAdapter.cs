using System.Collections.Generic;
using EndPointConnector.JsonStatParser.Adapters.Commons;

namespace EndPointConnector.JsonStatParser.Adapters.Interfaces
{
    public interface IDatasetStructureAdapter
    {

        // TODOS
        // - emptyDimensions -> true/false
        //
        //- getDimensionById(dimId) -> { 
        //	id: dim1, 
        //	name: {en: "dimension 1", it: "Dimensione 1"}
        //   codes: {c1: {en:"code 1", it: "codice 1"} , c2: {en:"code 2", it: "codice 2"}, c3:  {en:"code 3", it: "codice 3"}...}, 
        //	hiddenCodes: [c2] 
        //	}

        LocalizedString Title { get; }

        string[] DimensionIds { get; }

        string[] HiddenDimensionIds { get; }

        string[] GeoDimensionIds { get; }

        string MainGeoDimensionId { get; }

        string[] TimeDimensionIds { get; }

        string MainTimeDimensionId { get; }

        string[] AlternativeObservationsDimensionIds { get; }

        Dictionary<string, string> CustomRoles { get; }

        Dictionary<string, object> Extras { get; }

        bool HasSeries();

        IDimensionAdapter GetDimensionById(string dimensionId);

        HashSet<string> GetBannedCodesByDimensionById(string dimensionId, string language,
            string notDiplayedAnnotationName, HashSet<string> distinctDimensionValues);

        Dictionary<string, int> CalculateDimensionIndex(string dimensionId, string[] codes, string language);

    }
}