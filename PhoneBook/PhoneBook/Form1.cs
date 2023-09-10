using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneBook
{
    public partial class Telephonebook : Form
    {
        DataTable contacts = new DataTable("ContactsTable");
        bool editing = false;
        private string dataFilePath = "phonebook_data.xml";
        public Telephonebook()
        {
            InitializeComponent();

            // Prevent form resizing
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void SaveContactsToFile()
        {
            try
            {
                contacts.WriteXml(dataFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadContactsFromFile()
        {
            if (System.IO.File.Exists(dataFilePath))
            {
                try
                {
                    contacts.ReadXml(dataFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Telephonebook_Load(object sender, EventArgs e)
        {
            contacts.Columns.Add("First Name");
            contacts.Columns.Add("Last Name");
            contacts.Columns.Add("Phone");
            contacts.Columns.Add("E-mail");
            contacts.Columns.Add("Birthday");

            LoadContactsFromFile();

            ContactsDataGrid.DataSource = contacts;
        }


        private void FieldButt_Click(object sender, EventArgs e)
        {
            FirstNameBox.Text = "";
            LastNameBox.Text = "";
            PhoneBox.Text = "";
            EmailBox.Text = "";
            BirthdayBox.Text = "";
        }

        private void SaveButt_Click(object sender, EventArgs e)
        {
            if (!IsValidInput())
            {
                MessageBox.Show("Invalid input detected. Please correct and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (editing)
            {
                contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex]["First Name"] = FirstNameBox.Text;
                contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex]["Last Name"] = LastNameBox.Text;
                contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex]["Phone"] = PhoneBox.Text;
                contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex]["E-mail"] = EmailBox.Text;
                contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex]["Birthday"] = BirthdayBox.Text;
            }
            else
            {
                contacts.Rows.Add(FirstNameBox.Text, LastNameBox.Text, PhoneBox.Text, EmailBox.Text, BirthdayBox.Text);
            }

            FirstNameBox.Text = "";
            LastNameBox.Text = "";
            PhoneBox.Text = "";
            EmailBox.Text = "";
            BirthdayBox.Text = "";
            editing = false;

            SaveContactsToFile();
        }

        private bool IsValidInput()
        {

            if (string.IsNullOrEmpty(FirstNameBox.Text) || FirstNameBox.Text.All(char.IsDigit))
            {
                MessageBox.Show("Invalid first name.");
                return false;
            }

            if (string.IsNullOrEmpty(LastNameBox.Text) || LastNameBox.Text.All(char.IsDigit))
            {
                MessageBox.Show("Invalid last name.");
                return false;
            }

            var phoneRegex = new System.Text.RegularExpressions.Regex(@"^[\d\+\-\(\)\s]+$");
            if (string.IsNullOrEmpty(PhoneBox.Text) || !phoneRegex.IsMatch(PhoneBox.Text))
            {
                MessageBox.Show("Invalid phone number.");
                return false;
            }

            try
            {
                var emailAddr = new System.Net.Mail.MailAddress(EmailBox.Text);
                if (emailAddr.Address != EmailBox.Text)
                {
                    MessageBox.Show("Invalid email address.");
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("Invalid email address.");
                return false;
            }

            DateTime birthday;
            if (!DateTime.TryParse(BirthdayBox.Text, out birthday))
            {
                MessageBox.Show("Invalid birthday date format.");
                return false;
            }

            return true;
        }

        private void DeleteButt_Click(object sender, EventArgs e)
        {
            try
            {
                contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex].Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ypu made a mistake while writting info in a row");
            }
            FirstNameBox.Text = "";
            LastNameBox.Text = "";
            PhoneBox.Text = "";
            EmailBox.Text = "";
            BirthdayBox.Text = "";

            SaveContactsToFile();
        }

        private void ContactsDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FirstNameBox.Text = contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex].ItemArray[0].ToString();
            LastNameBox.Text = contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex].ItemArray[1].ToString();
            PhoneBox.Text = contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex].ItemArray[2].ToString();
            EmailBox.Text = contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex].ItemArray[3].ToString();
            BirthdayBox.Text = contacts.Rows[ContactsDataGrid.CurrentCell.RowIndex].ItemArray[4].ToString();
            editing = true;
        }

    }
}

