namespace UnityGUIFramework
{
    /// <summary>
    /// This is utility class that wraps logging functions
    /// </summary>
    public static class DebugHelper
    {
        /// <summary>
        /// Outputs debug message
        /// </summary>
        /// <param name="str">format string</param>
        /// <param name="args">arguments</param>
        public static void Debug(string str, params object[] args)
        {
            UnityEngine.Debug.Log(string.Format(str, args));
        }

        /// <summary>
        /// Outputs error message
        /// </summary>
        /// <param name="str">format string</param>
        /// <param name="args">arguments</param>
        public static void Error(string str, params object[] args)
        {
            UnityEngine.Debug.LogError(string.Format(str, args));
        }

        /// <summary>
        /// Outputs warning message
        /// </summary>
        /// <param name="str">format string</param>
        /// <param name="args">arguments</param>
        public static void Warning(string str, params object[] args)
        {
            UnityEngine.Debug.LogWarning(string.Format(str, args));
        }
    }
}