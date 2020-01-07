using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FPSHelp
{
    public partial class FPS : Form
    {
        public FPS()
        {
            InitializeComponent();
        }
        public static Socket socket = null;
        private byte[] result = new byte[1024 * 1024 * 2];
       static  int port = 6000;
        static Socket client;
       static  IPAddress ip = null;
        SoftReg softReg = new SoftReg();
        private void FPS_Load(object sender, EventArgs e)
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
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(ip,port ));
            Thread t = new Thread(sendMsg);
            t.Start();
            Thread t2 = new Thread(ReciveMsg);
            t2.Start();
            //获取当前活动进程的模块名称
            string moduleName = Process.GetCurrentProcess().MainModule.ModuleName;
            //返回指定路径字符串的文件名
            string processName = System.IO.Path.GetFileNameWithoutExtension(moduleName);
            //根据文件名创建进程资源数组
            Process[] processes = Process.GetProcessesByName(processName);
            //如果该数组长度大于1，说明多次运行
            if (processes.Length > 1)
            {
                MessageBox.Show("本程序一次只能运行一个实例！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);//弹出提示信息
                System.Environment.Exit(System.Environment.ExitCode);
                this.Dispose();
                this.Close();
            }
            timer2.Start();
            //判断软件是否注册
            timer1.Start();
            RegistryKey retkey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("mySoftWare").CreateSubKey("Register.INI");

            foreach (string strRNum in retkey.GetSubKeyNames())

            {

                if (strRNum == softReg.GetRNum())

                {
                    this.label1.ForeColor = Color.Green;
                    this.label1.Text = "此软件已注册！";
                    this.button1.Enabled = false;

                    return;

                }

            }
            this.label1.ForeColor = Color.Red;
            this.label1.Text = "此软件尚未注册！";

            this.button1.Enabled = true;

            MessageBox.Show("您现在使用的是试用版，可以免费试用5次！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);



            Int32 tLong;    //已使用次数

            try

            {

                tLong = (Int32)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\mySoftWare", "UseTimes", 0);

                MessageBox.Show("您已经使用了" + tLong + "次！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            catch

            {

                MessageBox.Show("欢迎使用本软件！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\mySoftWare", "UseTimes", 0, RegistryValueKind.DWord);

            }



            //判断是否可以继续试用

            tLong = (Int32)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\mySoftWare", "UseTimes", 0);

            if (tLong < 5)

            {

                int tTimes = tLong + 1;

                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\mySoftWare", "UseTimes", tTimes);

            }

            else

            {

                DialogResult result = MessageBox.Show("试用次数已到！请复制机器码联系猫九先森QQ：481869314，\n请问您是否需要注册？", "信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)

                {

                    Form1.state = false; //设置软件状态为不可用

                    button1_Click(sender, e);    //打开注册窗口

                }

                else

                {

                    Application.Exit();

                }

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 frmRegister = new Form1();
            frmRegister.ShowDialog();
        }
        #region 压枪dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int vKey);
        static int st = 0;

        const int MOUSEEVENTF_MOVE = 0x0001;
        #endregion
        #region 放大镜dll
        int bs = 2;//倍数
        [DllImport("user32.dll")]//取设备场景
        private static extern int GetPixel(IntPtr hdc, Point p);

        [DllImport("user32.dll", EntryPoint = "FindWindowA")]
        public static extern IntPtr FindWindowA(string lp1, string lp2);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern IntPtr ShowWindow(IntPtr hWnd, int _value);
        //重写API函数
        [DllImport("user32.dll", EntryPoint = "ShowCursor")]
        public extern static bool ShowCursor(bool bShow);
        #endregion
        #region 图像透明度
        //定义图像透明度调整函数
        public Bitmap PTransparentAdjust(Bitmap src, int num)
        {
            try
            {
                int w = src.Width;
                int h = src.Height;
                Bitmap dstBitmap = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Drawing.Imaging.BitmapData srcData = src.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Drawing.Imaging.BitmapData dstData = dstBitmap.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                unsafe
                {
                    byte* pIn = (byte*)srcData.Scan0.ToPointer();
                    byte* pOut = (byte*)dstData.Scan0.ToPointer();
                    byte* p;
                    int stride = srcData.Stride;
                    int r, g, b;
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            p = pIn;
                            b = pIn[0];
                            g = pIn[1];
                            r = pIn[2];
                            pOut[1] = (byte)g;
                            pOut[2] = (byte)r;
                            pOut[3] = (byte)num;
                            pOut[0] = (byte)b;
                            pIn += 4;
                            pOut += 4;
                        }
                        pIn += srcData.Stride - w * 4;
                        pOut += srcData.Stride - w * 4;
                    }
                    src.UnlockBits(srcData);
                    dstBitmap.UnlockBits(dstData);
                    return dstBitmap;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                return null;
            }
        }
        #endregion
        #region udp
        /// <summary>  
        /// 向特定ip的主机的端口发送数据报  
        /// </summary>  
        static void sendMsg()
        {
            EndPoint point = new IPEndPoint(IPAddress.Parse("47.93.9.113"), port );
            while (true)
            {
                string msg = ip.ToString ();
               // client.SendTo(Encoding.UTF8.GetBytes(msg), point);
            }
        }
        static  string message;
        /// <summary>  
        /// 接收发送给本机ip对应端口号的数据报  
        /// </summary>  
        static void ReciveMsg()
        {
            while (true)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号  
                byte[] buffer = new byte[1024];
                int length = client.ReceiveFrom(buffer, ref point);//接收数据报  
                message = Encoding.UTF8.GetString(buffer, 0, length);              
            }
        }
        #endregion
        public Bitmap Magnifier(Bitmap srcbitmap, int multiple)
        {
            try
            {
                if (multiple <= 0) { multiple = 0; return srcbitmap; }
                Bitmap bitmap = new Bitmap(srcbitmap.Size.Width * multiple, srcbitmap.Size.Height * multiple);
                BitmapData srcbitmapdata = srcbitmap.LockBits(new Rectangle(new Point(0, 0), srcbitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData bitmapdata = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                unsafe
                {
                    byte* srcbyte = (byte*)(srcbitmapdata.Scan0.ToPointer());
                    byte* sourcebyte = (byte*)(bitmapdata.Scan0.ToPointer());
                    for (int y = 0; y < bitmapdata.Height; y++)
                    {
                        for (int x = 0; x < bitmapdata.Width; x++)
                        {
                            long index = (x / multiple) * 4 + (y / multiple) * srcbitmapdata.Stride;
                            sourcebyte[0] = srcbyte[index];
                            sourcebyte[1] = srcbyte[index + 1];
                            sourcebyte[2] = srcbyte[index + 2];
                            sourcebyte[3] = srcbyte[index + 3];
                            sourcebyte += 4;
                        }
                    }
                }
                srcbitmap.UnlockBits(srcbitmapdata);
                bitmap.UnlockBits(bitmapdata);
                return bitmap;
            }
            catch { this.Close(); return null; }
        }   
        private void FPS_FormClosed(object sender, FormClosedEventArgs e)
        {
            //显示任务栏
            FPS.ShowWindow(hTray, 5);
            ShowCursor(true);//鼠标显示
            timer1.Stop();
            this.Dispose();
            this.Close();
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState(1) != 1 && GetAsyncKeyState(1) != 0)
            {
                mouse_event(MOUSEEVENTF_MOVE, 0, st, 0, 0);
                
            }
        }


     
        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            timer1.Stop();
        }
        Graphics g;
        const int imgWidth = 240;//放大后图片的宽度
        const int imgHeight = 445;//放大后图片的高度
        Bitmap image;
        Size size = new Size(imgWidth, imgHeight);
        private void timer2_Tick(object sender, EventArgs e)
        {
            label6.Text = message;
            label6.Left = label6.Left + 1;
            if (this.label6.Left>this.Width )

            {

                this.label6.Left  = 0 ;

            }
            //if (GetAsyncKeyState(2) != 1 && GetAsyncKeyState(2) != 0)
            //{
            //    pictureBox1_Click(sender, e);

            //}
            
            image = new Bitmap(size.Width, size.Height);
            g = Graphics.FromImage(image);
            g.CopyFromScreen(new Point(Screen.GetWorkingArea(this).Width/2 - (imgWidth / 4), Screen.GetWorkingArea(this).Height/2 - (imgHeight / 20)), new Point(0, 0), size);
            g.Dispose();
            pictureBox1.Image = Magnifier(image, bs);
            image.Dispose();
            GC.Collect();
           
        }

        //获取任务栏
        IntPtr hTray = FPS.FindWindowA("Shell_TrayWnd", String.Empty);
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (TopMost == false)
            {
                TopMost = true;
                //隐藏任务栏
                FPS.ShowWindow(hTray, 0);
                //隐藏除图片框外所有空间
                panel1.Visible = false;
                ShowCursor(false);//鼠标隐藏              
                FormBorderStyle = FormBorderStyle.None;
                TransparencyKey = BackColor;
               // this.Opacity = 0.70;
            }
            else
            {
                //显示任务栏
                FPS.ShowWindow(hTray, 5);
                //显示除图片框外所有空间
                panel1.Visible = true;
                ShowCursor(true);//鼠标显示
                TopMost = false;
                FormBorderStyle = FormBorderStyle.FixedSingle;
                TransparencyKey = Color.Wheat;
                //this.Opacity = 1;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            st = trackBar1.Value;
            label2.Text = st.ToString ();
        }

        private void trackBar1_MouseMove(object sender, MouseEventArgs e)
        {
            timer1.Stop();
         
        }

        private void trackBar1_MouseLeave(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
