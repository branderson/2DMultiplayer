using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    [SerializableAttribute]
    public class CaseBase : IComparable<CaseBase>, ISerializable
    {
        [NonSerialized] public const int RecordFrames = 120;
        [NonSerialized] public byte Frame = 0;
        public int SituationIndex = 0;
        public int TotalRatio = 0;
        [NonSerialized] public short ActiveResponseState = 0;
        [NonSerialized] private ControllerStateSet activeSet;
        public List<ControllerSequence> ResponseStateList; 

        public CaseBase()
        {
            ResponseStateList = new List<ControllerSequence>();
            activeSet = new ControllerStateSet();
        }

        public bool Empty()
        {
            return ResponseStateList.Count == 0;
        }

        public void PushButtonStateResponse(List<byte> response)
        {
            foreach (byte button in response)
            {
                ActiveResponseState = BLF.SetBit(ActiveResponseState, button);
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
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 8);
                    break;
                case -2:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 9);
                    break;
                case 1:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 10);
                    break;
                case -1:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 11);
                    break;
            }
            switch (response[1])
            {
                case 2:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 12);
                    break;
                case -2:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 13);
                    break;
                case 1:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 14);
                    break;
                case -1:
                    ActiveResponseState = BLF.SetBit(ActiveResponseState, 15);
                    break;
            }
        }

        public void PushActiveResponseState()
        {
//            if (ResponseState[Frame].Any(item => item.Key == ActiveResponseState))
//            {
//                KeyValuePair<short, int> activeResponse = ResponseState[Frame].First(item => item.Key == ActiveResponseState);
//                activeResponse = new KeyValuePair<short, int>(activeResponse.Key, activeResponse.Value + 1);
//                ResponseState[Frame].Remove(ResponseState[Frame].First(item => item.Key == ActiveResponseState));
//                ResponseState[Frame].Add(activeResponse);
//            }
//            else
//            {
//                ResponseState[Frame].Add(new KeyValuePair<short, int>(ActiveResponseState, 1));
//            }
            // Only push when control state has changed and the control state is not 0 on the first frame
            if (activeSet.GetStateList().Count == 0 && !(ActiveResponseState == 0 && Frame == 0))
            {
                activeSet.PushState(ActiveResponseState, Frame);
            }
            else if (activeSet.GetStateList().Count == 0)
            {
                
            }
            else if (ActiveResponseState != activeSet.GetStateList().Last() && !(ActiveResponseState == 0 && Frame == 0))
            {
                activeSet.PushState(ActiveResponseState, Frame);
            }

            ActiveResponseState = 0;
        }

        public void PushActiveSet(int reward, int punish)
        {
            if (activeSet.Empty())
            {
//                MonoBehaviour.print("The set is empty");
            }
            else if (ResponseStateList.Any(item => item.Sequence.SequenceEqual(activeSet.GetStateList())))
            {
                activeSet.Rewarded = reward;
                activeSet.Punished = punish;
////                if (reward > 0)
////                {
////                    MonoBehaviour.print("Rewarding effective move " + reward);
//////                    MonoBehaviour.print(ResponseStateList.First(item => item.Sequence.SequenceEqual(activeSet.GetStateList())).Effectiveness);
////                }
////                if (punish > 0)
////                {
////                    MonoBehaviour.print("Punishing dangerous move " + punish);
//////                    MonoBehaviour.print(ResponseStateList.First(item => item.Sequence.SequenceEqual(activeSet.GetStateList())).Effectiveness);
//                }
                ResponseStateList.First(item => item.Sequence.SequenceEqual(activeSet.GetStateList())).PushSet(activeSet);
                ResponseStateList.OrderByDescending(item => item.Effectiveness);
            }
            else
            {
                ControllerSequence newSequence = new ControllerSequence();
                activeSet.Rewarded = reward;
                activeSet.Punished = punish;
//                if (reward > 0)
//                {
//                    MonoBehaviour.print("Rewarding effective move " + reward);
////                    MonoBehaviour.print(1);
//                }
//                if (punish > 0)
//                {
//                    MonoBehaviour.print("Punishing dangerous move " + punish);
////                    MonoBehaviour.print(-1);
//                }
                newSequence.PushSet(activeSet);
                ResponseStateList.Add(newSequence);
                ResponseStateList.OrderByDescending(item => item.Effectiveness);
            }
            activeSet = new ControllerStateSet();
            Frame = 0;
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

        // Deserialization constructor
        protected CaseBase(SerializationInfo information, StreamingContext context)
        {
//            for (int i = 0; i < RecordFrames; i++)
//            {
//                ResponseState[i] = new List<KeyValuePair<int, int>>();
//            }
            SituationIndex = information.GetInt32("i");
            TotalRatio = information.GetInt32("t");
            ResponseStateList = (List<ControllerSequence>)information.GetValue("r", typeof(List<ControllerSequence>));
            activeSet = new ControllerStateSet();
            //            MonoBehaviour.print("Loaded ResponseState: ");
            //            foreach (KeyValuePair<int, int> response in ResponseState[0])
            //            {
            //                BLF.PrintBinary(response.Key);
            //            }
        }


        // TODO: Don't add (or at least serialize) empty responses or situations
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("i", SituationIndex);
            info.AddValue("t", TotalRatio);
            // Chop off all but the 5 most common versions of each sequence, and store only the 5 most effective sequences
            List<ControllerSequence> storedSequences =
                ResponseStateList.Select(
                    sequence =>
                        new ControllerSequence(
                            sequence.GetVersions().OrderByDescending(item => item.Value).ToList()))
                    .ToList()
                    .OrderByDescending(item => item.Effectiveness)
                    .ToList();
            if (storedSequences.Count > 5)
            {
                storedSequences.RemoveRange(5, storedSequences.Count - 5);
            }
            foreach (ControllerSequence sequence in storedSequences)
            {
                if (sequence.GetVersions().Count > 5)
                {
                    sequence.GetVersions().RemoveRange(5, sequence.GetVersions().Count - 5);
                }
            }
            //            List<KeyValuePair<short, int>>[] storedResponses = new List<KeyValuePair<short, int>>[RecordFrames];
//            for (int i = 0; i < RecordFrames; i++)
//            {
//                if (ResponseState[i].Count < 5)
//                {
//                    storedResponses[i] = ResponseState[i];
//                }
//                else
//                {
//                    storedResponses[i] = ResponseState[i].OrderByDescending(item => item.Value).ToList().GetRange(0, 5);
//                }
//                foreach (KeyValuePair<int, int> response in storedResponses[i])
//                {
//                    BLF.PrintBinary(response.Key);
//                }
//            }
            info.AddValue("r", storedSequences);
        }

        [Serializable]
        public class ControllerStateSet
        {
            public List<KeyValuePair<short, byte>> ControllerStates;
            [NonSerialized] public int Rewarded = 0;
            [NonSerialized] public int Punished = 0;

            public ControllerStateSet()
            {
                ControllerStates = new List<KeyValuePair<short, byte>>();
            }

            public void PushState(short state, byte frame)
            {
                if (ControllerStates.Count == 0)
                {
                    
                }
                else if (ControllerStates.Last().Key == state)
                {
                    throw new UnchangedStateException("The state of the controller has not changed");
                }
                ControllerStates.Add(new KeyValuePair<short, byte>(state, frame));
            }

            public List<short> GetStateList()
            {
                return ControllerStates.Select(item => item.Key).ToList();
            }

            public bool NewStateAtFrame(byte frame)
            {
                return (ControllerStates.Any(item => item.Value == frame));
            }

            public short GetStateAtFrame(byte frame)
            {
                return ControllerStates.First(item => item.Value == frame).Key;
            }

            public bool Empty()
            {
//                foreach (short state in GetStateList())
//                {
//                    BLF.PrintBinary(state);
//                }
                return GetStateList().All(item => item == 0);
            }

            public bool PassedLastState(byte frame)
            {
                return ControllerStates.All(item => item.Value < frame);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (GetType() != obj.GetType())
                {
                    return false;
                }

                return Equals((ControllerStateSet) obj);
            }

            // Test this
            public bool Equals(ControllerStateSet other)
            {
                if (other == null)
                {
                    return false;
                }

                KeyValuePair<short, byte>[] otherDescending = other.ControllerStates.OrderByDescending(item => item.Value).ToArray();
                KeyValuePair<short, byte>[] stateArray = ControllerStates.OrderByDescending(item => item.Value).ToArray();
                if (otherDescending.Length != stateArray.Length)
                {
                    return false;
                }
                bool equal = true;
                for (int i = 0; i < stateArray.Length; i++)
                {
                    if (!(stateArray[i].Key == otherDescending[i].Key && stateArray[i].Value == otherDescending[i].Value))
                    {
                        equal = false;
                    }
                }

//                MonoBehaviour.print("Equal " + equal);
                return equal;
            }

            class UnchangedStateException : Exception
            {
                public UnchangedStateException()
                {
                }

                public UnchangedStateException(string message) : base(message)
                {
                }

                public UnchangedStateException(string message, Exception inner) : base(message, inner)
                {
                }
            }
        }

        [Serializable]
        public class ControllerSequence
        {
            private List<KeyValuePair<ControllerStateSet, int>> SequenceVersions; // Value is how many times that version came up
            public List<short> Sequence = null;
            public int Effectiveness = 0;

            public ControllerSequence()
            {
                SequenceVersions = new List<KeyValuePair<ControllerStateSet, int>>();
            }

            public ControllerSequence(List<KeyValuePair<ControllerStateSet, int>> versions)
            {
                SequenceVersions = versions;
                Sequence = versions.First().Key.GetStateList();
            }

            public List<KeyValuePair<ControllerStateSet, int>> GetVersions()
            {
                return SequenceVersions;
            }

            public void SetVersions(List<KeyValuePair<ControllerStateSet, int>> versions)
            {
                SequenceVersions = versions;
            }

            public void PushSet(ControllerStateSet set)
            {
                if (Sequence == null)
                {
                    Sequence = set.GetStateList();
                }
                // Adding a sequence that has been seen before exactly
                if (SequenceVersions.Any(item => item.Key.Equals(set)))
                {
//                    MonoBehaviour.print("Adding identical sequence");
//                    foreach (KeyValuePair<short, byte> state in set.ControllerStates)
//                    {
//                        BLF.PrintBinary(state.Key);
//                    }
                    KeyValuePair<ControllerStateSet, int> activeSequence = SequenceVersions.First(item => item.Key.Equals(set));
                    activeSequence = new KeyValuePair<ControllerStateSet, int>(activeSequence.Key, activeSequence.Value + 1);
//                    MonoBehaviour.print("Seen " + activeSequence.Value + " times");
                    SequenceVersions.Remove(SequenceVersions.First(item => item.Key.Equals(activeSequence.Key)));
                    SequenceVersions.Add(activeSequence);
                    SequenceVersions = SequenceVersions.OrderByDescending(item => item.Value).ToList();
                }
                // Adding a new version of the sequence
                else
                {
//                    MonoBehaviour.print("Adding new sequence");
//                    foreach (KeyValuePair<short, byte> state in set.ControllerStates)
//                    {
//                        BLF.PrintBinary(state.Key);
//                        MonoBehaviour.print(state.Value);
//                    }
                    SequenceVersions.Add(new KeyValuePair<ControllerStateSet, int>(set, 1));
                }
                Effectiveness += set.Rewarded;
                Effectiveness -= set.Punished;
            }
        }
    }

}