using System;
using System.IO;
using System.Collections.Generic;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
//using Odyssey.Controls;
//using DevComponents.DotNetBar;
//using DevComponents.DotNetBar.Rendering;

using System.Runtime.InteropServices;

namespace BitmapTest
{

    //baseColorScheme = eOffice2007ColorScheme.Black;
//RibbonPredefinedColorSchemes.ChangeOffice2007ColorTable(this, baseColorScheme, Color.FromArgb(56, 129, 148));

   // public partial class Form1 : Office2007Form
            public partial class Form1 : Form
	{
		private Bitmap drawBmp;
        SerialPort SP;
        Thread trd;
        int isPortOpened;
        //string fileList = "";
        char[]  fileList;
        int index1;
        int isTextReady;
        int fileStartReceived;
        int fileEndReceived;
        int subIndex1;
        int subIndex2;
        int isFileReading;
        int isHashReceived;
        int isDollarReceived;
        bool isFileListAvailable = false;
               // DateTime time = new DateTime();
                long time = 0;
        //eOffice2007ColorScheme baseColorScheme;
        Color baseColor;

		public Form1()
		{
			drawBmp = null;
            fileList = new char[10000];
            index1 = 0;
            fileEndReceived = 0;
            fileStartReceived = 0;
            subIndex1 = 0;
            subIndex2 = 0;
            isFileReading = 0;
            isHashReceived = 0;
            isDollarReceived = 0;
			InitializeComponent();
 //           baudRate = 19200;
 //           SP = new SerialPort(ComName, baudRate, System.IO.Ports.Parity.None, dataBits, System.IO.Ports.StopBits.One);
		}

		private void button1_Click(object sender, EventArgs e)
		{
            string ComName = "";
            int baudRate;
            int dataBits = 8;
            char[] x = new char[5];

            index1 = 0;
            
            subIndex1 = 0;
            subIndex2 = 0;
            fileStartReceived = 0;
            isFileReading = 100;

            listBox1.Items.Clear();

            if (isPortOpened == 0)
            {
                
                try
                {
                    ComName = comboBox1.SelectedItem.ToString();
                }
                catch (Exception ec)
                {
                    MessageBox.Show("Please Select a COM port");
                    return;
                }
               baudRate = 19200;
               SP = new SerialPort(ComName, baudRate, System.IO.Ports.Parity.None, dataBits, System.IO.Ports.StopBits.One);

                try
                {

                    SP.Open();
                    if (SP.IsOpen)
                    {
                        button1.Text = "Disconnect";
                        connectToolStripMenuItem.Text = "Disconnect";
                        // richTextBox1.AppendText("Connected Man");
                    }
                    SP.DiscardInBuffer();
                    SP.DiscardOutBuffer();
                    //MessageBox.Show("Port Opened");
                   trd = new Thread(new ThreadStart(this.ThreadTask));
                   
                  // trd.IsBackground = true;
                    trd.Start();
                    
                   // label6.Text = "         ";

                }
                catch (Exception ec)
                {
                    MessageBox.Show(ec.Message);
                }
               // SP.DataReceived += new SerialDataReceivedEventHandler(SerialReceive);
                isPortOpened = 1;

                            
           // x[0] = 'a';
           // x[1] = 'b';
           // x[2] = 'b';
           // x[3] = 'b';
           // x[4] = '\0';
           // string a = new string(x);
           //// a = (string) Convert.ToString( 
           // richTextBox1.AppendText(a);

            }
            else //if (isPortOpened == 1)
            {

                try
                {
                    button1.Text = "Connect";
                    connectToolStripMenuItem.Text = "Connect";
                    isPortOpened = 0;

                    if (SP.IsOpen)
                    {
                        SP.Close();
                    }
                }catch(Exception ec){

                    MessageBox.Show(ec.Message);

                }
            }

            
		}
      

         
		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			if (drawBmp != null)
			{
				Graphics g = e.Graphics;
				g.DrawRectangle(Pens.Blue, new Rectangle(10, 10, drawBmp.Width, drawBmp.Height));
				g.DrawImage(drawBmp, new Point(10, 10));
				//g.DrawLine(Pens.Red, 0, 0, 10, 10);
			}
		}

        private void Form1_Load(object sender, EventArgs e)
        {
            //RibbonPredefinedColorSchemes.ChangeOffice2007ColorTable(this, eOffice2007ColorScheme.Black, Color.Black); 
            //baseColorScheme = eOffice2007ColorScheme.Black;
            //baseColor = Color.FromArgb(56, 129, 148);
            //RibbonPredefinedColorSchemes.ChangeOffice2007ColorTable(this, baseColorScheme, baseColor);

            string[] ports = SerialPort.GetPortNames();
            //Form1.DefaultBackColor = Color.Black;
            listBox1.ScrollAlwaysVisible = true;
            fileList.Initialize();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }

