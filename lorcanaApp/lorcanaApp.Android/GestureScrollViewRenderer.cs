using Android.Content;
using Android.Views;
using lorcanaApp;
using lorcanaApp.Droid;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GestureScrollView), typeof(GestureScrollViewRenderer))]
namespace lorcanaApp.Droid
{
    public class GestureScrollViewRenderer : ScrollViewRenderer
    {
        readonly CustomGestureListener _listener;
        readonly GestureDetector _detector;

        public GestureScrollViewRenderer(Context context) : base(context)
        {
            _listener = new CustomGestureListener();
            _detector = new GestureDetector(context, _listener);
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (_detector != null)
            {
                _detector.OnTouchEvent(e);
                base.DispatchTouchEvent(e);
                return true;
            }

            return base.DispatchTouchEvent(e);
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            base.OnTouchEvent(ev);

            if (_detector != null)
                return _detector.OnTouchEvent(ev);

            return false;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                _listener.OnSwipeLeft -= HandleOnSwipeLeft;
                _listener.OnSwipeRight -= HandleOnSwipeRight;
            }

            if (e.OldElement == null)
            {
                _listener.OnSwipeLeft += HandleOnSwipeLeft;
                _listener.OnSwipeRight += HandleOnSwipeRight;
            }
        }

        void HandleOnSwipeLeft(object sender, EventArgs e) =>
            ((GestureScrollView)Element).OnSwipeLeft();

        void HandleOnSwipeRight(object sender, EventArgs e) =>
            ((GestureScrollView)Element).OnSwipeRight();
    }


    public class CustomGestureListener : GestureDetector.SimpleOnGestureListener
    {
        static readonly int SWIPE_THRESHOLD = 100;
        static readonly int SWIPE_VELOCITY_THRESHOLD = 100;

        MotionEvent mLastOnDownEvent;

        public event EventHandler OnSwipeLeft;
        public event EventHandler OnSwipeRight;

        public override bool OnDown(MotionEvent e)
        {
            mLastOnDownEvent = e;

            return true;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (e1 == null)
                e1 = mLastOnDownEvent;

            float diffY = e2.GetY() - e1.GetY();
            float diffX = e2.GetX() - e1.GetX();

            if (Math.Abs(diffX) > Math.Abs(diffY))
            {
                if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                {
                    if (diffX > 0)
                        OnSwipeRight?.Invoke(this, null);
                    else
                        OnSwipeLeft?.Invoke(this, null);
                }
            }

            return base.OnFling(e1, e2, velocityX, velocityY);
        }
    }
}