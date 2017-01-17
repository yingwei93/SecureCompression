using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;
using System.Collections;
namespace SecureCompression
{
    class PrimeGenerate
    {
        //Xn+1=( 75 * X ) mod (231-1)
        public BigInteger T;//BigInteger表示任意大的带符号整数
        public BigInteger a = BigInteger.Pow(7, 5);//
        public BigInteger m = BigInteger.Pow(2, 31) - 1;
        public Random rd = new Random();
        public BigInteger X;
        public BigInteger aX;
        public BigInteger Xn;
        public int s = 5000;
        public static readonly int[] primesBelow2000 = {
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
        101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199,
	211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
	307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
	401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
	503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
	601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
	701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
	809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
	907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997,
	1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097,
	1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193,
	1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297,
	1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399,
	1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499,
	1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597,
	1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699,
	1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789,
	1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
	1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999 };

        /// <summary>
        /// 随机数
        /// </summary>
        /// <returns></returns>
        public BigInteger BigRandom()//伪随机数产生器
        {

            X = rd.Next();
            aX = BigInteger.Multiply(a, X);
            Xn = BigInteger.ModPow(aX, 1, m);
            return Xn;
        }
        /// <summary>
        /// Miller-Rabin素性概率检测法 核心部分    预处理
        /// </summary>
        /// <param name="aa"></param>
        /// <param name="tt"></param>
        /// <returns></returns>
        public bool Witness(BigInteger aa, BigInteger tt)
        {
            int i;
            UInt32 j = (UInt32)tt;
            string s = Convert.ToString((j - 1), 2);
            char[] bb = s.ToCharArray();
            int k = bb.Length;
            BigInteger d = 1;
            BigInteger x;
            for (i = 0; i < k; i++)//k次循环
            {
                x = d;
                d = BigInteger.ModPow(d, 2, tt);
                if (d.IsOne & !x.IsOne & (BigInteger.Compare(x, tt - 1)) != 0)
                    return false;
                if (bb[i] == '1')
                {
                    d = BigInteger.Remainder(BigInteger.Multiply(d, aa), tt);
                }
            }
            if (!d.IsOne)
                return false;
            return true;

        }
        /// <summary>
        /// Miller-Rabin素性概率检测法                      
        /// </summary>
        /// <param name="tt"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool Miler_Rabin(BigInteger tt, int s)
        {
            BigInteger mid;
            BigInteger reminder;
            for (int i = 0; i < primesBelow2000.Length; i++)//用两千以下的素数进行检验
            {
                mid = primesBelow2000[i];
                BigInteger.DivRem(tt, mid, out reminder);
                if (reminder == 0)
                    return false;
            }
            BigInteger aa;
            for (int i = 0; i < s; i++)//检查s次，s的选值
            {
                aa = rd.Next(1, (int)(tt - 1));
                if (!Witness(aa, tt))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 生成大素数
        /// </summary>
        /// <returns></returns>
        public BigInteger GetPrime()
        {
            T = BigRandom();
            if (BigInteger.ModPow(T, 1, 2).IsZero)//if((thisVal.data[0] & 0x1) == 0)// even numbers
                T = BigInteger.Add(T, 1);
            while (true)
            {
                if (Miler_Rabin(T, s))
                    return T;
                else
                    T = BigInteger.Add(T, 2);
            }
        }
    }
}
