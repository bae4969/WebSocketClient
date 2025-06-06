﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketClient.Pages;
using RecvFuncType = System.Func<Newtonsoft.Json.Linq.JObject, System.Threading.Tasks.Task>;

namespace WebSocketClient.Classes
{
	public static class BaeWebSocketClient
	{
		private static Uri _server_uri = new("ws://BaeIptimeDDNSAddress.iptime.org:49693/bae");
		private static ClientWebSocket _ws = new();
		private static CancellationTokenSource _cancel_source = new();
		private static ConcurrentDictionary<string, RecvFuncType?> _wait_service = new();
		private static bool _is_logined = false;
		private static string _login_hash = string.Empty;

		// WebSocket 연결 함수
		[Obsolete]
		public static async Task<bool> Connect(string id, string pw, bool isAutoLogin)
		{
			try
			{
				await Disconnect();
				_wait_service.Clear();
				_ws = new();
				_cancel_source = new();
				await _ws.ConnectAsync(_server_uri, _cancel_source.Token);
				_ = FuncRecvLoop();

				var req_dict = new JObject()
				{
					{ "id", id },
					{ "pw", BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(pw))).Replace("-", "").ToLower() }
				};
				RecvFuncType recvFunc = async (recv_msg) =>
				{
					var ret_code = recv_msg["result"].Value<int>();
					var ret_msg = recv_msg["msg"].ToString();

					if (ret_code == 200)
					{
						_login_hash = recv_msg["data"]["login_hash"].ToString();
						_ = FuncPingLoop();
						Preferences.Set("user_id", id);
						Preferences.Set("user_pw", pw);
						Preferences.Set("is_auto_login", isAutoLogin);
						Device.BeginInvokeOnMainThread(() => { Application.Current.MainPage = new AppShell(); });
						_is_logined = true;
					}
					else
					{
						_login_hash = string.Empty;
						_is_logined = false;
						await Application.Current.MainPage.DisplayAlert("Error", $"Fail to connect ({ret_msg})", "OK");
					}
				};
				return await Send("auth", "login", req_dict, recvFunc);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Connection failed: {ex.Message}");
				return false;
			}
		}

		// WebSocket 닫기
		[Obsolete]
		public static async Task Disconnect()
		{
			_cancel_source.Cancel();
			if (_ws != null)
			{
				try
				{
					if (_ws.State == WebSocketState.Open)
					{
						await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None);
					}
				}
				finally
				{
					_ws.Dispose();
					_ws = null;
				}
			}

			if (_is_logined)
			{
				_login_hash = string.Empty;
				_is_logined = false;
				await Application.Current.MainPage.DisplayAlert("Error", $"Disconnected with server", "OK");
				Device.BeginInvokeOnMainThread(() => { Application.Current.MainPage = new NavigationPage(new LoginPage()); });
			}
		}


		// 주기적으로 Ping을 보내는 함수
		[Obsolete]
		private static async Task FuncPingLoop()
		{
			var cancel_token = _cancel_source.Token;
			var ping_interval = TimeSpan.FromSeconds(3); // Ping을 보낼 주기
			var ping_dict = new JObject
			{
				{ "service", "auth" },
				{ "work", "ping" },
				{ "data", new JObject() }
			};
			ping_dict["data"]["login_hash"] = _login_hash;

			while (_ws.State == WebSocketState.Open && !cancel_token.IsCancellationRequested)
			{
				try
				{
					await Send("auth", "ping", ping_dict, null);
					await Task.Delay(ping_interval, cancel_token);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					break;
				}
			}

			await Disconnect();
		}

		private static async Task FuncRecvLoop()
		{
			var cancel_token = _cancel_source.Token;
			var buffer = new ArraySegment<byte>(new byte[1024 * 1024]);

			while (_ws.State == WebSocketState.Open && !cancel_token.IsCancellationRequested)
			{
				try
				{
					var mem_stream = new MemoryStream();
					WebSocketReceiveResult result;

					do
					{
						result = await _ws.ReceiveAsync(buffer, cancel_token);
						mem_stream.Write(buffer.Array, buffer.Offset, result.Count);
					} while (!result.EndOfMessage);

					mem_stream.Seek(0, SeekOrigin.Begin);

					var reader = new StreamReader(mem_stream, Encoding.UTF8);
					var recv_msg = await reader.ReadToEndAsync();

					var recv_data = JObject.Parse(recv_msg);
					if (_wait_service.TryRemove(recv_data["service"].ToString(), out var func))
					{
						try
						{
							func?.Invoke(recv_data);
						}
						catch
						{
							Console.WriteLine($"Fail to execute recv func {recv_data["service"]} ");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					break;
				}
			}
		}

		// 서비스 요청 및 응답 처리
		public static async Task<bool> Send(string service_name, string work_name, JObject req_dict, RecvFuncType? recv_func)
		{
			var cancel_token = _cancel_source.Token;
			bool is_added = false;
			try
			{
				if (!_wait_service.TryAdd(service_name, recv_func)) return false;
				is_added = true;

				var jsonObject = new JObject
				{
					{ "service", service_name },
					{ "work", work_name },
					{ "data", req_dict }
				};
				jsonObject["data"]["login_hash"] = _login_hash;
				var encoded_msg = Encoding.UTF8.GetBytes(jsonObject.ToString());
				var buffer = new ArraySegment<byte>(encoded_msg, 0, encoded_msg.Length);
				await _ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancel_token);

				return true;
			}
			catch (Exception ex)
			{
				if (is_added) _wait_service.TryRemove(service_name, out _);
				Console.WriteLine($"Fail to send {ex.ToString()} ");
				return false;
			}
		}
	}


	public class QueryInfoType
	{
		public string table_type { get; set; }
		public string stock_code { get; set; }
		public string query_type { get; set; }
		public string stock_name { get; set; }
		public string stock_market { get; set; }
		public string stock_type { get; set; }

		public string ToKeyString()
		{
			return $"[{stock_code}] {stock_name}";
		}
	}
}
