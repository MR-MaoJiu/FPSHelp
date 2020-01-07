using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FPSHelp注册机
{
    public partial class FPSServer : Form
    {
        public FPSServer()
        {
            InitializeComponent();       
        }
        static Socket server;
        static int port = 6000;
        static IPAddress ip = null;
        private void FPSServer_Load(object sender, EventArgs e)
        {

            string pcName = Dns.GetHostName();//得到本机名   

            //得到本机ip
            foreach (var item in Dns.GetHostAddresses(pcName))
            {
                if (item.AddressFamily.ToString() == "InterNetwork")
                {
                    ip = item;
                    break;
                }
            }
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(new IPEndPoint(ip,port ));//绑定端口号和IP              
            Thread t = new Thread(ReciveMsg);//开启接收消息线程  
            t.Start();

        }

        /// <summary>  
        /// 接收发送给本机ip对应端口号的数据报  
        /// </summary>  
       static  EndPoint point;
        static void ReciveMsg()
        {
            while (true)
            {
                point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号  
                byte[] buffer = new byte[1024];
                int length = server.ReceiveFrom(buffer, ref point);//接收数据报  
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                MessageBox .Show (point.ToString() + message);

            }
           
        }

        Form1 f = new FPSHelp注册机.Form1();
        private void button3_Click(object sender, EventArgs e)
        {
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            int i ;
            for(i=0;i < listBox1.Items.Count;i++)
            {
                EndPoint point = new IPEndPoint(IPAddress.Parse(listBox1.Items[i].ToString()), port);
                string msg = richTextBox1.Text;
                server.SendTo(Encoding.UTF8.GetBytes(msg), point);
            }
            richTextBox1.ForeColor = Color.Gray;

        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            richTextBox1.ForeColor = Color.Black;
        }

        private void FPSServer_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }
    }
}
