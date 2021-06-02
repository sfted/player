using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Player.Controls
{
    // https://stackoverflow.com/questions/27641606/loading-a-large-amount-of-images-to-be-displayed-in-a-wrappanel/
    // TODO: разобраться в коде
    public sealed class EffectiveScrollViewer : ScrollViewer
    {
        public static readonly DependencyProperty IsInViewportProperty =
            DependencyProperty.RegisterAttached("IsInViewport", typeof(bool), typeof(EffectiveScrollViewer));

        public static bool GetIsInViewport(UIElement element)
        {
            return (bool)element.GetValue(IsInViewportProperty);
        }

        public static void SetIsInViewport(UIElement element, bool value)
        {
            element.SetValue(IsInViewportProperty, value);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateInViewportValues();
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
            UpdateInViewportValues();
        }

        private void UpdateInViewportValues()
        {
            var panel = Content as Panel;
            if (panel == null) return;

            Rect viewport = new Rect(new Point(0, 0), RenderSize);

            foreach (UIElement child in panel.Children)
            {
                if (!child.IsVisible)
                {
                    SetIsInViewport(child, false);
                    continue;
                }

                GeneralTransform transform = child.TransformToAncestor(this);
                Rect childBounds = transform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
                SetIsInViewport(child, viewport.IntersectsWith(childBounds));
            }
        }
    }
}
