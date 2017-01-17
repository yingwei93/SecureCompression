using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SecureCompression
{
    class Decompress
    {
        private int prefix;
        private byte suffix;
        public const int alpha = 256;
        public const int codes = 4096;
        public const int ByteSize = 8;
        public const int excess = 4;
        public const int mask = 15;
        public int size;
        public int status = 0;
        public UInt32 used = alpha;
        public int LeftOver;
        public byte[] s = new byte[codes];
        public FileStream FP = new FileStream("E://cc.txt", FileMode.Open, FileAccess.ReadWrite);//写
        public struct element
        {
            public UInt32 prefix;
            public UInt32 suffix;
            public element(UInt32 prefix, UInt32 suffix)
            {
                this.prefix = prefix;
                this.suffix = suffix;
            }
        };
        public static element[] D = new element[codes];//建立的字典D是以element的数组，共有4096，包括前缀和后缀
        public static void InitializaDictionary()//初始化字典
        {

            for (UInt32 i = 0; i < alpha; i++)
            {
                D[i] = new element(i, i);//前缀和后缀都是i
            }
        }
        public static void EmptyDictionary()
        {
            for (UInt32 i = 0; i < codes; i++)
            {
                D[i] = new element();
            }

        }
        public void Output(int code)//输出与代码相对应的字符串
        {
            size = -1;
            while (code >= alpha)//大于单字符表时有前缀后缀问题
            {
                s[++size] = (byte)(D[code].suffix);
                code = (int)(D[code].prefix);
            }
            s[++size] = (byte)code;//小于alpha时，解码就是code的byte位形式
            for (int i = size; i >= 0; i--)
            {
                FP.WriteByte(s[i]);
            }
        }
        public bool GetCode(out int code, byte[] readByte, ref int length, ref int index)
        {
            byte c, d;
            if (length == 0)//被压缩的代码长度为0
            {
                code = (byte)0;
                return false;
            }
            else
            {
                c = readByte[index++];
                length--;//未读代码数减一
                if (status == 1)//上次有剩余的4位，可得12位
                    code = (LeftOver << ByteSize) | c;
                else
                {
                    d = readByte[index++];
                    length--;
                    code = (c << excess) | (d >> excess);
                    LeftOver = d & mask;
                }
                status = 1 - status;
                return true;
            }
        }
        public void Decompression(byte[] readByte, int takeout)
        {
            InitializaDictionary();
            int length = readByte.Length;
            int index = 0;
            byte c = readByte[0];
            int pcode = (int)c;//前一个代码
            int ccode = (int)readByte[1];//当前代码
            if (GetCode(out pcode, readByte, ref length, ref index))//文件不为空
            {
                s[0] = (byte)pcode;//pcode代表的字符
                FP.WriteByte(s[0]);//输出第一个字符
                size = 0;
                while (GetCode(out ccode, readByte, ref length, ref index))//取另外的代码
                {
                    if (ccode < used)//此代码已经存在
                    {
                        Output(ccode);
                        if (used < codes)//创建新代码
                        {
                            D[used].prefix = (UInt32)pcode;
                            D[used++].suffix = s[size];
                        }
                    }
                    else
                    {
                        D[used].prefix = (UInt32)pcode;
                        D[used++].suffix = s[size];
                        Output(ccode);
                    }
                    pcode = ccode;
                }
            }
            if (takeout == 1)
                FP.Close();
            EmptyDictionary();
            s = new byte[codes];
            used = alpha;
            status = 0;
        }
    }
}
