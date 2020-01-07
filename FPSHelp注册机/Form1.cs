using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FPSHelp注册机
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SoftReg softReg = new SoftReg();
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                Clipboard.SetDataObject(textBox2.Text);
                MessageBox.Show("已复制到剪贴板！");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try

               {
                    string strHardware = this.textBox1.Text;
                    string strLicence = softReg.GetRNum(strHardware);
                    this.textBox2.Text = strLicence;
                }        
                catch 
               {
                      MessageBox.Show("输入的机器码格式错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               }
            }
    }
}
