namespace PowerSystemPlanning.Models.SystemBaseData.Nodes
{
    /// <summary>
    /// An element that is connected to one bus and one bus only.
    /// </summary>
    public interface INodeElement : IPowerSystemElement
    {
        /// <summary>
        /// The node to which this element is connected in the power system.
        /// </summary>
        INode ConnectionNode { get; }
    }
}
