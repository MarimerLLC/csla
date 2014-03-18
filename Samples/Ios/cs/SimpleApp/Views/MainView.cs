using System;
using System.Drawing;

using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SimpleApp.Views
{
    [Register("MainView")]
    public class MainView : UIView
    {
        public UIButton btnMarkForDelete;
        public UIButton btnCancel;
        public UIButton btnSave;
        public UILabel lblTitle;
        public UILabel txtId;
        public UITextField txtName;
        public UILabel txtStatus;
        float buttonHeight = 50;

        public MainView()
        {
            Initialize();
        }

        public MainView(RectangleF bounds)
            : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            this.Frame = UIScreen.MainScreen.Bounds;
            this.BackgroundColor = UIColor.White;
            this.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            lblTitle = new UILabel(new RectangleF(10, 10, this.Frame.Width - 20, 50));
            lblTitle.Font = UIFont.FromName("Helvetica-Bold", 32f);
            lblTitle.Text = "simple app";
            this.AddSubview(lblTitle);

            var label = new UILabel(new RectangleF(10, 100, this.Frame.Width - 20, 30));
            label.Text = "Customer Id";
            this.AddSubview(label);

            txtId = new UILabel(new RectangleF(10, 140, this.Frame.Width - 20, 30));
            //txtId.BorderStyle = UITextBorderStyle.RoundedRect;
            this.AddSubview(txtId);

            label = new UILabel(new RectangleF(10, 180, this.Frame.Width - 20, 30));
            label.Text = "Customer Name";
            this.AddSubview(label);

            txtName = new UITextField(new RectangleF(10, 220, this.Frame.Width - 20, 30));
            txtName.BorderStyle = UITextBorderStyle.RoundedRect;
            this.AddSubview(txtName);

            label = new UILabel(new RectangleF(10, 260, this.Frame.Width - 20, 30));
            label.Text = "Status";
            this.AddSubview(label);

            txtStatus = new UILabel(new RectangleF(10, 300, this.Frame.Width - 20, 30));
            //txtStatus.BorderStyle = UITextBorderStyle.RoundedRect;
            this.AddSubview(txtStatus);

            btnMarkForDelete = UIButton.FromType(UIButtonType.RoundedRect);
            btnMarkForDelete.Frame = new RectangleF(1, this.Frame.Height - 50, this.Frame.Width / 3, buttonHeight);
            btnMarkForDelete.SetTitle("Mark for Delete", UIControlState.Normal);
            this.AddSubview(btnMarkForDelete);

            btnCancel = UIButton.FromType(UIButtonType.RoundedRect);
            btnCancel.Frame = new RectangleF((this.Frame.Width / 3) + 1, this.Frame.Height - 50, this.Frame.Width / 3, buttonHeight);
            btnCancel.SetTitle("Cancel", UIControlState.Normal);
            this.AddSubview(btnCancel);

            btnSave = UIButton.FromType(UIButtonType.RoundedRect);
            btnSave.Frame = new RectangleF(((this.Frame.Width / 3) * 2) + 1, this.Frame.Height - 50, this.Frame.Width / 3, buttonHeight);
            btnSave.SetTitle("Save", UIControlState.Normal);
            this.AddSubview(btnSave);

            //btnMarkForDelete.BackgroundColor = UIColor.Gray;
            //btnMarkForDelete.Layer.BorderWidth = 1.0f;
            //btnMarkForDelete.Layer.
            //btnMarkForDelete.Layer.BorderColor = new CGColor("Black");
            //button.TouchUpInside += (object sender, EventArgs e) =>
            //{
            //button.SetTitle(String.Format("clicked {0} times", numClicks++), UIControlState.Normal);
            //};

            //button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
            //    UIViewAutoresizing.FlexibleBottomMargin;
        }
    }
}