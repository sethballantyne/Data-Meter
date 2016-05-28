/*Copyright (c) 2016 Seth Ballantyne <seth.ballantyne@gmail.com>
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in
*all copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
*THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Configuration;

namespace DataMeter
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The available nics on the machine.
        /// </summary>
        static NetworkInterface[] nics;

        /// <summary>
        /// The description of the nic currently selected. 
        /// Eg, Realtek PCIe GBE Family Controller.
        /// </summary>
        string nicName;

        /// <summary>
        /// Index of the selected nic
        /// </summary>
        int nicIndex = -1;

        /// <summary>
        /// The dialog used to configure the application
        /// </summary>
        Configure cfgDialog;

        public MainForm()
        {
            InitializeComponent();      
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
#if DEBUG
            this.Show();

            WindowState = FormWindowState.Normal;
#endif
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Hide();

            try
            {
                nics = NetworkInterface.GetAllNetworkInterfaces();

                cfgDialog = new Configure(nics, this);

                nicName = app.Default.NIC;

                for (int i = 0; i < nics.Length; i++)
                {
                    if (nics[i].Description == nicName)
                    {
                        nicIndex = i;
                    }
                }

                if (nicIndex == -1)
                {
                    DialogResult dr;

                    dr = MessageBox.Show(Strings.MissingNICPrompt, Strings.MissingNICPromptTitle,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (dr == DialogResult.Yes)
                    {
                        cfgDialog.Show();
                    }

                }

            }
            catch (NetworkInformationException nie)
            {
                string msg = String.Format("Failed to retrieve network interfaces: {0}\nExiting.", nie.Message);

                MessageBox.Show(msg, "Unholy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // no nics = program can't do its job.
                Application.Exit();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!cfgDialog.Visible)
                cfgDialog.Show();
        }

        public int SelectedNIC
        {
            get
            {
                return nicIndex;
            }
            set
            {
                nicIndex = value;
            }
        }

        public NetworkInterface[] AvailableNICs
        {
            get
            {
                return nics;
            }
        }

        private void notifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (nicIndex != -1)
            {
                string uploadedData = Convert.ToString(
                    nics[nicIndex].GetIPv4Statistics().BytesSent
                    );

                string downloadedData = Convert.ToString(
                    nics[nicIndex].GetIPv4Statistics().BytesReceived
                    );
#if DEBUG
                uploadCount.Text = uploadedData;
                downloadCount.Text = downloadedData;
#endif

                string uploadText = MeasurementFormatter.Format(
                    nics[0].GetIPv4Statistics().BytesSent);
                string downloadText = MeasurementFormatter.Format(
                    nics[0].GetIPv4Statistics().BytesReceived);

                notifyIcon.Text = String.Format("Upload: {0} Download: {1}", uploadText, downloadText);
            }
            else
            {
                notifyIcon.Text = Strings.NoNICSelectedNotifyIconPrompt;
            }
        }
    }
}
