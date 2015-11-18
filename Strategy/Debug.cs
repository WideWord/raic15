using System;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	// цвет нужно задавать hex-числом, например 0xABCDEF, AB - red, CD - green, EF - blue, каждый цвет - число из двух hex-цифр в диапазоне от 00 до FF

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

		private static string doubleToString(double x) {
			return x.ToString(new System.Globalization.CultureInfo("en-US"));
		}

		private static string encodeColor(int color) {
			int red = (color & 0xFF0000) >> 16;
			int green = (color & 0x00FF00) >> 8;
			int blue = color & 0x0000FF;

			return String.Format("{0} {1} {2}", 
				doubleToString((double)red / 256.0),
				doubleToString((double)green / 256.0),
				doubleToString((double)blue / 256.0)
				);
		}

		private static string encodeVector(Vector vec) {
			return String.Format("{0} {1}", doubleToString(vec.x), doubleToString(vec.y));
		}

		public static void circle(Vector position, double radius, int color) {
			sendCommand(String.Format("circle {0} {1} {2}", encodeVector(position), doubleToString(radius), encodeColor(color)));
		}

		public static void fillCircle(Vector position, double radius, int color) {
			sendCommand(String.Format("fill_circle {0} {1} {2}", encodeVector(position), doubleToString(radius), encodeColor(color)));
		}

		public static void rect(Vector p1, Vector p2, int color) {
			sendCommand(String.Format("rect {0} {1} {2}",  encodeVector(p1), encodeVector(p2), encodeColor(color)));
		}

		public static void fillRect(Vector p1, Vector p2, int color) {
			sendCommand(String.Format("fill_rect {0} {1} {2}", encodeVector(p1), encodeVector(p2), encodeColor(color)));
		}

		public static void line(Vector p1, Vector p2, int color) {
			sendCommand(String.Format("line {0} {1} {2}", encodeVector(p1), encodeVector(p2), encodeColor(color)));
		}

		public static void print(Vector pos, string msg, int color = 0) {
			sendCommand(String.Format("text {0} {1} {2}", encodeVector(pos), msg, encodeColor(color)));
		}
	}

}

