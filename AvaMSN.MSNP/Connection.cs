﻿using System.Net.Sockets;
using System.Net;
using System.Text;

namespace AvaMSN.MSNP;

/// <summary>
/// Base class that represents a connection to a server implementing MSNP.
/// </summary>
public class Connection
{
    public Socket? Client { get; private set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 1863;
    public int TransactionID { get; protected set; }
    public bool Connected { get; private set; }

    public event EventHandler<DisconnectedEventArgs>? Disconnected;

    public CancellationTokenSource ReceiveSource { get; set; } = new CancellationTokenSource();

    /// <summary>
    /// Resolves host address and stablishes a connection to it.
    /// </summary>
    /// <returns></returns>
    protected async Task Connect()
    {
        IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(Host);
        IPAddress ipAddress = ipHostInfo.AddressList[0];

        IPEndPoint ipEndPoint = new(ipAddress, Port);

        Client = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        await Client.ConnectAsync(ipEndPoint);
        Connected = true;
    }

    /// <summary>
    /// Sends a text message.
    /// </summary>
    /// <param name="message"></param>
    protected void Send(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        Client!.Send(messageBytes, SocketFlags.None);
    }

    /// <summary>
    /// Asynchronously sends a text message.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected async Task SendAsync(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await Client!.SendAsync(messageBytes, SocketFlags.None);
    }

    /// <summary>
    /// Asynchronously sends a binary message.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected async Task SendAsync(byte[] message)
    {
        await Client!.SendAsync(message, SocketFlags.None);
    }

    /// <summary>
    /// Waits for a message from the server and returns it as a string.
    /// </summary>
    /// <returns>Message received in string format.</returns>
    protected async Task<string> ReceiveStringAsync()
    {
        ReceiveSource.Cancel();
        ReceiveSource = new CancellationTokenSource();
        ReceiveSource.CancelAfter(30000);

        var buffer = new byte[1664];
        var received = await Client!.ReceiveAsync(buffer, SocketFlags.None, ReceiveSource.Token);

        return Encoding.UTF8.GetString(buffer, 0, received);
    }

    /// <summary>
    /// Waits for a message from the server and returns it.
    /// </summary>
    /// <returns>Message received.</returns>
    protected async Task<byte[]> ReceiveAsync()
    {
        ReceiveSource.Cancel();
        ReceiveSource = new CancellationTokenSource();
        ReceiveSource.CancelAfter(30000);

        var buffer = new byte[1664];
        var received = await Client!.ReceiveAsync(buffer, SocketFlags.None, ReceiveSource.Token);

        byte[] response = new byte[received];
        Buffer.BlockCopy(buffer, 0, response, 0, received);

        return response;
    }

    /// <summary>
    /// Continously receives and handles incoming messages.
    /// </summary>
    /// <returns></returns>
    protected async Task ReceiveIncomingAsync()
    {
        ReceiveSource.Cancel();
        ReceiveSource = new CancellationTokenSource();

        try
        {
            while (true)
            {
                var buffer = new byte[1664];
                var received = await Client!.ReceiveAsync(buffer, SocketFlags.None, ReceiveSource.Token);

                byte[] response = new byte[received];
                Buffer.BlockCopy(buffer, 0, response, 0, received);

                HandleIncoming(response);
            }
        }
        catch (OperationCanceledException) { return; }
    }

    /// <summary>
    /// Virtual function to handle incoming messages that aren't the result of a command.
    /// </summary>
    /// <param name="response">Message received.</param>
    /// <returns></returns>
    protected virtual object HandleIncoming(byte[] response) => response switch
    {
        _ => ""
    };

    /// <summary>
    /// Pings the server every 30 seconds so connection isn't lost.
    /// </summary>
    /// <returns></returns>
    public async Task Ping()
    {
        while (true)
        {
            // Send ping
            var message = "PNG\r\n";
            await SendAsync(message);

            await Task.Delay(30000);
        }
    }

    /// <summary>
    /// Sends a disconnection command and invokes Disconnected event.
    /// </summary>
    /// <returns></returns>
    public async Task DisconnectAsync()
    {
        await SendAsync("OUT\r\n");
        Client!.Shutdown(SocketShutdown.Both);
        Client.Dispose();
        Connected = false;

        Disconnected?.Invoke(this, new DisconnectedEventArgs()
        {
            Requested = true
        });
    }
}
