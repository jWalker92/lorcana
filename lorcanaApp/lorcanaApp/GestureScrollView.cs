using System;
using Xamarin.Forms;

namespace lorcanaApp
{
    public class GestureScrollView : ScrollView
    {
        public event EventHandler SwipeLeft;
        public event EventHandler SwipeRight;

        public void OnSwipeLeft() =>
            SwipeLeft?.Invoke(this, null);

        public void OnSwipeRight() =>
            SwipeRight?.Invoke(this, null);

        public GestureScrollView()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                var swipeLeft = new SwipeGestureRecognizer() { Direction = SwipeDirection.Left };
                swipeLeft.Swiped += SwipeLeft_Swiped;
                this.GestureRecognizers.Add(swipeLeft);
                var swipeRight = new SwipeGestureRecognizer() { Direction = SwipeDirection.Right };
                swipeRight.Swiped += SwipeRight_Swiped;
                this.GestureRecognizers.Add(swipeRight);
            }
        }

        private void SwipeLeft_Swiped(object sender, SwipedEventArgs e)
        {
            SwipeLeft?.Invoke(this, null);
        }

        private void SwipeRight_Swiped(object sender, SwipedEventArgs e)
        {
            SwipeRight?.Invoke(this, null);
        }
    }
}

