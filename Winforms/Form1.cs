using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winforms
{
    public partial class Form1 : Form
    {
        private string apiURL;
        private HttpClient httpClient;

        public Form1()
        {
            InitializeComponent();
            apiURL = "https://localhost:44380";
            httpClient = new HttpClient();
        }

        private async void btnIniciar_Click(object sender, EventArgs e)
        {
            loadingGIF.Visible = true;

            var tarjetas = ObtenerTarjetasDeCredito(1000);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                await ProcesarTarjetas(tarjetas);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }

            MessageBox.Show($"Operación finalizada en {stopwatch.ElapsedMilliseconds / 1000.0} segundos");

            loadingGIF.Visible = false;
        }

        private async Task ProcesarTarjetas(List<string> tarjetas)
        {
            var tareas = new List<Task>();

            foreach (var tarjeta in tarjetas)
            {
                var json = JsonConvert.SerializeObject(tarjeta);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var respuestaTask = httpClient.PostAsync($"{apiURL}/tarjetas", content);
                tareas.Add(respuestaTask);
            }

            await Task.WhenAll(tareas);
        }

        private List<string> ObtenerTarjetasDeCredito(int cantidadDeTarjetas)
        {
            var tarjetas = new List<string>();

            for (int i = 0; i < cantidadDeTarjetas; i++)
            {
                tarjetas.Add(i.ToString().PadLeft(16, '0'));
            }

            return tarjetas;
        }

        private async Task Esperar()
        {
            await Task.Delay(TimeSpan.FromSeconds(0));
        }

        private async Task<string> ObtenerSaludo(string nombre)
        {
            using (var respuesta = await httpClient.GetAsync($"{apiURL}/saludos/{nombre}"))
            {
                respuesta.EnsureSuccessStatusCode();
                var saludo = await respuesta.Content.ReadAsStringAsync();
                return saludo;
            }
        }
    }
}
