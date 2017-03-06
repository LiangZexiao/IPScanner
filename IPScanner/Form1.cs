using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

using System.Net;
using System.Net.NetworkInformation;


namespace IPScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
           
            InitializeComponent();
            textBox1.Enabled = false;
        }

        private void 扫描IPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanIP();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FileOpr opr = new FileOpr(Application.StartupPath + "\\opcsvc.ini");
            StringBuilder result = new StringBuilder();
            FileOpr.GetPrivateProfileString("DefaultHosts", "Machine01", "", result, 1024, Application.StartupPath + "\\opcsvc.ini");

            MessageBox.Show(result.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                string machine_name = "Machine" + i.ToString();
                FileOpr.WritePrivateProfileString("DefaultHosts", machine_name, listBox1.SelectedItems[i].ToString(), Application.StartupPath + "\\opcsvc.ini");
            }
            MessageBox.Show("Successful");
        }
        /********************************************************************************/
        public IPAddress GetLocalIP()
        {
            foreach (IPAddress _ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_ip.AddressFamily.ToString() == "InterNetwork")
                {
                    return _ip;
                }
            }
            return IPAddress.None;
        }

        public void ScanIP()
        {
            for (int i = 1; i < 225; i++)
            {
                Ping myPing;
                myPing = new Ping();
                myPing.PingCompleted += new PingCompletedEventHandler(_myPing_PingCompleted);

                string LocalIP = GetLocalIP().ToString();
                textBox1.Text = LocalIP;

                string pingIP = StringOpr.GetNetSegment(LocalIP) + i.ToString();

                myPing.SendAsync(pingIP, 1000, null);
            }
        }
       
        private void _myPing_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply.Status == IPStatus.Success)
            {
                listBox1.Items.Add(e.Reply.Address.ToString());
            }         
        }






    }
}
