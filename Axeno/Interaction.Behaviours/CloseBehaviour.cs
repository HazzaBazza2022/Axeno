using System.ComponentModel;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media.Animation;

namespace Axeno.Interaction.Behaviours
{
    public class CloseBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(CloseBehavior), new PropertyMetadata(default(Storyboard)));

        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
            
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += onWindowClosing;
        }

        private void onWindowClosing(object sender, CancelEventArgs e)
        {
            if (Storyboard == null)
            {
                return;
            }
            e.Cancel = true;
            AssociatedObject.Closing -= onWindowClosing;

            Storyboard.Completed += (o, a) => AssociatedObject.Close();
            Storyboard.Begin(AssociatedObject);
        }
    }
}