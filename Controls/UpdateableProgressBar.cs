using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Player.Controls
{
    // Источник: https://www.codeproject.com/Questions/682276/How-to-update-the-a-progress-bar-with-a-click-even

    public class UpdateableProgressBar : ProgressBar
    {
        public static readonly DependencyProperty RealValueProperty =
            DependencyProperty.Register(
                nameof(RealValue),
                typeof(double),
                typeof(UpdateableProgressBar),
                new FrameworkPropertyMetadata
                (
                    0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRealValueChangedCallBack
                )
            );

        public double RealValue
        {
            get { return (double)GetValue(RealValueProperty); }
            set { SetValue(RealValueProperty, value); }
        }

        public UpdateableProgressBar() : base()
        {
            Cursor = Cursors.Hand;

            MouseDown += MouseDownHandler;
            MouseMove += MouseMoveHandler;
            MouseUp += MouseUpHandler;
        }

        private bool isMouseMoving = false;

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            isMouseMoving = true;
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double mousePosition = e.GetPosition(this).X;
                Value = SetProgressBarValue(mousePosition);
            }
        }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            RealValue = Value;
            Mouse.Capture(null);
            isMouseMoving = false;
        }

        private double SetProgressBarValue(double mousePosition)
        {
            double ratio = mousePosition / ActualWidth;
            double value = ratio * Maximum;

            return value;
        }

        private static void OnRealValueChangedCallBack(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is UpdateableProgressBar u)
                u.OnRealValueChanged(u.RealValue);
        }

        protected virtual void OnRealValueChanged(double value)
        {
            if (!isMouseMoving)
                Value = value;
        }
    }
}
