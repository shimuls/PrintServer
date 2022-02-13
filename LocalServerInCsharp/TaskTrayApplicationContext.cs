using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace PrintServer
{
    class TaskTrayApplicationContext
    {
       
        static NotifyIcon notifyIcon = new NotifyIcon();
        static bool Visible = true;
        //Configuration configWindow = new Configuration();

        public TaskTrayApplicationContext()
        {
            //MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            //notifyIcon.Icon = ;
            notifyIcon.DoubleClick += new EventHandler(ShowMessage);
            //notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { configMenuItem, exitMenuItem });
            notifyIcon.Visible = true;
        }


        void ShowMessage(object sender, EventArgs e)
        {
            // Only show the message if the settings say we can.
            if (PrintServer.Properties.Settings.Default.ShowMessage)
                MessageBox.Show("This is the Serenity TaskTray Agent.");
        }



        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;

            Application.Exit();
        }
    }
}
