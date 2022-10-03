using UnityEditor;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(MusicalDurationDefinition))]
    public class MusicalDurationDefinitionDrawer : IntDefinitionDrawer
    {
        protected override (IntSoundProperty, int) GetSoundPropertyAndDefaultValue()
        {
            MusicalDuration property = BuiltInData.Instance.properties.GetSoundPropertyByType<MusicalDuration>();
            int defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}