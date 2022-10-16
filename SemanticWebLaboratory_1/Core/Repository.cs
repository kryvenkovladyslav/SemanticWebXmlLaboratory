using System.Collections.Generic;

namespace SemanticWebXmlLaboratory.Core
{
    public static class Repository<T>
    {
        public static List<T> Items { get; set; }

        static Repository() => Items = new List<T>();

        public static void Add(T item) => Items.Add(item);
    }
}
