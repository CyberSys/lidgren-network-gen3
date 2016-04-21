using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SamplesCommon;

namespace ManyClients
{
	static class Program
	{
		public static List<Client> Clients;

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Clients = new List<Client>();

			//Application.Idle += new EventHandler(AppIdle);
			Application.Run(new Form1());

			foreach (var c in Clients)
				c.Shutdown();
		}

		static void AppIdle(object sender, EventArgs e)
		{
			while (NativeMethods.AppStillIdle)
			{
                foreach (var c in Clients) {
                    if (c.Nets == null) continue;
                    for (int i = 0; i < c.Nets.Length; i++) {
                        c.Heartbeat(i);
                    }
                }
                
				System.Threading.Thread.Sleep(1);
			}
		}

		internal static void CreateClient(int netClientCount)
		{
			Client client = new Client(netClientCount);
			client.Show();
			Clients.Add(client);
		}
	}
}
