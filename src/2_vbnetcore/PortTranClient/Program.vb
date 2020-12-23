Imports System
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Module Program
    Sub Main(args As String())
        If (args.Length <> 4) Then
            Console.WriteLine("端口数据转发")
            Console.WriteLine("使用方式  dotnet PortTranClient.dll 目标IP 目标端口 跳板机IP 跳板机中转端口")
        Else
            Try
                Dim str As String = args(0)
                Dim num As Integer = Integer.Parse(args(1))
                Dim str2 As String = args(2)
                Dim num2 As Integer = Integer.Parse(args(3))
                Console.WriteLine(String.Concat(New Object() {"[+] 连接至跳板机中转端口 ", str2, ":", num2, "..."}))
                smethod_0(str, num, str2, num2)
                smethod_1(str, num, str2, num2)
                WaitHandle.WaitAll(New ManualResetEvent() {New ManualResetEvent(False)})
            Catch
            End Try
        End If
    End Sub

    Private Sub smethod_0(ByVal string_0 As String, ByVal int_0 As Integer, ByVal string_1 As String, ByVal int_1 As Integer)
        networkStream_0 = New TcpClient(string_1, int_1).GetStream
        Dim bytes As Byte() = Encoding.Default.GetBytes("ok")
        networkStream_0.Write(bytes, 0, bytes.Length)
    End Sub

    Public Sub smethod_1(ByVal string_0 As String, ByVal int_0 As Integer, ByVal string_1 As String, ByVal int_1 As Integer)
        Do While True
            Dim buffer As Byte() = New Byte(4 - 1) {}
            networkStream_0.Read(buffer, 0, buffer.Length)
            Dim client As New TcpClient(string_1, int_1)
            Dim client2 As New TcpClient(string_0, int_0)
            client.SendTimeout = &H493E0
            client.ReceiveTimeout = &H493E0
            client2.SendTimeout = &H493E0
            client2.ReceiveTimeout = &H493E0
            client.GetStream.Write(buffer, 0, buffer.Length)
            Dim state As Object = New TcpClient() {client, client2}
            Dim obj3 As Object = New TcpClient() {client2, client}
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf smethod_2), state)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf smethod_2), obj3)
        Loop
    End Sub

    Public Sub smethod_2(ByVal object_0 As Object)
        Dim client As TcpClient = DirectCast(object_0, TcpClient())(0)
        Dim client2 As TcpClient = DirectCast(object_0, TcpClient())(1)
        Dim stream As NetworkStream = client.GetStream
        Dim stream2 As NetworkStream = client2.GetStream
        Do While True
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
    Public networkStream_0 As NetworkStream = Nothing
End Module
