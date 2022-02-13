using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;
using System.Windows.Forms;
using ESC_POS_USB_NET.Printer;
using System.Threading;

namespace LocalServerInCsharp
{
    class printHtml
    {
        public void PrintImageEsc()
        {
            string curDir = Directory.GetCurrentDirectory();
            PrinterSettings settings = new PrinterSettings();
            Console.WriteLine(settings.PrinterName);

            Printer printer = new Printer(settings.PrinterName);
            Bitmap image = new Bitmap(Bitmap.FromFile(curDir+"\\data.jpg"));
            printer.Image(image);
            printer.FullPaperCut();
            printer.PrintDocument();
        }

        private readonly System.ComponentModel.Container components;
        //private System.Windows.Forms.Button printButton;
        private Font printFont;
        private StreamReader streamToPrint;


        public  void htmlToImage()
        {
            var th = new Thread(() =>
            {
                var webBrowser = new WebBrowser();
                webBrowser.ScrollBarsEnabled = true;
                webBrowser.IsWebBrowserContextMenuEnabled = true;
                webBrowser.AllowNavigation = true;

                webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
                //webBrowser.DocumentText = source;
                string curDir = Directory.GetCurrentDirectory();
                webBrowser.Url = new Uri(String.Format("file:///{0}/data.html", curDir));

                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }
        static void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string curDir = Directory.GetCurrentDirectory();

            Console.WriteLine(curDir + "\\data.jpg");
            var webBrowser = (WebBrowser)sender;
            webBrowser.Size = webBrowser.Document.Body.ScrollRectangle.Size;
            using (Bitmap bitmap =
                new Bitmap(
                    webBrowser.Width,
                    webBrowser.Height))
            {
                webBrowser
                    .DrawToBitmap(
                    bitmap,
                    new System.Drawing
                        .Rectangle(0, 0, bitmap.Width, bitmap.Height));
                bitmap.Save(curDir+"\\data.jpg",
                    System.Drawing.Imaging.ImageFormat.Jpeg);
                
                printHtml ph = new printHtml();
                ph.PrintImageEsc();
            }

        }


        public void print_image_doc() {
            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += new PrintPageEventHandler(printDoc_Print);
                printDoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void printDoc_Print(object sender, PrintPageEventArgs ev)
        {
            try
            {
                string curDir = Directory.GetCurrentDirectory();
                Image img = Image.FromFile(curDir+"\\data.jpg");
                PointF pf = new PointF(10, 10);
                ev.Graphics.DrawImage(img, pf);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void PrintHelpPage()
        {
            // Create a WebBrowser instance. 
            WebBrowser webBrowserForPrinting = new WebBrowser();

            // Add an event handler that prints the document after it loads.
            webBrowserForPrinting.DocumentCompleted +=
                new WebBrowserDocumentCompletedEventHandler(PrintDocument);

            string curDir = Directory.GetCurrentDirectory();
            // Set the Url property to load the document.
            webBrowserForPrinting.Url = new Uri(String.Format("file:///{0}/data.html", curDir));
        }

        private void PrintDocument(object sender,
            WebBrowserDocumentCompletedEventArgs e)
        {
            // Print the document now that it is fully loaded.
            ((WebBrowser)sender).Print();

            // Dispose the WebBrowser now that the task is complete. 
            ((WebBrowser)sender).Dispose();
        }



        public void PrintHtml()
        {
            try
            {
                streamToPrint = new StreamReader
                   ("data.txt");
                try
                {
                    //printFont = new Font("Arial", 10);
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler
                       (this.pd_PrintPage);
                    pd.Print();
                }
                finally
                {
                    streamToPrint.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }


        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while (count < linesPerPage &&
               ((line = streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }
    }

}
