using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindCare
{
    public partial class infoMindCare : Form
    {
        public infoMindCare()
        {
            InitializeComponent();
            ReproducirVideo();


        }


        private void ReproducirVideo()
        {
            string videoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logo.mp4");
            axWindowsMediaPlayer1.URL = videoPath;
            axWindowsMediaPlayer1.settings.autoStart = true;
            axWindowsMediaPlayer1.settings.setMode("loop", true);


        }
    }
}
