namespace Fray
{
    public interface IElementOfInterest
    {
        bool Visible { get; }

        float Importance { get; }
    }
}