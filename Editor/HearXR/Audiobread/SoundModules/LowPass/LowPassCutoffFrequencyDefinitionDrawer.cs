using HearXR.Audiobread.SoundProperties;
using UnityEditor;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(LowPassCutoffFrequencyDefinition))]
    public class LowPassCutoffFrequencyDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            var property = BuiltInData.Instance.properties.GetSoundPropertyByType<LowPassCutoffFrequency>();
            var defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}