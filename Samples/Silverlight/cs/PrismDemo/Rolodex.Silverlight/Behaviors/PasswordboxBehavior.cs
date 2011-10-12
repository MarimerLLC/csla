using System.Windows;
using System.Windows.Controls;

namespace Rolodex.Silverlight.Behaviors
{
    public class PasswordboxBehavior : System.Windows.Interactivity.Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var updating = (bool?)AssociatedObject.GetValue(PasswordboxBehavior.UpdatingPasswordProperty);
            if (!updating.HasValue || updating.Value == false)
            {
                AssociatedObject.SetValue(PasswordboxBehavior.UpdatingPasswordProperty, true);
                BoundPassword = AssociatedObject.Password;
                AssociatedObject.SetValue(PasswordboxBehavior.UpdatingPasswordProperty, false);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
            base.OnDetaching();
        }


        public string BoundPassword
        {
            get { return (string)GetValue(BoundPasswordProperty); }
            set { SetValue(BoundPasswordProperty, value); }
        }

        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.Register("BoundPassword", typeof(string), typeof(PasswordboxBehavior), new PropertyMetadata(OnBoundPasswordChanged));

        private static void OnBoundPasswordChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PasswordboxBehavior behavior = dependencyObject as PasswordboxBehavior;
            var updating = (bool?)behavior.AssociatedObject.GetValue(PasswordboxBehavior.UpdatingPasswordProperty);
            if (!updating.HasValue || updating.Value == false)
            {
                behavior.AssociatedObject.SetValue(PasswordboxBehavior.UpdatingPasswordProperty, true);
                if (e.NewValue != null)
                {
                    behavior.AssociatedObject.Password = e.NewValue as string;
                }
                else
                {
                    behavior.AssociatedObject.Password = string.Empty;
                }
                behavior.AssociatedObject.SetValue(PasswordboxBehavior.UpdatingPasswordProperty, false);
            }
        }

        private static readonly DependencyProperty UpdatingPasswordProperty =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool?), typeof(PasswordboxBehavior), new PropertyMetadata(null));

        public bool? UpdatingPassword
        {
            get { return (bool?)GetValue(UpdatingPasswordProperty); }
            set { SetValue(UpdatingPasswordProperty, value); }
        }
    }
}
