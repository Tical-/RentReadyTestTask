using System;
using System.IO;

namespace Tests
{
    public partial class Tests
    {
        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private string GenerateJson(DateTime first, DateTime second)
        {
            var str = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""StartOn"": {
      ""type"": ""string"",
      ""format"": ""date"",
      ""value"": ""first""
    },
    ""EndOn"": {
      ""type"": ""string"",
      ""format"": ""date"",
      ""value"": ""second""
    }
  },
  ""required"": [
    ""StartOn"",
    ""EndOn""
  ]
}";
            return str.Replace("first", first.ToShortDateString()).Replace("second", second.ToShortDateString());
        }
    }
}
