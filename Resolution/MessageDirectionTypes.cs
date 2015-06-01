
namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System.Xml.Serialization;

    /// <summary>
    /// Enumeration for Message Direction values
    /// </summary>
    public enum MessageDirectionTypes
    {
        /// <summary>
        /// The message direction is not specified.
        /// </summary>
        [XmlIgnore]
        NotSpecified,

        /// <summary>
        /// The message direction is in.
        /// </summary>
        [XmlIgnore]
        MsgIn,

        /// <summary>
        /// The message direction is out.
        /// </summary>
        [XmlIgnore]
        MsgOut,

        /// <summary>
        /// The message direction is both in and out.
        /// </summary>
        [XmlIgnore]
        Both
    }
}
