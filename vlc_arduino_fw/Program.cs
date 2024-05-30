using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Management;
using ArduinoUploader;
using ArduinoUploader.Hardware;

namespace vlc_arduino_fw {
    class Program {
        private static System.Threading.Timer close_program;
        private static bool debug = false;
        static ArduinoSketchUploader uploader;
        private static string port = "COM5";
        static void Main(string[] args) {
            string hex = "design.hex";
            while (true) {
                try { port = File.ReadAllText("vlc_arduino_fw_port.txt"); break; } catch { Thread.Sleep(50); }
            }
            File.WriteAllText("call_exe_tric.txt", "");

            try { hex = File.ReadAllText("vlc_arduino_fw_hex.txt"); } catch { }
            close_program = new System.Threading.Timer(TimerCallback, null, 0, 25000);
            uploader = new ArduinoSketchUploader(new ArduinoSketchUploaderOptions() {
                FileName = hex,
                PortName = port,
                ArduinoModel = ArduinoModel.NanoR3});

            Console.WriteLine("Comport = " + port);
            Console.WriteLine("Hex = " + hex);

            uploader.UploadSketch();
            if (!Directory.Exists("result")) Directory.CreateDirectory("result");
            File.WriteAllText("result/vlc_"+ port + "_OK.txt", "");
        }

        private static bool flag_close = false;
        private static void TimerCallback(Object o) {
            if (!flag_close) { flag_close = true; return; }
            if (debug) return;
            File.WriteAllText("result/vlc_" + port + "_Err.txt", "");
            Environment.Exit(0);
        }

        private static void get_name_e2lite() {
            ManagementObjectSearcher objOSDetails2 =
               new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity where DeviceID Like ""USB%""");
            ManagementObjectCollection osDetailsCollection2 = objOSDetails2.Get();
            foreach (ManagementObject usblist in osDetailsCollection2) {
                string arrport = usblist.GetPropertyValue("NAME").ToString();
                if (arrport.Contains("Renesas")) {
                    name_st = arrport;
                }
            }
        }
        private static string name_st = "Renesas E2 Lite";
        private static void discom(string cmd) {//enable disable//
            Process devManViewProc = new Process();
            devManViewProc.StartInfo.FileName = "DevManView.exe";
            devManViewProc.StartInfo.Arguments = "/" + cmd + " \"" + name_st + "\"";
            devManViewProc.Start();
            devManViewProc.WaitForExit();
        }
    }
}
