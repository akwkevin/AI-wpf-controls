using System.Windows;

namespace AIStudio.Wpf.Panels
{
    interface IFluidWrapPanel : IInputElement
    {
        void BeginFluidDrag(UIElement child, Point position);
        void FluidDrag(UIElement child, Point position, Point positionInParent);
        void EndFluidDrag(UIElement child, Point position, Point positionInParent);
    }
}
