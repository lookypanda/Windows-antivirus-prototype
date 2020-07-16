using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Threading;

namespace UIB_test_interface
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Add_to_reg("testNEW", "protected");

            refresh_list();
            log_refresh();
            File_drv_log_refresh();
            Start_logging();
            net_rules_refresh();
        }
        private void Add_to_reg(string name, string reg_name)
        {
            //RegistryValueKind.MultiString
            Microsoft.Win32.RegistryKey key;

            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\\filefilter");
            //key.SetValue("protected", new string[] { } , RegistryValueKind.MultiString); //обнуление
            key.SetValue(reg_name, name);
            key.Close();
        }
        int rulecount;
        private void refresh_list()
        {
            filesList.Items.Clear();
            reg_list.Items.Clear();
            RegistryKey readKey = Registry.CurrentUser.OpenSubKey(@"Software\\filefilter");
            if (readKey == null)
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\\filefilter");

                key.Close();
                readKey = Registry.CurrentUser.OpenSubKey(@"Software\\filefilter");
            }

            var keys = readKey.GetValueNames();
            rulecount = keys.Length;
            foreach (var subkey in keys)
            {
                string temp = (string)readKey.GetValue(subkey, null);
                if (subkey.Contains("reg"))
                {
                    reg_list.Items.Add(temp);
                }
                else
                    if (temp != "DEFAULT")
                {
                    filesList.Items.Add(temp);
                }


            }
            readKey.Close();
        }
        private void clear_list()
        {
            if(File_Filt_checkBox.IsChecked == true)
            {
            File_exe_stop();
            }
           
            try
            {
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(@"Software\\filefilter");
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
            filesList.Items.Clear();
            reg_list.Items.Clear();
            rulecount = 1;
            add_default_reg_key();
            if (File_Filt_checkBox.IsChecked == true)
            {
                File_drv_start();
            }


        }
        private void FilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void add_default_reg_key()
        {
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\file_drv\\add_default.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clear_list();
        }

        private void Add_butt_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog OPF = new OpenFileDialog();
            //if (OPF.ShowDialog() == true)
            //{
            //    MessageBox.Show(OPF.FileName);
            //}
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".*"; // Default file extension
            dlg.Filter = "All files (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                Console.WriteLine(filename);
                filename = filename.Replace("C:", "");
                Add_to_reg(filename, "file" + rulecount);
                refresh_list();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            refresh_list();
        }

        private int cloudScan(string pathSrtr, string IPStr)
        {
            if (File.Exists("C:/AV/cloud/AV/cloud/client.exe"))
            {
                cloudProc = new System.Diagnostics.Process();
                cloudProc.StartInfo.FileName = "cmd.exe";
                cloudProc.StartInfo.Arguments = @"/C cd C:/AV/cloud/AV/cloud/  &  client.exe " + pathSrtr + @" check " + IPStr;
                //cloudProc.StartInfo.FileName = "C:/AV/cloud/CloudClient.exe"; //проверить сохранность ярлыков при распаковке
                //cloudProc.StartInfo.Arguments = pathSrtr + @" down " + IPStr;
                cloudProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                //cloudProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;// нужно скрытое окно
                cloudProc.StartInfo.CreateNoWindow = true;

                cloudProc.StartInfo.RedirectStandardOutput = true;
                cloudProc.StartInfo.UseShellExecute = false;
                cloudProc.Start();

                // To avoid deadlocks, always read the output stream first and then wait.  
                string output = cloudProc.StandardOutput.ReadToEnd(); //1-вирус 0 -нет 5-ошибка пример:"1\r\n"
                cloudProc.WaitForExit();
                if (output.Contains("1"))
                {
                    MessageBox.Show("Virus File: " + (string)pathSrtr, "Cloud scan");
                    return 1;
                }
                if (output.Contains("0"))
                {
                    MessageBox.Show("OK File: " + (string)pathSrtr, "Cloud scan");
                    return 0;
                }
                else
                {
                    MessageBox.Show("Err File: " + (string)pathSrtr, "Cloud scan");
                    return 2;
                }

            }
            return 2;
        }
        private async void Button_Click_2(object sender, RoutedEventArgs e)//cloud check
        {
            string pathSrtr = upload_textBox.Text;
            string IPStr = Ip_addrBox.Text;


            if (!IsIpAddress(Ip_addrBox.Text))
            {
                MessageBox.Show("Wrong IP");
                return;
            }
            if(upload_textBox.Text.Equals("TextBox") || !File.Exists(pathSrtr) )
            {
                MessageBox.Show("Wrong file choose");
                return;
            }
            CloudScan_checkBox.IsChecked = true;
            int result = await Task.Factory.StartNew<int>(
                                         () => cloudScan(pathSrtr, IPStr),
                                         TaskCreationOptions.LongRunning);

            CloudScan_checkBox.IsChecked = false;

        }

        private void select_upload_butt_Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ""; // Default file extension
            dlg.Filter = "All files|*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                Console.WriteLine(filename);
                upload_textBox.Text = filename;

            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
        private System.Diagnostics.Process dynDrvStartProc;
        private System.Diagnostics.Process loggingProcess;
        private System.Diagnostics.Process cloudProc;
        private System.Diagnostics.Process UpdatecloudProc;
        private System.Diagnostics.Process StaticProc;
        private void Button_Click_4(object sender, RoutedEventArgs e) // start dinamic driver button
        {
            if (Dyn_checkBox.IsChecked == true)
            {
                MessageBox.Show("Already started");
                return;
            }
            if (File.Exists("C:/AV/bats/start_injdrv.bat"))
            {
                var psi = new System.Diagnostics.ProcessStartInfo();
                psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
                psi.FileName = @"cmd.exe";
                psi.Verb = "runas"; //This is what actually runs the command as administrator
                psi.Arguments = "/C " + "C:/AV/bats/start_injdrv.bat";
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

                try
                {
                    dynDrvStartProc = new System.Diagnostics.Process();
                    dynDrvStartProc.StartInfo = psi;
                    dynDrvStartProc.Start();
                    //dynDrvStartProc.WaitForExit();
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                    Console.WriteLine(s);//If you are here the user clicked decline to grant admin privileges (or he's not administrator)
                }

                Dyn_checkBox.IsChecked = true;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)//stop dinamic driver button
        {
            if (Dyn_checkBox.IsChecked == false)
            {
                MessageBox.Show("Already stopped");
                return;
            }
            if (dynDrvStartProc != null)
            {
                try
                {
                    dynDrvStartProc.Kill();
                } catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                }
            }
            else {
                MessageBox.Show("forget to start");
            }
            if (File.Exists("C:/AV/bats/stop_injdrv.bat"))
            {
                var psi = new System.Diagnostics.ProcessStartInfo();
                psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
                psi.FileName = @"cmd.exe";
                psi.Verb = "runas"; //This is what actually runs the command as administrator
                psi.Arguments = "/C " + "C:/AV/bats/stop_injdrv.bat";
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

                try
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                    Console.WriteLine(s);//If you are here the user clicked decline to grant admin privileges (or he's not administrator)
                }
                Dyn_checkBox.IsChecked = false;
            }
        }



        public partial class Log
        {
            [JsonProperty("FUNCT")]
            public string Funct { get; set; }

            [JsonProperty("PARAM")]
            public string Param { get; set; }

            [JsonProperty("PID")]
            public int Pid { get; set; }

            [JsonProperty("TID")]
            public int Tid { get; set; }

            [JsonProperty("TIME")]
            public string Time { get; set; }
        }

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }
        public partial class Log
        {
            public static Log FromJson(string json) => JsonConvert.DeserializeObject<Log>(json, Converter.Settings);
        }
        private void log_refresh()
        {
            LogList.Items.Clear();
            string path = @"C:\AV\log.txt";
            if (File.Exists(path))
            {
                string[] readText = File.ReadAllLines(path);
                foreach (string s in readText)
                {
                    if (!(s.Equals("/n") || s.Equals(" /n") || s.Equals("")))
                    {
                        if (s.Contains("{\"FUNCT\""))
                        {
                            Log temp = Log.FromJson(s);
                            LogList.Items.Add(new LogItem { TIME = temp.Time, PID = temp.Pid, TID = temp.Tid, Params = temp.Param, Function = temp.Funct });
                            Console.WriteLine(s);
                        }

                    }

                }
            }

            //dynamic stuff = JsonConvert.DeserializeObject("{ 'Name': 'Jon Smith', 'Address': { 'City': 'New York', 'State': 'NY' }, 'Age': 42 }");

            //string name = stuff.Name;
            //string address = stuff.Address.City;
        }
        private void Button_Click_7(object sender, RoutedEventArgs e) //обновление списка лога
        {
            log_refresh();

        }

        private void LogListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void LogList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void copy_install()
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            //myProcess.StartInfo.FileName = System.Environment.CurrentDirectory + "/bats/fix_links.bat";
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = @"/C cd " + System.Environment.CurrentDirectory + "  &  robocopy install C:/AV /E";
            myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.Start();
            myProcess.WaitForExit();

        }
        private void Fix_links()
        {
            if (File.Exists("C:/AV/bats/fix_links.bat"))
            {
                System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                myProcess.StartInfo.FileName = "C:/AV/bats/fix_links.bat";
                //myProcess.StartInfo.FileName = "cmd.exe";
                //myProcess.StartInfo.Arguments = @"/C cd " + System.Environment.CurrentDirectory + "/bats & start.bat";
                myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                myProcess.WaitForExit();
            }
        }
        private void Fix_links_button_Click(object sender, RoutedEventArgs e)
        {

            Fix_links();

        }
        private void Start_logging()
        {
            if (File.Exists("C:/AV/bin/logging/PipeServer.exe"))
            {
                loggingProcess = new System.Diagnostics.Process();
                loggingProcess.StartInfo.FileName = "C:/AV/bin/logging/PipeServer.exe";
                //myProcess.StartInfo.FileName = "cmd.exe";
                //myProcess.StartInfo.Arguments = @"/C cd " + System.Environment.CurrentDirectory + "/bats & start.bat";
                loggingProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                loggingProcess.StartInfo.CreateNoWindow = true;
                loggingProcess.Start();
                //loggingProcess.WaitForExit();
                Log_checkBox.IsChecked = true;
            }
        }
        private void Start_logging_Click(object sender, RoutedEventArgs e)
        {
            if (Log_checkBox.IsChecked == true)
            {
                MessageBox.Show("Already started");
                return;
            }
            Start_logging();
        }

        private void Copy_button_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(@"C:\AV"))
            {
                MessageBox.Show("Already installed");
                return;
            }
            if (loggingProcess != null)
            { loggingProcess.Kill();
                Log_checkBox.IsChecked = false;
            }
            Directory.Delete(@"C:\AV", true);
            copy_install();
            //Fix_links();
        }

        private void Log_clear_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.Delete(@"C:\AV\log.txt");
            log_refresh();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Log_checkBox_Copy_Checked(object sender, RoutedEventArgs e)
        {

        }
        private int CloudUpdate(string IPStr)
        {
            if (File.Exists("C:/AV/cloud/AV/update/client.exe"))
            {
                UpdatecloudProc = new System.Diagnostics.Process();
                UpdatecloudProc.StartInfo.FileName = "cmd.exe";
                UpdatecloudProc.StartInfo.Arguments = @"/C cd C:/AV/cloud/AV/update/ & client.exe " + IPStr;

                //cloudProc.StartInfo.FileName = "C:/AV/cloud/CloudClient.exe"; //проверить сохранность ярлыков при распаковке
                //cloudProc.StartInfo.Arguments = pathSrtr + @" down " + IPStr;
                UpdatecloudProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                //cloudProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;// нужно скрытое окно
                //cloudProc.StartInfo.CreateNoWindow = true;

                //myProcess.StartInfo.RedirectStandardOutput = true;
                //myProcess.StartInfo.UseShellExecute = false;
                UpdatecloudProc.Start();

                // To avoid deadlocks, always read the output stream first and then wait.  
                //string output = myProcess.StandardOutput.ReadToEnd();
                UpdatecloudProc.WaitForExit();

            }
            return 1;
        }
        private async void Update_button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsIpAddress(Ip_addrBox.Text))
            {
                MessageBox.Show("Wrong IP");
                return;
            }


            string pathSrtr = upload_textBox.Text;
            string IPStr = Ip_addrBox.Text;
            CloudUpdate_checkBox.IsChecked = true;
            int result = await Task.Factory.StartNew<int>(
                                         () => CloudUpdate(IPStr),
                                         TaskCreationOptions.LongRunning);

            CloudUpdate_checkBox.IsChecked = false;

        }

        private int stat_scan(object pathSrtr)
        {
            if (File.Exists("C:/AV/cloud/AV/sign/signPrint.exe"))
            {
                StaticProc = new System.Diagnostics.Process();
                StaticProc.StartInfo.FileName = "cmd.exe";
                StaticProc.StartInfo.Arguments = @"/C cd C:/AV/cloud/AV/sign/  &  signPrint.exe " + (string)pathSrtr;

                //cloudProc.StartInfo.FileName = "C:/AV/cloud/CloudClient.exe"; //проверить сохранность ярлыков при распаковке
                //cloudProc.StartInfo.Arguments = pathSrtr + @" down " + IPStr;
                StaticProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                //cloudProc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;// нужно скрытое окно
                StaticProc.StartInfo.CreateNoWindow = true;

                StaticProc.StartInfo.RedirectStandardOutput = true;
                StaticProc.StartInfo.UseShellExecute = false;
                StaticProc.Start();

                // To avoid deadlocks, always read the output stream first and then wait.  
                string output = StaticProc.StandardOutput.ReadToEnd(); //True - virus   false-norm
                StaticProc.WaitForExit();
                //TODO: добавить меседж бокс
                if (output.Contains("True"))
                {
                    MessageBox.Show("Virus File: " + (string)pathSrtr, "Static scan");
                    return 1;
                }
                if (output.Contains("False"))
                {
                    MessageBox.Show("OK File: " + (string)pathSrtr, "Static scan");
                    return 0;
                }

            }
            return 2;
        }
        Thread StatScanThread;
        private async void Static_scan_Click(object sender, RoutedEventArgs e)
        {
            if(upload_textBox.Text.Equals("TextBox") || !File.Exists(upload_textBox.Text))
            {
                MessageBox.Show("forget to choose file");
                return;
            }
            StatScan_checkBox.IsChecked = true;
            string pathSrtr = upload_textBox.Text;
            int result = await Task.Factory.StartNew<int>(
                                         () => stat_scan(pathSrtr),
                                         TaskCreationOptions.LongRunning);

            StatScan_checkBox.IsChecked = false;

        }

        private void Lab5_IP_URL1_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("C:/AV/bin/5lab/IP_URL.bat"))
            {
                var psi = new System.Diagnostics.ProcessStartInfo();
                psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
                psi.FileName = @"cmd.exe";
                psi.Verb = "runas"; //This is what actually runs the command as administrator
                psi.Arguments = "/C " + "C:/AV/bin/5lab/IP_URL.bat";
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

                try
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                    //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
                }

            }
        }

        private void Lab5_IP_URL2_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("C:/AV/bin/5lab/IP_URL2.bat"))
            {
                var psi = new System.Diagnostics.ProcessStartInfo();
                psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
                psi.FileName = @"cmd.exe";
                psi.Verb = "runas"; //This is what actually runs the command as administrator
                psi.Arguments = "/C " + "C:/AV/bin/5lab/IP_URL2.bat";
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

                try
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                    //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
                }
            }
        }

        private void Add_ref_button_Click(object sender, RoutedEventArgs e)
        {
            //File_exe_stop();
            Add_to_reg(reg_path_text.Text, "reg" + rulecount);
            refresh_list();
            //File_drv_start();
        }

        private System.Diagnostics.Process FFiltProcess;
        private void File_exe_stop()
        {
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = @"cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "taskkill /im file-filter-app.exe /f";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                //process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
        }
        private void File_drv_start()
        {
            if(File_Filt_checkBox.IsChecked==true)
            {
                MessageBox.Show("Already started");
                return;
            }
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = @"cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\file_drv\\file_start.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                FFiltProcess = new System.Diagnostics.Process();
                FFiltProcess.StartInfo = psi;
                FFiltProcess.Start();
                //process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
            File_Filt_checkBox.IsChecked = true;
        }
        private void File_start_button1_Click(object sender, RoutedEventArgs e)
        {
            File_drv_start();
        }

        private void File_stop_button1_Click(object sender, RoutedEventArgs e)
        {
            if (File_Filt_checkBox.IsChecked == false)
            {
                MessageBox.Show("Already stopped");
                return;
            }
            if (FFiltProcess != null)
            {
                try
                {
                    FFiltProcess.Kill();
                }
                catch
                {

                }
            }
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = @"cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\file_drv\\file_stop.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                //process.WaitForExit();
                File_exe_stop();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
            File_Filt_checkBox.IsChecked = false;
        }
        private void add_rule_file()
        {
            //regedit / s  "12.reg"

            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = @"cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\file_drv\\file_stop.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                //process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
        }
        private string IP_converter(String IP)
        {
            string[] Bytes = IP.Split('.');
            int i = 0;
            string[] HexBytes = new string[4];
            while (i < 4)
            {
                int temp = int.Parse(Bytes[i]);
                HexBytes[i] = temp.ToString("x2");
                i++;
            }
            string res = HexBytes[3] + HexBytes[2] + HexBytes[1] + HexBytes[0];






            return res;
        }
        private string Port_converter(String Port)
        {
            string res = "ffff0000";
            if (Port.Length != 0)
            {
                string[] Bytes = Port.Split('-');

                if (Bytes.Length == 1)
                {
                    int temp = int.Parse(Bytes[0]);
                    res = temp.ToString("x4") + temp.ToString("x4");
                }
                else
                {
                    int i = 0;
                    string[] HexBytes = new string[2];
                    while (i < 2)
                    {
                        int temp = int.Parse(Bytes[i]);
                        HexBytes[i] = temp.ToString("x4");
                        i++;
                    }
                    res = HexBytes[1] + HexBytes[0];
                }
            }



            return res;
        }
        private string data_converter(String data)
        {
            byte[] ba = Encoding.Default.GetBytes(DataPattText.Text);
            var hexString = BitConverter.ToString(ba);
            hexString = hexString.Replace("-", ",");
            return hexString;
        }
        private string param_converter()
        {
            int res = 0;
            if (DataPattText.Text.Length != 0)
            {
                res = res | 512;
            }

            //direction
            var direction = DirectionBox.SelectedItem.ToString();
            if (direction.Contains("In"))
            {
                res = res | 0;
            }
            else if (direction.Contains("Out"))

            {
                res = res | 1;
            }
            //protocol
            var protocol = ProtocolBox.SelectedItem.ToString();

            if (protocol.Contains("TCP"))
            {
                res = res | 12;
            }
            else if (protocol.Contains("UDP"))

            {
                res = res | 34;
            }
            else if (protocol.Contains("ICMP"))

            {
                res = res | 2;
            }
            String resStr = res.ToString("x8");
            return resStr;
        }

        private string pattern_param_converter()
        {


            if (DataOffsetText.Text.Length != 0)
            {
                int temp = int.Parse(DataOffsetText.Text);
                return temp.ToString("x8");
            }
            return "00000000";

        }
        private string action_converter()
        {
            int temp = 0;
            var action = ActionBox.SelectedItem.ToString();
            if (action.Contains("Log"))
            {
                temp = 0;
            }
            else if (action.Contains("Block"))

            {
                temp = 1;
            }
            return temp.ToString("x8");

        }
        private void add_net_reg_rule_()
        {
            //regedit / s  "12.reg"

            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\net_drv\\net_rule.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
        }
        int net_rul_num = 0;
        private void net_rules_refresh()
        {
            NetRuleList.Items.Clear();
            string path = @"C:\AV\net_drv\rule.txt";
            if (File.Exists(path))
            {
                string[] readText = File.ReadAllLines(path);
                net_rul_num = readText.Length;
                foreach (string s in readText)
                {
                    dynamic ruleObj = JsonConvert.DeserializeObject(s);
                    int num = ruleObj.number;

                    NetRuleList.Items.Add(ruleObj);
                    Console.WriteLine(s);
                }
            }
            else
            {
                net_rul_num = 0;
            }

            //dynamic stuff = JsonConvert.DeserializeObject("{ 'Name': 'Jon Smith', 'Address': { 'City': 'New York', 'State': 'NY' }, 'Age': 42 }");

            //string name = stuff.Name;
            //string address = stuff.Address.City;
        }
        private void send_to_net_drv()
        {

            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\net_drv\\restart.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                //Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
        }
        private void Add_net_rule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                net_rules_refresh();
                string TrafficAction = action_converter();
                string ParametersFlag = param_converter();
                string RemoteIpv4Address;
                string RemoteIpv4Mask;
                if (!RemIPText.Text.Equals("Any") || !RemIPText.Text.Equals("any"))
                {
                    if (!IsIpAddress(RemIPText.Text))
                    {
                        MessageBox.Show("Wrong remote IP");
                        return;
                    }
                    RemoteIpv4Address = IP_converter(RemIPText.Text);
                    RemoteIpv4Mask = "ffffffff";
                }
                else
                {
                    RemoteIpv4Address = "12345678";
                    RemoteIpv4Mask = "00000000";
                }


                //обработка ANY
                string LocalIpv4Address;
                string LocalIpv4Mask;
                if (!LocIPText.Text.Equals("Any") || !LocIPText.Text.Equals("any"))
                {
                    if (!IsIpAddress(LocIPText.Text))
                    {
                        MessageBox.Show("Wrong local IP");
                        return;
                    }
                    LocalIpv4Address = IP_converter(LocIPText.Text);
                    LocalIpv4Mask = "ffffffff";
                }
                else
                {
                    LocalIpv4Address = "12345678";
                    LocalIpv4Mask = "00000000";
                }
                if(LocPortText.Text.Length!=0)
                {
                     if (!IsPort(LocPortText.Text))
                    {
                        MessageBox.Show("Wrong local PORT");
                        return;
                    }
                }
                if (RemPortText.Text.Length != 0)
                {
                    if (!IsPort(RemPortText.Text))
                    {
                        MessageBox.Show("Wrong remote PORT");
                        return;
                    }
                }
                string TcpUdpRemotePort = Port_converter(RemPortText.Text);
                string TcpUdpLocalPort = Port_converter(LocPortText.Text);
                if (DataOffsetText.Text.Length != 0)
                {
                 if (!IsNum(DataOffsetText.Text))
                                {
                                    MessageBox.Show("Wrong offset");
                                    return;
                                        }
                }
                   

                string PatternParam = pattern_param_converter();
                string PatternData = data_converter(DataPattText.Text);

                string source = System.IO.File.ReadAllText(@"C:\AV\net_drv\rule.reg");

                source = source.Replace("rules\\1", "rules\\" + (net_rul_num+1));
                source = source.Replace("ffffff01", TrafficAction);
                source = source.Replace("ffffff02", ParametersFlag);
                source = source.Replace("ffffff03", RemoteIpv4Address);
                source = source.Replace("ffffff04", RemoteIpv4Mask);
                source = source.Replace("ffffff05", LocalIpv4Address);
                source = source.Replace("ffffff06", LocalIpv4Mask);
                source = source.Replace("ffffff07", TcpUdpRemotePort);
                source = source.Replace("ffffff08", TcpUdpLocalPort);
                source = source.Replace("ffffff09", PatternParam);
                source = source.Replace("ffffff10", PatternData);


                File.WriteAllText(@"C:\AV\net_drv\last_rule.reg", source);

                Net_rule ruleObj = new Net_rule();

                ruleObj.number = (net_rul_num + 1);
                ruleObj.Action = ActionBox.Text;
                ruleObj.Protocol = ProtocolBox.Text;
                ruleObj.LocalIP = LocIPText.Text;
                ruleObj.LocalPort = LocPortText.Text;
                ruleObj.RemIP = RemIPText.Text;
                ruleObj.RemPort = RemPortText.Text;
                ruleObj.DataPattern = DataPattText.Text;
                ruleObj.DataOffset = DataOffsetText.Text;
                ruleObj.Direction = DirectionBox.Text;

                string SerStr = JsonConvert.SerializeObject(ruleObj);

                StreamWriter myfile = new StreamWriter(@"C:\AV\net_drv\rule.txt", true);
                myfile.WriteLine(SerStr);
                myfile.Close();

                //dynamic stuff = JsonConvert.DeserializeObject("{ 'Name': 'Jon Smith', 'Address': { 'City': 'New York', 'State': 'NY' }, 'Age': 42 }");

                //string name = stuff.Name;
                //string address = stuff.Address.City;

                add_net_reg_rule_();
                net_rules_refresh();
                if (Net_Drv_checkBox.IsChecked == true)
                {
                    send_to_net_drv();
                }
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void LocIPText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LocPortText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            NetRuleList.Items.Clear();
            net_rules_refresh();
        }

        private void Net_Filter_start_button_Click(object sender, RoutedEventArgs e)
        {
            if (Net_Drv_checkBox.IsChecked == true)
            {
                MessageBox.Show("Already started");
                return;
            }
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\net_drv\\net_filt_start.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
            Net_Drv_checkBox.IsChecked = true;
        }

        private void Net_Filter__stop_button_Click(object sender, RoutedEventArgs e)
        {
            if (Net_Drv_checkBox.IsChecked == false)
            {
                MessageBox.Show("Already started");
                return;
            }
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\net_drv\\net_filt_stop.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
            Net_Drv_checkBox.IsChecked = false;
        }

        private void RemPortText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_8(object sender, RoutedEventArgs e) //del net rules
        {
            //REG DELETE HKLM\Software\MyCo\MyApp\Timeout
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + @"REG DELETE HKLM\SYSTEM\CurrentControlSet\Services\netfilter /f";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }

            File.Delete(@"C:\AV\net_drv\rule.txt");
            net_rules_refresh();
            if (Net_Drv_checkBox.IsChecked == true)
            {
                send_to_net_drv();
            }
        }


        System.Diagnostics.Process Stat_Process;
        private void Stat_start_button_Click(object sender, RoutedEventArgs e)
        {
            if (Static_checkBox_Copy.IsChecked == true)
            {
                MessageBox.Show("Already started");
                return;
            }
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + @"C:\AV\cloud\AV\sign\start.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                Stat_Process = new System.Diagnostics.Process();
                Stat_Process.StartInfo = psi;
                Stat_Process.Start();
                //Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }

            Static_checkBox_Copy.IsChecked = true;
        }

        private void Stat_stop_button_Click(object sender, RoutedEventArgs e)
        {
            if (Static_checkBox_Copy.IsChecked == false)
            {
                MessageBox.Show("Already stoped");
                return;
            }
            if (Stat_Process != null)
            {
                try
                {
                    Stat_Process.Kill();
                }
                catch
                {

                }
            }

            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + @"C:\AV\cloud\AV\sign\stop.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
            Static_checkBox_Copy.IsChecked = false;
        }

        private void Net_Filter_restart_button_Click(object sender, RoutedEventArgs e)
        {
            if (Net_Drv_checkBox.IsChecked == false)
            {
                MessageBox.Show("Already stopped");
                return;
            }
            send_to_net_drv();
        }
        private void File_drv_log_refresh()
        {
            File_log_List.Items.Clear();
            string path = @"C:\AV\file_drv\Release\log_json.txt";
            if (File.Exists(path))
            {
                string[] readText = File.ReadAllLines(path);
                net_rul_num = readText.Length;
                foreach (string s in readText)
                {
                    string temp = s.Replace('\\', '/');
                    FileLogItem logObj = JsonConvert.DeserializeObject<FileLogItem>(temp);

                    File_log_List.Items.Add(logObj);
                    Console.WriteLine(temp);
                }
            }
        }

        private void File_drv_log_refresh_Click(object sender, RoutedEventArgs e)
        {
            File_drv_log_refresh();
        }

        private void File_drv_log_clear_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.Delete(@"C:\AV\file_drv\Release\log_json.txt");
            File_drv_log_refresh();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(@"C:\AV"))
            {
                MessageBox.Show("Already installed");
                return;
            }
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = "cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + "C:\\AV\\bats\\drv_inf_install.bat";
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            try
            {
                var Process = new System.Diagnostics.Process();
                Process.StartInfo = psi;
                Process.Start();
                Process.WaitForExit();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
            }
        }

        private bool IsIpAddress(string Address)
        {
            //Инициализируем новый экземпляр класса System.Text.RegularExpressions.Regex
            //для указанного регулярного выражения.
            System.Text.RegularExpressions.Regex IpMatch = new System.Text.RegularExpressions.Regex(@"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
            //Выполняем проверку обнаружено ли в указанной входной строке
            //соответствие регулярному выражению, заданному в
            //конструкторе System.Text.RegularExpressions.Regex.
            //если да то возвращаем true, если нет то false
            return IpMatch.IsMatch(Address);
        }

        private bool IsPort(string Address)
        {
            //Инициализируем новый экземпляр класса System.Text.RegularExpressions.Regex
            //для указанного регулярного выражения.
            System.Text.RegularExpressions.Regex IpMatch = new System.Text.RegularExpressions.Regex(@"\b([1-9]|[1-5]?[0-9]{2,4}|6[1-4][0-9]{3}|65[1-4][0-9]{2}|655[1-2][0-9]|6553[1-5])\b");
            //Выполняем проверку обнаружено ли в указанной входной строке
            //соответствие регулярному выражению, заданному в
            //конструкторе System.Text.RegularExpressions.Regex.
            //если да то возвращаем true, если нет то false
            return IpMatch.IsMatch(Address);
        }
        private bool IsNum(string Address)
        {
            //Инициализируем новый экземпляр класса System.Text.RegularExpressions.Regex
            //для указанного регулярного выражения.
            System.Text.RegularExpressions.Regex IpMatch = new System.Text.RegularExpressions.Regex(@"\b[0-9]{1,5}\b");
            //Выполняем проверку обнаружено ли в указанной входной строке
            //соответствие регулярному выражению, заданному в
            //конструкторе System.Text.RegularExpressions.Regex.
            //если да то возвращаем true, если нет то false
            return IpMatch.IsMatch(Address);
        }
    }

    internal class LogItem
    {
        public string TIME { get; set; }
        public int PID { get; set; }
        public int TID { get; set; }
        public string Params { get; set; }
        public string Function { get; set; }
    }

    internal class FileLogItem
    {
        public string Event { get; set; }
        public string data { get; set; }
        public string time { get; set; }
    }

    internal class Net_rule
    {
        public int number { get; set; }
        public string Action { get; set; }
        public string Protocol { get; set; }
        public string LocalIP { get; set; }
        public string LocalPort { get; set; }
        public string RemIP { get; set; }
        public string RemPort { get; set; }
        public string DataPattern { get; set; }
        public string DataOffset { get; set; }
        public string Direction { get; set; }
    }
}
