using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Assets.Scripts.Helpers
{
    [SerializableAttribute]
    public class CaseBase : IComparable<CaseBase>, ISerializable
    {
        [NonSerialized]public const int RecordFrames = 10;
        [NonSerialized]public int Frame = 0;
        public int SituationIndex = 0;
        public int TotalRatio = 0;
        [NonSerialized]public int ActiveResponseState = 0;
        public List<KeyValuePair<int, int>>[] ResponseState = new List<KeyValuePair<int, int>>[RecordFrames];

        public CaseBase()
        {
            for (int i = 0; i < RecordFrames; i++)
            {
                ResponseState[i] = new List<KeyValuePair<int, int>>();
            }
        }

        // Deserialization constructor
        protected CaseBase(SerializationInfo information, StreamingContext context)
        {
            for (int i = 0; i < RecordFrames; i++)
            {
                ResponseState[i] = new List<KeyValuePair<int, int>>();
            }
            SituationIndex = information.GetInt32("i");
            TotalRatio = information.GetInt32("t");
            ResponseState = (List<KeyValuePair<int, int>>[])information.GetValue("r", typeof(List<KeyValuePair<int, int>>[]));
        }

        public void PushButtonPressResponse(List<byte> response)
        {
            foreach (byte button in response)
            {
                ActiveResponseState = BLF.SetBit(ActiveResponseState, button);
            }
        }

        public void PushButtonHoldResponse(List<byte> response)
        {
            foreach (byte button in response)
            {
                ActiveResponseState = BLF.SetBit(ActiveResponseState, button + 8);
            }
        }

        public void PushAnalogResponse(sbyte[] response)
        {
            if (response[0] == 0 || response[1] == 0)
            {
                return;
            }

            switch (response[0])
            {
                case 2:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 16);
                    break;
                case -2:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 17);
                    break;
                case 1:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 18);
                    break;
                case -1:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 19);
                    break;
            }
            switch (response[1])
            {
                case 2:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 20);
                    break;
                case -2:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 21);
                    break;
                case 1:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 22);
                    break;
                case -1:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 23);
                    break;
            }
        }

        public void PushActiveResponseState()
        {
            if (ResponseState[Frame].Any(item => item.Key == ActiveResponseState))
            {
                KeyValuePair<int, int> activeResponse = ResponseState[Frame].First(item => item.Key == ActiveResponseState);
                activeResponse = new KeyValuePair<int, int>(activeResponse.Key, activeResponse.Value + 1);
                ResponseState[Frame].Remove(ResponseState[Frame].First(item => item.Key == ActiveResponseState));
                ResponseState[Frame].Add(activeResponse);
            }
            else
            {
                ResponseState[Frame].Add(new KeyValuePair<int, int>(ActiveResponseState, 0));
            }

            ActiveResponseState = 0;
        }

        public int CompareTo(CaseBase other)
        {
            if (other.SituationIndex > SituationIndex)
            {
                return -1;
            }
            if (other.SituationIndex == SituationIndex)
            {
                return 0;
            }
            return 1;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("i", SituationIndex);
            info.AddValue("t", TotalRatio);
            List<KeyValuePair<int, int>>[] storedResponses = new List<KeyValuePair<int, int>>[RecordFrames];
            for (int i = 0; i < RecordFrames; i++)
            {
                List<KeyValuePair<int, int>> orderedResponses = ResponseState[i].OrderByDescending(item => item.Value).ToList();
                for (int j = 0; j < 5; j++)
                {
                    if (orderedResponses.Count() > j + 1)
                    {
                        storedResponses[i].Add(orderedResponses[j]);
                    }
                }
            }
            info.AddValue("r", storedResponses);
        }
    }
}