using System;
using System.Collections.Generic;

namespace CherryStudio.Combat
{
    /// <summary>
    /// This implementation IS NOT RECOMMENDED, see https://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/
    /// To improve this use DI or the mincode infrastructure to not directly reference entities: https://assetstore.unity.com/packages/tools/utilities/mincode-193678
    /// </summary>
    public static class EntitiesLocator
    {
        public const string Player = "Player";

        private static readonly Dictionary<Type, object> registeredTypes = new Dictionary<Type, object>();
        private static readonly Dictionary<string, object> registeredNamed = new Dictionary<string, object>();

        public static void Register<T>(T toRegister)
        {
            registeredTypes.Add(typeof(T), toRegister);
        }

        public static void Register<T>(string name, T toRegister)
        {
            registeredNamed.Add(name, toRegister);
        }

        public static T Get<T>()
        {
            return registeredTypes.TryGetValue(typeof(T), out var result) && result is T resultT
                ? resultT
                : default(T);
        }

        public static T Get<T>(string name)
        {
            return registeredNamed.TryGetValue(name, out var result) && result is T resultT
                ? resultT
                : default(T);
        }
    }
}
