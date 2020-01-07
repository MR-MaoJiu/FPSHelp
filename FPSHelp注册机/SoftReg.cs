using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSHelp注册机
{
    class SoftReg

    {

        public int[] intCode = new int[127];    //存储密钥

        public char[] charCode = new char[25];  //存储ASCII码

        public int[] intNumber = new int[25];   //存储ASCII码值



        //初始化密钥

        public void SetIntCode()

        {

            for (int i = 1; i < intCode.Length; i++)

            {

                intCode[i] = i % 9;

            }

        }



        ///<summary>

        /// 生成注册码

        ///</summary>

        ///<returns></returns>

        public string GetRNum(string strMNum)

        {

            SetIntCode();



            for (int i = 1; i < charCode.Length; i++)   //存储机器码

            {

                charCode[i] = Convert.ToChar(strMNum.Substring(i - 1, 1));

            }

            for (int j = 1; j < intNumber.Length; j++)  //改变ASCII码值

            {

                intNumber[j] = Convert.ToInt32(charCode[j]) + intCode[Convert.ToInt32(charCode[j])];

            }

            string strAsciiName = "";   //注册码

            for (int k = 1; k < intNumber.Length; k++)  //生成注册码

            {



                if ((intNumber[k] >= 48 && intNumber[k] <= 57) || (intNumber[k] >= 65 && intNumber[k]

                    <= 90) || (intNumber[k] >= 97 && intNumber[k] <= 122))  //判断如果在0-9、A-Z、a-z之间

                {

                    strAsciiName += Convert.ToChar(intNumber[k]).ToString();

                }

                else if (intNumber[k] > 122)  //判断如果大于z

                {

                    strAsciiName += Convert.ToChar(intNumber[k] - 10).ToString();

                }

                else

                {

                    strAsciiName += Convert.ToChar(intNumber[k] - 9).ToString();

                }

            }

            return strAsciiName;

        }

    }
}
