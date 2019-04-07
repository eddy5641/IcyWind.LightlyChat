using IcyWind.Chat.Jid;

namespace IcyWind.Chat.Presence
{
    /// <summary>
    /// Used to set a person's presence or invoked when getting presences
    /// </summary>
    public class ChatPresence
    {
        /// <summary>
        /// Player's JID
        /// </summary>
        public UserJid FromJid { get; internal set; }

        /// <summary>
        /// The player the presence is for
        /// </summary>
        public UserJid ToJid { get; internal set; }

        /// <summary>
        /// The presence type
        /// </summary>
        public PresenceType PresenceType { get; internal set; }

        /// <summary>
        /// The show type of the presence
        /// </summary>
        public PresenceShow PresenceShow { get; internal set; }

        /// <summary>
        /// The status of the user's presence
        /// </summary>
        public string Status { get; internal set; }

        /// <summary>
        /// The last time the player was online
        /// </summary>
        public string LastOnline { get; set; }
    }
}
