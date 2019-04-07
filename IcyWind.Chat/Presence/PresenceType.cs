namespace IcyWind.Chat.Presence
{
    /// <summary>
    /// The presence type that a player has
    /// </summary>
    public enum PresenceType
    {
        /// <summary>
        /// Available for communication
        /// </summary>
        Available,

        /// <summary>
        /// The sender wishes to subscribe to the recipient's presence
        /// </summary>
        Subscribe,

        /// <summary>
        /// The sender has allowed the recipient to receive their presence
        /// </summary>
        Subscribed,

        /// <summary>
        /// The sender is unsubscribing from another entity's presence
        /// </summary>
        Unsubscribe,

        /// <summary>
        /// The subscription request has been denied or a previously-granted subscription has been cancelled
        /// </summary>
        Unsubscribed,

        /// <summary>
        /// Signals that the entity is no longer available for communication
        /// </summary>
        Unavailable,

        /// <summary>
        /// Tell the server that sender wants to appear offline
        /// </summary>
        Invisible,

        /// <summary>
        /// Tell the server that the sender wants to appear online
        /// </summary>
        Visible,

        /// <summary>
        /// An error has occurred regarding processing or delivery of a previously-sent presence stanza
        /// </summary>
        Error,

        /// <summary>
        /// A request for an entity's current presence; SHOULD be generated only by a server on behalf of a user
        /// </summary>
        Probe
    }

    /// <summary>
    /// A simple converter class to convert <see cref="PresenceType"/> to an XMPP string value
    /// </summary>
    public class ConvertPresenceType
    {
        /// <summary>
        /// Converts <see cref="PresenceType"/> to a string
        /// </summary>
        /// <param name="type">The presence to convert</param>
        /// <returns>The XMPP string equivalent</returns>
        public static string ConvertPresenceTypeToString(PresenceType type)
        {
            switch (type)
            {
                case PresenceType.Available:
                    return "available";
                case PresenceType.Error:
                    return "error";
                case PresenceType.Invisible:
                    return "invis";
                case PresenceType.Probe:
                    return "probe";
                case PresenceType.Subscribe:
                    return "subscribe";
                case PresenceType.Subscribed:
                    return "subscribed";
                case PresenceType.Unavailable:
                    return "unavailabe";
                case PresenceType.Unsubscribe:
                    return "unsubscribe";
                case PresenceType.Unsubscribed:
                    return "unsubscribed";
                case PresenceType.Visible:
                    return "visible";
                default:
                    return type.ToString("G").ToLower();
            }
        }
    }
}
