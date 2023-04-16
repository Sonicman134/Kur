using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kur
{
    public partial class FormProc : Form
    {
        Form1 mainform;
        Process procc;
        public FormProc()
        {
            InitializeComponent();
        }
        public FormProc(Form1 form, Process pro)
        {
            InitializeComponent();
            NameLabel.Text = pro.GetName();
            IDLabel.Text = Convert.ToString(pro.GetID());
            mainform = form;
            procc = pro;
            UpdateTable();
        }
        public void UpdateTable()
        {
            dataGridView1.Rows.Clear();
            string[] str = new string[4];
            BindingList<Note> tab = procc.GetTable();
            for (int i = 0; i < tab.Count; i++)
            {
                str[0] = Convert.ToString(i);
                str[1] = Convert.ToString(tab[i].realNum);
                if (tab[i].inOP) str[2] = "Да";
                else str[2] = "Нет";
                str[3] = Convert.ToString(tab[i].R);
                dataGridView1.Rows.Add(str);
            }
        }
        private void ChangeButton_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(textBox1.Text);
                int vpid = -1;
                if (!procc.GetTable()[id].inOP)
                {
                    vpid = procc.GetTable()[id].realNum;
                }
                procc.Input(textBox2.Text, id);
                mainform.UpdateTable();
                UpdateTable();
            }
            catch
            {
                MessageBox.Show("Введены неверные данные");
            }
        }
    }
}
