using GJ_TimeLine.Common;
using GJ_TimeLine.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJ_TimeLine
{
    class TimeLineThread
    {
        public TimelineConfig tc; // 传递数据

        public int nowtime { get; set; }
        private int nowact { get; set; }
        private int nowalert { get; set; }
        public bool isStart { get; set; }


        public TTSHelper tsh;

        private Dictionary<double, string> voiceLine;

        public delegate void UpActDelegate(int nowact);

        public delegate void UpTimeDelegate(int nowtime);
        public UpActDelegate upact;
        public UpTimeDelegate uptime;

        // 构造函数
        public TimeLineThread(TimelineConfig pData)
        {
            this.tc = pData;
            this.tsh = new TTSHelper();
            nowact = 1;
            nowalert = 0;
            nowtime = 0;
            voiceLine = new Dictionary<double, string>();
            foreach (var item in tc.Items)
            {
                foreach (var voiceitem in tc.AlertAlls)
                {
                    if (item.value == voiceitem.ActivityName)
                    {
                        voiceLine.Add(item.time - voiceitem.ReminderTime, voiceitem.AlertSound);
                    }
                }
            }

        }
        private void listentts(string tt)
        {
            string voice = "";
            if (tt.Split(']').Count() > 1)
            {
                voice = tt.Split(']')[1];
            }
            else
            {
                voice = tt;
            }
            this.tsh.StartTTS(voice);
        }
        public void ThreadFun() // 来自委托：ThreadStart 
        {
            while (isStart)
            {
                System.Threading.Thread.Sleep(1000);

                Console.WriteLine(nowtime);
                if (nowact < tc.Items.Count())
                {

                    if (nowtime == tc.Items[nowact].time)
                    {
                        Console.WriteLine(tc.Items[nowact].value);
                        upact(nowact);
                        nowact++;
                    }
                    if (voiceLine.ContainsKey(nowtime))
                    {
                        string[] ttsstring = voiceLine[nowtime].Split(' ');
                        if (ttsstring.Count() >= 2)
                        {
                            listentts(ttsstring[1] + "");
                        }
                        else
                        {
                            listentts(voiceLine[nowtime] + "");
                        }
                    }
                }
                uptime(nowtime);
                nowtime++;
            }
        }
    }
}
