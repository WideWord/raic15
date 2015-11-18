using System;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {


	public struct Debug {
		
		private static TcpClient client;
		private static StreamWriter writer;

		public static void connect(string host, int port) {
			client = new TcpClient(host, port);
		}

		public static void disconnect() {
			client.Close();
		}

		private static void sendCommand(string command) {
			if (client != null) {
				if (writer == null) {
					writer = new StreamWriter(client.GetStream (), Encoding.ASCII);
				}
				writer.WriteLine(command);
			}
			System.Console.WriteLine(command);
		}

		public static void beginPre() {
			sendCommand("begin pre");
		}

		public static void beginPost() {
			sendCommand("begin post");
		}
	
		public static void endPre() {
			sendCommand("end pre");
		}

		public static void endPost() {
			sendCommand("end post");
		}

		private static string encodeColor(int color) {
			int red = (color & 0xFF0000) >> 16;
			int green = (color & 0x00FF00) >> 8;
			int blue = color & 0x0000FF;

			return String.Format("{0:0.0####} {1:0.0####} {2:0.0####}", (double)red / 256.0, (double)green / 256.0, (double)blue / 256.0);
		}

		private static string encodeVector(Vector vec) {
			return String.Format("{0:0.0####} {1:0.0####}", vec.x, vec.y);
		}

		public static void circle(Vector position, double radius, int color) {
			sendCommand(String.Format("circle {0} {1:0.0####} {2}", encodeVector(position), radius, encodeColor(color)));
		}

		public static void fillCircle(Vector position, double radius, int color) {
			sendCommand("fill_circle " + position.x + " " + position.y + " " + radius + " " + encodeColor (color));
		}

		public static void rect(Vector p1, Vector p2, int color) {
			sendCommand(String.Format("rect {0} {1} {2} {3} {4}", p1.x, p1.y, p2.x, p2.y, encodeColor(color)));
		}

		public static void fillRect(Vector p1, Vector p2, int color) {
			sendCommand(String.Format("fill_rect {0} {1} {2} {3} {4}", p1.x, p1.y, p2.x, p2.y, encodeColor(color)));
		}

		public static void line(Vector p1, Vector p2, int color) {
			sendCommand(String.Format("line {0} {1} {2} {3} {4}", p1.x, p1.y, p2.x, p2.y, encodeColor(color)));
		}

		public static void print(Vector pos, string msg, int color = 0) {
			sendCommand(String.Format("text {0} {1} {2} {3}", pos.x, pos.y, msg, encodeColor(color)));
		}
	}

}

