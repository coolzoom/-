Imports System
Imports System.Collections.Generic
Imports System.Net.Sockets
Imports System.Threading

Module Program
    Sub Main(args As String())
        If (args.Length <> 2) Then
            Console.WriteLine("PortTran by k8gege")
            Console.WriteLine("usage: PortTranS.exe TranPort ConnPort")
        Else
            Dim state As String = args(0)
            Dim str2 As String = args(1)
            Console.WriteLine(("[+] Listening TranPort " & state & " ..."))
            Console.WriteLine(("[+] Listening ConnPort " & str2 & " ..."))
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf smethod_0), state)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf smethod_1), str2)
            WaitHandle.WaitAll(New ManualResetEvent() {New ManualResetEvent(False)})
        End If
    End Sub

    Public Sub smethod_0(ByVal object_0 As Object)
        Dim listener As New TcpListener(Integer.Parse(object_0.ToString))
        listener.Start()

        Do While True
            smethod_2(listener.AcceptTcpClient)
        Loop
    End Sub

    Public Sub smethod_1(ByVal object_0 As Object)
        Dim listener As New TcpListener(Integer.Parse(object_0.ToString))
        listener.Start()

        Do While True
            Dim client As TcpClient = listener.AcceptTcpClient
            Dim key As Integer = New Random().Next(&H3B9ACA00, &H77359400)
            dictionary_0.Add(key, client)
            Dim bytes As Byte() = BitConverter.GetBytes(key)
            networkStream_0.Write(bytes, 0, bytes.Length)
        Loop
    End Sub

    Public Sub smethod_2(ByVal tcpClient_0 As TcpClient)
        Dim stream As NetworkStream = tcpClient_0.GetStream
        Dim buffer As Byte() = New Byte(4 - 1) {}
        If (((stream.Read(buffer, 0, buffer.Length) = 2) AndAlso (buffer(0) = &H6F)) AndAlso (buffer(1) = &H6B)) Then
            networkStream_0 = stream
            Console.WriteLine("[+] Accept Connect OK!")
            Console.WriteLine("[+] Waiting Another Client on ConnPort ...")
        Else
            smethod_3(BitConverter.ToInt32(buffer, 0), tcpClient_0)
        End If
    End Sub

    Public Sub smethod_3(ByVal int_0 As Integer, ByVal tcpClient_0 As TcpClient)
        Dim client As TcpClient = Nothing
        If dictionary_0.ContainsKey(int_0) Then
            dictionary_0.TryGetValue(int_0, client)
            dictionary_0.Remove(int_0)
            tcpClient_0.SendTimeout = &H493E0
            tcpClient_0.ReceiveTimeout = &H493E0
            client.SendTimeout = &H493E0
            client.ReceiveTimeout = &H493E0
            Dim state As Object = New TcpClient() {tcpClient_0, client}
            Dim obj3 As Object = New TcpClient() {client, tcpClient_0}
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf smethod_4), state)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf smethod_4), obj3)
        End If
    End Sub

    Public Sub smethod_4(ByVal object_0 As Object)
        Dim client As TcpClient = DirectCast(object_0, TcpClient())(0)
        Dim client2 As TcpClient = DirectCast(object_0, TcpClient())(1)
        Dim stream As NetworkStream = client.GetStream
        Dim stream2 As NetworkStream = client2.GetStream
        Do While True
            Console.WriteLine("transferring...")
            Try
                Dim buffer As Byte() = New Byte(&H2800 - 1) {}
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
