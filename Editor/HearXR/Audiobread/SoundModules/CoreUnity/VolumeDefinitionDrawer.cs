using UnityEditor;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(VolumeDefinition))]
    public class VolumeDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            var property = BuiltInData.Instance.properties.GetSoundPropertyByType<Volume>();
            var defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}