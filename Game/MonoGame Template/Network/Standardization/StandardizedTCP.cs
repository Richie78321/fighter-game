using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using FighterGame.Network.Message;
using ProtoBuf;

namespace FighterGame.Network.Standardization
{
    public class StandardizedTCP : INetworkStandardization
    {
        private NetworkStream networkStream;

        /// <summary>
        /// Creates a new standardized reader with a network stream.
        /// </summary>
        /// <param name="networkStream">The network stream to be read from.</param>
        public StandardizedTCP(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }

        /// <summary>
        /// Reads the next object in the network stream.
        /// </summary>
        /// <returns>Returns the object read. Returns null if a connection error occurred while reading.</returns>
        public async Task<object> ReadObject()
        {
            try
            {
                //Read preceding object tag and await specified bytes
                int specifiedBytes = TransferStandards.ByteSequenceToInteger(await AwaitIncoming(TransferStandards.LENGTH_PRECEDING_BYTES));
                Type specifiedType = TransferStandards.MessageTypes[TransferStandards.ByteSequenceToInteger(await AwaitIncoming(TransferStandards.TYPE_PRECEDING_BYTES))];
                return Serializer.Deserialize(specifiedType, new MemoryStream(await AwaitIncoming(specifiedBytes)));
            }
            catch
            {
                //Error while reading object, disable
                Disable();
                return null;
            }
        }

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        /// <summary>
        /// Awaits a set number of incoming bytes.
        /// </summary>
        /// <param name="bytesToRead">The number of bytes to await.</param>
        /// <returns>Returns the awaited bytes read from the stream.</returns>
        private async Task<byte[]> AwaitIncoming(int bytesToRead)
        {
            //Wait for access
            await semaphoreSlim.WaitAsync();

            try
            {
                List<byte> readBytes = new List<byte>();
                while (readBytes.Count < bytesToRead)
                {
                    //Await bytes from stream
                    byte[] buffer = new byte[bytesToRead];
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, bytesToRead - readBytes.Count);

                    //Ensure no error has occurred
                    if (bytesRead <= 0) throw new Exception("An error has occurred while attempting to read from the stream.");

                    //Trim array
                    Array.Resize(ref buffer, bytesRead);

                    //Add bytes to read bytes
                    readBytes.AddRange(buffer);
                }

                return readBytes.ToArray();
            }
            catch
            {
                Disable();
                return null;
            }
            finally
            {
                //Completed, release lock
                semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Sends an object to the network stream.
        /// </summary>
        /// <param name="messageToSend">The object to send to the network stream.</param>
        public async Task<bool> SendMessage(INetworkMessage messageToSend)
        {
            //Serialize object with TCPStandard
            byte[] buffer = TransferStandards.MessageToStandardSequence(messageToSend);

            try
            {
                //Send serialized object
                await networkStream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch
            {
                Disable();
                return false;
            }

            return true;
        }

        public void Disable()
        {
            //Close network stream
            networkStream.Close();
        }
    }
}
