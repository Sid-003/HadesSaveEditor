using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HadesEditor
{
    public static class MemoryExtensions
    {

        public static bool ReadUInt(this Stream stream, out uint val)
        {
            Span<byte> bytes = stackalloc byte[4]
            {
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte()
            };

            val = BinaryPrimitives.ReadUInt32LittleEndian(bytes);
            return true;
        }

        public static bool WriteUInt(this Stream stream, uint val)
        {
            Span<byte> bytes = stackalloc byte[4];


            var b1 = (byte)val >> 0;


            for (int i = 0; i < 4; i++)
                stream.WriteByte((byte)(val >> i * 8));

            BinaryPrimitives.WriteUInt32LittleEndian(bytes, val);

            stream.Write(bytes);

            return true;
        }



    }
}
