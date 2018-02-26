using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace winform_cpulimit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }
        public class Combobox2Item
        {
            public string Text { get; set; }
            public int interval_suspend { get; set; }
            public int interval_resume { get; set; }
            public override string ToString()
            {
                return Text;
            }
        }

        private void comboBox2_add(string text, int interval_suspend, int interval_resume)
        {
            Combobox2Item item = new Combobox2Item();
            item.Text = text;
            item.interval_suspend = interval_suspend;
            item.interval_resume = interval_resume;
            comboBox2.Items.Add(item);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            //this.Text = "CPU限速工具（飞儿云专用版）";
            //textBox1.Text = Properties.Settings.Default.keyword;
            comboBox1.Items.Clear();
            comboBox1.Items.Add(@"\笨笨熊管家.exe");
            comboBox1.Items.Add(@"\calc.exe");
            comboBox1.SelectedIndex = 0;
            comboBox2.Items.Clear();
            comboBox2_add("降速到 5%, 延长20倍使用时间", 300, 1);
            comboBox2_add("降速到10%, 延长10倍使用时间", 150, 1);
            comboBox2_add("降速到20%, 延长 4倍使用时间", 30, 1);
            comboBox2_add("降速到50%, 延长 1倍使用时间", 1, 1);
            comboBox2.SelectedIndex = 0;
            string msg = "欢迎使用[CPU降速工具]飞儿云专用版";
            msg += "\r\n\r\n本工具专门针对某些无法进行限速的挂机软件，";
            msg += "\r\n\r\n使用本工具可帮助您延长[CPU可用时间]";
            msg += "\r\n\r\n但有可能导致信息处理速度变慢";
            msg += "\r\n\r\n请根据您需要进行调整设置";
            msg += "\r\n\r\n技术支持阿盛QQ: 309385018";
            MessageBox.Show(msg, "欢迎使用[CPU降速工具]飞儿云专用版", MessageBoxButtons.OK);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            interval_resume = (comboBox2.SelectedItem as Combobox2Item).interval_resume;
            interval_suspend = (comboBox2.SelectedItem as Combobox2Item).interval_suspend;
        }
        private bool isRun = false;
        private int action_step = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            pid_list.Clear();
            search_process_by_keyword(comboBox1.Text);
            if (pid_list.Count == 0)
            {
                MessageBox.Show("您所要查找的关键字未找到！", "提示");
                button1.Enabled = true;
                return;
            }
            button2.Enabled = true;
            groupBox1.Enabled = false;
            isRun = true;
            process_suspend();
            action_step = 1;//这里的1表示间隔时间过后就process_resume
            timer1.Interval = interval_suspend;
            //MessageBox.Show(timer1.Interval.ToString());
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            groupBox1.Enabled = true;
            isRun = false;
            timer1.Stop();
            process_resume();
        }
        int interval_suspend;
        int interval_resume;
        private List<int> pid_list = new List<int>();
        static bool likestring(string fullstr, string searchstr)
        {
            return fullstr.ToLower().IndexOf(searchstr.ToLower()) != -1;
        }
        private void search_process_by_keyword(string keyword)
        {
            Process[] processes = Process.GetProcesses();
            Process selfProcess = Process.GetCurrentProcess();
            foreach (Process process in processes)
            {
                if (process.Id == 0 || process.Id == 4)
                {
                    //跳过无权设置的
                    continue;
                }
                if (process.SessionId == 0)
                {
                    //跳过系统会话
                    continue;
                }
                if (selfProcess.Id == process.Id)
                {
                    //不对自身程序限速
                    continue;
                }
                if (selfProcess.SessionId != process.SessionId)
                {
                    //跳过非自身会话的
                    continue;
                }
                string filename;
                try
                {
                    filename = process.MainModule.FileName;
                }
                catch (Exception ex)
                {
                    //跳过无权设置的
                    continue;
                }
                //textBox2.AppendText(filename);
                if (likestring(filename, keyword))
                {
                    pid_list.Add(process.Id);
                }

            }
        }
        private void process_suspend()
        {
            foreach (int pid in pid_list)
            {
                cpulimit.ProcessMgr.SuspendProcess(pid);
            }
        }
        private void process_resume()
        {
            foreach (int pid in pid_list)
            {
                cpulimit.ProcessMgr.ResumeProcess(pid);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            if (!isRun)
            {
                return;
            }
            if (action_step == 1)
            {
                process_resume();
                action_step = 2;//这里的1表示间隔时间过后就process_suspend
                timer1.Interval = interval_resume;
                timer1.Start();
                return;
            }
            if (action_step == 2)
            {
                process_suspend();
                action_step = 1;//这里的1表示间隔时间过后就process_resume
                timer1.Interval = interval_suspend;
                timer1.Start();
                return;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isRun)
            {
                e.Cancel = false;
                return;
            }
            DialogResult result = MessageBox.Show("你确定要关闭吗？\r\n\r\n关闭后CPU降速功能就失效了！\r\n\r\n技术支持阿盛QQ:309385018", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }
            isRun = false;
            timer1.Stop();
            process_resume();
            e.Cancel = false;
        }

    }
}
