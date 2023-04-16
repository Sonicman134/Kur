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
    public partial class Form1 : Form
    {
        public Systeme Sys;
        int procNum;
        public Form1()
        {
            InitializeComponent();
            Sys = new Systeme();
            for(int i = 0; i< 20; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView2.Rows.Add();
            }
            UpdateTable();
            procNum = 1;
        }
        public void UpdateTable()
        {
            List<string> op = Sys.GetOP();
            List<string> vp = Sys.GetVP();
            for (int i = 0; i < 20; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = i;
                dataGridView1.Rows[i].Cells[1].Value = op[i];
                dataGridView2.Rows[i].Cells[0].Value = i;
                dataGridView2.Rows[i].Cells[1].Value = vp[i];
            }
            string[] str = new string[2];
            dataGridView4.Rows.Clear();
            List<Note> notes = Sys.GetNotes();
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].inOP)
                {
                    str[0] = Convert.ToString(notes[i].realNum);
                    str[1] = Convert.ToString(notes[i].R);
                    dataGridView4.Rows.Add(str);
                    if(Sys.GetClock() == i)
                    {
                        dataGridView4.Rows[dataGridView4.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                }
            }
        }

        private void NewProcButton_Click(object sender, EventArgs e)
        {
            Process pr = new Process();
            List<Note> OldVP = new List<Note>();
            List<Note> changedNote = new List<Note>();
            try
            {
                pr = Sys.NewProc(Convert.ToInt32(textBox1.Text), procNum, textBox2.Text);
                if(pr == null)
                {
                    MessageBox.Show("Нет памяти");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Введены не верные данные");
                return;
            }
            string[] str = new string[2];
            str[0] = Convert.ToString(procNum);
            str[1] = textBox2.Text;
            dataGridView3.Rows.Add(str);
            procNum++;
            UpdateTable();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            List<Process> proc = Sys.GetProcess();
            int id = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells[0].Value);
            for (int i = 0; i < proc.Count; i++)
            {
                if(id == proc[i].GetID())
                {
                    FormProc form = new FormProc(this, proc[i]);
                    form.ShowDialog();
                }
            }
        }

        private void DeleteProcButton_Click(object sender, EventArgs e)
        {
            try
            {
                Sys.EndProc(Convert.ToInt32(textBox3.Text));
            }
            catch
            {
                MessageBox.Show("Введены неверные данные");
                return;
            }
            for(int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                if(textBox3.Text == Convert.ToString(dataGridView3.Rows[i].Cells[0].Value))
                {
                    dataGridView3.Rows.RemoveAt(i);
                    break;
                }
            }
            UpdateTable();
        }
    }
    public class Process
    {
        private int id;
        private string Name;
        private BindingList<Note> Table;
        private Systeme sys;
        public Process()
        {

        }
        public Process(int i, string name, int n, Systeme s)
        {
            id = i;
            Name = name;
            sys = s;
            Table = sys.GiveSpace(n);
        }
        public void Input(string str, int virnum)
        {
            if(!Table[virnum].inOP)
            {
                Table[virnum].realNum = sys.LoadToOP(Table[virnum].realNum);
                Table[virnum].inOP = true;
            }
            sys.ChangeStr(Table[virnum].realNum, str);
        }
        public int GetID()
        {
            return id;
        }
        public string GetName()
        {
            return Name;
        }
        public BindingList<Note> GetTable()
        {
            return Table;
        }
    }
    public class Note
    {
        public int realNum { get; set; }
        public bool inOP { get; set; }
        public int R { get; set; }
        public Note()
        {

        }
        public Note(int num, bool f)
        {
            realNum = num;
            inOP = f;
            R = 1;
        }
    }
    public class Systeme
    {
        private List<string> OP;
        private List<string> VP;
        private int Clock;
        private List<Note> Notes;
        private List<Process> Proc;
        public List<string> GetOP()
        {
            return OP;
        }
        public List<string> GetVP()
        {
            return VP;
        }
        public int GetClock()
        {
            return Clock;
        }
        public List<Note> GetNotes()
        {
            return Notes;
        }
        public List<Process> GetProcess()
        {
            return Proc;
        }
        public Systeme()
        {
            OP = new List<string>();
            VP = new List<string>();
            Notes = new List<Note>();
            Proc = new List<Process>();
            for (int i = 0; i < 20; i++)
            {
                OP.Add(null);
                VP.Add(null);
            }
            Clock = 0;
        }
        public Process NewProc(int n, int id, string name)
        {
            Proc.Add(new Process(id, name, n, this));
            if(Proc[Proc.Count - 1].GetTable() == null)
            {
                Proc.RemoveAt(Proc.Count - 1);
                return null;
            }
            return Proc[Proc.Count - 1];
        }
        public void EndProc(int n)
        {
            BindingList<Note> tab;
            for (int i = 0; i < Proc.Count; i++)
            {
                if (Proc[i].GetID() == n)
                {
                    tab = Proc[i].GetTable();
                    for (int j = 0; j < tab.Count; j++)
                    {
                        if (tab[j].inOP)
                        {
                            OP[tab[j].realNum] = null;
                        }
                        else
                        {
                            VP[tab[j].realNum] = null;
                        }
                        for (int z = 0; z < Notes.Count; z++)
                        {
                            if (Notes[z] == tab[j])
                            {
                                Notes.RemoveAt(z);
                                if (z < Clock) Clock--;
                                z--;
                            }
                        }
                    }
                    Proc.RemoveAt(i);
                    return;
                }
            }
        }
        public BindingList<Note> GiveSpace(int n)
        {
            BindingList<Note> notes = new BindingList<Note>();
            int i = 0;
            int help;
            for(int j = 0; j < OP.Count && i < n; j++)
            {
                if (OP[j] == null)
                {
                    Notes.Add(new Note(j, true));
                    notes.Add(Notes[Notes.Count - 1]);
                    OP[Notes[Notes.Count - 1].realNum] = "";
                    i++;
                }
            }
            while(i < n)
            {
                help = LoadToVP();
                if(help == -1)
                {
                    return null;
                }
                Notes.Add(new Note(help, true));
                notes.Add(Notes[Notes.Count - 1]);
                OP[Notes[Notes.Count - 1].realNum] = "";
                i++;
            }
            return notes;
        }
        public Note ClockMove()
        {
            Note res = null;
            while (true)
            {
                if (Notes[Clock].inOP)
                {
                    if (res != null) return res;
                    else if(Notes[Clock].R == 1) Notes[Clock].R = 0;
                    else if(Notes[Clock].R == 0) res = Notes[Clock];
                }
                if (Clock == Notes.Count - 1)
                {
                    Clock = 0;
                }
                else Clock++;
            }
        }
        public int LoadToVP()
        {
            Note target = ClockMove();
            for (int i = 0; i < VP.Count; i++)
            {
                if (VP[i] == null)
                {
                    int save;
                    save = target.realNum;
                    VP[i] = OP[save];
                    target.realNum = i;
                    target.inOP = false;
                    target.R = 1;
                    return save;
                }
            }
            return -1;
        }
        public int LoadToOP(int num)
        {
            Note target = ClockMove();
            int OPNote = -1, VPNote = -1;
            string mid;
            int save;
            save = target.realNum;
            mid = OP[save];
            OP[save] = VP[num];
            VP[num] = mid;
            for(int i = 0; i < Notes.Count; i++)
            {
                if (Notes[i].realNum == num && !Notes[i].inOP) VPNote = i;
                if (Notes[i].realNum == target.realNum && Notes[i].inOP) OPNote = i;
            }
            target.realNum = num;
            target.inOP = false;
            target.R = 1;
            Note s;
            s = Notes[OPNote];
            Notes[OPNote] = Notes[VPNote];
            Notes[VPNote] = s;
            return save;
        }
        public void ChangeStr(int num, string str)
        {
            OP[num] = str;
        }
    }
}
