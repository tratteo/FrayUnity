namespace Fray.Character.Input
{
    /// <summary>
    ///   Generic structure to handle any kind of input data
    /// </summary>
    public readonly struct CharacterInputData
    {
        public readonly CharacterAction action;
        public readonly object payload;

        public CharacterInputData(CharacterAction action, object payload = null)
        {
            this.action = action;
            this.payload = payload;
        }

        /// <summary>
        ///   Get the payload casted to the specified type
        /// </summary>
        public T CastPayload<T>(T defaultValue = default)
        {
            try
            {
                var res = (T)payload;
                return res;
            }
            catch { return defaultValue; }
        }
    }
}