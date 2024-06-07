using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace P_Server
{
    public partial class Form1 : Form
    {
        Socket server;
        Socket client;
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        string path;
        private string selectedPath;
        byte[] dataimg = new byte[1024];
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {/*
        private TcpListener server;
        private List<TcpClient> clients = new List<TcpClient>();
         button lesn
            try
            {
                server = new TcpListener(IPAddress.Any, 8888);
                server.Start();
                Task.Run(() => ListenForClients());
                Log("Server started...");
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
             private async Task ListenForClients()
        {
            try
            {
                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    clients.Add(client);
                    UpdateClientsList();
                    Task.Run(() => HandleClient(client));
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }
            private async Task HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Broadcast(message, client);
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }
             private async Task Broadcast(string message, TcpClient sender)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            foreach (TcpClient client in clients)
            {
                if (client != sender)
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                    await stream.FlushAsync();
                }
            }
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(Log), message);
                return;
            }

            listBox1.Items.Add(message + Environment.NewLine);
        }

        private void UpdateClientsList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateClientsList));
                return;
            }
          }
          
          private TcpClient selectedClient;
            button send
            try
            {
                string message = textBox1.Text; 
                byte[] buffer = Encoding.ASCII.GetBytes(message); 

                foreach (TcpClient client in clients)
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(buffer, 0, buffer.Length); 
                    await stream.FlushAsync(); 
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}"); 
            }
          
          
          
          */

            
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipep);
            server.Listen(10);
            textBox1.Text += "Server: I'm Listening on 127.0.0.1:9050";
            textBox1.Text += Environment.NewLine;


            
            client = await Task.Run(() => server.Accept());
            IPEndPoint clientep = client.RemoteEndPoint as IPEndPoint ?? throw new Exception("Couldn't extract the ip endpoint for the client");
            textBox1.Text += "Client: I'm Connected, my ipep = " + clientep.ToString() + "\n";
            textBox1.Text += Environment.NewLine;


            // Connect the ns 
            ns = new NetworkStream(client);
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
            // Send Welcome Message
            string welcome = "Welcome to my test server!";
            sw.WriteLine(welcome);
            sw.Flush();

            // Recieve messages from the client
            while (true)
            {
                string data = await Task.Run(() => sr.ReadLine());
                if (data.StartsWith("DIR"))
                {
                     path = data.Substring(4);
                    textBox1.Text += "Client:" + path;
                    textBox1.Text += Environment.NewLine;
                    DirectoryInfo d = new DirectoryInfo(path);
                    string name = d.FullName;
                    string[] dirs = Directory.GetDirectories(name);
                    string[] fils = Directory.GetFiles(name);
                    sw.WriteLine("dir");
                    sw.Flush ();
                    foreach (string dd in dirs)
                    {
                        DirectoryInfo dx = new DirectoryInfo(dd);

                        sw.WriteLine(dx.Name);
                        
                    }
                    sw.Flush();
                    foreach (string f in fils)
                    {

                        FileInfo dx = new FileInfo(f);
                        sw.WriteLine(dx.Name);
                    }
                    sw.Flush();
                }
                 
                else if(data.StartsWith("FIL"))
                {
                    path = data.Substring(4);
                    textBox1.Text += "Client:" + path;
                    textBox1.Text += Environment.NewLine;
                    FileStream fs = new FileStream("asd2.txt", FileMode.Create, FileAccess.Write);
                    FileStream fd = new FileStream(path, FileMode.Open, FileAccess.Read);
                    StreamWriter writer = new StreamWriter(fs);
                    StreamReader reader = new StreamReader(fd);
                    writer.Write(reader.ReadToEnd());
                    writer.Flush();
                }
                else if (data.StartsWith("IMG"))

                {
                    path = data.Substring(4);
                    textBox1.Text += "Client:"+path;
                    textBox1.Text += Environment.NewLine;

                    sw.WriteLine("img");
                        sw.Flush();
                        byte[] img = File.ReadAllBytes(path);
                        byte[] size = BitConverter.GetBytes(img.Length);
                        ns.Write(size, 0, size.Length); 
                        ns.Flush();
                        ns.Write(img, 0, img.Length); 
                        ns.Flush();

                }
                else
                {
                    textBox1.Text += "Client: " + data ;
                    textBox1.Text += Environment.NewLine;

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sw.Close();
            sr.Close();
            ns.Close();
            client.Close();
            server.Close();
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            sw.WriteLine("mess");
            sw.Flush();
            sw.WriteLine(textBox2.Text);
            sw.Flush();
            textBox1.Text += "Server: " + textBox2.Text ;
            textBox1.Text += Environment.NewLine;
            textBox2.Text = string.Empty;
        }
        /*
         button compress
        if (dlgOpenFile.ShowDialog(this) == DialogResult.OK)
            {
                GZipStream gz = new GZipStream(new FileStream(dlgOpenFile.FileName + ".QZip.BCompressed", FileMode.Create, FileAccess.Write), CompressionMode.Compress);

                BinaryWriter bw = new BinaryWriter(gz);

                FileStream fs = new FileStream(dlgOpenFile.FileName, FileMode.Open, FileAccess.Read);

                BinaryReader br = new BinaryReader(fs);

                byte[] _da;

                _da = br.ReadBytes((int)br.BaseStream.Length);
                 progressBar1.Maximum = (int)_da.Length;
                progressBar1.Value = (int)_da.Length;
                bw.Write(_da);

                bw.Flush();

                bw.Close();
                progressBar2.Maximum = progressBar1.Maximum;
                progressBar2.Value = (int.Parse(new FileStream(dlgOpenFile.FileName + ".QZip.BCompressed", FileMode.Open, FileAccess.Read).Length.ToString()));
            }
        button restorebinaryfile
         if (dlgOpenFile.ShowDialog(this) == DialogResult.OK)
            {
         GZipStream gz = new GZipStream(new FileStream(dlgOpenFile.FileName, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);

                FileStream fs_out = 
                    new FileStream(dlgOpenFile.FileName + ".DECOMPRESSED", FileMode.Create, FileAccess.Write);

                var ms = new MemoryStream();

                gz.CopyTo(ms);

                BinaryWriter bw = new BinaryWriter(fs_out);

                bw.Write(ms.ToArray(), 0, (int) ms.Length);

                bw.Close();
                try
                {
                    pictureBox1.Image = Image.FromFile(dlgOpenFile.FileName + ".DECOMPRESSED");
                  
                }
                catch { }
        }

         
         
         
         */

    }
}
