using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Continue {
    class Program {
        private static ConsoleColor[] CustomColors = new ConsoleColor[] { 
            ConsoleColor.Black, //0
            ConsoleColor.White, //1
            ConsoleColor.Red, //2
            ConsoleColor.Green, //3
            ConsoleColor.Blue, //4
            ConsoleColor.DarkBlue, //5
            ConsoleColor.DarkGreen, //6
            ConsoleColor.Gray, //7
            ConsoleColor.Yellow //8
        };

        private const int BUFFER_SIZE = 1024;
        private const int PORT = 9989;
        private const int SERVER_PORT = 9998;

        static void Main(string[] args) {
            TcpListener listener = null;
            TcpClient client = null;
            string server = Console.In.ReadLine();
            bool isServer = server == "localhost";


            byte[] buffer = new byte[BUFFER_SIZE];

            //client = new TcpClient(server, SERVER_PORT);

            if (isServer) {
                printf("§3Iniciando servidor na porta {0}\n", new object[] { SERVER_PORT });
                try {
                    listener = new TcpListener(IPAddress.Any, SERVER_PORT);
                    listener.Start();
                } catch (SocketException e) {
                    printf("§2Erro ao iniciar: §r {0}", new object[] { e.Message });
                }
                println("§1Esperando cliente...");
                try {
                    TcpClient serverClient = listener.AcceptTcpClient();
                    NetworkStream stream = serverClient.GetStream();

                    println("Testando conexão");
                    var testMessage = Encoding.ASCII.GetBytes("hello");

                    stream.Write(testMessage, 0, testMessage.Length);

                    int bytesReceived = stream.Read(buffer, 0, buffer.Length);
                    string received = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                    printf("§3Resposta: {0}\n", new object[] { received });
                } catch (SocketException e) {
                    printf("§2Falhou: {0}\n", new object[] { e.Message });
                }
            } else {
                println("§1Conectando...");
                try {
                    client = new TcpClient(server, SERVER_PORT);
                } catch (SocketException e) {
                    printf("§2Erro ao iniciar: §r {0}", new object[] { e.Message });
                }

                try {
                    NetworkStream stream = client.GetStream();

                    int bytesReceived = stream.Read(buffer, 0, buffer.Length);
                    string received = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                    printf("§3Recebido: {0}\n\nEnviando de volta\n", new object[] { received });

                    var testMessage = Encoding.ASCII.GetBytes(received);
                    stream.Write(testMessage, 0, testMessage.Length);
                } catch (SocketException e) {
                    printf("§2Falhou: §r {0}", new object[] { e.Message });
                }
            }

            while (true) {

            }

            //Console.In.Read();
        }

        public static void print(string str) {
            printf(str, null);
        }

        public static void printf(string str, object[] p) {
            bool prevsim = false;
            bool prevCurly = false;
            bool curlyStart = false;
            string parambuffer = "";
            ConsoleColor originalColor = Console.ForegroundColor;
            for (int i = 0; i < str.Length; i++) {
                char c = str.ToCharArray(i, 1)[0];
                int cv = getCharValue(c);
                if (prevsim) {
                    switch (c) {
                        case 'r':
                            Console.ForegroundColor = originalColor;
                            break;
                        default:
                            if (cv > 0 && cv < CustomColors.Length) {
                                Console.ForegroundColor = CustomColors[cv];
                            }
                            break;
                    }


                    prevsim = false;
                } else if (prevCurly) {
                    if (!curlyStart) {
                        parambuffer = "";
                        curlyStart = true;
                    }

                    if (c.Equals('}')) {
                        int k = int.Parse(parambuffer);
                        try {
                            print(p[k].ToString());
                        } catch (Exception e) {
                            continue;
                        }

                        curlyStart = false;
                        prevCurly = false;
                    } else if (c <= '9' && c >= '0') {
                        parambuffer += c;
                    }


                } else {
                    if (c.Equals('§')) {
                        prevsim = true;
                    } else if (c.Equals('{')) {
                        prevCurly = true;
                        curlyStart = false;
                    } else {
                        Console.Out.Write(c);
                    }
                }
            }
        }

        public static void println(string str) {
            print(str);
            Console.Out.WriteLine();
        }

        public static int getCharValue(char c) {
            if (c >= '0' && c <= '9') {
                return c - '0';
            } else if (c >= 'a' && c <= 'f') {
                return c - 'a' + '9' - '0';
            }

            return -1;
        }
    }
}
