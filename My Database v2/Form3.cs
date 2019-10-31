using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My_Database_v2
{
    public partial class Form3 : Form
    {
        MySqlConnection connection;
        MySqlCommand command;
        string query;

        public Form3()
        {
            InitializeComponent();

            Data.EventHandler = new Data.MyEvent(Connect);

            LoadDatabases(comboBox1);
        }

        void Connect(MySqlConnection conn, int mode_, string del_name_, string del_date_)
        {
            connection = conn;
        }

        public void LoadDatabases(ComboBox comboBox)
        {
            string connStr = "server=localhost;user=root;";

            connection = new MySqlConnection(connStr);

            connection.Open();

            query = String.Format("show databases;");
            command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string row = "";
                for (int i = 0; i < reader.FieldCount; i++)
                    row += reader.GetValue(i).ToString();
                if (row != "information_schema")
                    comboBox.Items.Add(row);
            }
            reader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                try
                {
                    query = string.Format("create database {0} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;",
                                comboBox1.Text.ToString());
                    command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    query = string.Format("use {0};",
                                comboBox1.Text.ToString());
                    command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    query = string.Format("create table Costs (name varchar(50) not null, price float not null, costs_type varchar(30) not null, " +
                        "market varchar(20) not null, costs_date date not null, " +
                        "comments varchar(100), primary key(name, costs_date));",
                                comboBox1.Text.ToString());
                    command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    this.Close();
                }
                catch
                {
                    MessageBox.Show("База данных с таким именем уже существует.");
                }
            }
            else
            {
                MessageBox.Show("Функция экстрасенсорики будет добавлена в будущих релизах. Пока же никто не знает, что подразумевается в этой пустой форме.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                try
                {
                    query = string.Format("drop database {0};",
                                comboBox1.Text.ToString());
                    command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    this.Close();
                }
                catch
                {
                    MessageBox.Show("База данных с таким именем уже существует.");
                }
            }
            else
            {
                MessageBox.Show("Функция экстрасенсорики будет добавлена в будущих релизах. Пока же никто не знает, что подразумевается в этой пустой форме.");
            }
        }
    }
}
