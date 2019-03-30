using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using FighterGame.Network.Message;
using System.IO;
using System.Threading;
using ProtoBuf;

namespace FighterGame.Network.Standardization
{
    public class StandardizedUDP : INetworkStandardization
    {
        private UdpClient udpSender;
        private UdpClient udpListener;

        public StandardizedUDP(IPEndPoint listenPoint, IPEndPoint sendPoint)
        {
            udpSender = new UdpClient();
            udpSender.Connect(sendPoint);

            udpListener = new UdpClient(listenPoint);
        }

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public async Task<object> ReadObject()
        {
            //Wait for access
            await semaphoreSlim.WaitAsync();

            if (enabled)
            {
                UdpReceiveResult result;

                //Read object from the stream
                try
                {
                    result = await udpListener.ReceiveAsync();
                }
                catch
                {
                    //Network failure
                    Disable();
                    return null;
                }
                finally
                {
                    semaphoreSlim.Release();
                }

                if (result.Buffer.Length < TransferStandards.TYPE_PRECEDING_BYTES) return null;
                byte[] objectData = result.Buffer.Skip(TransferStandards.TYPE_PRECEDING_BYTES).ToArray();
                byte[] typeData = result.Buffer.Take(TransferStandards.TYPE_PRECEDING_BYTES).ToArray();

                Type specifiedType = TransferStandards.MessageTypes[TransferStandards.ByteSequenceToInteger(typeData)];

                //Attempt deserialization
                try
                {
                    return Serializer.Deserialize(specifiedType, new MemoryStream(objectData));
                }
                catch
                {
                    //Serialization failure, likely lost data
                    return null;
                }
            }
            else return null;
        }

        public async Task<bool> SendMessage(INetworkMessage messageToSend)
        {
            //Convert to standard sequence
            byte[] buffer = TransferStandards.MessageToStandardSequence(messageToSend, sizeData: false, typeData: true);

            try
            {
                //Send serialized object
                await udpSender.SendAsync(buffer, buffer.Length);
            }
            catch
            {
                //Network error
                Disable();
                return false;
            }

            return true;
        }

        private bool enabled = true;
        public void Disable()
        {
            if (enabled)
            {
                udpSender.Close();
                udpListener.Close();
                enabled = false;
            }
        }
    }
}
