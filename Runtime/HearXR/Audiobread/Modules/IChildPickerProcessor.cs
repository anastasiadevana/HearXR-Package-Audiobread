namespace HearXR.Audiobread
{
    public interface IChildPickerProcessor : ISoundModuleProcessor
    {
        int GetNextChildIndex(int lastChildIndex = -1);
    }
}