using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;
namespace GJ_TimeLine.TimeLine
{
    class TimeLine_Core
    {
        public void initTconfig(string path)
        {
            string text = ReadConfig(path);
            TimelineConfig c = TConfigParser.TimelineConfig.Parse(text);

            Console.WriteLine(c.AlertAlls.Count);

        }

        public string ReadConfig(string path)
        {
            String alltext = File.ReadAllText(path, System.Text.Encoding.UTF8);
            return alltext;
        }
    }
   
}
