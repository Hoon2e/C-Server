using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class CListener
{
    // 비동기 Accept를 위한 EventArgs;
    SocketAsyncEventArgs accept_args;

    // 클라이언트의 접속을 처리할 소켓
    Socket listen_socket;

    // Accept 처리의 순서를 제어하기 위한 이벤트 변수
    AutoResetEvent flow_control_event;

    // 새로운 클라이언트가 접속했을 때 호출되는 델리게이트
    public delegate void NewclientHandler(Socket client_socket, object token);
    public NewclientHandler callback_on_newclient;

    public CListener()
    {
        this.callback_on_newclient = null;
    }

    public void start(string host, int port, int backlog)
    {
        // 소켓을 생성한다
        this.listen_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress address;
        if(host == "0.0.0.0")
        {
            address = IPAddress.Any;
        }
        else
        {
            address = IPAddress.Parse(host);
        }
        IPEndPoint endpoint = new IPEndPoint(address,port);

        try
        {
            // 소켓에 host 정보를 바인딩시킨 뒤 Listen 메서드를 호출하여 대기한다
            this.listen_socket.Bind(endpoint);
            this.listen_socket.Listen(backlog);

            this.accept_args = new SocketAsyncEventArgs();
            this.accept_args.Completed += new EventHandler<SocketAsyncEventArgs>(on_accept_completed);

            // 클라이언트가 들어오기를 기다린다.
            // 비동기 메서드이므로 블로킹되지 않고 바로 리턴되며
            // 콜백 메서드를 통해서 접속 통보를 받는다.
            this.listen_socket.AcceptAsync(this.accept_args);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
        }
    }
}
