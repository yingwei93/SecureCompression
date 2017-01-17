using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Numerics;

namespace SecureCompression
{
    class Compress
    {
        public const int alpha = 256;
        public const int codes = 4096;
        public const int ByteSize = 8;
        public const int excess = 4;
        public const int mask1 = 255;
        public const int mask2 = 15;
        public int status = 0;
        public UInt32 used = alpha;
        public int LeftOver;

        public static Stopwatch sw = new Stopwatch();
      
        static Dictionary<UInt32, UInt32> D = new Dictionary<UInt32, UInt32>();
        public Compress() { }
       
        public void Output(UInt32 pcode, ref List<byte> writeData)
        {
            byte c, d;
            if (status == 1)
            {
                d = (byte)(pcode & mask1);
                c = (byte)((LeftOver << excess) | (pcode >> ByteSize));
                writeData.Add(c);
                writeData.Add(d);
                status = 0;
            }
            else
            {
                LeftOver = (byte)(pcode & mask2);
                c = (byte)(pcode >> excess);
                writeData.Add(c);
                status = 1;
            }
        }
        //初始化字典
        public static void InitializaDictionary()
        {
            for (int i = 0; i < alpha; i++)
            {
                D.Add((UInt32)(i), (UInt32)(i));
            }
        }
        //清空字典
        public static void EmptyDictionary()
        {
            D.Clear();
        }
        public static bool Search(UInt32 k, out KeyValuePair<UInt32, UInt32> kkk)
        {
            foreach (KeyValuePair<UInt32, UInt32> kv in D)
            {
                if (kv.Value.Equals(k))
                {
                    kkk = new KeyValuePair<UInt32, UInt32>(kv.Key, kv.Value);
                    return true;
                }
            }
            kkk = new KeyValuePair<UInt32, UInt32>(0, 0);
            return false;
        }
        public byte[] Compression(ref UInt32 location, byte[] readData, ref int position, ref byte[] comsequence,BigInteger prime)
        {
            sw.Start();

            InitializaDictionary();
            List<byte> writeData = new List<byte>();
            byte c = readData[location];
            UInt32 pcode = c;
            KeyValuePair<UInt32, UInt32> kkk;
            UInt32 i;
            for (i = location + 1; i < readData.Length; i++)
            {
                c = readData[i];
                UInt32 k = (pcode << ByteSize) + c;//这个和下一个字符
                if (Search(k, out kkk))
                {
                    pcode = kkk.Key;
                }
                else
                {
                    Output(pcode, ref writeData);
                    if (used < codes)
                    {
                        D.Add(used++, ((pcode << ByteSize) | c));
                        pcode = c;
                        if (used == codes) break;
                    }
                }
            }
            Output(pcode, ref writeData);
            if (status == 1)
            {
                c = (byte)(LeftOver << excess);
                writeData.Add(c);
            }
            location = i + 1;//下一个要读的
            used = alpha;
            byte[] writeData1 = writeData.ToArray();
            
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("压缩花费{0}ms.", ts2.TotalMilliseconds);
            System.Threading.Thread.Sleep(5);
            sw.Reset();

            LegendreSequence ee = new LegendreSequence();
            byte[] key = ee.comsequence(writeData1, ref position, ref comsequence,prime);
            EmptyDictionary();
            status = 0;
            return key;
        }
    }
}
