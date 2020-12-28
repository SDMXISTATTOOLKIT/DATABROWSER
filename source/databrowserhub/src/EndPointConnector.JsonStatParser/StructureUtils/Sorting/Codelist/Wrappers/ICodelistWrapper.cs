using System.Collections.Generic;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist.Wrappers
{
    public interface ICodelistWrapper<T> : IEnumerable<TreeNode<T>>
    {

        List<TreeNode<T>> ToTreeStructure();

        new IEnumerator<TreeNode<T>> GetEnumerator();

        bool HasTreeStructure();

    }
}