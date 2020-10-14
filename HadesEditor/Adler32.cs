using System;
using System.Collections.Generic;
using System.Text;

namespace HadesEditor
{


    // lmao not my code, i don't care if it's ugly as shit

    public static class Adler32
    {
        // Token: 0x06002E9E RID: 11934 RVA: 0x000BBC54 File Offset: 0x000B9E54
        public static uint checksum(uint adler, byte[] buf, int index, int len)
        {
            if (buf == null)
            {
                return 1u;
            }
            long num = (long)((ulong)(adler & 65535u));
            long num2 = (long)((ulong)(adler >> 16 & 65535u));
            while (len > 0)
            {
                int i = (len < 5552) ? len : 5552;
                len -= i;
                while (i >= 16)
                {
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    num += (long)(buf[index++] & byte.MaxValue);
                    num2 += num;
                    i -= 16;
                }
                if (i != 0)
                {
                    do
                    {
                        num += (long)(buf[index++] & byte.MaxValue);
                        num2 += num;
                    }
                    while (--i != 0);
                }
                num %= 65521L;
                num2 %= 65521L;
            }
            return (uint)(num2 << 16 | num);
        }

        // Token: 0x04001864 RID: 6244
        private const int BASE = 65521;

        // Token: 0x04001865 RID: 6245
        private const int NMAX = 5552;
    }
}
