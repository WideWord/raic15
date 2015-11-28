using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	// цвет нужно задавать hex-числом, например 0xABCDEF, AB - red, CD - green, EF - blue, каждый цвет - число из двух hex-цифр в диапазоне от 00 до FF

	public struct Color {
		public double RedComponent { get; private set; }
		public double GreenComponent { get; private set; }
		public double BlueComponent { get; private set; }

		public Color(double red, double green, double blue) : this() {
			RedComponent = red;
			GreenComponent = green;
			BlueComponent = blue;
		}

		public static readonly Color Red = new Color(1, 0, 0);
		public static readonly Color Green = new Color(0, 1, 0);
		public static readonly Color Blue = new Color(0, 0, 1);
		public static readonly Color Black = new Color(0, 0, 0);

	}

	public struct Debug {

		public enum Layer {
			Back, Top
		}

		private static TcpClient client;
		private static StreamWriter writer;

		public static void Connect(string host, int port) {
			client = new TcpClient(host, port);
		}

		public static void Disconnect() {
			client.Close();
		}

		private static List<string> preMessages = new List<string>();
		private static List<string> postMessages = new List<string>();

		public static void Flush() {
			if (client != null) {
				if (writer == null) {
					writer = new StreamWriter(client.GetStream(), Encoding.ASCII);
				}
				writer.WriteLine("begin pre");
				foreach (var msg in preMessages) {
					writer.WriteLine(msg);
				}
				writer.WriteLine("end pre");
				writer.WriteLine("begin post");
				foreach (var msg in preMessages) {
					writer.WriteLine(msg);
				}
				writer.WriteLine("end post");
			}
			preMessages.Clear();
			postMessages.Clear();
		}

		private static void SendCommand(string command, Layer drawInPost) {
			if (drawInPost == Layer.Top) {
				postMessages.Add(command);
			} else {
				preMessages.Add(command);
			}
		}
		
		private static string DoubleToString(double x) {
			return x.ToString(new System.Globalization.CultureInfo("en-US"));
		}

		private static string EncodeColor(Color color) {

			return String.Format("{0} {1} {2}", 
				DoubleToString(color.RedComponent),
				DoubleToString(color.GreenComponent),
				DoubleToString(color.BlueComponent)
			);
		}

		private static string EncodeVector(Vector vec) {
			return String.Format("{0} {1}", DoubleToString(vec.x), DoubleToString(vec.y));
		}

		public static void Circle(Vector position, double radius, Color color, Layer layer = Layer.Top) {
			SendCommand(String.Format("circle {0} {1} {2}", EncodeVector(position), DoubleToString(radius), EncodeColor(color)), layer);
		}

		public static void FillCircle(Vector position, double radius, Color color, Layer layer = Layer.Top) {
			SendCommand(String.Format("fill_circle {0} {1} {2}", EncodeVector(position), DoubleToString(radius), EncodeColor(color)), layer);
		}

		public static void Rect(Vector p1, Vector p2, Color color, Layer layer = Layer.Top) {
			SendCommand(String.Format("rect {0} {1} {2}",  EncodeVector(p1), EncodeVector(p2), EncodeColor(color)), layer);
		}

		public static void FillRect(Vector p1, Vector p2, Color color, Layer layer = Layer.Top) {
			SendCommand(String.Format("fill_rect {0} {1} {2}", EncodeVector(p1), EncodeVector(p2), EncodeColor(color)), layer);
		}

		public static void Line(Vector p1, Vector p2, Color color, Layer layer = Layer.Top) {
			SendCommand(String.Format("line {0} {1} {2}", EncodeVector(p1), EncodeVector(p2), EncodeColor(color)), layer);
		}

		public static void Print(Vector pos, string msg, Color color, Layer layer = Layer.Top) {
			SendCommand(String.Format("text {0} {1} {2}", EncodeVector(pos), msg, EncodeColor(color)), layer);
		}
			
	}

}

