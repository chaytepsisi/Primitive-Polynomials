using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IsPrimitive
{
    
    public partial class Form1 : Form
    {
        [DllImport("PrimitivePolynomialDll.dll")]
        public static extern int ProcessRequest(int count, string[] argv);

        [DllImport("PrimitivePolynomialDll.dll")]
        public static extern int GeneratePolynomial(int count, string[] argv, byte[] polynomial);

        string[] arguments = null;
        int operationIndex = 0;
        string polynomial = null;
        //List<string> historyList;
        Stopwatch timer = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
            label2.Text = String.Empty;
            label3.Text = String.Empty;
            //historyList = new List<string>();
        }

        string IntToPoly(long polyInt)
        {
            List<long> katsayiList = new List<long>();
            while (polyInt != 0)
            {
                katsayiList.Add(polyInt & 0x1);
                polyInt /= 2;
            }

            string[] katsayiStr = new string[katsayiList.Count];
            for (int i = 0; i < katsayiList.Count; i++)
            {
                if (katsayiList[i] == 0x0)
                {
                    katsayiStr[i] = String.Empty;
                }
                else
                {
                    if (i == 0)
                    {
                        katsayiStr[i] = "1";
                    }
                    else if (i == 1)
                    {
                        katsayiStr[i] = "x";
                    }
                    else
                    {
                        katsayiStr[i] = "x" + i;
                    }
                }
            }

            katsayiStr = katsayiStr.Reverse().ToArray();
            string polyStr = String.Empty;

            for (int i = 0; i < katsayiStr.Length; i++)
            {
                polyStr += katsayiStr[i];
                if (i != katsayiStr.Length - 1)
                    polyStr += " + ";
            }
            return polyStr;
        }

        string ParsePolynomial(string inputPolynomial)
        {
            inputPolynomial = inputPolynomial.ToLower().Replace(" ", "");
            var foundIndexes = new List<int>();

            for (int i = inputPolynomial.IndexOf('x'); i > -1; i = inputPolynomial.IndexOf('x', i + 1))
            {
                foundIndexes.Add(i);
            }

            StringBuilder strB = new StringBuilder();
            char[] possibleChars = new char[]{'+','^' };
            for(int i = 0; i < inputPolynomial.Length; i++)
            {
                strB.Append(inputPolynomial[i]);
                if (foundIndexes.Contains(i)) {
                    if (i + 1 < inputPolynomial.Length)
                    {
                        if (!possibleChars.Contains(inputPolynomial[i + 1]))
                            strB.Append('^');
                    }
                }
            }
            return strB.ToString();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            
            operationIndex = 0;
            string field = "2";
            polynomial = ParsePolynomial(richTextBox2.Text);
            arguments = new string[] { "PrimitivePoly", "-t", polynomial + "," +field};
            backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            operationIndex = 1;
            string degreeStr = textBox1.Text;
            int degree = 0;
            try
            {
                degree = Int32.Parse(degreeStr);
            }
            catch
            {
                MessageBox.Show("Check the degree");
                return;
            }
            arguments = new string[] { "PrimitivePoly", "2", degreeStr };
            backgroundWorker1.RunWorkerAsync();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(null, null);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;

            timer.Start();
            if(operationIndex==0)
            {
                try
                {
                    int result = ProcessRequest(3, arguments);
                    if (result == -2)
                    {
                        richTextBox1.Text = "The polynomial IS primitive\n" + polynomial;
                   //     Invoke(new MethodInvoker(
                   //delegate { historyList.Add("The polynomial IS primitive\n" + polynomial); }
                   //));
                    }
                    else if (result == -1)
                    {
                        richTextBox1.Text = "The polynomial is NOT primitive\n" + polynomial;

                   //     Invoke(new MethodInvoker(
                   //delegate { historyList.Add("The polynomial IS NOT primitive\n" + polynomial); }
                   //));
                    }
                    else
                    {
                        richTextBox1.Text = "An error occured!!\n" + polynomial;
                        //historyList.Add("An error occured!!\n" + polynomial);
                   //     Invoke(new MethodInvoker(
                   //delegate { historyList.Add("An error occured!!\n" + polynomial); }
                   //));
                    }
                }
                catch (Exception ex)
                {
                    richTextBox1.Text = ex.Message;
                }
            }
            else if(operationIndex==1)
            {
                Stopwatch stp = new Stopwatch();
                try
                {
                    stp.Start();
                    byte[] resultArr = new byte[300]; ;
                    int result = GeneratePolynomial(3, arguments, resultArr);
                    stp.Stop();
                    if (result == 0)
                    {
                        string polyStr = System.Text.Encoding.ASCII.GetString(resultArr).Replace(" ", "");
                        polyStr = polyStr.Substring(0, polyStr.IndexOf(','));
                        richTextBox1.Text = "Degree "+arguments[2]+" polynomial ==> " +polyStr;
                   //     Invoke(new MethodInvoker(
                   //delegate { historyList.Add(arguments[2] + " " + polyStr); }
                   //));
                    }

                    else
                    {
                        richTextBox1.Text = "An error occured!!\n";
                   //     Invoke(new MethodInvoker(
                   //delegate { historyList.Add(arguments[2] + " " + "An error occured!!\n"); }
                   //));
                    }
                }
                catch (Exception ex)
                {
                    richTextBox1.Text = ex.Message;
                }
            }
            /*
            //x37+x5+x4+x3+x2+x+1
            label3.Text = String.Empty;
            timer.Start();
            long i = 0;
            string polinom = richTextBox1.Text.ToLower();
            string[] katsayiStrArr = polinom.Split('+');
            int[] powers = new int[katsayiStrArr.Length];

            for (i = 0; i < powers.Length; i++)
            {
                if (katsayiStrArr[i].Contains('x'))
                {
                    string powerString = katsayiStrArr[i].Replace("x", String.Empty);
                    if (powerString.Length > 0)
                        powers[i] = Int32.Parse(powerString);
                    else powers[i] = 1;
                }
                else powers[i] = 0;
            }

            long index = (long)Math.Pow(2, powers[0]);
            long equivalent = 0;
            for (i = 1; i < powers.Length; i++)
                equivalent += (long)Math.Pow(2, powers[i]);

            if (equivalent == 1 || equivalent == 0)
            {
                richTextBox1.Text = polinom + " GF(2^" + powers[0] + ")'da primitif DEĞİLdir.";
                richTextBox1.Text += "\nPolinom irreducible değildir, kökü GF(2)'dedir.";
                return;
            }

            long limit = (long)Math.Pow(2, powers[0]);

            long tempVal = equivalent;

            int prevPerc = 0;
            label2.Text = "% 0";
            for (i = powers[0] + 1; i < limit; i++)
            {
                if (prevPerc < (int)((100.0 / limit) * i))
                {
                    progressBar1.Value = (int)((100.0 / limit) * i);
                    prevPerc = (int)((100.0 / limit) * i);
                    label2.Text = "% " + prevPerc.ToString();
                    Point newLoc = new Point((progressBar1.Location.X + progressBar1.Width / 2) - label2.Width / 2, label2.Location.Y);
                    label2.Location = newLoc;
                }

                tempVal = (tempVal << 1);
                if (tempVal > index)
                    tempVal = (tempVal ^ index ^ equivalent);

                //richTextBox1.Text += "a"+i+" = "+IntToPoly(tempVal) + "\n";

                if (tempVal == 1)
                    break;
            }


            if (i == limit - 1)
                richTextBox1.Text = polinom + " GF(2^" + powers[0] + ")'da primitiftir.";
            else
            {
                richTextBox1.Text = polinom + " GF(2^" + powers[0] + ")'da primitif DEĞİLdir.";

                for (int j = 1; j <= powers[0]; j++)
                {
                    long fieldBorder = (long)Math.Pow(2, j) - 1;
                    if (i == fieldBorder)
                    {
                        richTextBox1.Text += "\nPolinomun kökü  GF(2^" + j + ")'dedir.";
                        break;
                    }

                    else if (i < fieldBorder)
                    {
                        richTextBox1.Text += "\nPolinomun kökü   GF(2^" + powers[0] + ")\\GF(2^" + (j - 1) + ")'dedir.";
                        break;
                    }
                }
            }*/
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer.Stop();
            richTextBox1.Text+="\n\nTime: " + (timer.ElapsedMilliseconds / 1000.0) + " sec.";
            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hakkında hakkinda = new Hakkında();
            hakkinda.ShowDialog();
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //History history = new History(historyList);
        }

        private void textBox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button2_Click(null, null);
        }
    }
}
