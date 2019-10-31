using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My_Database_v2
{
    public partial class Form2 : Form
    {
        MySqlConnection connection;
        MySqlCommand command;
        string query;
        int mode;
        string del_name;
        string del_date;

        public Form2()
        {
            InitializeComponent();
            
            Data.EventHandler = new Data.MyEvent(Connect);
        }

        void Connect(MySqlConnection conn, int mode_, string del_name_, string del_date_)
        {
            connection = conn;
            mode = mode_;
            del_name = del_name_;
            del_date = del_date_;

            if (mode == 1)
                ChangeLoad();
            else
                button1.Text = "Добавить";

            SetValues();
        }

        void SetValues()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();

            MySqlDataReader reader;

            query = "SET NAMES cp1251";
            command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();

            query = "SELECT name FROM Costs group by name";
            command = new MySqlCommand(query, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
                comboBox1.Items.Add(reader[0].ToString());
            reader.Close();

            query = "SELECT costs_type FROM Costs group by costs_type";
            command = new MySqlCommand(query, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
                comboBox2.Items.Add(reader[0].ToString());
            reader.Close();

            query = "SELECT market FROM Costs group by market";
            command = new MySqlCommand(query, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
                comboBox3.Items.Add(reader[0].ToString());
            reader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mode == 1)
            {
                query = "SET NAMES utf8";
                command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();

                query = String.Format("delete from Costs where name = '{0}' and costs_date = '{1}';",
                                    del_name, del_date);
                command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();
            }

            query = "SET NAMES utf8";
            command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();

            String[] price = textBox1.Text.Split(new char[] { ',' });
            string right_price;
            try
            {
                right_price = String.Format("{0}.{1}",
                                price[0], price[1]);
            }
            catch
            {
                right_price = price[0];
            }

            query = string.Format("insert into costs values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'); ",
                                comboBox1.Text, right_price, comboBox2.Text.ToString(), comboBox3.Text.ToString(), dateTimePicker1.Value.ToString("yyyy-MM-dd"), richTextBox1.Text);
            command = new MySqlCommand(query, connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Уже в базе данных");
            }

            comboBox1.ResetText();
            textBox1.Clear();
            comboBox2.ResetText();
            richTextBox1.Clear();

            SetValues();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mode == 0)
            {
                query = "SET NAMES utf8";
                command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();

                query = String.Format("select * from Costs where name = '{0}';",
                                    comboBox1.Text);
                command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    String[] price = reader[1].ToString().Split(new char[] { ',' });
                    try
                    {
                        textBox1.Text = String.Format("{0}.{1}",
                                        price[0], price[1]);
                    }
                    catch
                    {
                        textBox1.Text = price[0];
                    }
                    comboBox2.Text = reader[2].ToString();
                    richTextBox1.Text = reader[5].ToString();
                }

                reader.Close();
            }
        }

        private void ChangeLoad()
        {
            
            button1.Text = "Изменить";

            query = "SET NAMES utf8";
            command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();

            query = String.Format("select * from Costs where name = '{0}' and costs_date = '{1}';",
                                    del_name, del_date);
            command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                comboBox1.Text = reader[0].ToString();

                String[] price = reader[1].ToString().Split(new char[] { ',' });
                try
                {
                    textBox1.Text = String.Format("{0}.{1}",
                                    price[0], price[1]);
                }
                catch
                {
                    textBox1.Text = price[0];
                }

                comboBox2.Text = reader[2].ToString();
                comboBox3.Text = reader[3].ToString();

                String[] words = reader[4].ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                String[] date = words[0].Split(new char[] { '.' });
                dateTimePicker1.Value = new DateTime(Int32.Parse(date[2]), Int32.Parse(date[1]), Int32.Parse(date[0]));

                richTextBox1.Text = reader[5].ToString();
            }

            reader.Close();
        }
    }
}
