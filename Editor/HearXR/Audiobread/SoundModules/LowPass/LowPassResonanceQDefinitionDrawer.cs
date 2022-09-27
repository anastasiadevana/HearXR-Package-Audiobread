using HearXR.Audiobread.SoundProperties;
using UnityEditor;

namespace HearXR.Audiobread
{
    [CustomPropertyDrawer(typeof(LowPassResonanceQDefinition))]
    public class LowPassResonanceQDefinitionDrawer : FloatDefinitionDrawer
    {
        protected override (FloatSoundProperty, float) GetSoundPropertyAndDefaultValue()
        {
            var property = BuiltInData.Instance.properties.GetSoundPropertyByType<LowPassResonanceQ>();
            var defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}