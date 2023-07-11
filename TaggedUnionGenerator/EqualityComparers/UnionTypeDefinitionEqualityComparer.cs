using System.Collections;
using System.Collections.Generic;
using TaggedUnionGenerator.UnionGen;

namespace TaggedUnionGenerator.EqualityComparers
{
    internal class UnionTypeDefinitionEqualityComparer : IEqualityComparer<UnionTypeDefinition>
    {
        public bool Equals(UnionTypeDefinition? x, UnionTypeDefinition? y)
        {
            return ReferenceEquals(x, y)
                || x?.Name == y?.Name
                    && x?.Namespace == y?.Namespace
                    && StructuralComparisons.StructuralEqualityComparer.Equals(x?.Options, y?.Options);
        }

        public int GetHashCode(UnionTypeDefinition obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + obj.Name.GetHashCode();
                hash = hash * 31 + obj.Namespace?.GetHashCode() ?? 0;
                hash = hash * 31 + StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj.Options);
                return hash;
            }
        }
    }

}

