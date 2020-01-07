using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FPSHelp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static bool state = true;  //软件是否为可用状态
         SoftReg softReg = new SoftReg();
        private void button1_Click(object sender, EventArgs e)
        {
            try

               {       
                    if (textBox1.Text == softReg.GetRNum())
                        {             
                            MessageBox.Show("注册成功！重启软件后生效！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RegistryKey retkey = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("mySoftWare").CreateSubKey("Register.INI").CreateSubKey(textBox1.Text);
                            retkey.SetValue("UserName", "Rsoft");
                            this.Close();
                        }
                   else
                   {

                        MessageBox.Show("注册码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox1.SelectAll();
                    }
                }
                catch (Exception ex)
                {
                        throw new Exception(ex.Message);
                }
            }

        private void button3_Click(object sender, EventArgs e)
        {
            //复制机器码
            if (textBox2.Text != "")
            {
                Clipboard.SetDataObject(textBox2.Text);
                MessageBox.Show("已复制到剪贴板！");
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox2.Text = softReg.GetMNum();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

   
    }
}
