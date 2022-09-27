using HearXR.Audiobread.SoundProperties;
using UnityEditor;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(HighPassCutoffFrequencyDefinition))]
    public class HighPassCutoffFrequencyDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            var property = BuiltInData.Instance.properties.GetSoundPropertyByType<HighPassCutoffFrequency>();
            var defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}