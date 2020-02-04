using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GSGE.Code.Helpers;
using Microsoft.Xna.Framework;

namespace SimpleDecoder
{

    // ripped from in-game code using dnSpy with a slight modification.

    // Token: 0x020002F6 RID: 758
    public sealed class BSaveData
    {
        // Token: 0x06002F89 RID: 12169 RVA: 0x000BED58 File Offset: 0x000BCF58
        public BSaveData(int size, bool checksum)
        {
            int num = checksum ? (size - 8) : size;
            byte[] buffer = new byte[num];
            this.m_buffer = new MemoryStream(buffer, 0, num, true, true);
            this.m_buffer.SetLength((long)num);
            this.m_writer = new BinaryWriter(this.m_buffer);
            this.m_checksum = checksum;
        }

        // Token: 0x06002F8A RID: 12170 RVA: 0x000BEDB1 File Offset: 0x000BCFB1
        public BSaveData()
        {
            this.m_buffer = new MemoryStream(1048576);
            this.m_writer = new BinaryWriter(this.m_buffer);
        }

        // Token: 0x06002F8B RID: 12171 RVA: 0x000BEDDA File Offset: 0x000BCFDA
        public void clear()
        {
            this.m_writer.Seek(0, SeekOrigin.Begin);
            Array.Clear(this.m_buffer.GetBuffer(), 0, (int)this.m_buffer.Length);
        }

        // Token: 0x06002F8C RID: 12172 RVA: 0x000BEE07 File Offset: 0x000BD007
        public void saveShort(short value)
        {
            this.m_writer.Write(value);
        }


        // Token: 0x06002F8E RID: 12174 RVA: 0x000BEE23 File Offset: 0x000BD023
        public void saveUShort(ushort value)
        {
            this.m_writer.Write(value);
        }


        // Token: 0x06002F91 RID: 12177 RVA: 0x000BEE48 File Offset: 0x000BD048
        public void saveInt(int value)
        {
            this.m_writer.Write(value);
        }

        // Token: 0x06002F92 RID: 12178 RVA: 0x000BEE48 File Offset: 0x000BD048
        public void saveInt32(int value)
        {
            this.m_writer.Write(value);
        }

        // Token: 0x06002F93 RID: 12179 RVA: 0x000BEE56 File Offset: 0x000BD056
        public void saveLong(long value)
        {
            this.m_writer.Write(value);
        }

        // Token: 0x06002F94 RID: 12180 RVA: 0x000BEE64 File Offset: 0x000BD064
        public void saveFloat(float value)
        {
            this.m_writer.Write(value);
        }

        // Token: 0x06002F95 RID: 12181 RVA: 0x000BEE72 File Offset: 0x000BD072
        public void saveSingle(float value)
        {
            this.m_writer.Write(value);
        }

        // Token: 0x06002F96 RID: 12182 RVA: 0x000BEE80 File Offset: 0x000BD080
        public void saveBool(bool value)
        {
            this.m_writer.Write(value);
        }

        // Token: 0x06002F97 RID: 12183 RVA: 0x000BEE8E File Offset: 0x000BD08E
        public void saveBoolean(bool value)
        {
            this.m_writer.Write(value);
        }

        // Token: 0x06002F99 RID: 12185 RVA: 0x000BEEA8 File Offset: 0x000BD0A8
        public void saveString(string value)
        {
            string s = (value != null) ? value : string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            this.saveInt(bytes.Length);
            this.m_writer.Write(bytes, 0, bytes.Length);
        }

        // Token: 0x06002F9A RID: 12186 RVA: 0x000BEEE6 File Offset: 0x000BD0E6
        public void saveStringAllowNull(string value)
        {
            this.m_writer.Write(value != null);
            if (value != null)
            {
                this.saveString(value);
            }
        }

        // Token: 0x06002F9B RID: 12187 RVA: 0x000BEF04 File Offset: 0x000BD104
        public void saveStringList(List<string> strings)
        {
            this.m_writer.Write(strings.Count);
            foreach (string value in strings)
            {
                this.saveString(value);
            }
        }

        // Token: 0x06002F9C RID: 12188 RVA: 0x000BEF64 File Offset: 0x000BD164
        public void saveIntList(List<int> ints)
        {
            this.m_writer.Write(ints.Count);
            foreach (int value in ints)
            {
                this.m_writer.Write(value);
            }
        }

