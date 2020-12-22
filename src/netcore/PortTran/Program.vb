Imports System
Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Module Program
    Sub Main(args As String())
        If (args.Length <> 2) Then
            Console.WriteLine("端口数据转发")
            Console.WriteLine("使用方式: dotnet PortTranServer.dll 中转端口 待连接端口")
        Else
            Dim intPortTrans As Integer = Integer.Parse(args(0).ToString)
            Dim intPortConn As Integer = Integer.Parse(args(1).ToString)
            Console.WriteLine(("[+] 监听中转端口中 " & intPortTrans & " ..."))
            Console.WriteLine(("[+] 监听待连接端口中 " & intPortConn & " ..."))
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf TransPortHandler), intPortTrans)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ConnPortHandler), intPortConn)
            WaitHandle.WaitAll(New ManualResetEvent() {New ManualResetEvent(False)})
        End If
    End Sub

    Public Sub TransPortHandler(ByVal intPort As Integer)
        Dim localAddr As IPAddress = IPAddress.Parse("127.0.0.1")
        Dim listener As New TcpListener(localAddr, intPort)
        listener.Start()

        Do While True
            ClientListener(listener.AcceptTcpClient)
        Loop
    End Sub

    Public Sub ConnPortHandler(ByVal intPort As Integer)
        Dim localAddr As IPAddress = IPAddress.Parse("127.0.0.1")
        Dim listener As New TcpListener(localAddr, intPort)
        listener.Start()

        Do While True
            Dim client As TcpClient = listener.AcceptTcpClient
            Dim key As Integer = New Random().Next(1000000000, 2000000000)
            dictionary_0.Add(key, client)
            Dim bytes As Byte() = BitConverter.GetBytes(key)
            networkStream_0.Write(bytes, 0, bytes.Length)
        Loop
    End Sub

    Public Sub ClientListener(ByVal tcpClient_0 As TcpClient)
        Dim stream As NetworkStream = tcpClient_0.GetStream
        Dim buffer As Byte() = New Byte(4 - 1) {}
        If (((stream.Read(buffer, 0, buffer.Length) = 2) AndAlso (buffer(0) = &H6F)) AndAlso (buffer(1) = &H6B)) Then
            networkStream_0 = stream
            Console.WriteLine("[+] 中转端口连接就绪! ")
            Console.WriteLine("[+] 等待其他客户端连接待连接端口 ...")
        Else
            ClientQueueHandler(BitConverter.ToInt32(buffer, 0), tcpClient_0)
        End If
    End Sub

    Public Sub ClientQueueHandler(ByVal int_0 As Integer, ByVal tcpClient_0 As TcpClient)
        Dim client As TcpClient = Nothing
        If dictionary_0.ContainsKey(int_0) Then
            dictionary_0.TryGetValue(int_0, client)
            dictionary_0.Remove(int_0)
            tcpClient_0.SendTimeout = 300000
            tcpClient_0.ReceiveTimeout = 300000
            client.SendTimeout = 300000
            client.ReceiveTimeout = 300000
            Dim state As TcpClient() = New TcpClient() {tcpClient_0, client}
            Dim obj3 As TcpClient() = New TcpClient() {client, tcpClient_0}
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ClientDataTransferHandler), state)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ClientDataTransferHandler), obj3)
        End If
    End Sub

    Public Sub ClientDataTransferHandler(ByVal tcpClientArray As TcpClient())
        Dim client As TcpClient = tcpClientArray(0)
        Dim client2 As TcpClient = tcpClientArray(1)
        Dim stream As NetworkStream = client.GetStream
        Dim stream2 As NetworkStream = client2.GetStream
        Console.WriteLine("转发中...")
        Do While True
            Try
                Dim buffer As Byte() = New Byte(10240 - 1) {}
                Dim count As Integer = stream.Read(buffer, 0, buffer.Length)
                stream2.Write(buffer, 0, count)
            Catch
                stream.Dispose()
                stream2.Dispose()
                client.Close()
                client2.Close()
                Return
            End Try
        Loop
    End Sub


    ' Fields
    Public dictionary_0 As Dictionary(Of Integer, TcpClient) = New Dictionary(Of Integer, TcpClient)
    Public networkStream_0 As NetworkStream = Nothing
End Module
