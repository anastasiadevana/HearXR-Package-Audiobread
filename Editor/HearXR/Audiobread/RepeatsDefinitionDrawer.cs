using UnityEditor;

namespace HearXR.Audiobread.SoundProperties
{
    [CustomPropertyDrawer(typeof(RepeatsDefinition))]
    public class RepeatsDefinitionDrawer : IntDefinitionDrawer
    {
        protected override (IntSoundProperty, int) GetSoundPropertyAndDefaultValue()
        {
            Repeats property = BuiltInData.Instance.properties.GetSoundPropertyByType<Repeats>();
            int defaultValue = property.DefaultValue;
            return (property, defaultValue);
        }
    }
}