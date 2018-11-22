using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GJ_TimeLine.Common
{
    public class TTSHelper
    {
        public async void StartTTS(string data)
        {

            Hashtable table = new Hashtable();
            table.Add("lan", "zh");
            table.Add("text", data);
            table.Add("spd", 5);
            table.Add("source", "web");
            await Task.Run(() => {
                string ttsurl = makeUrl("http://fanyi.baidu.com/gettts", table);
                using (var mf = new MediaFoundationReader(ttsurl))
                using (var wo = new WaveOutEvent())
                {
                    wo.Init(mf);
                    wo.Play();
                    while (wo.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }

            });



        }

        public static string makeUrl(string url, Hashtable dic)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic != null && dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (DictionaryEntry item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            return builder.ToString();

        }
    }
}
