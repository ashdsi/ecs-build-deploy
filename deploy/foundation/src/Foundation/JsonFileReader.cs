using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Foundation
{
    public class JsonFileReader
    {
        public static Item LoadJson()
        {
            using (StreamReader r = new StreamReader("src\\Foundation\\params.json"))
            {
                string json = r.ReadToEnd();
                Item items = JsonConvert.DeserializeObject<Item>(json);
                return items;
            }
        }
    }
}
