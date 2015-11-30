using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Lidgren.Network;
using SamplesCommon;

namespace ChatClient
{
	static class Program
	{
		private static List<NetClient> s_clients=new List<NetClient>();
		private static Form1 s_form;
		private static NetPeerSettingsWindow s_settingsWindow;

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            //System.Windows.Forms.ThreadExceptionDialog.CheckForIllegalCrossThreadCalls = false;
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            for (int i = 0; i < 1; i++) {
                NetPeerConfiguration config = new NetPeerConfiguration("chat");
                config.AutoFlushSendQueue = false;
                NetClient s_client = new NetClient(config);
                s_client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage),new SynchronizationContext());
                s_clients.Add(s_client);
            }
            s_form = new Form1();
            s_form.Show();
            while (true) {
                Application.DoEvents();
                foreach (var s_client in s_clients) { 
                    
                }
            }
			//Application.Run(s_form);
            foreach(var s_client in s_clients)
			s_client.Shutdown("Bye");
		}

		private static void Output(string text)
		{
            Console.WriteLine(text);
			//NativeMethods.AppendText(s_form.richTextBox1, text);
		}

		public static void GotMessage(object peer)
		{
            NetClient s_client = peer as NetClient;
            //foreach (var s_client in s_clients) {
                NetIncomingMessage im;
                while ((im = s_client.ReadMessage()) != null) {
                    // handle incoming message
                    switch (im.MessageType) {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = im.ReadString();
                            Output(s_client.Port.ToString()+" recevied:"+text);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                            if (status == NetConnectionStatus.Connected)
                                s_form.EnableInput();
                            else
                                s_form.DisableInput();

                            if (status == NetConnectionStatus.Disconnected)
                                s_form.button2.Text = "Connect";

                            string reason = im.ReadString();
                            Output(status.ToString() + ": " + reason);

                            break;
                        case NetIncomingMessageType.Data:
                            string chat = im.ReadString();
                            Output(s_client.Port.ToString() + " recevied:" + chat);
                            break;
                        default:
                            Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                            break;
                    }
                    s_client.Recycle(im);
                //}
            }
		}

		// called by the UI
		public static void Connect(string host, int port)
		{
            foreach (var s_client in s_clients) {
                s_client.Start();
                NetOutgoingMessage hail = s_client.CreateMessage("This is the hail message");
                s_client.Connect(host, port, hail);
            }
		}

		// called by the UI
		public static void Shutdown()
		{
            foreach (var s_client in s_clients) {
                s_client.Disconnect("Requested by user");
                // s_client.Shutdown("Requested by user");
            }
		}
 static Random rn = new Random();
		// called by the UI
		public static void Send(string text)
		{

            int ri=rn.Next(s_clients.Count);
            NetClient s_client = s_clients[ri];
            //foreach (var s_client in s_clients) {
                NetOutgoingMessage om = s_client.CreateMessage(text);
                s_client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                Output(s_client.Port.ToString()+" Sending '" + text + "'");
                s_client.FlushSendQueue();
            //}
		}

		// called by the UI
		public static void DisplaySettings()
		{
			if (s_settingsWindow != null && s_settingsWindow.Visible)
			{
				s_settingsWindow.Hide();
			}
			else
			{
				if (s_settingsWindow == null || s_settingsWindow.IsDisposed)
					s_settingsWindow = new NetPeerSettingsWindow("Chat client settings", s_clients[0]);
				s_settingsWindow.Show();
			}
		}
	}
}
