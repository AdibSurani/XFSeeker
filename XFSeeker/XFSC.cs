namespace XFSeeker.XFSC
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class rCharacter
    {
        [XmlElement("mpModel")]
        public TypePath mpModel;
        [XmlElement("mpMotionList")]
        public TypePath mpMotionList;
        [XmlElement("PartsDisp")]
        public List<bool> PartsDisp;
        [XmlElement("mVisibilityList")]
        public List<mpNode> mVisibilityList;
        [XmlElement("mMotion")]
        public List<mpNode> mMotion;
        [XmlElement("mRotConstraints")]
        public List<mRotConstraintNode> mRotConstraints;
        [XmlElement("mpFSM")]
        public TypePath mpFSM;
        [XmlAttribute]
        public (int a, int b, int c, int d) mBaseColor;
        [XmlElement("mQuality")]
        public List<uint> mQuality;
    }

    public class TypePath
    {
        [XmlAttribute]
        public string typeName;
        [XmlElement("paths")]
        public List<string> paths;
    }

    public class mpNode
    {
        [XmlAttribute]
        public bool mAutoDelete;
        [XmlElement("mpArray")]
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
            [XmlAttribute]
            public bool loop;
            [XmlElement("scheduler")]
            public TypePath scheduler;
        }
    }

    public class mRotConstraintNode
    {
        [XmlAttribute]
        public byte mBaseNo;
        [XmlAttribute]
        public byte mNo;
        [XmlAttribute]
        public (float x, float y, float t, float w) mRotRatio;
    }
}
