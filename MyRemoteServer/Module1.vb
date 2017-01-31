Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Security
Imports System.Threading


Module Module1

    Dim server As TcpListener
    Dim port As Int32
    Dim localAddr As IPAddress
    Dim t As Thread

    ' This subroutine runs in a background thread and accepts incoming connections.
    ' It starts a new thread for each connection accepted.
    Private Sub AcceptConnections()

        ' Buffer for reading data
        Dim bytes(256) As Byte
        Dim data As String = Nothing

        Try
            ' Enter the listening loop.
            While True
                'Console.Write("Waiting for a connection... ")

                ' Perform a blocking call to accept requests.
                ' You could also user server.AcceptSocket() here.
                Dim client As TcpClient = server.AcceptTcpClient()

                Try
                    data = Nothing

                    ' Get a stream object for reading and writing
                    Dim stream As NetworkStream = client.GetStream()

                    Dim sr As StreamReader = New StreamReader(stream)
                    Dim sw As StreamWriter = New StreamWriter(sr.BaseStream)

                    data = sr.ReadLine()

                    Select Case data.ToUpper()
                        Case "STARTITUNES"

                            Dim pArray As Process() = Process.GetProcessesByName("iTunes")
                            If (pArray.Length > 0) Then
                                sw.WriteLine("iTunes running")
                            Else
                                Dim psi As New ProcessStartInfo

                                psi.UseShellExecute = True
                                psi.WorkingDirectory = "c:\users\rui"
                                psi.FileName = "C:\Program Files\iTunes\iTunes.exe"
                                'psi.Arguments = ""
                                'psi.Verb = "runas"
                                Process.Start(psi)

                                sw.WriteLine("iTunes started")
                            End If

                        Case "SHUTDOWN"
                            sw.WriteLine("Shutdown ok")
                    End Select
                    sw.Flush()

                    ' Shutdown and end connection
                    client.Close()
                Catch e As Exception
                End Try

            End While

        Catch e As SocketException
            Console.WriteLine("SocketException: {0}", e)
        Finally
            server.Stop()
        End Try

    End Sub


    Sub Main()

        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.

        Try
            ' Set the TcpListener on port 13000.
            port = 13000
            localAddr = IPAddress.Parse("192.168.1.33")

            server = New TcpListener(localAddr, port)

            ' Start listening for client requests.
            server.Start()

            ' Begin accepting connections...

            t = New Thread(AddressOf AcceptConnections)
            t.Start()


        Catch e As SocketException
            Console.WriteLine("SocketException: {0}", e)
        End Try

    End Sub

End Module
