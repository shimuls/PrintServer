﻿using System;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LocalServerInCsharp
{
    class Program
    {
        static HttpListener _httpListener = new HttpListener();
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();


        static NotifyIcon notifyIcon;
        static IntPtr processHandle;
        static IntPtr WinShell;
        static IntPtr WinDesktop;
        static MenuItem HideMenu;
        static MenuItem RestoreMenu;



        static void Main(string[] args)
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("terminal.ico");
            notifyIcon.Text = "TerminalBD";
            notifyIcon.Visible = true;

            ContextMenu menu = new ContextMenu();
            HideMenu = new MenuItem("Hide", new EventHandler(Minimize_Click));
            RestoreMenu = new MenuItem("Restore", new EventHandler(Maximize_Click));

            menu.MenuItems.Add(RestoreMenu);
            menu.MenuItems.Add(HideMenu);
            menu.MenuItems.Add(new MenuItem("Exit", new EventHandler(CleanExit)));

            notifyIcon.ContextMenu = menu;

            //You need to spin off your actual work in a different thread so that the Notify Icon works correctly
            Task.Factory.StartNew(Run);

            processHandle = Process.GetCurrentProcess().MainWindowHandle;

            WinShell = GetShellWindow();

            WinDesktop = GetDesktopWindow();

            //Hide the Window
            ResizeWindow(false);
            

            Console.WriteLine("Starting server...");
            _httpListener.Prefixes.Add("http://localhost:9000/"); // add prefix "http://localhost:5000/"
            _httpListener.Start(); // start server (Run application as Administrator!)
            Console.WriteLine("Server started.");
            Thread _responseThread = new Thread(ResponseThread);
            _responseThread.SetApartmentState(ApartmentState.STA);
            _responseThread.Start(); // start the response thread
            
            Application.Run();
        }
        static void ResponseThread()
        {
            while (true)
            {
                HttpListenerContext context = _httpListener.GetContext(); // get a context
                HttpListenerRequest request = context.Request;
                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                //Console.WriteLine($"Recived request for {request.Url}");
                File.WriteAllText("data.html", documentContents);
                //Console.WriteLine(documentContents);
                Console.WriteLine("Printing...");
                printHtml phtml = new printHtml();
                phtml.htmlToImage();

                // Now, you'll find the request URL in context.Request.Url
                byte[] _responseArray = Encoding.UTF8.GetBytes("<html><head><title>Localhost server -- port 9000</title></head>" + 
                    "<body>Welcome to the <strong>Localhost server</strong> -- <em>port 5000!</em></body></html>"); // get the bytes to response
                context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                context.Response.KeepAlive = false; // set the KeepAlive bool to false
                context.Response.Close(); // close the connection
                //Console.WriteLine("Respone given to a request.");
            }
        }
        static void Run()
        {
            Console.WriteLine("Listening to messages");

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }


        private static void CleanExit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
            Environment.Exit(1);
        }


        static void Minimize_Click(object sender, EventArgs e)
        {
            ResizeWindow(false);
        }


        static void Maximize_Click(object sender, EventArgs e)
        {
            ResizeWindow();
        }

        static void ResizeWindow(bool Restore = true)
        {
            if (Restore)
            {
                RestoreMenu.Enabled = false;
                HideMenu.Enabled = true;
                SetParent(processHandle, WinDesktop);
            }
            else
            {
                RestoreMenu.Enabled = true;
                HideMenu.Enabled = false;
                SetParent(processHandle, WinShell);
            }
        }



    }
}
