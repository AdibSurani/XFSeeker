namespace XFSeeker.XFSC
{
    using System.Collections.Generic;

    public class rCharacter
    {
        public TypePath mpModel;
        public TypePath mpMotionList;
        public List<bool> PartsDisp;
        public List<mpNode> mVisibilityList;
        public List<mpNode> mMotion;
        public List<mRotConstraintNode> mRotConstraints;
        public TypePath mpFSM;
        public (int a, int b, int c, int d) mBaseColor;
        public List<uint> mQuality;
    }

    public class TypePath
    {
        public string typeName;
        public List<string> paths;
    }

    public class mpNode
    {
        public bool mAutoDelete;
        public List<MpArray.IMpArray> mpArray;
    }

    public class MpArray
    {
        public interface IMpArray
        {

        }

        public class VisibilityArray : IMpArray
        {

        }

        public class SchedulerArray : IMpArray
        {
            public bool loop;
            public TypePath scheduler;
        }
    }

    public class mRotConstraintNode
    {
        public byte mBaseNo;
        public byte mNo;
        public (float x, float y, float t, float w) mRotRatio;
    }
}
