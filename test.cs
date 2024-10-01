using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;

namespace formPendaftaran
{
    public partial class Form1 : Form
    {
        private HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadProvinsi();
        }

        private async Task LoadProvinsi()
        {
            try
            {
                var response = await client.GetStringAsync("https://www.emsifa.com/api-wilayah-indonesia/api/provinces.json");
                var provinces = JsonConvert.DeserializeObject<List<Province>>(response);
                comboBoxProvinsi.DataSource = provinces;
                comboBoxProvinsi.DisplayMember = "name";
                comboBoxProvinsi.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading provinces: {ex.Message}");
            }
        }

        private async void comboBoxProvinsi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxProvinsi.SelectedValue != null)
            {
                await LoadKabupaten(comboBoxProvinsi.SelectedValue.ToString());
            }
        }

        private async Task LoadKabupaten(string provinceId)
        {
            try
            {
                var response = await client.GetStringAsync($"https://www.emsifa.com/api-wilayah-indonesia/api/regencies/{provinceId}.json");
                var regencies = JsonConvert.DeserializeObject<List<Regency>>(response);
                comboBoxKabupaten.DataSource = regencies;
                comboBoxKabupaten.DisplayMember = "name";
                comboBoxKabupaten.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading regencies: {ex.Message}");
            }
        }

        private async void comboBoxKabupaten_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxKabupaten.SelectedValue != null)
            {
                await LoadKecamatan(comboBoxKabupaten.SelectedValue.ToString());
            }
        }

        private async Task LoadKecamatan(string regencyId)
        {
            try
            {
                var response = await client.GetStringAsync($"https://www.emsifa.com/api-wilayah-indonesia/api/districts/{regencyId}.json");
                var districts = JsonConvert.DeserializeObject<List<District>>(response);
                comboBoxKecamatan.DataSource = districts;
                comboBoxKecamatan.DisplayMember = "name";
                comboBoxKecamatan.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading districts: {ex.Message}");
            }
        }

        private async void comboBoxKecamatan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxKecamatan.SelectedValue != null)
            {
                await LoadDesa(comboBoxKecamatan.SelectedValue.ToString());
            }
        }

        private async Task LoadDesa(string districtId)
        {
            try
            {
                var response = await client.GetStringAsync($"https://www.emsifa.com/api-wilayah-indonesia/api/villages/{districtId}.json");
                var villages = JsonConvert.DeserializeObject<List<Village>>(response);
                comboBoxDesa.DataSource = villages;
                comboBoxDesa.DisplayMember = "name";
                comboBoxDesa.ValueMember = "id";


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading villages: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
               (!radioButton1.Checked && !radioButton2.Checked) ||
               string.IsNullOrWhiteSpace(textBox2.Text) ||
               comboBoxProvinsi.SelectedItem == null ||
               comboBoxKabupaten.SelectedItem == null ||
               comboBoxKecamatan.SelectedItem == null ||
               comboBoxDesa.SelectedItem == null)
            {
                MessageBox.Show("Mohon lengkapi semua data.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var data = new
            {
                Nama = textBox1.Text,
                JenisKelamin = radioButton1.Checked ? "Laki-laki" : "Perempuan",
                Alamat = textBox2.Text,
                Provinsi = ((Province)comboBoxProvinsi.SelectedItem).name,
                Kabupaten = ((Regency)comboBoxKabupaten.SelectedItem).name,
                Kecamatan = ((District)comboBoxKecamatan.SelectedItem).name,
                Desa = ((Village)comboBoxDesa.SelectedItem).name
            };

            // Buat dan tampilkan form popup
            MessageBox.Show("data berhasil di kirim", "berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

            labelNama.Text = data.Nama;
            labelKelamin.Text = data.JenisKelamin;
            labelAlamat.Text = data.Alamat;
            labelProv.Text = data.Provinsi;
            labelKab.Text = data.Kabupaten;
            labelKec.Text = data.Kecamatan;
            labelDes.Text = data.Desa;
        }
    }

    public class Province
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Regency
    {
        public string id { get; set; }
        public string province_id { get; set; }
        public string name { get; set; }
    }

    public class District
    {
        public string id { get; set; }
        public string regency_id { get; set; }
        public string name { get; set; }
    }

    public class Village
    {
        public string id { get; set; }
        public string district_id { get; set; }
        public string name { get; set; }
    }
}