using System;
using System.Collections.Generic;
using System.Text;

namespace EndPointConnector.Interfaces.JsonStat
{
    public interface IFromSDMXToJsonStatConverterConfig : ISDMXParsingConfig, IJsonStatConverterConfig
    {
        public abstract IFromSDMXToJsonStatConverterConfig Clone();
        public void Merge(IFromSDMXToJsonStatConverterConfig otherConfig);
    }
}
