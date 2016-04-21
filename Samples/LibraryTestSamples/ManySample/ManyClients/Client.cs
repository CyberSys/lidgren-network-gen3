using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Lidgren.Network;

namespace ManyClients
{
    public partial class Client : Form
    {
        private const double c_sendFrequency = 1.0;
        int m_NetClientCount = 100;
        public NetClient[] Nets;// = new NetClient[20];
        string[] TextNetMsgS;// = new string[20];
        private double[] m_lastSent;// = new double[20];
        StringBuilder TextNetMsgSB;//= new StringBuilder();
        public Client() {

            InitializeComponent();
            InitClients(m_NetClientCount);
        }
        public Client(int netClientcount) {
            InitializeComponent();
            InitClients(netClientcount);
        }
        public void InitClients(int netClientcount) {
            m_NetClientCount = netClientcount;
            Nets = new NetClient[m_NetClientCount];
            TextNetMsgS = new string[m_NetClientCount];
            m_lastSent = new double[m_NetClientCount];
            TextNetMsgSB = new StringBuilder();
            System.Net.IPAddress mask = null;
            System.Net.IPAddress local = NetUtility.GetMyAddress(out mask);
            //
            for (int i = 0; i < Nets.Length; i++) {
                NetPeerConfiguration config = new NetPeerConfiguration("many");

                config.LocalAddress = local;
#if DEBUG
			config.SimulatedLoss = 0.02f;
#endif
                NetClient Net = new NetClient(config);
                Nets[i] = Net;
                Net.Start();
                //Net.Connect("localhost", 14242);
                Net.Connect(new System.Net.IPEndPoint(config.LocalAddress, 14242));
                //
                Application.DoEvents();
            }
           
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            //
            this.timer1.Start();
        }
       

        protected override void OnClosed(EventArgs e) {
            this.timer1.Stop();
            this.timer1.Dispose();
            Program.Clients.Remove(this);
            for (int i = 0; i < Nets.Length; i++) {
                Nets[i].Shutdown("closed");
            }

        }
        
        
        internal void Shutdown() {
            for (int i = 0; i < Nets.Length; i++) {
                Nets[i].Shutdown("bye");
            }
        }

        internal void Heartbeat(int netIndex) {
            bool needUpdateText = false;
            NetIncomingMessage inc;
            while ((inc = Nets[netIndex].ReadMessage()) != null) {
                switch (inc.MessageType) {
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)inc.ReadByte();
                        double delay = -1d;
                        this.TextNetMsgS[netIndex] = status.ToString() + "StatusChanged 延时：" + Nets[netIndex].ServerConnection ?? NetTime.ToReadable(Nets[netIndex].ServerConnection.PingTimeDelay);
                        needUpdateText = true;
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        this.TextNetMsgS[netIndex] = inc.ReadString() + "ErrorMessage 延时：" + Nets[netIndex].ServerConnection ?? NetTime.ToReadable(Nets[netIndex].ServerConnection.PingTimeDelay);
                        needUpdateText = true;
                        break;
                }
            }

            // send message?
            if (NetTime.Now > m_lastSent[netIndex] + c_sendFrequency) {
                var om = Nets[netIndex].CreateMessage();
                om.Write("Hi!");
                Nets[netIndex].SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                m_lastSent[netIndex] = NetTime.Now;

                // also update title
#if DEBUG
				/*this.Text*/this.TextNetMsgS[netIndex] = Nets[netIndex].Statistics.SentBytes + " bytes sent; " + Nets[netIndex].Statistics.ReceivedBytes + " bytes received";
                //if (needUpdateText) {
                    TextNetMsgSB.Remove(0, TextNetMsgSB.Length);
                    for (int i = 0; i < TextNetMsgS.Length; i++) {
                        TextNetMsgSB.AppendLine(TextNetMsgS[i]);
                    }
                    this.label1.Text = TextNetMsgSB.ToString();
                //}
#else
                string str = Nets[netIndex].ServerConnection == null ? "No connection" : Nets[netIndex].ServerConnection.Status.ToString();
                //if (this.TextNetMsgS[netIndex] != str) {
                if (Nets[netIndex].ServerConnection != null) {
                    this.TextNetMsgS[netIndex] = str + " 延时：" + NetTime.ToReadable(Nets[netIndex].ServerConnection.PingTimeDelay);
                    needUpdateText = true;
                }
                //if (needUpdateText) {
                //TextNetMsgSB.Remove(0, TextNetMsgSB.Length);
                //for (int i = 0; i < TextNetMsgS.Length; i++) {
                //    TextNetMsgSB.AppendLine(TextNetMsgS[i]);
                //}
                //this.label1.Text = TextNetMsgSB.ToString();
                //}
#endif
            }
            
        }

        private void button1_Click(object sender, EventArgs e) {
            this.button1.Enabled = false;
            for (int i = 0; i < Nets.Length; i++) {
                var om = Nets[i].CreateMessage();
                om.Write("Manual hi!");

                Nets[i].SendMessage(om, NetDeliveryMethod.ReliableOrdered);
               
            }
            this.button1.Enabled = true;
            Application.DoEvents();
        }
        int m_OldTick;
        private void timer1_Tick(object sender, EventArgs e) {
            for (int i = 0; i < Nets.Length; i++) {
                Heartbeat(i);               
            } 
            Application.DoEvents();
            if (Environment.TickCount - m_OldTick>1000) {
                TextNetMsgSB.Remove(0, TextNetMsgSB.Length);
                for (int i = 0; i < TextNetMsgS.Length; i++) {
                    TextNetMsgSB.AppendLine(TextNetMsgS[i]);
                }
                this.label1.Text = TextNetMsgSB.ToString();
            }

        }
    }
}
