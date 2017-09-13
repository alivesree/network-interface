using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Configuration;
using System.Net.NetworkInformation;
using System.Threading;
namespace Network_Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
    NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
    foreach (NetworkInterface adapter in interfaces)
    {                
        Console.WriteLine ("Name: {0}", adapter.Name);
        Console.WriteLine(adapter.Description);
        Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length,'='));
        Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
        Console.WriteLine("  Operational status ...................... : {0}", 
            adapter.OperationalStatus);
        string versions ="";
        Console.ReadLine();
        // Create a display string for the supported IP versions.
        if (adapter.Supports(NetworkInterfaceComponent.IPv4))
        {
             versions = "IPv4";
         }
        if (adapter.Supports(NetworkInterfaceComponent.IPv6))
        {
            if (versions.Length > 0)
            {
                versions += " ";
             }
            versions += "IPv6";
        }
        Console.WriteLine("  IP version .............................. : {0}", versions);
        Console.WriteLine();
    }
    Console.WriteLine();
    Console.ReadLine();
            loadAdptrNames_Click(this,null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string arguments = @"interface ipv4 set address name="+adptrNames.SelectedItem.ToString()+" static " + txtIp.Text + " " + subnet.Text + " " + gateway.Text + "";
                string arguments2 = @"interface ip set dns name="+adptrNames.SelectedItem.ToString()+" static "+dns.Text+"";
                Process p = new Process();
                ProcessStartInfo psi2 = new ProcessStartInfo("netsh", arguments2);
                ProcessStartInfo psi = new ProcessStartInfo("netsh", arguments);
                p.StartInfo = psi;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                p.StartInfo = psi2;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                //procStartInfo.RedirectStandardOutput = true;
                //procStartInfo.UseShellExecute = false;
                //procStartInfo.CreateNoWindow = true;

                //Process.Start(procStartInfo);
                MessageBox.Show("Applied");
            }
            catch (Exception eg)
            {
                MessageBox.Show(this,eg.ToString());
            }
        }

        private void loadAdptrNames_Click(object sender, RoutedEventArgs e)
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                adptrNames.Items.Add(nic.Name);
            }
            adptrNames.SelectedIndex = 0;
        }

        private void DHCPen_Checked(object sender, RoutedEventArgs e)
        {
            string arguments = "interface ip set address "+adptrNames.SelectedItem.ToString()+" dhcp";
            string arguments2 = "interface ip set dns " + adptrNames.SelectedItem.ToString() + " dhcp";
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("netsh", arguments);
            ProcessStartInfo psi2 = new ProcessStartInfo("netsh", arguments2);
            p.StartInfo = psi;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StartInfo = psi2;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //IP_list ex = new IP_list();
            //ex.Show();
            //DataGrid dr = ex.IP_availability;
            //dr.Items.Add(new  { id="1" });
            ips.Items.Clear();
            for (int i = 1; i < 255; i++)
            {
                string ip = "192.168.1." + i;
                try
                {
                    Task t = Task.Run(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        Ping ping = new Ping();
                        PingReply pingresult = ping.Send(ip);
                        this.Dispatcher.Invoke(() =>
                        {
                            ips.Items.Add(ip + " " + pingresult.Status.ToString());

                            //if(pingresult.Status.ToString()=="success")

                        });
                    });
                }
                catch(Exception eg)
                {
                    ips.Items.Add(eg.ToString());
                }
            }
        
        }
        
    }
}
