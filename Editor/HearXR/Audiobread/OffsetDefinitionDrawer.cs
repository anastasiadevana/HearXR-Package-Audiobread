using UnityEditor;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(OffsetDefinition))]
    public class OffsetDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            Offset property = BuiltInData.Instance.properties.GetSoundPropertyByType<Offset>();
            float defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}