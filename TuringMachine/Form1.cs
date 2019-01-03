using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TuringMachine
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            char[] chars = alphabetBox.Text.ToArray();
            bool[] exist = new bool[chars.Length];

            foreach (DataGridViewRow itm in dataGridView1.Rows)
            {
                for(int i =0; i < chars.Length;i++)
                {
                    if (itm.Cells["char"].Value.ToString()[0] == chars[i])
                    {
                        exist[i] = true;
                    }
                }
            }

            for(int i =0; i < chars.Length;i++)
            {
                if(!exist[i])
                {
                    dataGridView1.Rows.Add(chars[i], "");
                }
            }
        }

        private void addNewQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = 0;

            DataGridViewColumn clmn = dataGridView1.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);


            if (int.TryParse(clmn.HeaderText.Replace("Q", ""), out index))
            {
                index++;


                DataGridViewColumn state = new DataGridViewColumn();
                state.HeaderText = "Q" + index.ToString();
                state.Name = "Q" + index.ToString();
                state.CellTemplate = new DataGridViewTextBoxCell();


                dataGridView1.Columns.Add(state);
            }

        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                DataGridViewSelectedRowCollection selected = dataGridView1.SelectedRows;

                foreach(DataGridViewRow row in selected)
                {
                    dataGridView1.Rows.Remove(row);
                }
            }
        }

        protected char[] GetTape()
        {
            List<TextBox> textboxes = new List<TextBox>()
                {
                     textBox2,
                     textBox3,
                     textBox5,
                     textBox4,
                     textBox9,
                     textBox8,
                     textBox7,
                     textBox6,
                     textBox17,
                     textBox16,
                     textBox15,
                     textBox14,
                     textBox13,
                     textBox12,
                     textBox11,
                     textBox10
                };

            StringBuilder chars = new StringBuilder();

            foreach (var textbox in textboxes)
            {
                chars.Append(textbox.Text.Replace("_", " "));
            }

            return chars.ToString().ToCharArray();

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            TextBox text = (TextBox)sender;

            char[] alphabet = alphabetBox.Text.ToCharArray();
            bool valid = false;
            for(int i =0;i < alphabet.Length;i++)
            {
                if (text.Text[0] == alphabet[i]) valid = true;
            }

            if (!valid) MessageBox.Show("Error, this symbol doesn't exist in alphabet.");

        }

        private void button6_Click(object sender, EventArgs e)
        {
            label2.Location = new Point(label2.Location.X + 38, label2.Location.Y);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label2.Location = new Point(label2.Location.X - 38, label2.Location.Y);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TuringMachine my_turing = new TuringMachine(alphabetBox.Text.Replace("_", " ").ToCharArray(), GetTape(), dataGridView1);

            my_turing.LoadCommands();
            my_turing.StartMachine();
          

            List<TextBox> textboxes = new List<TextBox>()
                {
                     textBox2,
                     textBox3,
                     textBox5,
                     textBox4,
                     textBox9,
                     textBox8,
                     textBox7,
                     textBox6,
                     textBox17,
                     textBox16,
                     textBox15,
                     textBox14,
                     textBox13,
                     textBox12,
                     textBox11,
                     textBox10
                };
            try
            {
                char[] tape = my_turing.GetTape();
                for (int i = 0; i < textboxes.Count; i++)
                {
                    textboxes[i].Text = tape[i].ToString().Replace(" ", "_");
                }
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter turingfile = new StreamWriter(saveFileDialog1.FileName);


                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        char symbol = row.Cells[0].Value.ToString()[0];
                        turingfile.Write(symbol + ";");

                        for (int j = 1; j < row.Cells.Count; j++)
                        {
                            string Q = dataGridView1.Columns[j].HeaderText.Replace("Q", "");
                            string action = "";
                            if (row.Cells[j].Value != null)
                            {
                                action = row.Cells[j].Value.ToString();
                            }

                            turingfile.Write(Q + "#" + action + "|");
                        }
                        turingfile.WriteLine("");


                    }

                    turingfile.Close();
                }
                catch {
                    MessageBox.Show("Error", "There was an error saving this file, please try again!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public bool alpha_charexist(char a)
        {
            foreach(char c in alphabetBox.Text)
            {
                if (c == a) return true;
            }

            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    dataGridView1.Rows.Clear();

                    using (var prog = new StreamReader(@openFileDialog1.FileName))
                    {
                        string line = prog.ReadLine();
                        line = line.Substring(0, line.Length - 1);


                        string[] states = line.Split(';')[1].Split('|');

                        bool[] used = new bool[states.Length];


                        if (dataGridView1.Columns.Count - 1 <= states.Length)
                        {
                            for (int i = 1; i < dataGridView1.Columns.Count; i++)
                            {
                                string Q = dataGridView1.Columns[i].HeaderText.Replace("Q", "");

                                if (Q.Equals(states[i - 1].Split('#')[0]))
                                {
                                    used[i - 1] = true;
                                }
                            }
                        }

                        for (int i = 0; i < used.Length; i++)
                        {
                            if (!used[i])
                            {
                                DataGridViewColumn clmn = dataGridView1.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);

                                DataGridViewColumn xstate = new DataGridViewColumn();
                                xstate.HeaderText = "Q" + states[i].Split('#')[0];
                                xstate.Name = "Q" + states[i].Split('#')[0];
                                xstate.CellTemplate = new DataGridViewTextBoxCell();

                                dataGridView1.Columns.Add(xstate);
                            }
                        }
                    }


                    StreamReader program = new StreamReader(@openFileDialog1.FileName);
                    string lline = null;
                    int index = 0;
                    List<string> chars = new List<string>();

                    while ((lline = program.ReadLine()) != null)
                    {
                        string chr = lline.Split(';')[0];
                        chars.Add(chr);

                        lline = lline.Substring(0, lline.Length - 1);
                        string[] states = lline.Split(';')[1].Split('|');

                        dataGridView1.Rows.Add(chr);

                        DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[index++];
                        for (int i = 0; i < states.Length; i++)
                        {
                            row.Cells[i + 1].Value = states[i].Split('#')[1];
                        }
                    }
                    foreach (string s in chars)
                    {
                        if (!alpha_charexist(s[0])) alphabetBox.Text += s;
                    }

                }
                catch (IOException)
                {
                    //TODO
                }
                catch {
                    MessageBox.Show("There was an error parsing this file, please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            List<TextBox> textboxes = new List<TextBox>()
                {
                     textBox2,
                     textBox3,
                     textBox5,
                     textBox4,
                     textBox9,
                     textBox8,
                     textBox7,
                     textBox6,
                     textBox17,
                     textBox16,
                     textBox15,
                     textBox14,
                     textBox13,
                     textBox12,
                     textBox11,
                     textBox10
                };

            foreach(TextBox b in textboxes)
            {
                b.Text = "_";
            }
        }
    }
}
