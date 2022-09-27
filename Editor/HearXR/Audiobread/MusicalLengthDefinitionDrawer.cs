using UnityEditor;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(MusicalLengthDefinition))]
    public class MusicalLengthDefinitionDrawer : IntDefinitionDrawer
    {
        protected override (IntSoundProperty, int) GetSoundPropertyAndDefaultValue()
        {
            MusicalLength property = BuiltInData.Instance.properties.GetSoundPropertyByType<MusicalLength>();
            int defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}