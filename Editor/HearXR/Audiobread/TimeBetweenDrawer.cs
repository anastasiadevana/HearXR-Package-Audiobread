using UnityEditor;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(TimeBetweenDefinition))]
    public class TimeBetweenDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            TimeBetween soundProperty = BuiltInData.Instance.properties.GetSoundPropertyByType<TimeBetween>();
            float defaultValue = soundProperty.DefaultValue;
            return (soundProperty, defaultValue);
        }
    }
}