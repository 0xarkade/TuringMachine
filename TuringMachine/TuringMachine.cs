using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace TuringMachine
{
    class TuringMachine
    {
        public char[] tape = new char[14] { ' ', ' ', ' ', ' ', '1', '1', '1', '0', '0', '1', ' ', ' ',  ' ', ' ' };
        char[] alphabet = new char[] { '1', '0', ' ' };

        protected DataGridView turing_states;
        protected List<Symbol> commands = new List<Symbol>();

        public TuringMachine(char[] alphabet, char[] tape, DataGridView data)
        {
            this.alphabet = alphabet;
            turing_states = data;
            this.tape = tape;
        }

        public char[] GetTape()
        {
            return tape;
        }


        public void LoadCommands()
          {
            foreach (DataGridViewRow row in turing_states.Rows)
            {
                Symbol cmd = new Symbol();
                cmd.symbol = row.Cells[0].Value.ToString().Replace("_", " ")[0];
                cmd.action = new List<Command>();


                for (int j = 1; j < row.Cells.Count; j++)
                {
                     string Q = turing_states.Columns[j].HeaderText.Replace("Q", "");
                     string action = "";

                     if (row.Cells[j].Value != null) action = row.Cells[j].Value.ToString().Replace("_", " ");

                    if (action != "")
                    {
                        if (!action.Contains('.'))
                        {
                            cmd.action.Add(new Command() { write = action[0], move = (Turing)action[1], to_state = action.Substring(2, action.Length - 2), Q = Q });
                        }
                        else
                        {
                            cmd.action.Add(new Command() { write = cmd.symbol, move = (Turing)action[0], to_state = action.Substring(1, action.Length - 1), Q = Q });
                        }
                    }

                }

                commands.Add(cmd);
            }
        }


        protected Command? FindAction(char chr, string Q)
        {
            foreach(Symbol cmd in commands)
            {
                if(cmd.symbol == chr)
                {
                    foreach(Command com in cmd.action)
                    {
                        if(com.Q == Q)
                        {
                            return com;
                        }
                    }
                }
            }

            return null;
        }

        public void StartMachine()
        {
            Machine m = new Machine();
            m.current_state = "1";  //Изначальное состояние Q1

            int i = 0;

            while(true && i <= tape.Length - 1)
            {
                try
                {
                    m.symbol = tape[i];
                    if (m.symbol == '\0') m.symbol = '0';

                    if (FindAction(m.symbol, m.current_state).HasValue)
                    {
                        Command c = (Command)FindAction(m.symbol, m.current_state);

                        if (c.move != Turing.NOP) tape[i] = c.write;

                        if (c.move == Turing.RIGHT) i++;
                        if (c.move == Turing.LEFT) i--;
                        if (c.move == Turing.END) break;


                        m.current_state = c.to_state;
                    }
                    else break;
                }
                catch { break; }
            }
        }



        protected class Machine
        {
            public char symbol;
            public string current_state;
        }

        protected struct Symbol
        {
            public char symbol;
            public List<Command> action;
        }

        protected struct Command
        {
            public string Q;
            public char write;
            public Turing move;
            public string to_state;
        }

        protected enum Turing
        {
            RIGHT = '>',
            LEFT  = '<',
            NOP   = '.',
            END   = '!',
            STAY = '@',
        }
    }
}
