using System;
using System.Collections.Generic;

namespace Core.Utilities
{
    /// <summary>
    /// Class <c> EnumParser </c> : Fast parser for two different variable types
    /// </summary>
    /// <typeparam name="T"> enum type </typeparam>
    [Serializable]
    public class EnumParser<T>
    {
        /** Constants */
        private const string NULL_STR = "NULL";

        /** Properties */
        public Dictionary<string, T> Parser { get; private set; }


        public EnumParser()
        {
            Parser = new Dictionary<string, T>();

            ParserInit();
        }

        public void ParserInit()
        {
            string[] names = Enum.GetNames(typeof(T));
            T[] types = (T[])Enum.GetValues(typeof(T));

            for (int i = 0; i < names.Length; i++)
            {
                Parser.Add(names[i], types[i]);
            }
        }

        public T GetTypeOrDefault(string key, T def)
        {
            if (key == null || !Parser.ContainsKey(key)) { return def; }

            T type = Parser[key];

            return type != null ? type : def;
        }

        /// <summary>
        /// Returns the key (string) with the same name as input's (enum type)
        /// </summary>
        /// <param name="value"> enum type </param>
        /// <returns> EnumType </returns>
        public string GetString(T value)
        {
            foreach (KeyValuePair<string, T> pair in Parser)
            {
                if (EqualityComparer<T>.Default.Equals(pair.Value, value))
                {
                    return pair.Key;
                }
            }

            return NULL_STR;
        }
    }
}