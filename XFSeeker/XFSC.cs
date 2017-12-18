namespace XFSeeker.XFSC
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class rCharacter
    {
        [XmlElement("mpModel")]
        public List<TypePath> mpModel;
        [XmlElement("mpMotionList")]
        public List<TypePath> mpMotionList;
        [XmlElement("PartsDisp")]
        public List<bool> PartsDisp;
        [XmlElement("mVisibilityList")]
        public MtArray mVisibilityList;
        [XmlElement("mMotion")]
        public MtArray mMotion;
        [XmlElement("mRotConstraints")]
        public List<RotConstraint> mRotConstraints;
        [XmlElement("mpFSM")]
        public List<TypePath> mpFSM;
        [XmlAttribute]
        public (int a, int b, int c, int d) mBaseColor;
        [XmlElement("mQuality")]
        public List<uint> mQuality;

        public class Motion
        {
            [XmlAttribute]
            public bool loop;
            [XmlElement("scheduler")]
            public List<TypePath> scheduler;
        }

        public class RotConstraint
        {
            [XmlAttribute]
            public sbyte mBaseNo;
            [XmlAttribute]
            public sbyte mNo;
            [XmlAttribute]
            public (float x, float y, float t, float w) mRotRatio;
        }
    }

    public class TypePath
    {
        [XmlAttribute]
        public string typeName;
        [XmlElement("paths")]
        public List<string> paths;
    }

    public struct MtArray
    {
        [XmlAttribute]
        public bool mAutoDelete;
        [XmlElement("mpArray")]
        public List<object> mpArray;
    }
}
