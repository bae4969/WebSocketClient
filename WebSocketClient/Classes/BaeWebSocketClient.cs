using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketClient.Pages;

namespace WebSocketClient.Classes
{
	public static class BaeWebSocketClient
	{
		private static ClientWebSocket _webSocket = new();
		private static string _id;
		private static string _pw;
		private static Uri _serverUri = new("ws://BaeIptimeDDNSAddress.iptime.org:49693/bae");
		private static bool _isConnected = false;
		private static bool _normalDisconnect = false;
		private static CancellationTokenSource _cancellationTokenSource = new();

		// WebSocket 연결 함수
		public static async Task<Tuple<int, string>> Connect()
		{
			_id = Preferences.Get("user_id", "");
			_pw = Preferences.Get("user_pw", "");

			if (_webSocket.State == WebSocketState.Open)
			{
				await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None);
			}

			try
			{
				_webSocket = new();
				await _webSocket.ConnectAsync(_serverUri, CancellationToken.None);
				_isConnected = true;

				JObject req_dict = new()
				{
					{ "id", _id },
					{ "pw", BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(_pw))).Replace("-", "").ToLower() }
				};
				var rep_msg = await RequestService("req", "login", "login", req_dict);
				var ret = new Tuple<int, string>(
					rep_msg["result"].Value<int>(),
					rep_msg["msg"].ToString()
					);
				if (ret.Item1 == 200)
				{
					_isConnected = true;
					_cancellationTokenSource = new();
					_ = StartPingLoop();
				}
				else
				{
					_isConnected = false;
				}

				return ret;
			}
			catch (Exception ex)
			{
				_isConnected = false;
				return new Tuple<int, string>( 400, $"Connection failed: {ex.Message}");
			}
		}

		// 주기적으로 Ping을 보내는 함수
		public static async Task StartPingLoop()
		{
			var pingInterval = TimeSpan.FromSeconds(3); // Ping을 보낼 주기
			var cancellationToken = _cancellationTokenSource.Token;

			while (_isConnected && _webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
			{
				try
				{
					var jsonObject = new JObject
					{
						{ "type", "set" },
						{ "service", "ping" },
						{ "work", "ping" },
						{ "data", new JObject() }
					};
					var encodedMessage = Encoding.UTF8.GetBytes(jsonObject.ToString());
					var sendBuffer = new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length);

					// 실제 Ping 메시지 전송
					await _webSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
					Console.WriteLine("Ping sent.");

					// 다음 Ping까지 대기
					await Task.Delay(pingInterval, cancellationToken);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ping failed: {ex.Message}");
					break;
				}
			}

			if (_isConnected)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Disconnected with server", "OK");
				Application.Current.MainPage = new NavigationPage(new LoginPage());
			}
			_isConnected = false;
		}

		// WebSocket 닫기
		public static async Task Disconnect()
		{
			if (_webSocket.State == WebSocketState.Open)
			{
				_isConnected = false;
				_cancellationTokenSource.Cancel();
				await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
			}
		}

		// 서비스 요청 및 응답 처리
		public static async Task<JObject> RequestService(string type, string service_name, string work_name, JObject req_dict)
		{
			try
			{
				var jsonObject = new JObject
				{
					{ "type", type },
					{ "service", service_name },
					{ "work", work_name },
					{ "data", req_dict }
				};
				var encodedMessage = Encoding.UTF8.GetBytes(jsonObject.ToString());
				var sendBuffer = new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length);

				// 메시지 전송
				await _webSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);

				// 메시지 수신
				var recvBuffer = new ArraySegment<byte>(new byte[1024]);
				var result = await _webSocket.ReceiveAsync(recvBuffer, CancellationToken.None);

				// 수신된 메시지 처리
				var receivedMessage = Encoding.UTF8.GetString(recvBuffer.Array, 0, result.Count);
				return JObject.Parse(receivedMessage);
			}
			catch (Exception ex)
			{
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
