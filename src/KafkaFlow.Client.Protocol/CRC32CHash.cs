namespace KafkaFlow.Client.Protocol
{
    using System;

    public class Crc32CHash
    {
        private const uint Poly = 0x82f63b78;

        private static readonly uint[,] Table = new uint[8, 256];

        static Crc32CHash()
        {
            uint n, crc, k;

            for (n = 0; n < 256; ++n)
            {
                crc = n;
                crc = (crc & 1) != 0 ? (crc >> 1) ^ Poly : crc >> 1;
                crc = (crc & 1) != 0 ? (crc >> 1) ^ Poly : crc >> 1;
                crc = (crc & 1) != 0 ? (crc >> 1) ^ Poly : crc >> 1;
                crc = (crc & 1) != 0 ? (crc >> 1) ^ Poly : crc >> 1;
                crc = (crc & 1) != 0 ? (crc >> 1) ^ Poly : crc >> 1;
                crc = (crc & 1) != 0 ? (crc >> 1) ^ Poly : crc >> 1;
                crc = (crc & 1) != 0 ? (crc >> 1) ^ Poly : crc >> 1;
                crc = (crc & 1) != 0 ? (crc >> 1) ^ Poly : crc >> 1;
                Table[0, n] = crc;
            }

            for (n = 0; n < 256; n++)
            {
                crc = Table[0, n];
                for (k = 1; k < 8; k++)
                {
                    crc = Table[0, crc & 0xff] ^ (crc >> 8);
                    Table[k, n] = crc;
                }
            }
        }

        static uint InternalCompute(uint crci, byte[] data, int offset, int len)
        {
            ulong crc = crci ^ 0xffffffff;

            while (len > 0 && (data[offset] & 7) != 0)
            {
                crc = Table[0, (crc ^ data[offset++]) & 0xff] ^ (crc >> 8);
                len--;
            }

            while (len >= 8)
            {
                var ncopy = BitConverter.ToUInt64(data, offset);

                crc ^= ncopy;
                crc = Table[7, crc & 0xff] ^
                      Table[6, (crc >> 8) & 0xff] ^
                      Table[5, (crc >> 16) & 0xff] ^
                      Table[4, (crc >> 24) & 0xff] ^
                      Table[3, (crc >> 32) & 0xff] ^
                      Table[2, (crc >> 40) & 0xff] ^
                      Table[1, (crc >> 48) & 0xff] ^
                      Table[0, crc >> 56];
                offset += 8;
                len -= 8;
            }

            while (len > 0)
            {
                crc = Table[0, (crc ^ data[offset++]) & 0xff] ^ (crc >> 8);
                len--;
            }

            return (uint) crc ^ 0xffffffff;
        }

        public static uint Compute(byte[] data)
        {
            return InternalCompute(0, data, 0, data.Length);
        }

        public static uint Compute(byte[] data, int offset, int length)
        {
            return InternalCompute(0, data, offset, length);
        }
    }
}
