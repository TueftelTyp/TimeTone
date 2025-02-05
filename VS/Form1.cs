using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace TimeTone
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer checkTimer;
        private MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
        private MMDevice defaultDevice;
        private float volVar = 0.0f;

        public MainForm()
        {
            InitializeComponent();

            // DateTimePicker für Uhrzeiten formatieren
            dtpStart.Format = DateTimePickerFormat.Custom;
            dtpStart.CustomFormat = "HH:mm";
            dtpStart.ShowUpDown = true;

            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.CustomFormat = "HH:mm";
            dtpEnd.ShowUpDown = true;
            dtpEnd.Value = DateTime.Now.AddHours(1);

            // Standard-Audio-Device abrufen
            defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        private void tbVolume_Scroll(object sender, EventArgs e)
        {
            volVar = tbVolume.Value / 100f;
            txtVolVar.Text = tbVolume.Value.ToString();
            AudioControl.SetMasterVolume(volVar);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (chkMonday.Checked || chkTuesday.Checked || chkWednesday.Checked ||
                chkThursday.Checked || chkFriday.Checked || chkSaturday.Checked ||
                chkSunday.Checked)
            {
                dtpStart.Enabled = false;
                dtpEnd.Enabled = false;
                chkMonday.Enabled = false;
                chkTuesday.Enabled = false;
                chkWednesday.Enabled = false;
                chkThursday.Enabled = false;
                chkFriday.Enabled = false;
                chkSaturday.Enabled = false;
                chkSunday.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                tbVolume.Enabled = false;

                checkTimer = new System.Timers.Timer(10000); // Alle 10 Sekunden prüfen
                checkTimer.Elapsed += CheckTimeAndSetVolume;
                checkTimer.Start();
            }
            else
            {
                MessageBox.Show("Set at least one day.");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (chkMonday.Checked || chkTuesday.Checked || chkWednesday.Checked ||
                chkThursday.Checked || chkFriday.Checked || chkSaturday.Checked ||
                chkSunday.Checked)
            {   
                dtpStart.Enabled = true;
                dtpEnd.Enabled = true;
                chkMonday.Enabled = true;
                chkTuesday.Enabled = true;
                chkWednesday.Enabled = true;
                chkThursday.Enabled = true;
                chkFriday.Enabled = true;
                chkSaturday.Enabled = true;
                chkSunday.Enabled = true;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                tbVolume.Enabled = true;

                checkTimer.Stop();
                checkTimer.Dispose();

                AudioControl.SetMasterVolume(0.3f);
                tbVolume.Value = 30;
                txtVolVar.Text = "30";
            }
            else
            {
                return;
            }
        }


        private void CheckTimeAndSetVolume(object sender, ElapsedEventArgs e)
        {
            // Hole aktuelle Zeit & Wochentag
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            DayOfWeek currentDay = DateTime.Now.DayOfWeek;

            // Hole die gesetzte Zeitspanne
            TimeSpan startTime = dtpStart.Value.TimeOfDay;
            TimeSpan endTime = dtpEnd.Value.TimeOfDay;

            // Prüfe, ob der aktuelle Tag aktiviert wurde
            bool isDayActive = GetCheckBoxForDay(currentDay).Checked;

            // Prüfe, ob wir innerhalb der erlaubten Zeit sind
            bool isInTimeRange = startTime <= currentTime && currentTime <= endTime;

            // Wenn der Tag aktiv ist und die Zeitspanne passt, Lautstärke anpassen
            if (isDayActive && isInTimeRange)
            {
                AudioControl.SetMasterVolume(volVar);// 100% Lautstärke
            }
            else
            {
                AudioControl.SetMasterVolume(0.0f); // Lautstärke aus
            }
        }

        public class AudioControl
        {
            public static void SetMasterVolume(float volume)
            {
                using (var deviceEnumerator = new MMDeviceEnumerator())
                {
                    var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
                }
            }
        }

        private CheckBox GetCheckBoxForDay(DayOfWeek day)
        {
            if (day == DayOfWeek.Monday) return chkMonday;
            if (day == DayOfWeek.Tuesday) return chkTuesday;
            if (day == DayOfWeek.Wednesday) return chkWednesday;
            if (day == DayOfWeek.Thursday) return chkThursday;
            if (day == DayOfWeek.Friday) return chkFriday;
            if (day == DayOfWeek.Saturday) return chkSaturday;
            if (day == DayOfWeek.Sunday) return chkSunday;

            return null;
        }

        private void lblUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/TueftelTyp/TimeTone";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}