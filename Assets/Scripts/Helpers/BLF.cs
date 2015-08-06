using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class BLF
    {
        public static bool IsEven(int value)
        {
            return (value & 1) == 0;
        }
        
        public static bool IsBitSet(int value, int index)
        {
            if (index < 0 || index >= sizeof(int) * 8)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (value & (1 << index)) != 0;
        }

        public static bool IsBitSet(short value, int index)
        {
            if (index < 0 || index >= sizeof(short) * 8)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (value & (1 << index)) != 0;
        }

        public static bool IsBitSet(byte value, int index)
        {
            if (index < 0 || index >= 16)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (value & (1 << index)) != 0;
        }

        public static int SetBit(int value, int index)
        {
            if (index < 0 || index >= sizeof(int) * 8)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value | (1 << index);
        }

        public static short SetBit(short value, int index)
        {
            if (index < 0 || index >= sizeof(short) * 8)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (short)(value | (1 << index));
        }

        public static int UnsetBit(int value, int index)
        {
            if (index < 0 || index >= sizeof(int) * 8)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value & ~(1 << index);
        }

        public static int ToggleBit(int value, int index)
        {
            if (index < 0 || index >= sizeof(int) * 8)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value ^ (1 << index);
        }

        public static int IsolateRightmostSetBit(int value)
        {
            return value & (-value);
        }

        public static void PrintBinary(int value)
        {
            string finalValue = "";
            for (int i = 31; i >= 0; i--)
            {
                if (BLF.IsBitSet(value, i))
                {
                    finalValue += 1.ToString();
                }
                else
                {
                    finalValue += 0.ToString();
                }
            }
            MonoBehaviour.print(finalValue);
        }
    }
}