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
                }
                else
                {
                    optionpath = c.path;
                    textBox1.Text = optionpath;
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
            TimeLine_Core tc = new TimeLine_Core();
            foreach (var key in tlist) {
                if (key.Key == listBox1.SelectedItem.ToString()) { 
                    tc.initTconfig(key.Value);
                }
            }
        }

        private void MainWIndow_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
