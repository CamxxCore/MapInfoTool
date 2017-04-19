using GTA.Native;

namespace MapInfoTool
{
    /// <summary>
    /// Represents a native hash.
    /// </summary>
    public sealed class CustomNative
    {
        public static CustomNative GetIsVehicleEngineRunning = new CustomNative(0xAE31E7DF9B5B132E);

        public static CustomNative GetPedSourceOfDeath = new CustomNative(0x93C8B64DEB84728C);

        public static CustomNative SetTextEntryForWidth = new CustomNative(0x54CE8AC98E120CAB);

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="value">The hash value.</param>
        public CustomNative(ulong value)
        {
            Value = value;
        }

        public ulong Value { get; }

        public static implicit operator Hash(CustomNative hash)
        {
            return (Hash)hash.Value;
        }

        public static implicit operator ulong(CustomNative hash)
        {
            return hash.Value;
        }
    }
}


