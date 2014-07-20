using System.Drawing;
using MonoTouch.UIKit;

namespace SimpleApp.Views
{
    public class WaitingOverlay : UIView
    {
        // control declarations
        private UIActivityIndicatorView activitySpinner;
        private UILabel loadingLabel;

        public WaitingOverlay(RectangleF frame, string message)
            : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Black;
            Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

            float labelHeight = 22;
            float labelWidth = Frame.Width - 20;

            // derive the center x and y
            float centerX = Frame.Width/2;
            float centerY = Frame.Height/2;

            // create the activity spinner, center it horizontally and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Frame = new RectangleF(
                centerX - (activitySpinner.Frame.Width/2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new RectangleF(
                centerX - (labelWidth/2),
                centerY + 20,
                labelWidth,
                labelHeight
                ));
            loadingLabel.BackgroundColor = UIColor.Clear;
            loadingLabel.TextColor = UIColor.White;
            loadingLabel.Text = message;
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            this.AddSubview(loadingLabel);
        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            UIView.Animate(
                0.5, // duration
                () => { Alpha = 0; },
                () => { RemoveFromSuperview(); }
                );
        }
    }
}