using System;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	// цвет нужно задавать hex-числом, например 0xABCDEF, AB - red, CD - green, EF - blue, каждый цвет - число из двух hex-цифр в диапазоне от 00 до FF

	public struct Debug {
		
		private static TcpClient client;
		private static StreamWriter writer;

		public static void Connect(string host, int port) {
			client = new TcpClient(host, port);
		}

		public static void Disconnect() {
			client.Close();
		}

		private static void SendCommand(string command) {
			if (client != null) {
				if (writer == null) {
					writer = new StreamWriter(client.GetStream (), Encoding.ASCII);
				}
				writer.WriteLine(command);
			}
			//System.Console.WriteLine(command);
		}

		public static void BeginPre() {
			SendCommand("begin pre");
		}

		public static void BeginPost() {
			SendCommand("begin post");
		}
	
		public static void EndPre() {
			SendCommand("end pre");
		}

		public static void EndPost() {
			SendCommand("end post");
		}

		private static string DoubleToString(double x) {
			return x.ToString(new System.Globalization.CultureInfo("en-US"));
		}

		private static string encodeColor(int color) {
			int red = (color & 0xFF0000) >> 16;
			int green = (color & 0x00FF00) >> 8;
			int blue = color & 0x0000FF;

			return String.Format("{0} {1} {2}", 
				DoubleToString((double)red / 256.0),
				DoubleToString((double)green / 256.0),
				DoubleToString((double)blue / 256.0)
				);
		}

		private static string EncodeVector(Vector vec) {
			return String.Format("{0} {1}", DoubleToString(vec.x), DoubleToString(vec.y));
		}

		public static void Circle(Vector position, double radius, int color) {
			SendCommand(String.Format("circle {0} {1} {2}", EncodeVector(position), DoubleToString(radius), encodeColor(color)));
		}

		public static void FillCircle(Vector position, double radius, int color) {
			SendCommand(String.Format("fill_circle {0} {1} {2}", EncodeVector(position), DoubleToString(radius), encodeColor(color)));
		}

		public static void Rect(Vector p1, Vector p2, int color) {
			SendCommand(String.Format("rect {0} {1} {2}",  EncodeVector(p1), EncodeVector(p2), encodeColor(color)));
		}

		public static void FillRect(Vector p1, Vector p2, int color) {
			SendCommand(String.Format("fill_rect {0} {1} {2}", EncodeVector(p1), EncodeVector(p2), encodeColor(color)));
		}

		public static void Line(Vector p1, Vector p2, int color) {
			SendCommand(String.Format("line {0} {1} {2}", EncodeVector(p1), EncodeVector(p2), encodeColor(color)));
		}

		public static void Print(Vector pos, string msg, int color = 0) {
			SendCommand(String.Format("text {0} {1} {2}", EncodeVector(pos), msg, encodeColor(color)));
		}
			
	}

}

