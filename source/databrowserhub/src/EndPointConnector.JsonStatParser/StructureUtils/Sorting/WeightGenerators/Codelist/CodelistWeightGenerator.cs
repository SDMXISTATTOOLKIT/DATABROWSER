using System;
using System.Collections.Generic;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist.Wrappers;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.Codelist
{
    public abstract class CodelistWeightGenerator<T> : WeightGenerator
    {

        public ICodelistWrapper<T> Codelist;

        protected CodelistWeightGenerator(ICodelistWrapper<T> codelist, HashSet<string> bannedCodes) : base(bannedCodes)
        {
            Codelist = codelist;
            BannedCodes = bannedCodes ?? throw new ArgumentNullException(nameof(bannedCodes));
        }

    }
}