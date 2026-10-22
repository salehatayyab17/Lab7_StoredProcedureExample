using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Lab7_StoredProcedureExample
{
    public partial class Form1 : Form
    {
        // Docker SQL Server container (localdb is not available in Docker)
        private string connectionString =
            @"Data Source=localhost,1433;Initial Catalog=Lab7_StoredProc_DB;User ID=sa;Password=YOUR_SA_PASSWORD;TrustServerCertificate=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            ExecuteProcedure("InsertCustomer", "Customer inserted successfully!",
                new SqlParameter("@CustomerName", txtCustomerName.Text),
                new SqlParameter("@CustomerEmail", txtCustomerEmail.Text));
            LoadCustomers();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCustomerID.Text, out int id))
            {
                MessageBox.Show("Enter a valid Customer ID.");
                return;
            }
            ExecuteProcedure("UpdateCustomer", "Customer updated successfully!",
                new SqlParameter("@CustomerID", id),
                new SqlParameter("@CustomerEmail", txtCustomerEmail.Text));
            LoadCustomers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCustomerID.Text, out int id))
            {
                MessageBox.Show("Enter a valid Customer ID.");
                return;
            }
            ExecuteProcedure("DeleteCustomer", "Customer deleted successfully!",
                new SqlParameter("@CustomerID", id));
            LoadCustomers();
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];
            txtCustomerID.Text = row.Cells["CustomerID"].Value.ToString();
            txtCustomerName.Text = row.Cells["Name"].Value.ToString();
            txtCustomerEmail.Text = row.Cells["Email"].Value.ToString();
        }

        // Runs a non-query stored procedure (insert / update / delete)
        private void ExecuteProcedure(string procedureName, string successMessage, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(procedureName, connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(successMessage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Calls GetAllCustomers with SqlDataReader and binds the result to the grid
        private void LoadCustomers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetAllCustomers", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        dgvCustomers.DataSource = table;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
