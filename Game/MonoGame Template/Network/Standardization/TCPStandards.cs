using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using FighterGame.Network.Message;
using ProtoBuf;

namespace FighterGame.Network.Standardization
{
    public static class TransferStandards
    {
        public static readonly Type[] MessageTypes;

        static TransferStandards()
        {
            //Determine maximum serialization size
            for (int i = 0; i < LENGTH_PRECEDING_BYTES; i++) for (int j = 0; j < BITS_IN_BYTE; j++) MAXIMUM_SERIALIZATION_SIZE += (int)Math.Pow(2, (i * BITS_IN_BYTE) + j);

            //Gather message types
            MessageTypes = Assembly.GetAssembly(typeof(INetworkMessage)).GetTypes().Where(t => (t.GetInterfaces().Contains(typeof(INetworkMessage)) && !t.IsInterface)).ToArray();
        }

        public const int LENGTH_PRECEDING_BYTES = 2;
        public const int TYPE_PRECEDING_BYTES = 2;
        private static readonly int MAXIMUM_SERIALIZATION_SIZE;

        public static int ByteSequenceToInteger(byte[] byteSequence)
        {
            //Collect as single binary sequence
            List<bool> binarySequence = new List<bool>();
            for (int i = byteSequence.Length; i > 0; i--) binarySequence.AddRange(ByteToBinary(byteSequence[i - 1]));

            return BinaryToDecimal(binarySequence.ToArray());
        }

        private const int BITS_IN_BYTE = 8;
        public static bool[] ByteToBinary(byte value)
        {
            bool[] binaryValues = new bool[BITS_IN_BYTE];
            for (int i = 0; i < binaryValues.Length; i++)
            {
                if (value - Math.Pow(2, binaryValues.Length - i - 1) >= 0)
                {
                    //Apply value
                    value -= (byte)Math.Pow(2, binaryValues.Length - i - 1);
                    binaryValues[i] = true;
                }
            }

            return binaryValues;
        }

        public static int BinaryToDecimal(bool[] binarySequence)
        {
            int value = 0;
            for (int i = 0; i < binarySequence.Length; i++) if (binarySequence[i]) value += (int)Math.Pow(2, binarySequence.Length - i - 1);
            return value;
        }

        /// <summary>
        /// Converts an object to a sever-client, agreed-upon, binary-serialized format.
        /// </summary>
        /// <param name="value">The object to be serialized.</param>
        /// <returns>The finalized byte buffer.</returns>
        public static byte[] MessageToStandardSequence(INetworkMessage value, bool sizeData = true, bool typeData = true)
        {
            //Serialize object
            Type objectType = value.GetType();
            MemoryStream serializedStream = new MemoryStream();
            Serializer.Serialize(serializedStream, value);

            //Generate preceding value
            byte[] precedingBytes;
            if (sizeData && serializedStream.Length > MAXIMUM_SERIALIZATION_SIZE) throw new Exception("Object serialization is larger than maximum serialization size.");
            else
            {
                //Generate byte tag
                List<byte> byteTag = new List<byte>();

                if (sizeData)
                {
                    byteTag.AddRange(BitConverter.GetBytes((uint)serializedStream.Length).ToList());

                    //Ensure correct length
                    if (byteTag.Count < LENGTH_PRECEDING_BYTES) while (LENGTH_PRECEDING_BYTES - byteTag.Count > 0) byteTag.Insert(0, byte.MinValue);
                    else byteTag.RemoveRange(LENGTH_PRECEDING_BYTES, byteTag.Count - LENGTH_PRECEDING_BYTES);
                }

                if (typeData)
                {
                    //Add type identification
                    uint typeIndex = 0;
                    bool typeIndexFound = false;
                    for (int i = 0; i < MessageTypes.Length; i++)
                    {
                        if (objectType.Equals(MessageTypes[i]))
                        {
                            typeIndex = (uint)i;
                            typeIndexFound = true;
                            break;
                        }
                    }
                    if (!typeIndexFound) throw new Exception("Message has an unknown type.");
                    else byteTag.AddRange(BitConverter.GetBytes(typeIndex));

                    //Ensure correct length
                    if (sizeData)
                    {
                        if (byteTag.Count < LENGTH_PRECEDING_BYTES + TYPE_PRECEDING_BYTES) while ((LENGTH_PRECEDING_BYTES + TYPE_PRECEDING_BYTES) - byteTag.Count > 0) byteTag.Insert(LENGTH_PRECEDING_BYTES, byte.MinValue);
                        else byteTag.RemoveRange(LENGTH_PRECEDING_BYTES + TYPE_PRECEDING_BYTES, byteTag.Count - (LENGTH_PRECEDING_BYTES + TYPE_PRECEDING_BYTES));
                    }
                    else
                    {
                        if (byteTag.Count < TYPE_PRECEDING_BYTES) while (TYPE_PRECEDING_BYTES - byteTag.Count > 0) byteTag.Insert(0, byte.MinValue);
                        else byteTag.RemoveRange(TYPE_PRECEDING_BYTES, byteTag.Count - TYPE_PRECEDING_BYTES);
                    }
                }

                //Set tag
                precedingBytes = byteTag.ToArray();
            }

            //Merge
            List<byte> byteSequence = new List<byte>((int)serializedStream.Length + precedingBytes.Length);
            byteSequence.AddRange(precedingBytes);
            byteSequence.AddRange(serializedStream.GetBuffer().ToList().GetRange(0, (int)serializedStream.Length));
            return byteSequence.ToArray();
        }
    }
}
