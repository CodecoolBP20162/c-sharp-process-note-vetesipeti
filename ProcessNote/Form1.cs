using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace ProcessNote
{
    public partial class Form1 : Form
    {
        PerformanceCounter totalCpuCounter;
        PerformanceCounter totalRamCounter;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitPerformanceCounters()
        {
            totalCpuCounter = new PerformanceCounter
               ("Processor", "% Processor Time", "_Total");
            totalRamCounter = new PerformanceCounter
                ("Memory", "Available MBytes");
        }
        

        public string BytesToReadableValue(long number)
        {
            List<string> suffixes = new List<string> { " B", " KB", " MB", " GB", " TB", " PB" };

            for (int i = 0; i < suffixes.Count; i++)
            {
                long temp = number / (int)Math.Pow(1024, i + 1);

                if (temp == 0)
                {
                    return (number / (int)Math.Pow(1024, i)) + suffixes[i];
                }
            }

            return number.ToString();
        }
    
        public void RenderProcessesOnListView()
        {
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                string[] attributes =
                {
                    process.ProcessName,
                    process.Id.ToString(),
                    BytesToReadableValue(process.PrivateMemorySize64),                    
                };
                ListViewItem item = new ListViewItem(attributes);
                listView1.Items.Add(item);

            }            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitPerformanceCounters();
            RenderProcessesOnListView();
            timer1.Interval = 300;
            timer2.Interval = 1000;
            timer1.Start();
            timer2.Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = !TopMost;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //String.Format("{0:##0} %", cpuCounter.NextValue());
            totalMemory.Text = String.Format("{0} GB",Math.Round(7.9 - (totalRamCounter.NextValue()/1000), 1).ToString());
            Process[] processList = Process.GetProcesses();
            List<string> processIds = new List<string>();
            List<string> listViewProcessIds = new List<string>();

            foreach (Process process in processList) processIds.Add(process.Id.ToString());
            foreach (ListViewItem item in listView1.Items) listViewProcessIds.Add(item.SubItems[1].Text);

            if (processList.Length == listView1.Items.Count)
            {
                foreach (Process process in processList)
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        if (item.SubItems[1].Text == process.Id.ToString() && item.SubItems[2].Text != BytesToReadableValue(process.PrivateMemorySize64))
                        {
                            item.SubItems[2].Text = BytesToReadableValue(process.PrivateMemorySize64);
                            break;
                        }
                    }
                }
            }
            else if (processList.Length > listView1.Items.Count)
            {
                foreach (Process process in processList)
                {
                    if (!listViewProcessIds.Contains(process.Id.ToString()))
                    {
                        string[] attributes = {
                            process.ProcessName,
                            process.Id.ToString(),
                            BytesToReadableValue(process.PrivateMemorySize64),
                        };
                        ListViewItem newItem = new ListViewItem(attributes);
                        listView1.Items.Add(newItem);
                    }

                }
            }
            else
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (!processIds.Contains(item.SubItems[1].Text)) listView1.Items.Remove(item);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            totalCpu.Text = Math.Round(totalCpuCounter.NextValue(), 0).ToString() + " %";  
        }
    }
}
