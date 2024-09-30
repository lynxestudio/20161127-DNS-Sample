using System;
using System.Net;
using System.Net.Sockets;
using Gtk;
using System.Text;
using System.Text.RegularExpressions;

namespace Samples.GtkDns
{
	public class MainWindow : Gtk.Window
	{
		VBox mainLayout = new VBox();
		HBox controlLayout = new HBox(false, 2);
		Entry txtHost = new Entry();
		Button btnResolve = new Button("Resolve");
		TextView txtResults = new TextView();
		Label lblMessage = new Label(" ");
		ScrolledWindow scroll = new ScrolledWindow();

		public MainWindow() : base(WindowType.Toplevel)
		{
			this.Title = "GTK# DNS Lookup";
			this.SetDefaultSize(347, 216);
			this.DeleteEvent += new DeleteEventHandler(OnWindowDelete);
			this.btnResolve.Clicked += new EventHandler(Resolve);
			mainLayout.BorderWidth = 8;

			controlLayout.BorderWidth = 8;
			controlLayout.PackStart(new Label("Hostname:"), false, true, 0);
			controlLayout.PackStart(txtHost, true, true, 0);
			controlLayout.PackStart(btnResolve, false, false, 0);

			mainLayout.PackStart(new Label("Type the name to be resolved"), false, true, 0);
			mainLayout.PackStart(controlLayout, false, true, 0);
			txtResults.Editable = false;
			scroll.Add(txtResults);
			mainLayout.PackStart(scroll, true, true, 0);
			mainLayout.PackStart(lblMessage, false, true, 0);
			this.Add(mainLayout);
			this.ShowAll();
			lblMessage.Text = " Application running";
		}

		protected void OnWindowDelete(object o, DeleteEventArgs args)
		{
			System.Environment.Exit(System.Environment.ExitCode);
		}

		protected void Resolve(object o, EventArgs args)
		{
			try
			{
				if (txtHost.Text.Length > 0)
				{
					string output = null;
					string hostName = txtHost.Text;
					IPAddress[] ipAddresses = null;
					//Determines whether is a IP or a Host
					if (IsIP(hostName))
					{
						IPAddress ip = IPAddress.Parse(hostName);
						IPHostEntry ipInfo = Dns.GetHostEntry(ip);
						ipAddresses = ipInfo.AddressList;
					}
					else
					{
						ipAddresses = Dns.GetHostAddresses(hostName);
					}
					foreach (IPAddress ipAddress in ipAddresses)
					{
						output += PrintInfo(ipAddress);
					}
					txtResults.Buffer.Text = output.ToString();
				}
				else
					lblMessage.Text = "Host can not be empty";
			}
			catch (ApplicationException ex)
			{
				lblMessage.Text = ex.Message;
			}
		}

		bool IsIP(string host)
		{
			string pattern = @"^([0-2]?[0-9]?[0-9]\.){3}[0-2]?[0-9]?[0-9]$";
			if(Regex.IsMatch(host,pattern))
				return true;
			return false;
		}

		StringBuilder PrintInfo(IPAddress ipAddress)
		{
			StringBuilder buf = new StringBuilder();
			buf.AppendFormat("{0}-------+-----------+---------------",Environment.NewLine);
			buf.AppendFormat("{0} Address\t[ {1} ]",Environment.NewLine,
			                 ipAddress.ToString());
			buf.AppendFormat("{0} Type\t[ {1} ]", Environment.NewLine,
							 ipAddress.AddressFamily);
			//Get the bytes
			byte[] octets = ipAddress.GetAddressBytes();
			//then clasify the Ip Address by the 1st octect
			//A 1-127
			//B 128-191
			//C 192-223
			//D 224-239
			//E 240-255
			char? networkType = null;
			if (octets[0] > 0 && octets[0] < 128)
				networkType = 'A';
			else
				if (octets[0] > 127 && octets[0] < 192)
				networkType = 'B';
			else
					if (octets[0] > 191 && octets[0] < 224)
				networkType = 'C';
			else
						if (octets[0] > 223 && octets[0] < 240)
				networkType = 'D';
			else
							if (octets[0] > 239 && octets[0] == 255)
				networkType = 'E';
			buf.AppendFormat("{0} Class\t[ {1} ]",Environment.NewLine,networkType);
			return buf;
		}
	}
}