            InstalledFontCollection fontsch = new InstalledFontCollection();

            for (int u = 0; u < 170;u++ )
            {
                //comboBox1.Items.Add(fontsch.Families[u].Name);
            }

            for (int ui = 8; ui < 14; ui++)
            {
                //comboBox2.Items.Add(Convert.ToString(ui+1));

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string cmdLS = "LS\r\n";
            
            isFileReading = 0;
            isFileListAvailable = true;
            try
            {

                SP.Write(cmdLS);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Not Connected to Storage", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                isFileListAvailable = false;
            }

            //try
            //{
               
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);

            //}

        }


        void ThreadTask()
        {
            while (true)
            {
                int inputC = -1;
                if (isPortOpened == 1)
                {
                    if (SP.IsOpen)
                    {

                        if (SP.BytesToRead > 0)
                        {

                            inputC = SP.ReadByte();
                            if (inputC == 0)
                            {
                                inputC = 8;
                            }
                            fileList[index1] = (char)inputC;
                            index1++;

                        }
                    }
                    else
                    {

                        MessageBox.Show("Port closed ");

                    }
                }
                // File List Read Handling

                if (inputC >= 0 && inputC != 0x0a && inputC != 0x0d)
                {

                }

                /// File List updating task
                
                if (inputC == '>' && fileStartReceived == 0)
                {
                    fileStartReceived = 100;
                    subIndex1 = index1;
                }
                else if (inputC == '>' && fileStartReceived == 100 )
                {

                    fileStartReceived = 0;

                    try
                    {
                     
                        this.Invoke(new UpdateHandler(changeText1));
                    }
                    catch (Exception ec)
                    {

                        break;
                     }

                }
                else if (inputC == 'g' && fileStartReceived == 100)
                {

                    subIndex2 = index1;

                }
                else if (inputC == 'S' && isFileReading != 100)
                {

                    index1 = 0;
                    fileList.Initialize();
                    subIndex1 = 0;
                    subIndex2 = 0;
                    fileStartReceived = 0;
                }
                else if (inputC == '#' && isFileReading == 100)
                {

                    index1 = 0;
                    fileList.Initialize();
                    subIndex1 = 0;
                    subIndex2 = 0;
                    fileStartReceived = 0;
                }
                // File Contents Updating Task

                if (isFileReading == 100)
                {
                    if (inputC == '#' && isHashReceived == 0)
                    {
                        isHashReceived = 100;
                        subIndex1 = index1;

                        try
                        {

                            this.Invoke(new UpdateHandler(changeText2));
                        }
                        catch (Exception ec)
                        {

                            break;
                        }
                    }
                    else if (inputC == '@' && isHashReceived == 100)
                    {

                        isHashReceived = 0;
                        subIndex2 = index1;

                        try
                        {

                            this.Invoke(new UpdateHandler(changeText2));
                        }
                        catch (Exception ec)
                        {

                            break;
                        }

                    }
                    else if (inputC == '*' || inputC == '@')
                    {

                        index1 = 0;
                        fileList.Initialize();
                        subIndex1 = 0;
                        subIndex2 = 0;
                        fileStartReceived = 0;
                       // Thread.Sleep(20);
                    }





                }



                //Thread.Sleep(1);

               
            }
        }

        private void changeText1()
        {
            //string a = "";
            String k = "hello";
            
            char[] tempFileName = new char[12];
            char[] x = new char[5];
          //  tempFileName.Initialize();
            if (SP.IsOpen)
            {

                for (int j = 0; j < subIndex2 - subIndex1; j++)
                {

                    tempFileName[j] = fileList[subIndex1 + j];
                }
                //string c = new string(tempFileName,Array.IndexOf(tempFileName, '\0'));

               //c = tempFileName.ToString();

                string c = new string(tempFileName);
                

                if (tempFileName[0] == 0 && tempFileName[1] == 0 && tempFileName[2] == 0)
                {

                    c = "noname.log";

                }



                //richTextBox1.Clear();
                x[0] = 'a';
                x[1] = 'b';
                x[2] = 'b';
                x[3] = 'b';
                x[4] = '\0';
                string a = new string(fileList);
                // a = (string) Convert.ToString( 
                //richTextBox1.AppendText(a);
                //richTextBox1.ScrollToCaret();
                listBox1.Items.Add(c);


                subIndex1 = 0;
                subIndex2 = 0;

            }
        }

        private void changeText2()
        {

            //string a = "";
            String k = "hello";
            string o = "00:00:00:000>";
            string p = "\n\n\r";
            char[] tempFile = new char[800];
            char[] tempTime = new char[14];
            char[] x = new char[5];

            long miliSec = 0;
            long sec     = 0;
            long min     = 0;
            long  hour    = 0;

            int time2 = 0;
            int time3 = 0;
            int j = 0;
            int newIndex = 0;
            int tempIndex = 0;

            int b = 0;
            //tempFile[0] = ':';
            //tempFile[1] = '\r';
            //tempFile[1] = '\n';
            for (j = 0; j < subIndex2 - subIndex1-1; j++)
            {
                if (fileList[subIndex1 + j ] > 0)
                {

                    newIndex = subIndex1 + j;



                }
                if (fileList[subIndex1 + j] == '\n')
                {

                    time = time + 70;
                    miliSec = time % 1000;
                    sec = (time / 1000);
                    min = sec / 60;
                    hour = min / 60;


                    sec = sec % 60;
                    min = min % 60;
                    hour = hour % 24;

                    tempTime[13] = ' ';
                    tempTime[12] = '>';
                    tempTime[11] = Convert.ToChar((miliSec % 10) + 48);
                    tempTime[10] = Convert.ToChar(((miliSec / 10) % 10) + 48);
                    tempTime[9] = Convert.ToChar(((miliSec / 100) % 10) + 48);
                    tempTime[8] = ':';
                    tempTime[7] = Convert.ToChar((sec % 10) + 48);
                    tempTime[6] = Convert.ToChar((sec / 10) + 48);
                    tempTime[5] = ':';
                    tempTime[4] = Convert.ToChar((min % 10) + 48);
                    tempTime[3] = Convert.ToChar((min / 10) + 48);
                    tempTime[2] = ':';
                    tempTime[1] = Convert.ToChar((hour % 10) + 48);
                    tempTime[0] = Convert.ToChar((hour / 10) + 48);

                    while (b < 14)
                    {

                        tempFile[tempIndex] = tempTime[b];
                        b++;
                        tempIndex++;

                    }
                    b = 0;
                    if (tempIndex < 20)
                    {

                        tempFile[tempIndex] = ':';
                        tempIndex++;
                    }


                }
                
                if (fileList[subIndex1 + j + 1] == '@' )
                {
                    //tempFile[j] = 'S';
                    //j++;
                    //tempFile[j] = 'I';
                    //tempFile[j] = 0x0a;
                    //tempFile[j] = ':';
                    //tempFile[j + 2] = '\n';
                    //tempFile[j+1] = ':';
                    //time = 0;

                    //j++;
                    //j++;
                    time = time + 180;
                }
                else if (fileList[subIndex1 + j + 1] != '#' && (fileList[subIndex1 + j + 1] < 127 && fileList[subIndex1 + j + 1] > 0) && fileList[subIndex1 + j + 1] != 8 && fileList[subIndex1 + j + 1] != 2)
                {
                    tempFile[tempIndex] = fileList[subIndex1 + j + 1];
                    //time = time + 60; 
                    //if(fileList[subIndex1 + j + 1] == ':'){

                    //    time = time + 30;
                    //    miliSec = time % 1000;
                    //    sec = (time /1000);
                    //    min = sec / 60;
                    //    hour = min / 60;


                    //    sec = sec % 60;
                    //    min = min % 60;
                    //    hour = hour % 24;

                    //    tempTime[0] = Convert.ToChar(miliSec % 10);
                    //    tempTime[1] = Convert.ToChar((miliSec / 10) % 10);
                    //    tempTime[2] = Convert.ToChar((miliSec / 100) % 10);
                    //    tempTime[3] = Convert.ToChar(miliSec / 1000);
                    //    tempTime[4] = ':';
                    //    tempTime[5] = Convert.ToChar(sec % 10);
                    //    tempTime[6] = Convert.ToChar(sec / 10);
                    //    tempTime[7] = ':';
                    //    tempTime[8] = Convert.ToChar(min % 10);
                    //    tempTime[9] = Convert.ToChar(min / 10);
                    //    tempTime[10] = ':';
                    //    tempTime[11] = Convert.ToChar(hour % 10);
                    //    tempTime[12] = Convert.ToChar(hour / 10);

                        
                        
                }
                tempIndex++;
                
            }

            //tempFile[14] = ':';

            //tempFile[newIndex] = '\r';
            //tempFile[newIndex + 1] = '\n';

           // string t = new string(tempTime);
            string c = new string(tempFile);
               

            


            //richTextBox1.Clear();
            //x[0] = 'a';
            //x[1] = 'b';
            //x[2] = 'b';
            //x[3] = 'b';
            //x[4] = '\0';
           // string a = new string(fileList);
            // a = (string) Convert.ToString( 
            richTextBox2.AppendText(String.Concat(c));
            richTextBox2.ScrollToCaret();
           // listBox1.Items.Add(c);


            subIndex1 = 0;
            subIndex2 = 0;

            for (int f = 0; f < 12; f++)
            {

                tempFile[f] = '\0';
            }
            
            



















        }




        public delegate void UpdateHandler();

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            fileList.Initialize();
            isFileListAvailable = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string a;
            string cmdSW = "SW,";
            string cmdNo = "";
            string cmdEnd = "\r\n";

            int fileIndex = 0;
            richTextBox2.Clear();
            time = 0;
            string[] subStrings = new string[2];
            char[] separator = new char[1];

            separator[0] = '.';
            int x = -1;

            x = listBox1.SelectedIndex;

            if (isPortOpened == 1)
            {
                if (x >= 0 && isFileListAvailable == true)
                {

                    a = listBox1.GetItemText(listBox1.SelectedItem);
                    //richTextBox2.ForeColor = Color.Red;

                    richTextBox2.AppendText(listBox1.GetItemText(listBox1.SelectedItem));
                    //richTextBox2.ForeColor = Color.DarkBlue;
                    isFileReading = 100;

                    index1 = 0;
                    fileList.Initialize();
                    subIndex1 = 0;
                    subIndex2 = 0;
                    fileStartReceived = 0;

                    subStrings = a.Split(separator);

                    fileIndex = Convert.ToInt32(subStrings[0], 10);
                    //fileIndex++;

                    cmdNo = Convert.ToString(fileIndex, 10);

                    SP.Write(cmdSW);
                    SP.Write(cmdNo);
                    SP.Write(cmdEnd);

                    isFileReading = 100;



                }
                else
                {
                    MessageBox.Show("No file selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
            else
            {

                MessageBox.Show("No file selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            time = 0;

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupPanel2_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            string cmdQuit = "q";
            if (isPortOpened == 1)
            {

                SP.Write(cmdQuit);
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {

            string path = "";
            char[] trimc = new char[1];
            trimc[0] = '\0';

            path = listBox1.GetItemText(listBox1.SelectedItem).Trim(trimc);

            if (path != "" && path.Contains(".log"))
            {
                saveFileDialog1.FileName = path;
                saveFileDialog1.Filter = "log files (*.log)|*.log";
                saveFileDialog1.ShowDialog();
            }
            else
            {
                MessageBox.Show("No File or Data Available ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //MessageBox.Show("No File or Data Available !");
            }
            //string path = "";
            //char[] trimc = new char[1];
            //trimc[0] = '\0';


            //fileItem = listBox1.GetItemText(listBox1.SelectedItem).ToCharArray();

            //path = listBox1.GetItemText(listBox1.SelectedItem).Trim(trimc);

            //if (path != "" && path.Contains(".log"))
            //{

            //    FileStream fileStrm = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            //    StreamWriter file = new StreamWriter(fileStrm);

            //    file.Write(richTextBox2.Text);

            //    file.Close();
            //    fileStrm.Close();

            //    MessageBox.Show("File saved ");
            //}
            //else
            //{

            //    MessageBox.Show("No File or Data Available !");
            //}
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (trd != null)
            {
                trd.Abort();
            }
            if (SP != null)
            {
                try
                {
                    // button1.Text = "Connect";

                    // isPortOpened = 0;

                    if (SP.IsOpen)
                    {
                        SP.Close();
                    }
                }
                catch (Exception ec)
                {

                    MessageBox.Show(ec.Message);

                }
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            FileStream fileStrm = new FileStream(saveFileDialog1.FileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            StreamWriter file = new StreamWriter(fileStrm);

            file.Write(richTextBox2.Text);

            file.Close();
            fileStrm.Close();

            MessageBox.Show("File saved ", "Ok", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void richTextBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //int k = 0;
            //int l = 0;
            //string selectedLine = "";
            //string line = "";
            //char[] trimm = new char[1];
            //trimm[0] = '\n';
            ////trimm[1] = ':';
            //k = richTextBox2.GetFirstCharIndexOfCurrentLine();

            //richTextBox2.Select(k, k+60);
            //selectedLine = richTextBox2.SelectedText;
            //l = selectedLine.Length;
            //line = selectedLine.TrimStart(trimm);
            //l = line.Length;
        }



        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            if (openFileDialog1.FileName != "")
            {
                if (openFileDialog1.FileName.Contains(".log") || openFileDialog1.FileName.Contains(".txt"))
                {

                    FileStream fileStrm2 = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                    StreamReader file2 = new StreamReader(fileStrm2);

                    richTextBox2.Text = "";

                    richTextBox2.Text = file2.ReadToEnd();

                }
                else
                {
                    MessageBox.Show("Please Select a .log or .txt File", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   // openFileDialog1.Filter = "log files (*.log)|*.log |All files (*.*)|*.*";
                    openFileDialog1.ShowDialog();
                    openFileDialog1.FileName = "";
                }

            }
            else
            {
                MessageBox.Show("Please Select a File", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
           // string path = "";
           // char[] trimc = new char[1];
           // trimc[0] = '\0';

            openFileDialog1.Filter = " log files (*.log)|*.log|All files (*.*)|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();
            
            //openFileDialog1.FileName = "";
        }

                private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
                {

                }

                //const and dll functions for moving form
                public const int WM_NCLBUTTONDOWN = 0xA1;
                public const int HT_CAPTION = 0x2;

                [DllImportAttribute("user32.dll")]
                public static extern int SendMessage(IntPtr hWnd,
                    int Msg, int wParam, int lParam);

                [DllImportAttribute("user32.dll")]
                public static extern bool ReleaseCapture();

                //call functions to move the form in your form's MouseDown event
                private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                    Capture = false;
                    Message msg = Message.Create(Handle, WM_NCLBUTTONDOWN, (IntPtr)HT_CAPTION, IntPtr.Zero);
                    base.WndProc(ref msg);
                    }

                }

                private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
                {
                    if (trd != null)
                    {
                        trd.Abort();
                    }
                    if (SP != null)
                    {
                        try
                        {
                            // button1.Text = "Connect";

                            // isPortOpened = 0;

                            if (SP.IsOpen)
                            {
                                SP.Close();
                            }
                        }
                        catch (Exception ec)
                        {

                            MessageBox.Show(ec.Message);

                        }
                    }
                    this.Close();
                }

                private void pictureBox1_MouseEnter(object sender, EventArgs e)
                {
                    try
                    {

                       // pictureBox1.Image = Image.FromFile("close_c.bmp");
                    }
                    catch (FileNotFoundException ex)
                    {
                    }
                   
                    //MessageBox.Show("mouse entered");
                }

                private void pictureBox1_MouseLeave(object sender, EventArgs e)
                {
                    //pictureBox1.BackgroundImage = "normal.bmp";
                   // pictureBox1.BackgroundImage = Image.FromFile("normal.bmp");
                    //MessageBox.Show("mouse left");
                }

                private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
                {
                    try
                    {

                        //pictureBox1.Image = Image.FromFile("close_c.bmp");
                    }
                    catch (FileNotFoundException ex)
                    {
                        if (trd != null)
                        {
                            trd.Abort();
                        }
                        if (SP != null)
                        {
                            try
                            {
                                // button1.Text = "Connect";

                                // isPortOpened = 0;

                                if (SP.IsOpen)
                                {
                                    SP.Close();
                                }
                            }
                            catch (Exception ec)
                            {

                                MessageBox.Show(ec.Message);

                            }
                        }
                        this.Close();
                    }


                   // MessageBox.Show("mouse entered");
                    if (trd != null)
                    {
                        trd.Abort();
                    }
                    if (SP != null)
                    {
                        try
                        {
                            // button1.Text = "Connect";

                            // isPortOpened = 0;

                            if (SP.IsOpen)
                            {
                                SP.Close();
                            }
                        }
                        catch (Exception ec)
                        {

                            MessageBox.Show(ec.Message);

                        }
                    }
                    this.Close();

                }

                private void pictureBox1_MouseHover(object sender, EventArgs e)
                {
                    //pictureBox1.BackgroundImage = "normal.bmp";
                     ///pictureBox1.Image = Image.FromFile("normal .bmp");
                   // MessageBox.Show("mouse left");
                    //if (pictureBox1.)
                    //{

                    //    pictureBox1.Image = Image.FromFile("normal .bmp");
                    //    MessageBox.Show("mouse left");
                    //}
                }

                private void pictureBox1_MouseLeave_1(object sender, EventArgs e)
                {
                    try
                    {
                       // pictureBox1.Image = Image.FromFile("close_n.bmp");
                    }
                    catch (FileNotFoundException ex)
                    {

                    }
                   // MessageBox.Show("mouse left");
                }

                private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
                {
                    try
                    {
                        //pictureBox1.Image = Image.FromFile("close_p.bmp");
                    }
                    catch (FileNotFoundException ex)
                    {

                    }
                }


    }
}           