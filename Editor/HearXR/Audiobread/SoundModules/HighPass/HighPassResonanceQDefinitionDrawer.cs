using HearXR.Audiobread.SoundProperties;
using UnityEditor;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(HighPassResonanceQDefinition))]
    public class HighPassResonanceQDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            var property = BuiltInData.Instance.properties.GetSoundPropertyByType<HighPassResonanceQ>();
            var defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}