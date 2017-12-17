// This file provides static classes for use with FSM files
namespace XFSeeker.FSM
{
    using System.Collections.Generic;

    public class rAIFSM
    {
        public List<uint> mQuality;
        public string mOwnerObjectName;
        public cAIFSMCluster mpRootCluster;
        public rAIConditionTree mpConditionTree;
        public uint mFSMAttribute;
        public uint mLastEditType;
    }

    public class cAIFSMCluster
    {
        public uint mId;
        public uint mOwnerNodeUniqueId;
        public uint mInitialStateId;
        public List<cAIFSMNode> mpNodeList;
    }

    public class cAIFSMNode
    {
        public string mName;
        public uint mId;
        public uint mUniqueId;
        public uint mOwnerId;
        public cAIFSMCluster mpSubCluster;
        public List<cAIFSMLink> mpLinkList;
        public List<cAIFSMNodeProcess> mpProcessList;
        public uint mUIPos;
        public byte mColorType;
        public uint mSetting;
        public uint mUserAttribute;
        public bool mExistConditionTrainsitionFromAll;
        public uint mConditionTrainsitionFromAllId;
    }

    public class cAIFSMLink
    {
        public string mName;
        public uint mDestinationNodeId;
        public bool mExistCondition;
        public uint mConditionId;
    }

    public class cAIFSMNodeProcess
    {
        public string mContainerName;
        public string mCategoryName;
        public uFSMCharacter.IParameter mpParameter;
    }

    public class uFSMCharacter
    {
        public interface IParameter
        { }

        public class cSetMotion : IParameter
        {
            public int mMotionNo;
            public int mBlendMotionNo;
            public int mBlendMotionNo2;
            public float mAngle;
            public (int x, int y, int z, int w) mSubOffset;
            public float mFrame;
        }

        public class cSpecifyChildAction : IParameter
        {
            public uint mChild;
            public uint mAction;
        }
    }

    public class rAIConditionTree
    {
        public List<uint> mQuality;
        public List<TreeInfo> mpTreeList;

        public class Node
        {
            public List<Node> mpChildList;
        }

        public class TreeInfo
        {
            public cAIDEnum mName;
            public Node mpRootNode;
        }

        public class OperationNode : Node
        {
            // derived: public List<Node> mpChildList;
            public uint mOperator;

            public override string ToString() => $"op{mOperator}({string.Join(",", mpChildList)})";
        }

        public class VariableNode : Node
        {
            // derived: public List<Node> mpChildList;
            public VariableInfo mVariable;
            public bool mIsBitNo;
            public bool mIsArray;
            public bool mIsDynamicIndex;
            public uint mIndex;
            public VariableInfo mIndexVariable;
            public bool mUseEnumIndex;
            public nAI.EnumProp mIndexEnum;

            public struct VariableInfo
            {
                public string mPropertyName;
                public string mOwnerName;
                public bool mIsSingletonOwner;
            }

            public override string ToString() => $"{mVariable.mPropertyName}";
        }

        public class ConstS32Node : Node
        {
            // derived: public List<Node> mpChildList;
            public int mValue;
            public bool mIsBitNo;

            public override string ToString() => $"{mValue}";
        }
    }

    public struct cAIDEnum
    {
        public string mElementName;
        public uint mId;

        public override string ToString() => $"({mElementName}, {mId})";
    }

    public class nAI
    {
        public struct EnumProp
        {
            public string mName;
            public string mEnumName;
        }
    }
}
