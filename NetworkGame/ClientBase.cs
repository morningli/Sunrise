using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace NetworkFrame
{
    public class ClientBase
    {
        public Socket m_soket = null;
        // Size of receive buffer.     
        public const int BufferSize = 1024;
        // Receive buffer.     
        public byte[] m_buffer = new byte[BufferSize];
        // Received data string.     
        public StringBuilder m_strBuiler = new StringBuilder();
        // 协议
        public ProtocolBase protocol = new ProtocolBase();

        public virtual void ReceiveCallback()
        {
            // 需要做分包操作
            int bytesRead = protocol.InputChcek(m_strBuiler.ToString());
            if (bytesRead > 0)
            {
                Debug.Log("Data arrive: " + protocol.Unpack(m_strBuiler.ToString(0, bytesRead)).ToString());
            }
            m_strBuiler.Remove(0, bytesRead);
        }

        public virtual void SendCallback(int bytesSent)
        {
            Debug.Log("Sent data success.bytes: " + bytesSent.ToString());
        }
    }
}
