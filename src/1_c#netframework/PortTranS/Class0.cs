using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

internal class Class0
{
    public static Dictionary<int, TcpClient> dictionary_0 = new Dictionary<int, TcpClient>();
    public static NetworkStream networkStream_0 = null;

    private static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("端口数据转发服务端(跳板机端)");
            Console.WriteLine("使用方式: PortTranS.exe 中转端口 待连接端口");
        }
        else
        {
            string state = args[0];
            string str2 = args[1];
            Console.WriteLine("[+] 监听中转端口中 " + state + " ...");
            Console.WriteLine("[+] 监听待连接端口中 " + str2 + " ...");
            ThreadPool.QueueUserWorkItem(new WaitCallback(Class0.smethod_0), state);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Class0.smethod_1), str2);
            WaitHandle.WaitAll(new ManualResetEvent[] { new ManualResetEvent(false) });
        }
    }

    public static void smethod_0(object object_0)
    {
        TcpListener listener = new TcpListener(int.Parse(object_0.ToString()));
        listener.Start();
        while (true)
        {
            smethod_2(listener.AcceptTcpClient());
        }
    }

    public static void smethod_1(object object_0)
    {
        TcpListener listener = new TcpListener(int.Parse(object_0.ToString()));
        listener.Start();
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            int key = new Random().Next(0x3b9aca00, 0x77359400);
            dictionary_0.Add(key, client);
            byte[] bytes = BitConverter.GetBytes(key);
            networkStream_0.Write(bytes, 0, bytes.Length);
        }
    }

    public static void smethod_2(TcpClient tcpClient_0)
    {
        NetworkStream stream = tcpClient_0.GetStream();
        byte[] buffer = new byte[4];
        if (((stream.Read(buffer, 0, buffer.Length) == 2) && (buffer[0] == 0x6f)) && (buffer[1] == 0x6b))
        {
            networkStream_0 = stream;
            Console.WriteLine("[+] 中转端口连接就绪! ");
            Console.WriteLine("[+] 等待其他客户端连接待连接端口 ...");
        }
        else
        {
            smethod_3(BitConverter.ToInt32(buffer, 0), tcpClient_0);
        }
    }

    public static void smethod_3(int int_0, TcpClient tcpClient_0)
    {
        TcpClient client = null;
        if (dictionary_0.ContainsKey(int_0))
        {
            dictionary_0.TryGetValue(int_0, out client);
            dictionary_0.Remove(int_0);
            tcpClient_0.SendTimeout = 0x493e0;
            tcpClient_0.ReceiveTimeout = 0x493e0;
            client.SendTimeout = 0x493e0;
            client.ReceiveTimeout = 0x493e0;
            object state = new TcpClient[] { tcpClient_0, client };
            object obj3 = new TcpClient[] { client, tcpClient_0 };
            ThreadPool.QueueUserWorkItem(new WaitCallback(Class0.smethod_4), state);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Class0.smethod_4), obj3);
        }
    }

    public static void smethod_4(object object_0)
    {
        TcpClient client = ((TcpClient[]) object_0)[0];
        TcpClient client2 = ((TcpClient[]) object_0)[1];
        NetworkStream stream = client.GetStream();
        NetworkStream stream2 = client2.GetStream();
        Console.WriteLine("转发中...");
        while (true)
        {
            //Console.WriteLine("transferring...");
            try
            {
                byte[] buffer = new byte[0x2800];
                int count = stream.Read(buffer, 0, buffer.Length);
                stream2.Write(buffer, 0, count);
            }
            catch
            {
                stream.Dispose();
                stream2.Dispose();
                client.Close();
                client2.Close();
                return;
            }
        }
    }
}

