using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public abstract class ResourceLoader
    {
        public IEnumerable<string> LoadRawData()
        {
            var type = this.GetType();
            var resourceName = $"{type.Namespace}.{type.Name}.txt";
            using var stream = type.Assembly.GetManifestResourceStream(resourceName);
            using var rdr = new StreamReader(stream);

            var output = new List<string>();
            while(!rdr.EndOfStream)
            {
                output.Add(rdr.ReadLine());
            }

            return output;
        }

        public IEnumerable<int> LoadIntData()
        {
            var source = LoadRawData();

            return source
                .Select(TryParseInt)
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToArray();
        }

        private static int? TryParseInt(string candidate)
        {
            if(int.TryParse(candidate, out var num))
            {
                return num;
            }

            return null;
        }
    }
}