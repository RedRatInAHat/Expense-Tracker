using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My_Database_v2
{
    public partial class Form1 : Form
    {
        MySqlConnection connection;
        MySqlCommand command;
        string query;

        public Form1()
        {
            InitializeComponent();
            Process.Start(@"C:\WebServers\denwer\Run.exe");
            Thread.Sleep(3000);
            StartDatabase();
            CheckAllBoxes();
        }

        public void StartDatabase()
        {
            string connStr = "server=localhost;user=root;database=outlay;CharSet=utf8;Convert Zero Datetime=True;";

            connection = new MySqlConnection(connStr);

            connection.Open();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            Data.EventHandler(connection, 0, "", "");
            f.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadTable();
        }

        private DataGridViewTextBoxColumn CreateColumn(string Name, string Text, int Width_)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            {
                Name = this.Name,
                HeaderText = Text,
                Width = Width_
            };
            return column;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            query = "SET NAMES utf8";
            command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();

            String[] words = dataGridView1.CurrentRow.Cells[4].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            String[] date = words[0].Split(new char[] { '.' });
            string dateToSql = string.Format("{2}-{1}-{0}", date[0], date[1], date[2]);
            
            query = String.Format("delete from Costs where name = '{0}' and costs_date = '{1}';",
                                dataGridView1.CurrentRow.Cells[0].Value.ToString(), dateToSql);
            command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();

            LoadTable();
        }

        void LoadTable()
        {
            bool[] b_column = new bool[6];

            int sum = this.dataGridView1.Columns.Count;
            for (int i = 0; i < sum; i++) { this.dataGridView1.Columns.RemoveAt(0); }
            dataGridView1.Rows.Clear();

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            if (radioButton1.Checked)
            {
                for (int i = 0; i < b_column.Length; i++)
                    b_column[i] = true;
            }
            if (radioButton2.Checked)
            {
                if (checkBox1.Checked)
                    b_column[0] = true;
                if (checkBox2.Checked)
                    b_column[1] = true;
                if (checkBox3.Checked)
                    b_column[2] = true;
                if (checkBox4.Checked)
                    b_column[3] = true;
                if (checkBox9.Checked)
                    b_column[4] = true;
                if (checkBox5.Checked)
                    b_column[5] = true;
            }

            LoadColums(b_column);
        }

        private void LoadColums(bool[] b_columns)
        {
            int how_many_columns = 0;

            if (b_columns[0])
            {
                DataGridViewTextBoxColumn name = CreateColumn("name", "Наименование", 150);
                dataGridView1.Columns.Add(name);
                how_many_columns++;
            }

            if (b_columns[1])
            {
                DataGridViewTextBoxColumn price = CreateColumn("price", "Цена", 50);
                dataGridView1.Columns.Add(price);
                how_many_columns++;
            }

            if (b_columns[2])
            {
                DataGridViewTextBoxColumn costs_type = CreateColumn("costs_type", "Тип покупки", 100);
                dataGridView1.Columns.Add(costs_type);
                how_many_columns++;
            }

            if (b_columns[3])
            {
                DataGridViewTextBoxColumn market = CreateColumn("market", "Магазин", 100);
                dataGridView1.Columns.Add(market);
                how_many_columns++;
            }

            if (b_columns[4])
            {
                DataGridViewTextBoxColumn costs_date = CreateColumn("costs_date", "Дата", 100);
                dataGridView1.Columns.Add(costs_date);
                how_many_columns++;
            }

            if (b_columns[5])
            {
                DataGridViewTextBoxColumn comments = CreateColumn("comments", "Комментарии", 100);
                dataGridView1.Columns.Add(comments);
                comments.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                how_many_columns++;
            }

            query = "SET NAMES cp1251";
            command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();

            MySqlDataReader reader;

            if (radioButton1.Checked)
            {
                query = "SELECT * FROM Costs order by costs_date desc;";
                command = new MySqlCommand(query, connection);
                reader = command.ExecuteReader();
            }
            else
            {
                query = "SELECT * FROM Costs";
                string GroupBy = "";
                string OrderBy = "";

                bool GB = false;

                if (checkBox6.Checked)
                {

                    if (checkBox19.Checked)
                    {
                        GroupBy += "market";
                        GB = true;
                    }
                    if (checkBox22.Checked)
                    {
                        if (GB)
                            GroupBy += ", ";
                        GroupBy += "costs_type";
                        GB = true;
                    }
                    if (checkBox21.Checked)
                    {
                        if (GB)
                            GroupBy += ", ";
                        GroupBy += "costs_date";
                        GB = true;
                    }
                    if (checkBox7.Checked)
                    {
                        if (GB)
                            GroupBy += ", ";
                        GroupBy += "year(costs_date), month(costs_date)";
                        GB = true;
                    }
                    if (checkBox8.Checked)
                    {
                        if (GB)
                            GroupBy += ", ";
                        GroupBy += "year(costs_date)";
                        GB = true;
                    }
                    if (checkBox17.Checked)
                    {
                        if (GB)
                            GroupBy += ", ";
                        GroupBy += "name";
                        GB = true;
                    }
                    if (checkBox20.Checked)
                    {
                        if (GB)
                            GroupBy += ", ";
                        GroupBy += "comments";
                        GB = true;
                    }
                    if (checkBox18.Checked)
                    {
                        if (GB)
                            GroupBy += ", ";
                        GroupBy += "price";
                        GB = true;
                    }

                    if (GB && b_columns[1])
                    {
                        query = "SELECT name, round(sum(price),2), costs_type, market, costs_date, comments FROM Costs";
                    }

                    if (GB)
                    {
                        GroupBy = " Group By " + GroupBy;

                    }
                    else
                    {
                        MessageBox.Show("Параметры группировки не были выбраны");
                    }
                }

                if (checkBox10.Checked)
                {
                    bool OB = false;

                    if (checkBox12.Checked)
                    {
                        if (GB && b_columns[1])
                            OrderBy += "sum(price)";
                        else
                            OrderBy += "price";
                        OB = true;
                    }
                    if (checkBox13.Checked)
                    {
                        if (OB)
                            OrderBy += ", ";
                        OrderBy += "market";
                        OB = true;
                    }
                    if (checkBox16.Checked)
                    {
                        if (OB)
                            OrderBy += ", ";
                        OrderBy += "costs_type";
                        OB = true;
                    }
                    if (checkBox15.Checked)
                    {
                        if (OB)
                            OrderBy += ", ";
                        if (OB && checkBox7.Checked)
                            OrderBy += "year(costs_date), month(costs_date)";
                        else if (OB && checkBox8.Checked)
                            OrderBy += "year(costs_date)";
                        else
                            OrderBy += "costs_date";
                        OB = true;
                    }
                    if (checkBox11.Checked)
                    {
                        if (OB)
                            OrderBy += ", ";
                        OrderBy += "name";
                        OB = true;
                    }
                    if (checkBox14.Checked)
                    {
                        if (OB)
                            OrderBy += ", ";
                        OrderBy += "comments";
                        OB = true;
                    }

                    if (OB)
                    {
                        OrderBy = " Order By " + OrderBy;
                        if (radioButton4.Checked)
                            OrderBy += " desc";
                    }
                    else
                    {
                        MessageBox.Show("Параметры сортировки не были выбраны");
                    }
                }
                else
                    OrderBy = " order by costs_date desc";

                query = String.Format("{0}{1}{2};",
                                    query, GroupBy, OrderBy);
                command = new MySqlCommand(query, connection);
                reader = command.ExecuteReader();
            }

            List<string[]> data = new List<string[]>();
            

            while (reader.Read())
            {
                data.Add(new string[how_many_columns]);
                int c_number = 0;

                for (int i = 0; i< b_columns.Length; i++)
                {
                    if (b_columns[i])
                    {
                        if (i == 4)
                        {
                            String[] date = reader[4].ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            data[data.Count - 1][c_number++] = date[0];
                        }
                        else
                            data[data.Count - 1][c_number++] = reader[i].ToString();
                    }
                }
            }

            reader.Close();

            try
            {
                foreach (string[] s in data)
                    dataGridView1.Rows.Add(s);
            }
            catch
            {
                MessageBox.Show("Не выбрано ни одного значения");
            }
        }

        private void CheckAllBoxes()
        {
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
            checkBox4.Enabled = false;
            checkBox5.Enabled = false;
            checkBox9.Enabled = false;
            checkBox6.Enabled = false;
            checkBox17.Enabled = false;
            checkBox18.Enabled = false;
            checkBox22.Enabled = false;
            checkBox19.Enabled = false;
            checkBox20.Enabled = false;
            checkBox21.Enabled = false;
            checkBox7.Enabled = false;
            checkBox8.Enabled = false;
            checkBox10.Enabled = false;
            checkBox11.Enabled = false;
            checkBox12.Enabled = false;
            checkBox16.Enabled = false;
            checkBox13.Enabled = false;
            checkBox14.Enabled = false;
            checkBox15.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;

            if (radioButton2.Checked)
            {
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                checkBox5.Enabled = true;
                checkBox9.Enabled = true;
                checkBox6.Enabled = true;
                checkBox10.Enabled = true;
            }
            if (checkBox6.Checked)
            {
                checkBox17.Enabled = true;
                checkBox18.Enabled = true;
                checkBox22.Enabled = true;
                checkBox19.Enabled = true;
                checkBox20.Enabled = true;
                checkBox21.Enabled = true;
                checkBox7.Enabled = true;
                checkBox8.Enabled = true;
            }
            if (checkBox10.Checked)
            {
                checkBox11.Enabled = true;
                checkBox12.Enabled = true;
                checkBox16.Enabled = true;
                checkBox13.Enabled = true;
                checkBox14.Enabled = true;
                checkBox15.Enabled = true;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String[] words = dataGridView1.CurrentRow.Cells[4].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            String[] date = words[0].Split(new char[] { '.' });
            string dateToSql = string.Format("{2}-{1}-{0}", date[0], date[1], date[2]);

            Form2 f = new Form2();
            Data.EventHandler(connection, 1, dataGridView1.CurrentRow.Cells[0].Value.ToString(), dateToSql);
            f.ShowDialog();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllBoxes();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllBoxes();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllBoxes();
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked)
            {
                checkBox7.Checked = false;
                checkBox8.Checked = false;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                checkBox21.Checked = false;
                checkBox8.Checked = false;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                checkBox7.Checked = false;
                checkBox21.Checked = false;
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            saveToCSV();
        }

        private void saveToCSV()
        {
            StreamWriter sw = new StreamWriter("data.csv");
            foreach (DataGridViewRow row in dataGridView1.Rows)
                if (!row.IsNewRow)
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        for (int j = 0; j < dataGridView1.ColumnCount; j++)
                        {
                            sw.Write(dataGridView1.Rows[i].Cells[j].Value);
                            if (j < dataGridView1.ColumnCount - 1)
                                sw.Write(";");
                        }
                        sw.WriteLine();
                    }
                }

            sw.Close();

            string[] my_data = File.ReadAllLines("data.csv");
            for (int i = 0; i < my_data.Length; i++)
            {
                if (!String.IsNullOrEmpty(my_data[i]))
                {
                    string[] data_Values = my_data[i].Split(';');
                    //создаём новую строку
                    MessageBox.Show(data_Values[0]);
                }
            }
        }
    }
}
