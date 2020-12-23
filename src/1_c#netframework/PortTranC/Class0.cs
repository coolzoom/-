using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

internal class Class0
{
    public static NetworkStream networkStream_0 = null;

    private static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("端口数据转发客户端(目标机端)");
            Console.WriteLine("使用方式: PortTranC.exe 目标IP 目标端口 跳板机IP 跳板机中转端口");
        }
        else
        {
            try
            {
                string str = args[0];
                int num = int.Parse(args[1]);
                string str2 = args[2];
                int num2 = int.Parse(args[3]);
                Console.WriteLine(string.Concat(new object[] { "[+] 连接至跳板机中转端口 ", str2, ":", num2, "..." }));
                smethod_0(str, num, str2, num2);
                smethod_1(str, num, str2, num2);
                WaitHandle.WaitAll(new ManualResetEvent[] { new ManualResetEvent(false) });
            }
            catch
            {
            }
        }
    }

    private static void smethod_0(string string_0, int int_0, string string_1, int int_1)
    {
        networkStream_0 = new TcpClient(string_1, int_1).GetStream();
        byte[] bytes = Encoding.Default.GetBytes("ok");
        networkStream_0.Write(bytes, 0, bytes.Length);
    }

    public static void smethod_1(string string_0, int int_0, string string_1, int int_1)
    {
        while (true)
        {
            byte[] buffer = new byte[4];
            networkStream_0.Read(buffer, 0, buffer.Length);
            TcpClient client = new TcpClient(string_1, int_1);
            TcpClient client2 = new TcpClient(string_0, int_0);
            client.SendTimeout = 0x493e0;
            client.ReceiveTimeout = 0x493e0;
            client2.SendTimeout = 0x493e0;
            client2.ReceiveTimeout = 0x493e0;
            client.GetStream().Write(buffer, 0, buffer.Length);
            object state = new TcpClient[] { client, client2 };
            object obj3 = new TcpClient[] { client2, client };
            ThreadPool.QueueUserWorkItem(new WaitCallback(Class0.smethod_2), state);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Class0.smethod_2), obj3);
        }
    }

    public static void smethod_2(object object_0)
    {
        TcpClient client = ((TcpClient[]) object_0)[0];
        TcpClient client2 = ((TcpClient[]) object_0)[1];
        NetworkStream stream = client.GetStream();
        NetworkStream stream2 = client2.GetStream();
        Console.WriteLine("转发中...");
        while (true)
        {
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