        // Token: 0x06002F9D RID: 12189 RVA: 0x000BEFC8 File Offset: 0x000BD1C8
        public void saveColor(Color color)
        {
            this.m_writer.Write(color.PackedValue);
        }
        // Token: 0x06002F9F RID: 12191 RVA: 0x000BEFF0 File Offset: 0x000BD1F0
        public void saveVector2(Vector2 vector)
        {
            this.m_writer.Write(vector.X);
            this.m_writer.Write(vector.Y);
        }

        // Token: 0x06002FA1 RID: 12193 RVA: 0x000BF038 File Offset: 0x000BD238
        public void saveVector2ListAsShort2List(List<Vector2> vector2s, int divisor = 1)
        {
            float divider = (float)divisor;
            this.saveUShort((ushort)vector2s.Count);
            this.saveShort((short)divisor);
            foreach (Vector2 value in vector2s)
            {
                Vector2 vector = value / divider;
                this.saveShort((short)vector.X);
                this.saveShort((short)vector.Y);
            }
        }

        // Token: 0x06002FA2 RID: 12194 RVA: 0x000BF0BC File Offset: 0x000BD2BC
        public void saveVector2List(List<Vector2> vector2s)
        {
            this.saveInt(vector2s.Count);
            foreach (Vector2 vector in vector2s)
            {
                this.saveVector2(vector);
            }
        }


        // Token: 0x06002FA4 RID: 12196 RVA: 0x000BF14D File Offset: 0x000BD34D
        public void saveVector3(Vector3 vector)
        {
            this.m_writer.Write(vector.X);
            this.m_writer.Write(vector.Y);
            this.m_writer.Write(vector.Z);
        }

        // Token: 0x06002FA5 RID: 12197 RVA: 0x000BF184 File Offset: 0x000BD384
        public void saveVector3List(List<Vector3> vectors)
        {
            this.saveInt(vectors.Count);
            foreach (Vector3 vector in vectors)
            {
                this.saveVector3(vector);
            }
        }

        // Token: 0x06002FA7 RID: 12199 RVA: 0x000BF1E9 File Offset: 0x000BD3E9
        public void saveEnum(Enum enval)
        {
            this.m_writer.Write(Convert.ToInt32(enval));
        }

        // Token: 0x06002FA8 RID: 12200 RVA: 0x000BF1FC File Offset: 0x000BD3FC
        public void saveEnumList<T>(List<T> enums)
        {
            this.saveInt(enums.Count);
            foreach (T t in enums)
            {
                this.saveEnum(t as Enum);
            }
        }

        // Token: 0x06002FA9 RID: 12201 RVA: 0x000BEE9C File Offset: 0x000BD09C
        public void saveLuaString(string value)
        {
            this.saveString(value);
        }

        // Token: 0x06002FAA RID: 12202 RVA: 0x000BF264 File Offset: 0x000BD464
        public unsafe void saveRawBytes(IntPtr ptr, uint length)
        {
            this.saveInt((int)length);
            byte* ptr2 = (byte*)ptr.ToPointer();
            for (uint num = 0u; num < length; num += 1u)
            {
                this.m_writer.Write(ptr2[num]);
            }
        }


        public unsafe void saveRawBytes(byte[] arr)
        {
            saveInt(arr.Length);
            fixed(byte* ptr = arr)
            {
                for (uint n = 0u; n < arr.Length; n += 1u)
                    this.m_writer.Write(ptr[n]);
            }
         
        }

        // Token: 0x06002FAB RID: 12203 RVA: 0x000BF29C File Offset: 0x000BD49C
        public void toStream(Stream stream)
        {

            stream.WriteUInt(826427219u);
            if (this.m_checksum)
            {
                int d = (int)826427219u;
                byte[] buffer = this.m_buffer.GetBuffer();
                uint num = Adler32.checksum(1u, buffer, 0, buffer.Length);
                byte[] bytes = BitConverter.GetBytes(num);
                
                stream.Write(bytes, 0, bytes.Length);
            }
          
            this.m_buffer.WriteTo(stream);
        }

        // Token: 0x040018A1 RID: 6305
        private BinaryWriter m_writer;

        // Token: 0x040018A2 RID: 6306
        private MemoryStream m_buffer;

        // Token: 0x040018A3 RID: 6307
        private bool m_checksum;
    }
}
