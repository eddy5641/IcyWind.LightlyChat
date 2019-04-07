namespace IcyWind.Chat.Presence
{
    /// <summary>
    /// The presence show for XMPP client
    /// </summary>
    public enum PresenceShow
    {
        /// <summary>
        /// Player is available to chat
        /// </summary>
        Chat,

        /// <summary>
        /// Player is away
        /// </summary>
        Away,

        /// <summary>
        /// Do not disturb
        /// </summary>
        Dnd,

        /// <summary>
        /// Extended away
        /// </summary>
        Xa,

        /// <summary>
        /// User is on a mobile device (Unique)
        /// </summary>
        Mobile
    }

    /// <summary>
    /// A simple converter class to convert <see cref="PresenceShow"/> to an XMPP string value
    /// </summary>
    public class ConvertPresenceShow
    {
        /// <summary>
        /// Converts <see cref="PresenceShow"/> to a string
        /// </summary>
        /// <param name="show">The presence to convert</param>
        /// <returns>The XMPP string equivalent</returns>
        public static string ConvertPresenceShowToString(PresenceShow show)
        {
            switch (show)
            {
                case PresenceShow.Away:
                    return "away";
                case PresenceShow.Chat:
                    return "chat";
                case PresenceShow.Mobile:
                    return "mobile";
                case PresenceShow.Dnd:
                    return "dnd";
                case PresenceShow.Xa:
                    return "xa";
                default:
                    return "chat";
            }
        }
    }
}
