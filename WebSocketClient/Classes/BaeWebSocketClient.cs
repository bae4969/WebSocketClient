using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketClient.Classes
{
	public static class BaeWebSocketClient
	{
		private static ClientWebSocket _client = new();
		private static readonly Uri _serverUri = new("ws://BaeIptimeDDNSAddress.iptime.org:49693/bae");


		public static async Task<Tuple<bool, string, JObject>> Connect(string id, string pw)
		{
			try
			{
                _client = new();

                var task_conn = _client.ConnectAsync(_serverUri, CancellationToken.None);
				if (task_conn.Wait(1000) == false)
					throw new Exception("Fail to connet");

                Console.WriteLine("Connected to WebSocket server.");

				string crypt_pw = "";
				using (SHA256 sha256Hash = SHA256.Create())
				{
					byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pw));
					var builder = new StringBuilder();
					for (int i = 0; i < bytes.Length; i++)
						builder.Append(bytes[i].ToString("x2"));

					crypt_pw = builder.ToString();
				}

				var data = new JObject
				{
					{ "id", id },
					{ "pw", crypt_pw },
				};

				var task_login = RequestService("auth", "login", data);
                if (task_conn.Wait(1000) == false)
                    throw new Exception("Fail to login");

                return new Tuple<bool, string, JObject>
                (
                    true,
                    "",
                    task_login.Result
                );
            }
			catch(Exception ex) {
				return new Tuple<bool, string, JObject>
				(
					false,
					ex.Message,
					new JObject
					{
						{ "type", "rep" },
						{ "result", 401 },
						{ "msg",  ex.Message },
						{ "data", new JArray() },
					}
				);
			}
		}
		public static async Task Close()
		{
			await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed connection", CancellationToken.None);
			Console.WriteLine("WebSocket connection closed.");
		}

		public static async Task<JObject> RequestService(string service_name, string service_type, JObject json_data)
		{
			try
			{
				var jsonObject = new JObject
				{
					{ "service", service_name},
					{ "type", service_type },
					{ "data", json_data },
				};
				var encodedMessage = Encoding.UTF8.GetBytes(jsonObject.ToString());


				var send_buffer = new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length);
				await _client.SendAsync(send_buffer, WebSocketMessageType.Text, true, CancellationToken.None);


                var recv_buffer = new byte[1024];
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(recv_buffer), CancellationToken.None);
                var receivedMessage = Encoding.UTF8.GetString(recv_buffer, 0, result.Count);

				return JObject.Parse(receivedMessage);
			}
			catch(Exception ex) {
				return new JObject
					{
						{ "type", "rep" },
						{ "result", 401 },
						{ "msg", ex.Message },
						{ "data", new JArray() },
					};
			}
		}
	}
}
