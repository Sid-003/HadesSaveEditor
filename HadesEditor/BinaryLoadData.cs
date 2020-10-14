using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace HadesEditor
{
    public sealed class BinaryLoadData
    {
        // Token: 0x17000AAD RID: 2733
        // (get) Token: 0x06002FCA RID: 12234 RVA: 0x000C09E3 File Offset: 0x000BEBE3
        // (set) Token: 0x06002FCB RID: 12235 RVA: 0x000C09EB File Offset: 0x000BEBEB
        public bool ChecksumFailed { get; private set; }

        // Token: 0x17000AAE RID: 2734
        // (get) Token: 0x06002FCC RID: 12236 RVA: 0x000C09F4 File Offset: 0x000BEBF4
        // (set) Token: 0x06002FCD RID: 12237 RVA: 0x000C09FC File Offset: 0x000BEBFC
        public int Version { get; set; }

        // Token: 0x17000AAF RID: 2735
        // (get) Token: 0x06002FCE RID: 12238 RVA: 0x000C0A05 File Offset: 0x000BEC05
        public long Position
        {
            get
            {
                return this.m_reader.BaseStream.Position;
            }
        }

        // Token: 0x06002FCF RID: 12239 RVA: 0x000C0A17 File Offset: 0x000BEC17
        public BinaryLoadData(bool checksum)
        {
            this.m_checksum = checksum;
            this.Version = int.MinValue;
        }


        public int loadByte() 
        {
            return this.m_reader.BaseStream.ReadByte();
        }


        // Token: 0x06002FD0 RID: 12240 RVA: 0x000C0A31 File Offset: 0x000BEC31
        public short loadShort()
        {
            return this.m_reader.ReadInt16();
        }

        // Token: 0x06002FD1 RID: 12241 RVA: 0x000C0A3E File Offset: 0x000BEC3E
        public ushort loadUShort()
        {
            return this.m_reader.ReadUInt16();
        }

        // Token: 0x06002FD2 RID: 12242 RVA: 0x000C0A4B File Offset: 0x000BEC4B
        public int loadInt()
        {
            return this.m_reader.ReadInt32();
        }

        // Token: 0x06002FD3 RID: 12243 RVA: 0x000C0A4B File Offset: 0x000BEC4B
        public int loadInt32()
        {
            return this.m_reader.ReadInt32();
        }

        // Token: 0x06002FD4 RID: 12244 RVA: 0x000C0A58 File Offset: 0x000BEC58
        public uint loadUInt32()
        {
            return this.m_reader.ReadUInt32();
        }

        // Token: 0x06002FD5 RID: 12245 RVA: 0x000C0A65 File Offset: 0x000BEC65
        public long loadLong()
        {
            return this.m_reader.ReadInt64();
        }

        // Token: 0x06002FD6 RID: 12246 RVA: 0x000C0A72 File Offset: 0x000BEC72
        public float loadFloat()
        {
            return this.m_reader.ReadSingle();
        }

        // Token: 0x06002FD7 RID: 12247 RVA: 0x000C0A72 File Offset: 0x000BEC72
        public float loadSingle()
        {
            return this.m_reader.ReadSingle();
        }

        // Token: 0x06002FD8 RID: 12248 RVA: 0x000C0A7F File Offset: 0x000BEC7F
        public bool loadBool()
        {
            return this.m_reader.ReadBoolean();
        }

        // Token: 0x06002FD9 RID: 12249 RVA: 0x000C0A7F File Offset: 0x000BEC7F
        public bool loadBoolean()
        {
            return this.m_reader.ReadBoolean();
        }

        // Token: 0x06002FDA RID: 12250 RVA: 0x000C0A8C File Offset: 0x000BEC8C
        public string loadString()
        {
            if (this.cppCompatVersion)
            {
                byte[] bytes = this.loadLuaString();
                return Encoding.UTF8.GetString(bytes);
            }
            return this.m_reader.ReadString();
        }

        // Token: 0x06002FDB RID: 12251 RVA: 0x000C0AC0 File Offset: 0x000BECC0
        public string loadStringAllowNull()
        {
            bool flag = this.m_reader.ReadBoolean();
            if (flag)
            {
                return this.loadString();
            }
            return null;
        }

        // Token: 0x06002FDC RID: 12252 RVA: 0x000C0AE4 File Offset: 0x000BECE4
        public List<string> loadStringList()
        {
            int num = this.m_reader.ReadInt32();
            List<string> list = new List<string>(num);
            for (int i = 0; i < num; i++)
            {
                list.Add(this.loadString());
            }
            return list;
        }

        // Token: 0x06002FDD RID: 12253 RVA: 0x000C0B20 File Offset: 0x000BED20
        public void loadStringList(List<string> strings)
        {
            int num = this.m_reader.ReadInt32();
            strings.Capacity = Math.Max(strings.Capacity, num);
            for (int i = 0; i < num; i++)
            {
                strings.Add(this.loadString());
            }
        }

        // Token: 0x06002FDE RID: 12254 RVA: 0x000C0B64 File Offset: 0x000BED64
        public List<int> loadIntList()
        {
            int num = this.m_reader.ReadInt32();
            List<int> list = new List<int>(num);
            for (int i = 0; i < num; i++)
            {
                list.Add(this.m_reader.ReadInt32());
            }
            return list;
        }

        // Token: 0x06002FDF RID: 12255 RVA: 0x000C0BA4 File Offset: 0x000BEDA4
        public Vector2 loadVector2()
        {
            Vector2 result = default(Vector2);
            result.X = this.m_reader.ReadSingle();
            result.Y = this.m_reader.ReadSingle();
            return result;
        }

        // Token: 0x06002FE0 RID: 12256 RVA: 0x000C0BE0 File Offset: 0x000BEDE0
        public List<Vector2> loadVector2ListFromShort2List(int countLimit = 0)
        {
            ushort num = this.m_reader.ReadUInt16();
            short num2 = this.m_reader.ReadInt16();
            if (countLimit > 0 && (int)num > countLimit)
            {
                return null;
            }
            if (num2 == 0)
            {
                num2 = 1;
            }
            float num3 = (float)num2;
            List<Vector2> list = new List<Vector2>((int)num);
            for (int i = 0; i < (int)num; i++)
            {
                list.Add(new Vector2((float)this.loadShort() * num3, (float)this.loadShort() * num3));
            }
            return list;
        }

        // Token: 0x06002FE1 RID: 12257 RVA: 0x000C0C4C File Offset: 0x000BEE4C
        public List<Vector2> loadVector2List()
        {
            int num = this.m_reader.ReadInt32();
            List<Vector2> list = new List<Vector2>(num);
            for (int i = 0; i < num; i++)
            {
                list.Add(this.loadVector2());
            }
            return list;
        }

        // Token: 0x06002FE2 RID: 12258 RVA: 0x000C0C85 File Offset: 0x000BEE85
        public Vector3 loadVector3()
        {
            return new Vector3(this.m_reader.ReadSingle(), this.m_reader.ReadSingle(), this.m_reader.ReadSingle());
        }

        // Token: 0x06002FE3 RID: 12259 RVA: 0x000C0CB0 File Offset: 0x000BEEB0
        public List<Vector3> loadVector3List()
        {
            int num = this.m_reader.ReadInt32();
            List<Vector3> list = new List<Vector3>(num);
            for (int i = 0; i < num; i++)
            {
                list.Add(this.loadVector3());
            }
            return list;
        }

        // Token: 0x06002FE4 RID: 12260 RVA: 0x000C0CEC File Offset: 0x000BEEEC
        public Color loadColor()
        {
            Color result = default(Color);
            result.PackedValue = this.m_reader.ReadUInt32();
            return result;
        }

        // Token: 0x06002FE5 RID: 12261 RVA: 0x000C0D14 File Offset: 0x000BEF14
        public object loadEnum(Type enumType)
        {
            if (this.cppCompatVersion)
            {
                int num = this.m_reader.ReadInt32();
                try
                {
                    return Enum.ToObject(enumType, num);
                }
                catch (Exception ex)
                {

                    return Activator.CreateInstance(enumType);
                }
            }
            string text = this.m_reader.ReadString();
            object result;
            try
            {
                result = Enum.Parse(enumType, text, true);
            }
            catch (Exception ex2)
            {
                result = Activator.CreateInstance(enumType);
            }
            return result;
        }

        // Token: 0x06002FE6 RID: 12262 RVA: 0x000C0DC0 File Offset: 0x000BEFC0
        public List<T> loadEnumList<T>(Type enumType)
        {
            int num = this.m_reader.ReadInt32();
            List<T> list = new List<T>(num);
            for (int i = 0; i < num; i++)
            {
                object obj = this.loadEnum(enumType);
                list.Add((T)((object)obj));
            }
            return list;
        }

        // Token: 0x06002FE7 RID: 12263 RVA: 0x000C0E04 File Offset: 0x000BF004
        public byte[] loadLuaString()
        {
            int count;
            if (this.cppCompatVersion)
            {
                count = this.m_reader.ReadInt32();
            }
            else
            {
                count = SystemIOExtensions.Read7BitEncodedInt(this.m_reader);
            }
            return this.m_reader.ReadBytes(count);
        }

        // Token: 0x06002FE8 RID: 12264 RVA: 0x000C0E40 File Offset: 0x000BF040
        public void fromStream(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.SetLength(stream.Length);
            byte[] buffer = memoryStream.GetBuffer();
            stream.Read(buffer, 0, (int)stream.Length);
            uint num;
            if (memoryStream.ReadUInt(out num) && num == 826427219u)
            {
                this.cppCompatVersion = true;
            }
            else
            {
                memoryStream.Position = 0L;
            }
            if (this.m_checksum)
            {
                int num2 = this.cppCompatVersion ? 8 : 4;
                if (stream.Length >= (long)num2)
                {
                    uint num3;
                    memoryStream.ReadUInt(out num3);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    uint num4 = Adler32.checksum(1u, buffer, num2, buffer.Length - num2);
                    stopwatch.Stop();
                    this.ChecksumFailed = (num3 != num4);
                }
                else
                {
                    this.ChecksumFailed = true;
                }
            }
            this.m_reader = new BinaryReader(memoryStream);
        }

        // Token: 0x04001908 RID: 6408
        private BinaryReader m_reader;

        // Token: 0x04001909 RID: 6409
        private bool m_checksum;

        // Token: 0x0400190A RID: 6410
        private bool cppCompatVersion;
    }
}
