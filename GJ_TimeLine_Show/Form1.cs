using GJ_TimeLine.TimeLine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GJ_TimeLine_Show
{
    public partial class Form1 : Form
    {
        public TimelineConfig tc { get; set; }
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(TimelineConfig tconfig)
        {
            InitializeComponent();

            tc = tconfig;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Thread fThread = new Thread(new ThreadStart(SleepT));//开辟一个新的线程
            fThread.Start();

        }

        private void SleepT()
        {
            for (int i = 0; i < 500; i++)
            {
                System.Threading.Thread.Sleep(100);//没什么意思，单纯的执行延时
                SetTextMessage(100 * i / 500);
            }
        }
        private delegate void SetPos(int ipos);

        private void SetTextMessage(int ipos)
        {
            if (this.InvokeRequired)
            {
                SetPos setpos = new SetPos(SetTextMessage);
                this.Invoke(setpos, new object[] { ipos });
            }
            else
            {
                this.label1.Text = ipos.ToString() + "/100";
                this.progressBar1.Value = Convert.ToInt32(ipos);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (tc == null)
            {
                MessageBox.Show("无法加载");
            }
            if (tc.Items.Count != 0)
            {

                foreach (var item in tc.Items)
                {

                    CCWin.SkinControl.ChatListItem cli = new CCWin.SkinControl.ChatListItem(item.value.ToString());
                    chatListBox1.Items.Add(cli);
                }
            }
        }
    }
}
