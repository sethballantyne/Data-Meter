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

namespace DataMeter
{
    public partial class Configure : Form
    {
        MainForm mainForm;

        public Configure()
        {
            InitializeComponent();
        }

        public Configure(NetworkInterface[] availableNICs, MainForm callingForm) :
            this()
        {
            mainForm = callingForm;

            foreach (NetworkInterface nic in availableNICs)
            {
                nicComboBox.Items.Add(nic.Description);
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            int index = nicComboBox.SelectedIndex;

            mainForm.SelectedNIC = index;
            app.Default.NIC = mainForm.AvailableNICs[index].Description;

            app.Default.Save();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Configure_Shown(object sender, EventArgs e)
        {
            nicComboBox.SelectedIndex = mainForm.SelectedNIC;
        }
    }
}
