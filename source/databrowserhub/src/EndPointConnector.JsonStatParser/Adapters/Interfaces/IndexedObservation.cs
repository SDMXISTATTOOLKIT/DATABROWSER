using System;
using System.Collections.Generic;
using EndPointConnector.JsonStatParser.Model;

namespace EndPointConnector.JsonStatParser.Adapters.Interfaces
{
    public readonly struct IndexedObservation
    {

        public readonly string[] Index;

        public readonly ObservationValue Value;


        public IndexedObservation(string[] index, ObservationValue observationValue) : this()
        {
            Index = index;
            Value = observationValue;
        }


        public override bool Equals(object obj)
        {
            return obj is IndexedObservation other &&
                   EqualityComparer<string[]>.Default.Equals(Index, other.Index) &&
                   EqualityComparer<ObservationValue>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, Value);
        }

        public void Deconstruct(out string[] index, out ObservationValue value)
        {
            index = Index;
            value = Value;
        }

    }
}