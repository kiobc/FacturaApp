using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using CrystalDecisions.CrystalReports.Engine;
using FacturaApp;

namespace Factura
{
    public partial class Form1 : Form
    {
        private string connectionString;

        public Form1()
        {
            InitializeComponent();
            connectionString = "Data Source=DESKTOP-9INOCP4\\SQLEXPRESS;Initial Catalog=FacturaPrueba;User Id=bray;Password=Hellblazer-7;";

        }


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text;
                int cantidad = int.Parse(txtCantidad.Text);
                double precio = double.Parse(txtPrecio.Text);
                int iva = int.Parse(comboIva.SelectedItem.ToString());
                double subtotal = precio * cantidad;
                double total = 0;
                double totalf = 0;
                if (iva == 12)
                {
                    total = subtotal * 0.12;
                    totalf = precio + total;
                }
                else
                {
                    totalf = subtotal;
                }

                DataGridViewRow fila = new DataGridViewRow();
                fila.CreateCells(dgvLista);
                fila.Cells[0].Value = nombre;
                fila.Cells[1].Value = cantidad;
                fila.Cells[2].Value = precio;
                fila.Cells[3].Value = iva;
                fila.Cells[4].Value = totalf;
                dgvLista.Rows.Add(fila);

                txtNombre.Clear();
                txtCantidad.Clear();
                txtPrecio.Clear();
                obtenerTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }





        private int ObtenerIdFactura()
        {
            int idFactura = 0;

            string query = "SELECT TOP 1 IdFactura FROM EncabezadoFactura ORDER BY IdFactura DESC";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    idFactura = (int)reader["IdFactura"];
                }
            }

            return idFactura;
        }


        public void obtenerTotal()
        {
            float costot = 0;
            int contador = 0;

            contador = dgvLista.RowCount;

            for (int i = 0; i < contador; i++)
            {
                costot += float.Parse(dgvLista.Rows[i].Cells[4].Value.ToString());
            }

            lblTotatlPagar.Text = costot.ToString();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult rppta = MessageBox.Show("¿Desea eliminar producto?",
                    "Eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (rppta == DialogResult.Yes)
                {
                    dgvLista.Rows.Remove(dgvLista.CurrentRow);
                }
            }
            catch { }
            obtenerTotal();
        }


        private void btnVender_Click(object sender, EventArgs e)
        {
            try
            {
                int idFactura = ObtenerIdFactura(); // obtiene el último IdFactura de la tabla EncabezadoFactura

                // Obtener la cadena de conexión a la base de datos
                string connectionString = "Data Source=DESKTOP-9INOCP4\\SQLEXPRESS;Initial Catalog=FacturaPrueba;Integrated Security=True";

                // Crear una nueva instancia de SqlConnection utilizando la cadena de conexión
                SqlConnection connection = new SqlConnection(connectionString);

                // Abrir la conexión
                connection.Open();

                // Insertar los valores en la tabla ProductosVendidos
                foreach (DataGridViewRow row in dgvLista.Rows)
                {
                    string nombre = row.Cells[0].Value.ToString();
                    int cantidad = int.Parse(row.Cells[1].Value.ToString());
                    double precio = double.Parse(row.Cells[2].Value.ToString());
                    int iva = int.Parse(row.Cells[3].Value.ToString());
                    double totalf = double.Parse(row.Cells[4].Value.ToString());

                    string query = "INSERT INTO ProductosVendidos (IdFactura, NombreProducto, Cantidad, PrecioUnitario, IVA, TotalF) VALUES (@idFactura, @nombre, @cantidad, @precio, @iva, @totalf)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idFactura", idFactura);
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.Parameters.AddWithValue("@cantidad", cantidad);
                    command.Parameters.AddWithValue("@precio", precio);
                    command.Parameters.AddWithValue("@iva", iva);
                    command.Parameters.AddWithValue("@totalf", totalf);
                    command.ExecuteNonQuery();
                }

                // Cerrar la conexión
                connection.Close();

                MessageBox.Show("Gracias por preferirnos");

                this.Close();
            }
            catch (SqlException ex)
            {
                // Manejar el error si la conexión no se puede establecer correctamente
                MessageBox.Show("Error al establecer la conexión a la base de datos: " + ex.Message);
            }
        }







        private void Form1_Load(object sender, EventArgs e)
        {
            int idFactura = ObtenerIdFactura();
            lblIdFactura.Text = idFactura.ToString();
            obtenerTotal();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvLista_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            Reporte reporte = new Reporte();
            reporte.ShowDialog();
        }

    }
}
