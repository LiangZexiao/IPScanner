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
using System.Collections.Specialized;


namespace IPScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
           
            InitializeComponent();
            textBox1.Enabled = false;
            comboBox1.Enabled = false;
        }

        private void 扫描IPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanIP();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FileOpr opr = new FileOpr(Application.StartupPath + "\\opcsvc.ini");
            StringBuilder result = new StringBuilder();
            if (comboBox1.SelectedItem != null)
            {
                string machine_name = comboBox1.SelectedItem.ToString();
                FileOpr.GetPrivateProfileString("DefaultHosts", machine_name, "", result, 1024, Application.StartupPath + "\\opcsvc.ini");
                MessageBox.Show(result.ToString());
            }
            else
            {
                MessageBox.Show("请先选择机器名");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0) 
            {
                MessageBox.Show("请先选择要添加的IP地址");
                return;
            }
            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                string machine_name = "Machine" + i.ToString();

                comboBox1.Enabled = true;
                comboBox1.Items.Add(machine_name);
                comboBox1.SelectedItem = comboBox1.Items[0];
                
                FileOpr.WritePrivateProfileString("DefaultHosts", machine_name, listBox1.SelectedItems[i].ToString(), Application.StartupPath + "\\opcsvc.ini");
            }
            MessageBox.Show("Successful");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /********************************************************************************/
        //扫描得本地IP
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

        //同时发送225个ping指令
        public void ScanIP()
        {
            for (int i = 1; i < 255; i++)
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
       
        //Ping指令异步接受到回应触发
        private void _myPing_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply.Status == IPStatus.Success)
            {
                listBox1.Items.Add(e.Reply.Address.ToString());
            }         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            INIClass inihelper = new INIClass(Application.StartupPath + "\\opcsvc.ini");
            //string[] address;
            List<string> result = new List<string>();
            if(inihelper.ExistINIFile())
            {
                result = inihelper.ReadSections(Application.StartupPath + "\\opcsvc.ini");
               
                foreach (string item in result)
                {
                    listBox2.Items.Add(item.ToString());
                }
            }
            else
            {
                MessageBox.Show("File dosen't exist");
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] result;
            INIClass inihelper = new INIClass(Application.StartupPath + "\\opcsvc.ini");
            if(inihelper.ExistINIFile())
            {
                result = inihelper.IniReadValues("DefaultHosts", null);
                string value = Encoding.Default.GetString(result);
                string[] list= value.Split('\0');
                foreach (string item in list)
                {
                    if(item!="")
                    {
                        string _value = inihelper.IniReadValue("DefaultHosts", item);
                        listBox2.Items.Add(item+":"+_value);
                    }
                }
                //MessageBox.Show(value);
            }
            
        }

    }
}
