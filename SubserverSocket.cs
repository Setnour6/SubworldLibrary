using System;
using System.IO.Pipes;
using System.Net;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace SubworldLibrary;

internal class SubserverSocket : ISocket
{
	private readonly int index;

	internal static NamedPipeClientStream pipe;

	public SubserverSocket(int index)
	{
		this.index = index;
	}

	void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null)
	{
	}

	void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null)
	{
		byte[] packet = new byte[size + 1];
		packet[0] = (byte)this.index;
		Buffer.BlockCopy(data, offset, packet, 1, size);
		SubworldSystem.SendToPipe(SubserverSocket.pipe, packet);
	}

	void ISocket.Close()
	{
	}

	void ISocket.Connect(RemoteAddress address)
	{
	}

	RemoteAddress ISocket.GetRemoteAddress()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		return (RemoteAddress)new TcpAddress(IPAddress.Any, 0);
	}

	bool ISocket.IsConnected()
	{
		return true;
	}

	bool ISocket.IsDataAvailable()
	{
		return true;
	}

	void ISocket.SendQueuedPackets()
	{
	}

	bool ISocket.StartListening(SocketConnectionAccepted callback)
	{
		return true;
	}

	void ISocket.StopListening()
	{
	}
}
