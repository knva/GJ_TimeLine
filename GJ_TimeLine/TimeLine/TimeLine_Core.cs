using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sprache;
namespace GJ_TimeLine.TimeLine
{
    class TimeLine_Core
    {
        public TimeLineShow tls;
        public void initTconfig(string path)
        {
        
   
            bool isExitFormConfig = false;//判断配置窗口是否已经打开，防止重复打开多个配置窗口Form2
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.Name == "TimeLineShow")
                {
                    openForm.Visible = true;//如果配置窗口已打开则将其显示
                    openForm.Activate();//并激活该窗体
                    isExitFormConfig = true;
                    break;
                }
            }
            if (!isExitFormConfig)
            {
                string text = ReadConfig(path);
                TimelineConfig c = TConfigParser.TimelineConfig.Parse(text);

                tls = new TimeLineShow(c);
                tls.Show();
                tls.TopMost = true;
            }

        }

        public string ReadConfig(string path)
        {
            String alltext = File.ReadAllText(path, System.Text.Encoding.UTF8);
            return alltext;
        }
    }
   
}
