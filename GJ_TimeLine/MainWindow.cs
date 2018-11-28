using GJ_TimeLine.Config;
using GJ_TimeLine.TimeLine;
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
    public partial class MainWIndow : Form
    {
       
        public MainWIndow()
        {
            InitializeComponent();
        }
        Dictionary<string, string> tlist = new Dictionary<string, string>();
        TimeLine_Core timelinecore;
        public string hotkey { get; set; }
        
        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", textBox1.Text);
        }

        private void MainWIndow_Load(object sender, EventArgs e)
        {
            string optionpath = settingInit();
            ListBoxInit(optionpath);
        }

        public string settingInit()
        {
            string optionpath = null;
            string path = Environment.CurrentDirectory;
            if (!File.Exists(@".\config.json"))
            {

                optionpath = path + "\\option";
                textBox1.Text = optionpath;

                MyConfig c = new MyConfig();
                c.path = optionpath;
                c.hotkey = "CTRL+F5";
                string json1 = JsonConvert.SerializeObject(c);
                File.WriteAllText(@".\config.json", json1, Encoding.UTF8);

            }
            else
            {
                string json2 = ReadConfig(@".\config.json");
                MyConfig c = JsonConvert.DeserializeObject<MyConfig>(json2);
                if (c.path == "")
                {
                    optionpath = path + "\\option";
                    textBox1.Text = optionpath;
                    hotkey = c.hotkey;
                }
                else
                {
                    optionpath = c.path;
                    textBox1.Text = optionpath;
                    hotkey = c.hotkey;
                }
            }
            return optionpath;
        }

        public void changeConfigPath(string path)
        {

            string json2 = ReadConfig(@".\config.json");
            if (json2 == null) { return; }

            MyConfig c1 = JsonConvert.DeserializeObject<MyConfig>(json2);
            if (c1.path != path)
            {
                MyConfig c = new MyConfig();
                c.path = path;
                string json1 = JsonConvert.SerializeObject(c);
                File.WriteAllText(@".\config.json", json1, Encoding.UTF8);
            }
        }

        public void ListBoxInit(string path)
        {
            tlist = loadTimeLineTxtList(path);

            listBox1.Items.Clear();
            if (tlist == null) { return; }
            foreach (var key in tlist)
            {
                listBox1.Items.Add(key.Key);
            }
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

        public Dictionary<string, string> loadTimeLineTxtList(string path)
        {
            if (!Directory.Exists(path)) {
                return null;
            }
            Dictionary<string, string> filelist = new Dictionary<string, string>();
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            System.IO.FileInfo[] files = dir.GetFiles(); // 获取所有文件信息。。
            //Console.WriteLine("{0} 该目录下的文件有: ", path);
            foreach (System.IO.FileInfo file in files)
            {
                if (file.Extension == ".txt")
                {
                    filelist.Add(file.Name, file.FullName);
                }
            }
            return filelist;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        
            changeConfigPath(textBox1.Text);
            string optionpath = settingInit();
            ListBoxInit(optionpath);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListBoxInit(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) {
                return;

            }
            timelinecore = new TimeLine_Core(this.hotkey);
            foreach (var key in tlist) {
                if (key.Key == listBox1.SelectedItem.ToString()) {
                    timelinecore.initTconfig(key.Value);
                }
            }
        }

        private void MainWIndow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timelinecore != null) { 
            timelinecore.exit();
            timelinecore = null;
            }
        }

        private void MainWIndow_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string json2 = ReadConfig(@".\config.json");
            MyConfig c = JsonConvert.DeserializeObject<MyConfig>(json2);
            HotKeyEdit hke = new HotKeyEdit(c.hotkey);
            hke.ShowDialog();
            settingInit();
        }
    }
}
