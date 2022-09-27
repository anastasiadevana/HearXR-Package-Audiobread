using UnityEditor;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(PitchDefinition))]
    public class PitchDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            Pitch soundProperty = BuiltInData.Instance.properties.GetSoundPropertyByType<Pitch>();
            float defaultValue = soundProperty.DefaultValue;
            return (soundProperty, defaultValue);
        }
    }
}