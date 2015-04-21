using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp;	//https://github.com/sta/websocket-sharp
using Newtonsoft.Json;



namespace UnityStomp
{
	public class StompClient
	{
		public WebSocket websocket;
		public static string acceptVersion = "1.1,1.0";
		public static string heartBeat = "10000,10000";
		public int subNo;


		public StompClient (string connectString)
		{
			websocket = new WebSocket(connectString);
			subNo = 0;
		}

		public void SetCookie(string name, string value)
		{
			websocket.SetCookie(new WebSocketSharp.Net.Cookie(name, value));
		}
			
		//Stomp Connect...
		public void StompConnect() {

			var connectString = StompString("CONNECT", new Dictionary<string, string>()
			{
				{"accept-version", acceptVersion},
				{"heart-beat", heartBeat}
			});

			websocket.Send(connectString);
		}


		//Subscribe...
		public void Subscribe(string destination) {

			var subscribeString = StompString("SUBSCRIBE", new Dictionary<string, string>()
			{
				{"id", "sub-" + subNo},
				{"destination", destination}
			});

			websocket.Send(subscribeString);
			subNo++;

			//return subscribeString;
		}


		//Send Message
		public void SendMessage(string destination, string message) {
			
			string jsonMessage = JsonConvert.SerializeObject(new {content = message});
			string contentLength = jsonMessage.Length.ToString();
			jsonMessage = jsonMessage.Replace("\"","\\\"");
			
			var sendString = "[\"SEND\\n" +
							 "destination:" + destination + "\\n" +
							 "content-length:" + contentLength + "\\n\\n" +
							 jsonMessage + "\\u0000\"]";

			//websocket.SendAsync(sendString, result => Console.WriteLine(result));
			websocket.Send(sendString);
		}

		//Close 
		public void CloseWebSocket() {		
			websocket.Close ();
		}
		
		public static string StompString(string method, Dictionary<string,string> content){

			string result;

			result = "[\"" + method + "\\n";

			foreach(var item in content){
				result = result + item.Key + ":" + item.Value + "\\n";
			}

			result = result + "\\n\\u0000\"]";

			return result;
		}

	}
}

