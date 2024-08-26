using System.Collections.Generic;

public class AnimStruct
{
    public class MotionGroup
    {
        public string Name;
        public float Weight;
    }

    public class MorphGroup
    {
        public string Name;
        public float Weight;
    }

    public class Action
    {
        public string Key;
        public int PlayType;
        public bool Wink;
        public List<string> PlayAnim;
        public List<MotionGroup> Motion;
        public List<MorphGroup> Morph;
    }
}
