using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Numerics;

namespace SecureCompression
{
    class Program
    {
        public static Stopwatch sw = new Stopwatch();
        static Dictionary<int, Byte[]> KEY = new Dictionary<int, Byte[]>();
        public static int l = 0;
        static void Main(string[] args)
        {
            sw.Start();
            FileStream FPlain = new FileStream("E://pp.txt", FileMode.Open);//从pp中读取
            FileStream FC1 = new FileStream("E://mm.txt", FileMode.Open);//mm中是安全压缩后的内容
            UInt32 location = 0;
            int position = 0;
            byte[] readData = new byte[FPlain.Length];
            byte[] comsequence = new byte[readData.Length];
            FPlain.Read(readData, 0, readData.Length);
            Compress cc = new Compress();
            
            PrimeGenerate ff = new PrimeGenerate();
            BigInteger prime = ff.GetPrime();
            
            while (true)
            {
                byte[] key1 = cc.Compression(ref location, readData, ref position, ref comsequence,prime);
                KEY.Add(l++, key1);
                if (location >= readData.Length)
                {
                    FPlain.Close();
                    break;
                }
            }
            FC1.Write(comsequence, 0, position);
            FC1.Close();

            //下为解压缩
            /*FileStream FC2 = new FileStream("E://mm.txt", FileMode.Open);//mm中是安全压缩后的内容
            byte[] getseq = new byte[FC2.Length];
            FC2.Read(getseq, 0, getseq.Length);
            int n, k;
            Decompress dd = new Decompress();
            int temp = 0;
            int takeout = 0;
            for (n = 0; n < l; n++)
            {
                byte[] keys;
                KEY.TryGetValue(n, out keys);
                byte[] getseq1 = new byte[keys.Length];//用来保存解密结果                
                for (k = 0; k < keys.Length; k++)//异或运算得到解密结果
                {
                    getseq1[k] = (byte)((int)keys[k] ^ (int)getseq[k + temp]);
                }
                temp = temp + keys.Length;
                if (n == l - 1)
                    takeout = 1;
                dd.Decompression(getseq1, takeout);
            }*/
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("total花费{0}ms.", ts2.TotalMilliseconds);
            System.Threading.Thread.Sleep(10000000);
        }
    }
}
