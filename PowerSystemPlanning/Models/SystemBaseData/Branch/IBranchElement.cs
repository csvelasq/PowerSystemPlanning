using PowerSystemPlanning.Models.SystemBaseData.Nodes;

namespace PowerSystemPlanning.Models.SystemBaseData.Branch
{
    /// <summary>
    /// A branch element within a power system
    /// (connects two nodes, e.g. transmission line, transformer).
    /// </summary>
    /// <remarks>
    /// Represents an element that connects two (and only two) nodes between themselves.
    /// The <see cref="NodeFrom"/> and <see cref="NodeTo"/> properties 
    /// are arbitrarily set by the user, and do not necessarily
    /// represent some physical distinction between the two terminal nodes.
    /// </remarks>
    public interface IBranchElement : IPowerSystemElement
    {
        /// <summary>
        /// The node from which this element begins.
        /// </summary>
        INode NodeFrom { get; set; }
        /// <summary>
        /// The node to which this element arrives.
        /// </summary>
        INode NodeTo   { get; set; }
    }
}
