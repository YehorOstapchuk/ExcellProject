using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace Excel
{



    public partial class Form1 : Form
    {
        Parser Parse = new Parser();
        int CurrentRow = 0;
        int CurrentColumn = 0;
        public Form1()
        {
            InitializeComponent();
            InitializeGrid(15, 20);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Focus();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            CurrentColumn = dataGridView1.CurrentCell.ColumnIndex;
            CurrentRow = dataGridView1.CurrentCell.RowIndex;
           // label1.Text = Data.GetData(CurrentRow, CurrentColumn).value_get() + "name: " + Data.GetData(CurrentRow, CurrentColumn).name_get()
             //   + "row+col " + LetterNumberConverter.NameToRaw(Data.GetData(CurrentRow, CurrentColumn).name_get()).ToString() + " " + LetterNumberConverter.NameToColumn(Data.GetData(CurrentRow, CurrentColumn).name_get()).ToString();
            textBox1.Text = Data.GetData(CurrentRow, CurrentColumn).expression_get();
            textBox1.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CurrentColumn = dataGridView1.CurrentCell.ColumnIndex;
            CurrentRow = dataGridView1.CurrentCell.RowIndex;
            textBox1.Text = Data.GetData(CurrentRow, CurrentColumn).expression_get();
            textBox1.Focus();
        }

        private void InitializeGrid(int rows, int columns)
        {

            try
            {
                dataGridView1.ColumnCount = columns;
                dataGridView1.ColumnHeadersVisible = true;

                DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
                columnHeaderStyle.BackColor = Color.Beige;
                columnHeaderStyle.Font = new Font("Times new roman", 12, FontStyle.Bold);

                dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;



                for (int i = 0; i < columns; i++)
                {

                    dataGridView1.Columns[i].Name = LetterNumberConverter.NumberToLetter(i);
                }


                dataGridView1.RowCount = rows;
                dataGridView1.RowHeadersVisible = true;

                DataGridViewCellStyle rowHeaderStyle = new DataGridViewCellStyle();
                rowHeaderStyle.BackColor = Color.Beige;
                rowHeaderStyle.Font = new Font("Times new roman", 12, FontStyle.Bold);

                dataGridView1.ColumnHeadersDefaultCellStyle = rowHeaderStyle;

                for (int i = 0; i < rows; i++)
                {
                    dataGridView1.Rows[i].HeaderCell.Value = (i).ToString();
                }

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        DataCell cell = new DataCell();
                        cell.name_set(dataGridView1.Columns[j].Name + dataGridView1.Rows[i].HeaderCell.Value);
                        cell.column = j;
                        cell.row = i;
                        Data.DataAdd(cell);
                    }

                }
                Data.columns = columns;
                Data.rows = rows;
            }
            catch (Exception) { }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Processing(int column, int raw, string expression)
        {
            try
            {
                DataCell temp = Data.GetData(raw, column);
                temp.expression_set(expression);
                if ((expression == "") || (expression == null)) { temp.expression_set(null); temp.value_set(null); temp.related = null; temp.related = new List<string>(); return; }
                string temp2 = Preparing(expression, column, raw);

                if (temp2 != "Error")
                {

                    temp.value_set(Parse.Parse(temp2));
                }
                else temp.value_set(temp2);
            }
            catch (Exception) { }
        }

        private string Preparing(string expression, int column, int row)
        {
            try
            {
                DataCell temp_ = Data.GetData(row, column);
                string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string operations = "+*/^incdecmoddiv-()";
                // string numbers = "0123456789";

                expression = NoSpaces(expression);
                List<string> temp3 = new List<string>();
                string expression2 = "+" + expression + "+";
                for (int i = 0; i < expression2.Length; i++)
                {

                    if (alphabet.Contains(expression2[i]))
                    {
                        string temp = "";
                        for (; ((!operations.Contains(expression2[i]))); i++)
                        {
                            temp += expression2[i];
                        }
                        if (!temp3.Contains(temp))
                        {
                            temp3.Add(temp);
                        }

                    }
                }


                temp_.related = temp3;


              //  if (MyCircle(row, column)) return "Error"; V2
                if (temp_.IsCycled()) return "Error";

                for (int i = 0; i < temp_.related.Count; i++)
                {
                    if ((Data.GetData(LetterNumberConverter.NameToRaw(temp_.related[i]), LetterNumberConverter.NameToColumn(temp_.related[i]))) == null) return "Error";
                    string m;
                    if (Data.GetData(LetterNumberConverter.NameToRaw(temp_.related[i]), LetterNumberConverter.NameToColumn(temp_.related[i])).value_get() == null)
                    {
                        m = "0";
                    }
                    else if (Data.GetData(LetterNumberConverter.NameToRaw(temp_.related[i]), LetterNumberConverter.NameToColumn(temp_.related[i])).value_get() == "Error") return "Error";
                    else m = Data.GetData(LetterNumberConverter.NameToRaw(temp_.related[i]), LetterNumberConverter.NameToColumn(temp_.related[i])).value_get();

                    expression = expression.Replace(temp_.related[i], m);
                }

                return expression;
            }
            catch (Exception) { return "Error"; }
        }

        public override void Refresh()
        {
            for (int i = ((Data.columns * Data.rows) - 1); i >= 0; i--)
            {
                if ((Data.Table[i].expression_get() != null) || (dataGridView1.Rows[Data.Table[i].row].Cells[Data.Table[i].column].Value != null))
                {
                    Processing(Data.Table[i].column, Data.Table[i].row, Data.Table[i].expression_get());
                    dataGridView1.Rows[Data.Table[i].row].Cells[Data.Table[i].column].Value = Data.Table[i].value_get();
                }
            } 

            for (int i = 0; i < (Data.columns * Data.rows); i++)
            {
                if ((Data.Table[i].expression_get() != null) || (dataGridView1.Rows[Data.Table[i].row].Cells[Data.Table[i].column].Value != null))
                {
                    Processing(Data.Table[i].column, Data.Table[i].row, Data.Table[i].expression_get());
                    dataGridView1.Rows[Data.Table[i].row].Cells[Data.Table[i].column].Value = Data.Table[i].value_get();
                }
            }

            for (int i = (Data.columns * Data.rows - 1); i >= 0; i--)
            {
                if (Data.Table[i].expression_get() != null)
                {
                    Processing(Data.Table[i].column, Data.Table[i].row, Data.Table[i].expression_get());
                    dataGridView1.Rows[Data.Table[i].row].Cells[Data.Table[i].column].Value = Data.Table[i].value_get();
                }
            } 


        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == 13)
                {
                    Processing(CurrentColumn, CurrentRow, textBox1.Text);
                    Refresh();

                }
            }
            catch(Exception) { MessageBox.Show("Error"); }
        }

        public string NoSpaces(string s)
        {
            string temp = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ') continue;
                temp += s[i];
            }
            return temp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //addrow

        }



        public int AddRow(int _c, DataGridView dgv)
        {
            _c++;

            DataGridViewRow row = (DataGridViewRow)dgv.Rows[0].Clone();
            row.HeaderCell.Value = (_c - 1).ToString();
            dgv.Rows.Add(row);
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                DataCell cell = new DataCell();
                cell.name_set(dataGridView1.Columns[i].Name + dataGridView1.Rows[_c - 1].HeaderCell.Value);
                cell.column = i;
                cell.row = _c - 1;
                Data.DataAdd(cell);

            }
            Data.rows++;
            return 0;
        }




        public int AddColumn(int _c, DataGridView dgv)
        {
            _c++;
            DataGridViewColumn column = (DataGridViewColumn)dgv.Columns[0].Clone();
            column.HeaderCell.Value = LetterNumberConverter.NumberToLetter(_c - 1);
            dgv.Columns.Add(column);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                DataCell cell = new DataCell();
                cell.name_set(dataGridView1.Columns[_c - 1].Name + dataGridView1.Rows[i].HeaderCell.Value);
                cell.column = _c - 1;
                cell.row = i;
                Data.DataAdd(cell);

            }
            Data.columns++;
            return 0;
        }





        private void button1_Click_1(object sender, EventArgs e)
        {
            // if (DialogResult.Yes == MessageBox.Show("Are you sure you want to exit?", "Confirm that you really want to exit", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            try
            {



                AddRow(dataGridView1.RowCount, dataGridView1);
                Refresh();
            } catch(Exception ) { MessageBox.Show("Error"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // if (DialogResult.Yes == MessageBox.Show("Are you sure you want to exit?", "Confirm that you really want to exit", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
           try {
                AddColumn(dataGridView1.ColumnCount, dataGridView1);
                Refresh();
            }catch (Exception) { MessageBox.Show("Error"); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this row? Cells that reference this row will recieve an Error value.", "Delete Row", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    if (dataGridView1.Rows.Count <= 2)
                    {
                        MessageBox.Show("You cannot delete more.");
                        return;
                    }
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        Data.Table.RemoveAt(Data.GetDataIndex(dataGridView1.Rows.Count - 1, i));
                    }
                    dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
                    Data.rows--;
                    //dataGridView1.Rows[ROWS].HeaderCell.Value = (ROWS).ToString();
                    // label1.Text = ((char)(dataGridView1.CurrentCell.ColumnIndex + 65)).ToString() + (dataGridView1.CurrentCell.RowIndex).ToString() + ":";
                    Refresh();
                }
            }
            catch (Exception) { MessageBox.Show("Error"); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this column? Cells that reference this column will recieve an Error value.", "Delete column", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    if (dataGridView1.Columns.Count <= 2)
                    {
                        MessageBox.Show("You cannot delete more.");
                        return;
                    }
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        Data.Table.RemoveAt(Data.GetDataIndex(i, dataGridView1.Columns.Count - 1));
                    }
                    dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
                    Data.columns--;
                    //dataGridView1.Rows[ROWS].HeaderCell.Value = (ROWS).ToString();
                    // label1.Text = ((char)(dataGridView1.CurrentCell.RowIndex + 65)).ToString() + (dataGridView1.CurrentCell.ColumnIndex).ToString() + ":";
                    Refresh();
                }
            }
            catch (Exception) { MessageBox.Show("Error"); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Для корректної роботи записувати inc,dec в дужках при використанні з іншими операторами.");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to exit?", "Confirm that you really want to exit", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {

            }
            else e.Cancel = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }




        private void Save()
        {


        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Stream mystream;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    if (((mystream = saveFileDialog1.OpenFile()) != null))
                    {
                        StreamWriter sw = new StreamWriter(mystream);
                        sw.WriteLine(dataGridView1.RowCount);
                        sw.WriteLine(dataGridView1.ColumnCount);
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            for (int j = 0; j < dataGridView1.ColumnCount; j++)
                            {
                                if (dataGridView1.Rows[i].Cells[j].Value != null)
                                    sw.WriteLine(dataGridView1.Rows[i].Cells[j].Value.ToString());
                                else sw.WriteLine("");


                            }
                        }


                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            for (int j = 0; j < dataGridView1.ColumnCount; j++)
                            {
                                string cell_name = LetterNumberConverter.NumberToLetter(j + 1) + (i + 1).ToString();
                                if (Data.GetData(i, j).expression_get() != null)
                                {
                                    sw.WriteLine(Data.GetData(i, j).expression_get());
                                }
                                else sw.WriteLine("");


                            }
                        }
                        sw.Close();
                        mystream.Close();

                    }
                }
            } catch(Exception) { MessageBox.Show("Saving Error"); } 
        }

        private void saveFileDialog1_FileOk_1(object sender, CancelEventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Stream mystr = null;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    if (((mystr = openFileDialog1.OpenFile()) != null))
                    {
                        using (mystr)
                        {
                            StreamReader sr = new StreamReader(mystr);
                            string scr = sr.ReadLine();
                            string scc = sr.ReadLine();
                            int cr = Convert.ToInt32(scr);
                            int cc = Convert.ToInt32(scc);
                            Data.Table = null;
                            Data.Table = new List<DataCell>();
                            InitializeGrid(cr, cc);
                            for (int i = 0; i < cr; i++)
                            {
                                for (int j = 0; j < cc; j++)
                                {
                                    dataGridView1.Rows[i].Cells[j].Value = null;
                                    Data.GetData(i, j).value_set(null);
                                    string temp = sr.ReadLine();
                                    if (temp != "")
                                    {
                                        dataGridView1.Rows[i].Cells[j].Value = temp;
                                        Data.GetData(i, j).value_set((string)dataGridView1.Rows[i].Cells[j].Value);
                                    }

                                }
                            }


                            for (int i = 0; i < cr; i++)
                            {
                                for (int j = 0; j < cc; j++)
                                {
                                    string temp = sr.ReadLine();
                                    if (temp != "")
                                    {
                                        Data.GetData(i, j).expression_set(temp);
                                    }


                                }
                            }

                            Refresh();
                            sr.Close();
                            mystr.Close();
                        }
                    }
                }
            } catch(Exception) { MessageBox.Show("Loading Error"); }
        }

        public bool MyCircle(int row, int column)  //!!!
        {
            DataCell cell = Data.GetData(row, column);
            List<string> depList = new List<string>();
            depList.Add(cell.name_get());
            string searchableCell = cell.name_get();
            string currentCell = searchableCell;
            bool withoutCircle = true;
            CircleRecursivHelper(cell);
            DataCell tempCell;

            void CircleRecursivHelper(DataCell cell1)
            {
                if (!withoutCircle) return;
               
                tempCell = Data.GetData(LetterNumberConverter.NameToRaw(currentCell), LetterNumberConverter.NameToColumn(currentCell));
                while (tempCell.related.Count != 0)
                {
                    if (!withoutCircle) return;
                    foreach (var depend in tempCell.related)
                    {
                        currentCell = depend;

                        if (currentCell == searchableCell)
                        {
                            withoutCircle = false;
                            //MessageBox.Show("Circle");
                            return;
                        }
                        depList.Add(currentCell);
                        if (withoutCircle == false) return;
                        CircleRecursivHelper(Data.GetData(LetterNumberConverter.NameToRaw(currentCell), LetterNumberConverter.NameToColumn(currentCell)));
                        if (withoutCircle == false) return;
                        depList.Remove(currentCell);
                    }
                    if (withoutCircle == false) return;
                }
            }
            
            if (!withoutCircle)
            {
                Data.GetData(LetterNumberConverter.NameToRaw(searchableCell), LetterNumberConverter.NameToColumn(searchableCell)).value_set("Error");
               // dataGridView1.Rows[int.Parse(searchableCell[1].ToString())].Cells[int.Parse((searchableCell[0] - 65).ToString())].Value = "#CIRCLE";
                for (int i = 0; i < depList.Count; i++)
                {

                    Data.GetData(LetterNumberConverter.NameToRaw(depList[i]), LetterNumberConverter.NameToColumn(depList[i])).value_set("Error");
                }
                return true;
            }
            else return false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Ostapchuk Yehor K-27" +
                " Operations:" +
                "inc|dec|" +
                "mod|div|" +
                "+|-|" +
                "/|*|" +
                "^");
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Якщо ви завантажете інщий файл - поточний не збережеться. Рекумендуємо зберегти поточні дані.");
        }
    }






    static class LetterNumberConverter
    {
         private static string[] alphabet = {"A", "B", "C", "D", "E", "F", "G",
            "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S",
             "T", "U", "V", "W", "X", "Y", "Z"};

        //  public static string  alphabet = "ABCDEFGHIJKLMNOPQRSTVWXYZ";

        public static string NumberToLetter(int i)
        {
            string temp = "";
            for (int j = 0; j < ((i / 26) + 1); j++)
            {
                temp += alphabet[(i % 26)];
            }
            return temp;
        }

        public static int LetterToNumber(string s)
        {
            string alphabet2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
           int number = 0; 
            for (int i = 0; i < s.Length; i++)
            {
                if (i >= 1) number += 26;
                int j = 0;
                while (true)
                {
               
                    if (s[i] == alphabet2[j]) { number += j;  break; }
                    j++;
                }
            }
            return number;
        }

        public static int NameToColumn(string s)
        {
            string temp = "";
            string alphabet2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < s.Length; i++)
            {
                if (alphabet2.Contains(s[i])) temp += s[i];
            }
           int m = LetterNumberConverter.LetterToNumber(temp);
            return m;
        }


        public static int NameToRaw(string s)
        {
           // Console.WriteLine(s);
            string temp = "";
            string alphabet2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ+*/^incdecmoddiv-()";
            for (int i = 0; i < s.Length; i++)
            {
                if (!alphabet2.Contains(s[i])) temp += s[i];
            }
           //Console.WriteLine(temp.ToString());
            return int.Parse(temp);
        }

    }



    static class strings
    {
        public static List<string> string_list = new List<string>();


        public static void Remove()
        {
            for (int i = 0; i < string_list.Count; i++)
            {
                string_list.RemoveAt(i);
            }
        }

        public static void Add (string s)
        {
            string_list.Add(s);
        }
    }

 


    class DataCell : DataGridViewTextBoxCell
    {
        private string value=null;
        private string expression=null;
        private string name=null;
        public int column;
        public int row;
        public List<string> related = new List<string>();

        public void name_set (string name)
        {
            this.name = name;
        }

        public void expression_set(string expression)
        {
            this.expression = expression;
        }

        public void value_set(string value)
        {
            this.value = value;
        }

        public string name_get()
        {
            return this.name;
        }

        public string expression_get()
        {
            return this.expression;
        }

        public string value_get()
        {
            return this.value;
        }




        public bool IsCycled()
        {
            try
            {
                List<string> temp = new List<string>();
                temp.Add(name);

                int i = 0;
                bool flag = false;
                if (related.Count == 1)
                {
                    if (name == related[0]) { value = "Error"; return true; }
                }
                while (true)
                {
                    int m = temp.Count;
                    if (i == (m)) break;

                    DataCell tempr = Data.GetData(LetterNumberConverter.NameToRaw(temp[i]), LetterNumberConverter.NameToColumn(temp[i]));
                    for (int j = 0; j < tempr.related.Count; j++)
                    {
                        temp.Add(tempr.related[j]);
                    }



                    for (int j = 0; j < temp.Count; j++)
                    {
                        for (int h = j + 1; h < temp.Count; h++)
                        {
                            if (temp[j] == temp[h]) { flag = true; break; }
                        }
                        if (flag == true) break;
                    }
                    if (flag == true) break;
                    i++;
                }


                string k = "";
                for (int j = 0; j < temp.Count; j++)
                {
                    k += temp[j];  //.ToString() + " ";
                }
                Console.WriteLine(k);




                if (flag == false) return false;

                for (int j = 0; j < temp.Count; j++)
                {
                    DataCell tempr = Data.GetData(LetterNumberConverter.NameToRaw(temp[j]), LetterNumberConverter.NameToColumn(temp[j]));
                    tempr.value_set("Error");
                }
                return true;
            }
            catch (Exception) { return true; }
        }

    }

    static class Data
    {
        public static List<DataCell> Table = new List<DataCell>();
       // private static int amount;
        public static int columns;
        public static int rows;

        public static void DataAdd(DataCell cell)
        {
            Table.Add(cell);
        }




        public static int GetDataIndex(int row, int column)
        {
            int i = 0;
            for (; i < Table.Count; i++)
            {

                if ((Table[i].column == column) && (Table[i].row == row)) return i;

            }
            return -1;
        }


        public static DataCell GetData(int row, int column)
        {
            for (int i = 0; i < Table.Count; i++)
            {

                if ((Table[i].column == column) && (Table[i].row == row)) return Table[i];

            }
            return null;
        }
    }

        class Number
        {
            public double value;
            public int first_index;
            public int second_index;
        }

        class Operation
        {
            public string value;
            public int priority;
            public int first_index;
            public int second_index;
        }


       public class Parser
        {
            string operations = "+*/^incdecmoddiv";
            string numbers = "0123456789-";
          //  string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            public string Parse(string s)
            {
               
                try
                {

                bool flag = false;
                for (int j = 0; j < s.Length; j++)
                {
                    if (numbers.Contains(s[j])) flag = true;
                }

                if (!flag) return "Error";


                    s = NoSpaces(s);
                    s = MinusToPlus(s);

                    while (true)
                    {
                        string temp = "";
                        string temp2 = "";
                        string temp3 = "";
                        int first_bracket = -1;
                        int second_bracket = -1;

                    double m;
                    if (validation(s) && (double.TryParse(s, out m))) {  if (s == "-0") return "0"; else return m.ToString(); } // if (m > 100000000000) return "inf"; else

                    for (int i = 0; i < s.Length; i++) if (s[i] == '(') first_bracket = i;

                        if (first_bracket == -1) { s = Calculate(s); }
                        if (first_bracket == -1) { s = Calculate(s); }
                        if (first_bracket == -1) { s = Calculate(s); }
                    else
                        {
                            for (int i = first_bracket; i < s.Length; i++) if (s[i] == ')') { second_bracket = i; break; }


                            for (int i = first_bracket + 1; i < second_bracket; i++)
                            {
                                temp += s[i];
                            }
                            temp = Calculate(temp);


                        }

                        for (int i = 0; i < s.Length; i++) if (i < first_bracket) temp2 += s[i];
                        for (int i = 0; i < s.Length; i++) if (i > second_bracket) temp3 += s[i];
                        s = temp2 + temp + temp3;

                    }
                }
                catch (Exception) { return "Error"; }
            }


            public bool validation(string s)
            {
                string operations2 = operations + "()";
                for (int i = 0; i < s.Length; i++)
                {
                    if (operations2.Contains(s[i])) return false;
                }
                return true;
            }

            private int find_n(int index, List<Number> Arr)
            {
                for (int i = 0; i < Arr.Count; i++)
                {
                    if (Arr[i].first_index == index) return i;
                    if (Arr[i].second_index == index) return i;
                }
                return -1;
            }


            private int find_o(int index, List<Operation> Arr)
            {
                for (int i = 0; i < Arr.Count; i++)
                {
                    if (Arr[i].first_index == index) return i;
                    if (Arr[i].second_index == index) return i;
                }
                return -1;
            }

            public string Calculate(string s)
            {
                List<Number> numbers_ = new List<Number>();
                List<Operation> operations_ = new List<Operation>();
                int i = 0;
                while (true)
                {
                    if (i == s.Length) break;
                    string temp = "";
                    for (; i < s.Length; i++)
                    {
                        if (operations.Contains(s[i])) break;
                        temp += s[i];
                    }
                    if (temp != "")
                    {
                        Number temp1 = new Number();
                        temp1.value = double.Parse(temp);
                        temp1.second_index = i - 1;
                        temp1.first_index = (i - temp.Length);
                        numbers_.Add(temp1);
                    }

                    if (i == s.Length) break;
                    string temp2 = "";
                    for (; i < s.Length; i++)
                    {
                        if (numbers.Contains(s[i])) break;
                        temp2 += s[i];
                    }
                    if (temp2 != "")
                    {
                        Operation temp3 = new Operation();
                        temp3.value = temp2;
                        temp3.second_index = i - 1;
                        temp3.first_index = (i - temp2.Length);
                        operations_.Add(temp3);
                    }
                }




                for (int j = 0; j < operations_.Count; j++)
                {
                    switch (operations_[j].value)
                    {
                        case "^":
                            operations_[j].priority = 5;
                            break;

                        case "dec":
                            operations_[j].priority = 4;
                            break;
                        case "inc":
                            operations_[j].priority = 4;
                            break;

                        case "mod":
                            operations_[j].priority = 4;
                            break;
                        case "div":
                            operations_[j].priority = 4;
                            break;
                        case "/":
                            operations_[j].priority = 2;
                            break;
                        case "*":
                            operations_[j].priority = 2;
                            break;
                        case "+":
                            operations_[j].priority = 1;
                            break;
                        default:
                            operations_[j].priority = -1;
                            break;
                    }
                }

                while (operations_.Count != 0)
                {
                    int max = operations_[0].priority; int index = 0;
                    for (int j = 0; j < operations_.Count; j++)
                    {
                        if (operations_[j].priority > max) { max = operations_[j].priority; index = j; }
                    }


                    switch (operations_[index].value)
                    {

                        case "^":
                            {
                                Number temp = new Number();
                                int index1 = operations_[index].first_index;
                                int index2 = operations_[index].second_index;
                                int n1 = find_n(index1 - 1, numbers_);
                                int n2 = find_n(index2 + 1, numbers_);
                                temp.value = Math.Pow(numbers_[n1].value, numbers_[n2].value);
                                temp.first_index = numbers_[n1].first_index;
                                temp.second_index = numbers_[n2].second_index;
                                numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                                numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                                operations_.RemoveAt(index);
                                numbers_.Add(temp);
                            }
                            break;
                        case "/":
                            {
                                Number temp = new Number();
                                int index1 = operations_[index].first_index;
                                int index2 = operations_[index].second_index;
                                int n1 = find_n(index1 - 1, numbers_);
                                int n2 = find_n(index2 + 1, numbers_);
                                if (numbers_[n2].value == 0) throw new Exception();
                                temp.value = numbers_[n1].value / numbers_[n2].value;
                                temp.first_index = numbers_[n1].first_index;
                                temp.second_index = numbers_[n2].second_index;
                                numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                                numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                                operations_.RemoveAt(index);
                                numbers_.Add(temp);
                            }
                            break;
                        case "*":
                            {
                                Number temp = new Number();
                                int index1 = operations_[index].first_index;
                                int index2 = operations_[index].second_index;
                                int n1 = find_n(index1 - 1, numbers_);
                                int n2 = find_n(index2 + 1, numbers_);
                                temp.value = numbers_[n1].value * numbers_[n2].value;
                                temp.first_index = numbers_[n1].first_index;
                                temp.second_index = numbers_[n2].second_index;
                                numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                                numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                                operations_.RemoveAt(index);
                                numbers_.Add(temp);
                            }
                            break;
                        case "+":
                            {
                                Number temp = new Number();
                                int index1 = operations_[index].first_index;
                                int index2 = operations_[index].second_index;
                                int n1 = find_n(index1 - 1, numbers_);
                                int n2 = find_n(index2 + 1, numbers_);
                                temp.value = numbers_[n1].value + numbers_[n2].value;
                                temp.first_index = numbers_[n1].first_index;
                                temp.second_index = numbers_[n2].second_index;
                                numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                                numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                                operations_.RemoveAt(index);
                                numbers_.Add(temp);
                            }
                            break;

                        case "inc":
                            {
                                Number temp = new Number();
                                int index2 = operations_[index].second_index;

                                int n2 = find_n(index2 + 1, numbers_);
                                temp.value = numbers_[n2].value + 1;
                                temp.first_index = operations_[index].first_index;
                                temp.second_index = numbers_[n2].second_index;
                                numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                                operations_.RemoveAt(index);
                                numbers_.Add(temp);
                            }
                            break;

                        case "dec":
                            {
                                Number temp = new Number();
                                // int index1 = operations_[index].first_index;
                                int index2 = operations_[index].second_index;

                                //  int n1 = find_n(index1 - 1, numbers_);
                                int n2 = find_n(index2 + 1, numbers_);
                                temp.value = numbers_[n2].value - 1;
                                temp.first_index = operations_[index].first_index;
                                temp.second_index = numbers_[n2].second_index;
                                // numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                                numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                                operations_.RemoveAt(index);
                                numbers_.Add(temp);
                            }
                            break;

                        case "mod":
                            {
                                Number temp = new Number();
                                int index1 = operations_[index].first_index;
                                int index2 = operations_[index].second_index;
                                int n1 = find_n(index1 - 1, numbers_);
                                int n2 = find_n(index2 + 1, numbers_);
                                temp.value = numbers_[n1].value % numbers_[n2].value;
                                temp.first_index = numbers_[n1].first_index;
                                temp.second_index = numbers_[n2].second_index;
                                numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                                numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                                operations_.RemoveAt(index);
                                numbers_.Add(temp);
                            }
                            break;

                        case "div":
                            {
                                Number temp = new Number();
                                int index1 = operations_[index].first_index;
                                int index2 = operations_[index].second_index;
                                int n1 = find_n(index1 - 1, numbers_);
                                int n2 = find_n(index2 + 1, numbers_);
                                temp.value = Math.Floor(numbers_[n1].value / numbers_[n2].value);
                                temp.first_index = numbers_[n1].first_index;
                                temp.second_index = numbers_[n2].second_index;
                                numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                                numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                                operations_.RemoveAt(index);
                                numbers_.Add(temp);
                            }
                            break;
                    default:
                        {
                            return "Error";
                        }
                    }

                }
                return (numbers_[0].value).ToString();
            }

            public string NoSpaces(string s)
            {
                string temp = "";
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == ' ') continue;
                    temp += s[i];
                }
                return temp;
            }

        public string MinusToPlus(string s)
        {
            string h = "--"; string after = "#"; string numbers2 = numbers + ")";
            s = s.Replace(h, after); Console.WriteLine(s);
            s = s.Replace("-+", "-"); Console.WriteLine(s);
            s = NoSpaces(s);

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '#')
                {

                    if (i == 0) { s = s.Substring(0, i) + " " + s.Substring(i + 1); continue; }
                    if (numbers2.Contains(s[i - 1])) { s = s.Substring(0, i) + "+" + s.Substring(i + 1); continue; }
                    else { s = s.Substring(0, i) + " " + s.Substring(i + 1); }
                }
            }
            s = NoSpaces(s);
            Console.WriteLine(s);

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '-')
                {

                    if (i == 0) continue;
                    if (numbers2.Contains(s[i - 1])) { s = s.Substring(0, i) + "+" + s.Substring(i); i++; }
                }
            }

            return s;
        }



    }
    }




