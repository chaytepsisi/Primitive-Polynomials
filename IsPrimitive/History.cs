using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IsPrimitive
{
    public partial class History : Form
    {
        public History()
        {
            InitializeComponent();
        }

        public History(List<string> historyList)
        {
            if (historyList.Count > 0)
            {
                string str = String.Empty;
                for (int i = 0; i < historyList.Count; i++)
                    str += historyList[i];
                str += historyList.Count;
                MessageBox.Show(historyList.Count.ToString());

                //historyList.Reverse();

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("Operation", "Operation");
                for (int i = 0; i < historyList.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1[0, i].Value = historyList[historyList.Count-i-1].Replace("\n", " ");
                }
            }
            else
            {
                MessageBox.Show("No operation..");
            }
        }
    }
}
