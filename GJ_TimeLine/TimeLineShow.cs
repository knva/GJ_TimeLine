
using GJ_TimeLine.Common;
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

namespace GJ_TimeLine
{
    public partial class TimeLineShow : Form
    {
        public TimelineConfig tc { get; set; }
    
        private Thread thread;
        private TimeLineThread tlt;
        public TimeLineShow()
        {
            InitializeComponent();
        }
        public TimeLineShow(TimelineConfig tconfig)
        {
            InitializeComponent();
            tc = tconfig;
          
        }

        private void TimeLineShow_Load(object sender, EventArgs e)
        {
            this.listView1.View = View.Details;
            this.listView1.Columns.Add("动作", 120, HorizontalAlignment.Left); //一步添加
            this.listView1.Columns.Add("时间", 120, HorizontalAlignment.Left); //一步添加
            if (tc == null)
            {
                MessageBox.Show("无法加载");
            }
            if (tc.Items.Count != 0)
            {
                this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度

                foreach (var item in tc.Items)
                {
                    ListViewItem lvi = new ListViewItem();
                    
                    lvi.Text = item.value;
                    lvi.SubItems.Add(convert2time((int)item.time));
                    this.listView1.Items.Add((lvi));

                }
                this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否退出?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (dr == DialogResult.OK)   //如果单击“是”按钮
            {
                stopThread();
                e.Cancel = false;                 //关闭窗体
            }
            else if (dr == DialogResult.Cancel)
            {
                e.Cancel = true;                  //不执行操作
            }
        }


              
    


        private void button1_Click(object sender, EventArgs e)
        {
            
            tlt = new TimeLineThread(this.tc);
            tlt.isStart = true;
            tlt.upact = new TimeLineThread.UpActDelegate(RefreshAct);
            tlt.uptime = new TimeLineThread.UpTimeDelegate(RefreshTime);
            thread = new Thread(new ThreadStart(tlt.ThreadFun));
            thread.Start();

        }
        private void stopThread() {
            if (tlt!=null){ 
            tlt.isStart = false;
                thread.Abort();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stopThread();
            selectItem(0);
            time_label.Text = "";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }




        public void selectItem(int nowact) {
            this.listView1.Select();
            foreach (ListViewItem item in this.listView1.Items) {
                item.BackColor = Color.Empty;
            }
        
            this.listView1.Items[nowact+1].BackColor = Color.Red;
            if (nowact + 1 > this.listView1.Items.Count)
            {
                this.listView1.EnsureVisible(nowact);//滚动到指定的行位置
            }
            else {
                this.listView1.EnsureVisible(nowact + 1);//滚动到指定的行位置
            }
            
        }

        private string convert2time(int duration) {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
            string str = "";
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + ": " + ts.Minutes.ToString() + ":" + ts.Seconds + "";
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString() + ":" + ts.Seconds + "";
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds + "";
            }
            return str;
        }

        private void RefreshTime(int nowtime) {

            if (this.time_label.InvokeRequired)
            {
                TimeLineThread mtlt = new TimeLineThread(tc);
                mtlt.uptime = new TimeLineThread.UpTimeDelegate(RefreshTime);
                this.Invoke(mtlt.uptime, new object[] { nowtime });
            }
            else {
                time_label.Text = convert2time(nowtime);
            }
        }
        private void RefreshAct(int nowact) {
            if (this.listView1.InvokeRequired)
            {
                TimeLineThread mtlt = new TimeLineThread(tc);
                mtlt.upact = new TimeLineThread.UpActDelegate(RefreshAct);
                this.Invoke(mtlt.upact, new object[] { nowact });
            }
            else
            {
                selectItem(nowact);
            }
        }

        private void TimeLineShow_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopThread();
        }
    }



}
