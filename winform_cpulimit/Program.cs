using System;
using System.Windows.Forms;
using System.DirectoryServices.AccountManagement;
using System.Net.NetworkInformation;

namespace winform_cpulimit
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            /*
            string hostName = ipGlobalProperties.HostName;
            string domainName = ipGlobalProperties.DomainName;
            if ("firadio.net" == domainName || "cn2027" == hostName)
            {
            }
            else
            {
                MessageBox.Show("启动失败，本程序为飞儿云专用工具\r\n\r\n请到飞儿云平台下载其它版本\r\n\r\n云平台网址www.firadio.net\r\n\r\n技术支持QQ:309385018", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Process.Start("http://www.firadio.net");
                return;
            }*/
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
