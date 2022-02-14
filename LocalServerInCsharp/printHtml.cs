using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;
using System.Windows.Forms;
using ESC_POS_USB_NET.Printer;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

namespace LocalServerInCsharp
{
    class printHtml
    {

        public void PrintImageEsc()
        {
            string curDir = Directory.GetCurrentDirectory();
            PrinterSettings settings = new PrinterSettings();
            Console.WriteLine(settings.PrinterName);

            Bitmap image = new Bitmap(Bitmap.FromFile(curDir+"\\data.jpg"));
            Printer printer = new Printer(settings.PrinterName);
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
                string curDir = Directory.GetCurrentDirectory();
                //if (File.Exists(curDir + "\\data.jpg"))
                //{
                //    File.Delete(curDir + "\\data.jpg");
                //}
                var webBrowser = new WebBrowser();
                webBrowser.ScrollBarsEnabled = true;
                webBrowser.IsWebBrowserContextMenuEnabled = true;
                webBrowser.AllowNavigation = true;

                webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
                //webBrowser.DocumentText = source;
                webBrowser.Url = new Uri(String.Format("file:///{0}/data.html", curDir));
                //webBrowser.Dispose();

                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }
        static void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string curDir = "c:\\temp";

            if (File.Exists(curDir + "\\data.jpg"))
            {
                File.Delete(curDir + "\\data.jpg");
            }

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
            }
            webBrowser.Dispose();
            //System.Threading.Thread.Sleep(1000);
            printHtml ph = new printHtml();
            ph.PrintImageEsc();

        }


        public void print_image_doc() {
            try
            {
                using (PrintDocument pd = new PrintDocument())
                {
                    pd.PrintPage += new PrintPageEventHandler(printDoc_Print);
                    pd.PrintPage += (this.pd_PrintPage);
                    pd.Print();
                }
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
        public static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            //encryptor.KeySize = 256;
            //encryptor.BlockSize = 128;
            //encryptor.Padding = PaddingMode.Zeros;

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

            // Convert the plainText string into a byte array
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            // Encrypt the input plaintext string
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);

            // Complete the encryption process
            cryptoStream.FlushFinalBlock();

            // Convert the encrypted data from a MemoryStream to a byte array
            byte[] cipherBytes = memoryStream.ToArray();

            // Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();

            // Convert the encrypted byte array to a base64 encoded string
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

            // Return the encrypted data as a string
            return cipherText;
        }

        public string DecryptString(string cipherText)
        {

            string password = "pos@keeper";

            // Create sha256 hash
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));

            // Create secret IV
            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };


            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            //encryptor.KeySize = 256;
            //encryptor.BlockSize = 128;
            //encryptor.Padding = PaddingMode.Zeros;

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string
            return plainText;
        }
    }

}
