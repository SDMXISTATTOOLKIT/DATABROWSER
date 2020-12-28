using System.Collections.Generic;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist
{
    public class TreeNode<T>
    {

        public string Id { get; }

        public double? Weight { get; }

        public T Value { get; }

        public List<TreeNode<T>> Children { get; }

        public TreeNode<T> Parent { get; set; }

        public TreeNode(double? val, string id, T wrappedValue)
        {
            Weight = val;
            Children = new List<TreeNode<T>>();
            Id = id;
            Parent = null;
            Value = wrappedValue;
        }

        public override bool Equals(object obj)
        {
            return obj is TreeNode<T> node &&
                   Id == node.Id;
        }

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }

        public void AddNode(TreeNode<T> node)
        {
            AddChild(node);
            node.SetParent(this);
        }

        protected bool Equals(TreeNode<T> other)
        {
            return Id == other.Id;
        }


        private void AddChild(TreeNode<T> node)
        {
            if (Children.Contains(node)) {
                return;
            }

            Children.Add(node);
        }

        private void SetParent(TreeNode<T> node)
        {
            Parent = node;
        }

    }
}