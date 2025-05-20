namespace Core.Common.Interfaces
{
    public interface IZoomable
    {
        bool IsZooming { get; }

        void Zoom();
        void Unzoom();
    }
}