using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Rolodex.Silverlight.Behaviors
{
    public class EnterKeyBehavior : System.Windows.Interactivity.Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }

        private void AssociatedObject_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (TargetButton != null && TargetButton.IsEnabled)
                {
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(TargetButton);
                    IInvokeProvider invokeProvider =  peer.GetPattern(PatternInterface.Invoke)  as IInvokeProvider;
                    invokeProvider.Invoke();
                }
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
            base.OnDetaching();
        }



        public Button TargetButton
        {
            get { return (Button)GetValue(TargetButtonProperty); }
            set { SetValue(TargetButtonProperty, value); }
        }

        public static readonly DependencyProperty TargetButtonProperty =
            DependencyProperty.Register("TargetButton", typeof(Button), typeof(EnterKeyBehavior), new PropertyMetadata(null));

        

    }
}
