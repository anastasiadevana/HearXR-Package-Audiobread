using UnityEngine;

namespace HearXR.Audiobread.SoundProperties
{
    public abstract class IntSoundProperty : SoundProperty<int>
    {
        public override ValueContainer<int> CreateValueContainer()
        {
            return new IntValueContainer(DefaultValue);
        }

        public override Calculator CreateCalculator()
        {
            return new IntCalculator(this);
        }
    }
}
