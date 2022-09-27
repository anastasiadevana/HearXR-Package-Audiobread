using UnityEditor;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(DelayDefinition))]
    public class DelayDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            Delay soundProperty = BuiltInData.Instance.properties.GetSoundPropertyByType<Delay>();
            float defaultValue = soundProperty.DefaultValue;
            return (soundProperty, defaultValue);
        }
    }
}