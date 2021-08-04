﻿using Logic.Assignatures;
using Newtonsoft.Json;
using Server.Contracts;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public abstract class ServerSocket
    {
            private const int EndPointPort = 11000;

        public static void StartServer()
        {
            // Get Host IP Address that is used to establish a connection  
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
            // If a host has multiple addresses, you will get a list of addresses  
            IPAddress ipAddress = GetIpAddress();
            IPEndPoint localEndPoint = GetLocalEndPoint(ipAddress, EndPointPort);

            Socket handler;
            var request = Listen(ipAddress, localEndPoint, out handler);
            ProcessRequest_1(handler, request);

            CloseServer(handler);
        }

        /// <summary>
        /// Exercicio 1
        /// </summary>
        private static void ProcessRequest_1(Socket handler, string request)
        {
            var dtoMaioridade = JsonConvert.DeserializeObject<DtoMaioridade>(request);

            var returnProcess = ServiceLocator.UseService<IMaioridade>()
                .EhMaiorIdade(dtoMaioridade.Nome, dtoMaioridade.Sexo, dtoMaioridade.Idade);

            SendResponse(handler, returnProcess);
        }

        private static void SendResponse(Socket handler, string returnProcess) => handler.Send(Encoding.ASCII.GetBytes(returnProcess));

        private static void CloseServer(Socket handler)
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static string Listen(IPAddress ipAddress, IPEndPoint localEndPoint, out Socket handler)
        {
            // Cria um Socket que usará o protocolo TCP     
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // Um Socket deve ser associado a um endpoint usando o método Bind  
            listener.Bind(localEndPoint);
            // Especifica quantas solicitações um Socket pode escutar antes de dar uma resposta de Servidor ocupado.    
            listener.Listen(1);

            Console.WriteLine("Listening...");
            handler = listener.Accept();

            string requestMsg = null;
            byte[] bytes = null;

            bytes = new byte[1024];
            int bytesRec = handler.Receive(bytes);
            requestMsg += Encoding.ASCII.GetString(bytes, 0, bytesRec);

            // Mensagem enviada pelo client.    
            return requestMsg;
        }

        private static IPEndPoint GetLocalEndPoint(IPAddress ipAddress, int port)
        {
            return new IPEndPoint(ipAddress, port);
        }

        private static IPAddress GetIpAddress()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            return ipAddress;
        }
    }
}
