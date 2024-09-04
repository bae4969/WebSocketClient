using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        public static async Task<JObject> Connect(string id, string pw)
        {
            try
            {
                _client = new();

                await _client.ConnectAsync(_serverUri, CancellationToken.None);

                string crypt_pw = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(pw))).Replace("-", "").ToLower();

                var data = new JObject
                {
                    { "id", id },
                    { "pw", crypt_pw },
                };

                return await RequestService("auth", "login", data);
            }
            catch (Exception ex)
            {
                return new JObject
                    {
                        { "type", "rep" },
                        { "result", 401 },
                        { "msg",  ex.Message },
                        { "data", new JArray() },
                    };
            }
        }
        public static async Task Close()
        {
            try
            {
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed connection", CancellationToken.None);
                Debug.WriteLine("WebSocket connection closed.");
            }
            catch { }
        }

        public static async Task<JObject> RequestService(string service_name, string service_type, JObject json_data)
        {
            try
            {
                var jsonObject = new JObject
                {
                    { "service", service_name},
                    { "type", service_type },
                    { "data", json_data }
                };
                var encodedMessage = Encoding.UTF8.GetBytes(jsonObject.ToString());


                var send_buffer = new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length);
                await _client.SendAsync(send_buffer, WebSocketMessageType.Text, true, CancellationToken.None);

                var recv_buffer = new byte[1024];
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(recv_buffer), CancellationToken.None);
                var receivedMessage = Encoding.UTF8.GetString(recv_buffer, 0, result.Count);

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
