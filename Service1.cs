using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.Runtime.InteropServices;
namespace KisnaService
{
    public partial class Service1 : ServiceBase
    {
        public string Api;
        private HttpClient client;
        private Timer _timer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {
            Api = ConfigurationSettings.AppSettings["ConnectionString"];
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                // Implement your certificate validation logic here
                // Return 'true' to trust the certificate or 'false' to reject it
                return true; // Not recommended for production
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri(Api);
            // Set up the timer to run every day at 5 PM
            _timer = new Timer();
            _timer.Interval = 60000;
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
            await GetDailySalesReport();
        }

        protected override void OnStop()
        {
            // Stop the timer when the service is stopped
            _timer.Stop();
            _timer.Dispose();
        }

        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime datetime = DateTime.Now;
            if (datetime.Hour == 18 && datetime.Minute==00)
            {
                await GetDailySalesReport();
            }
        }

        private async Task GetDailySalesReport()
        {
            try
            {
                string API1 = "api/DashBoard/GetSalesOrdersReportInExcel";
                HttpResponseMessage InsertApiResponce = client.GetAsync(API1).Result;
                if (InsertApiResponce.IsSuccessStatusCode)
                {
                }
            }
            catch (Exception ex)
            {
                var x = ex.ToString();
                // EventLog.WriteEntry("YourWindowsService", $"Error during API call: {ex.Message}", EventLogEntryType.Error);
            }
        }
        public void TestWindowService(string[] args)
        {
            OnStart(args);
        }
    }
}
