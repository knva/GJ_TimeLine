using GJ_TimeLine.Common;
using GJ_TimeLine.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GJ_TimeLine
{
    public partial class HotKeyEdit : Form
    {
        public HotKeyEdit()
        {
            InitializeComponent();
        }
        public HotKeyEdit(string hotkey)
        {
            InitializeComponent();
            zuhejian = hotkey;
            textBox1.Text = zuhejian;
        }
        public string zuhejian{get;set;}

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            StringBuilder keyValue = new StringBuilder();
            keyValue.Length = 0;
            keyValue.Append("");
            if (e.Modifiers != 0)
            {
                if (e.Control)
                    keyValue.Append("Ctrl + ");
                if (e.Alt)
                    keyValue.Append("Alt + ");
                if (e.Shift)
                    keyValue.Append("Shift + ");
            }
            if ((e.KeyValue >= 33 && e.KeyValue <= 40) ||
                (e.KeyValue >= 65 && e.KeyValue <= 90) ||   //a-z/A-Z
                (e.KeyValue >= 112 && e.KeyValue <= 123))   //F1-F12
            {
                keyValue.Append(e.KeyCode);
            }
            else if ((e.KeyValue >= 48 && e.KeyValue <= 57))    //0-9
            {
                keyValue.Append(e.KeyCode.ToString().Substring(1));
            }
            this.ActiveControl.Text = "";
            //设置当前活动控件的文本内容
            this.ActiveControl.Text = keyValue.ToString();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            string str = this.ActiveControl.Text.TrimEnd();
            int len = str.Length;
            if (len >= 1 && str.Substring(str.Length - 1) == "+")
            {
                this.ActiveControl.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zuhejian = textBox1.Text.Replace(" ","");

            string json2 = ReadConfig(@".\config.json");
            if (json2 == null) { return; }

            MyConfig c1 = JsonConvert.DeserializeObject<MyConfig>(json2);
     
            {
                MyConfig c = new MyConfig();
                c.path = c1.path;
                c.hotkey = zuhejian;
                string json1 = JsonConvert.SerializeObject(c);
                File.WriteAllText(@".\config.json", json1, Encoding.UTF8);
            }
            MessageBox.Show("配置成功,若开启了时间轴,请重开");
            this.Close();
        }

        public string ReadConfig(string path)
        {
            if (File.Exists(path))
            {
                String alltext = File.ReadAllText(path, System.Text.Encoding.UTF8);
                return alltext;
            }
            else
            {
                return null;
            }
        }
    }
}
