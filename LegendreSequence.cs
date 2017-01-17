using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Diagnostics;

namespace SecureCompression
{
    class LegendreSequence
    {
        public LegendreSequence() { }
        public static Stopwatch sw = new Stopwatch();
        public void swap(ref BigInteger a, ref BigInteger b)
        {
            BigInteger c;
            c = a;
            a = b;
            b = c;
        }
        public int Jacobi(BigInteger a, BigInteger b)
        {

            int count2 = 0;
            BigInteger d;
            BigInteger c;
            while (a > 1)
            {
                int count1 = 0;
                while (a.IsEven)
                {
                    a = a / 2;
                    count1 = count1 + 1;
                }
                d = ((b + 1) * (b - 1) / 8) * count1;
                if (!d.IsEven) count2 = count2 + 1;
                c = (a - 1) * (b - 1) / 4;
                if (!c.IsEven)
                {
                    count2 = count2 + 1;
                }
                swap(ref a, ref b);
                a = BigInteger.ModPow(a, 1, b);
            }
            if (count2 % 2 == 0)        
                return 1;            
            else
                return 0;
        }
        public byte[] comsequence(byte[] readData, ref int position, ref byte[] comsequence,BigInteger prime)
        {
            sw.Start();
            Random rd = new Random();
            BigInteger i = 8 * readData.Length;
            BigInteger start;
            while (true)
            {
                start = rd.Next();
                if (prime.CompareTo(start + i) >= 0)//从起始点算的终点在序列范围内
                    break;
                else
                    continue;
            }
            int[] Jacobisequence = new int[(int)i];
            int k = 0;
            int n;
            for (BigInteger j = start; j < start + i; j++)//生成雅克比序列
            {
                Jacobisequence[k] = Jacobi(j, prime);
                if (k % 8 == 0) Jacobisequence[k] = Jacobisequence[k] *128;
                if (k % 8 == 1) Jacobisequence[k] = Jacobisequence[k] *64;
                if (k % 8 == 2) Jacobisequence[k] = Jacobisequence[k] *32;
                if (k % 8 == 3) Jacobisequence[k] = Jacobisequence[k] *16;
                if (k % 8 == 4) Jacobisequence[k] = Jacobisequence[k] *8;
                if (k % 8 == 5) Jacobisequence[k] = Jacobisequence[k] *4;
                if (k % 8 == 6) Jacobisequence[k] = Jacobisequence[k] *2;
                k++;
            }
           
            byte []Jacobiseq1 = new byte[readData.Length];            
            int t = 0;
            for (int s = 0; s < readData.Length; s++)
            {
                t=Jacobisequence[8*s]+Jacobisequence[8*s+1]+Jacobisequence[8*s+2]+Jacobisequence[8*s+3]+Jacobisequence[8*s+4]+Jacobisequence[8*s+5]+Jacobisequence[8*s+6]+Jacobisequence[8*s+7];
                Jacobiseq1[s]=(byte)t;
            }          
            byte[] comsequence1 = new byte[readData.Length];
            for (n = 0; n < readData.Length; n++)//异或运算得到加密结果
            {
                comsequence1[n] = (byte)((int)Jacobiseq1[n] ^ (int)readData[n]);
            }
            for (int p = 0; p < readData.Length; p++)
            {
                comsequence[p + position] = comsequence1[p];
            }
            position = position + readData.Length;

            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("加密花费{0}ms.", ts2.TotalMilliseconds);
            System.Threading.Thread.Sleep(5);
            sw.Reset();

            return Jacobiseq1;//返回密钥      
        }
    }
}
