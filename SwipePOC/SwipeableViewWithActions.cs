using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace SwipePOC
{
    public abstract class SwipeableViewWithActions : FrameLayout, GestureDetector.IOnGestureListener, View.IOnTouchListener
    {
        private const long SwipeAnimationDuration = 100;

        private float _swipeSlop = -1;
        private Color _leftActionColor = Color.Transparent;
        private Color _rightActionColor = Color.Transparent;
        private GestureDetector _gestureDetector;
        private FrameLayout _leftViewGroup;
        private FrameLayout _rightViewGroup;

        private enum SwipingDirection
        {
            Left,
            Right,
            None
        }

        public Button LeftActionButton { get; private set; }
        public Button RightActionButton { get; private set; }

        public ViewGroup ContentView { get; private set; }

        public bool SwipeToActionEnabled { get; set; } = false;

        public SwipeableViewWithActions(Context context)
            : base(context)
        {
            Initialize();
        }

        public SwipeableViewWithActions(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
        }

        private void Initialize()
        {
            _gestureDetector = new GestureDetector(this);
            _swipeSlop = ViewConfiguration.Get(Context).ScaledTouchSlop;

            _leftViewGroup = new FrameLayout(Context);
            _rightViewGroup = new FrameLayout(Context);

            LayoutParams leftLayoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent, GravityFlags.Left);
            LayoutParams rightLayoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent, GravityFlags.Right);

            AddView(_leftViewGroup, leftLayoutParams);
            AddView(_rightViewGroup, rightLayoutParams);
        }

        protected void AddLeftActionButton(Button actionButton)
        {
            if (LeftActionButton != null)
            {
                _leftViewGroup.RemoveView(LeftActionButton);
                LeftActionButton = null;
                _leftActionColor = Color.Transparent;
            }

            if (actionButton != null)
            {
                LeftActionButton = actionButton;
                if (LeftActionButton.Background is ColorDrawable colorDrawable)
                {
                    _leftActionColor = colorDrawable.Color;
                }
                LayoutParams actionButtonLayoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent, GravityFlags.Left);
                _leftViewGroup.AddView(LeftActionButton, actionButtonLayoutParams);
            }
        }

        protected void AddRightActionButton(Button actionButton)
        {
            if (RightActionButton != null)
            {
                _rightViewGroup.RemoveView(RightActionButton);
                RightActionButton = null;
                _rightActionColor = Color.Transparent;
            }

            if (actionButton != null)
            {
                RightActionButton = actionButton;
                if (RightActionButton.Background is ColorDrawable colorDrawable)
                {
                    _rightActionColor = colorDrawable.Color;
                }
                LayoutParams actionButtonLayoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent, GravityFlags.Right);
                _rightViewGroup.AddView(RightActionButton, actionButtonLayoutParams);
            }
        }

        protected void AddContentView(ViewGroup viewGroup)
        {
            LayoutParams foregroundLayoutParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            ContentView = viewGroup;
            ContentView.SetOnTouchListener(this);
            base.AddView(ContentView, this.ChildCount, foregroundLayoutParams);
        }

        protected override void OnAttachedToWindow()
        {
            if (ContentView == null)
            {
                throw new NullReferenceException(nameof(ContentView));
            }
            base.OnAttachedToWindow();
        }

        #region IOnGestureDectector Implementation
        public bool OnDown(MotionEvent e)
        {
            return true;
        }

        /// <summary>
        /// Notified of a fling event when it occurs with the initial on down MotionEvent and the matching up MotionEvent. 
        /// </summary>
        /// <param name="initialDownMotionEvent">The first down motion event that started the fling</param>
        /// <param name="moveMotionEvent">The move motion event that triggered the current OnFling</param>
        /// <param name="velocityX">The velocity of this fling measured in pixels per second along the X axis</param>
        /// <param name="velocityY">The velocity of this fling measured in pixels per second along the Y axis</param>
        /// <returns>Returns true if consumed, false if not</returns>
        public bool OnFling(MotionEvent initialDownMotionEvent, MotionEvent moveMotionEvent, float velocityX, float velocityY)
        {
            if (CustomOnFling(initialDownMotionEvent, moveMotionEvent, velocityX, velocityY))
            {
                return true;
            }

            float dX = moveMotionEvent.GetX() - initialDownMotionEvent.GetX();
            if (!AllowSwiping(dX))
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Notified when a scroll occurs with the initial on down MotionEvent and the current move MotionEvent.
        /// </summary>
        /// <param name="initialDownMotionEvent">The first down motion event that started the scrolling</param>
        /// <param name="moveMotionEvent">The move motion event that triggered the current OnScroll</param>
        /// <param name="distanceX">The distance along the X axis that has been scrolled since the last call to OnScroll</param>
        /// <param name="distanceY">The distance along the Y axis that has been scrolled since the last call to OnScroll</param>
        /// <returns>Returns true if consumed, false if not</returns>
        public bool OnScroll(MotionEvent initialDownMotionEvent, MotionEvent moveMotionEvent, float distanceX, float distanceY)
        {
            if (CustomOnScroll(initialDownMotionEvent, moveMotionEvent, distanceX, distanceY))
            {
                return true;
            }

            float dX = moveMotionEvent.GetX() - initialDownMotionEvent.GetX();
            if (!AllowSwiping(dX))
            {
                return false;
            }

            ContentView.TranslationX += dX;
            return false;
        }

        public virtual bool CustomOnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public virtual bool CustomOnFling(MotionEvent initialDownEvent, MotionEvent moveMotionEvent, float velocityX, float velocityY)
        {
            return false;
        }

        /// <summary>
        /// Notified when a long press occurs with the initial on down MotionEvent that trigged it.
        /// </summary>
        /// <param name="initialOnDownEvent">Initial on down motion event</param>
        public virtual void OnLongPress(MotionEvent initialOnDownEvent)
        {
        }

        /// <summary>
        /// The user has performed a down MotionEvent and not performed a move or up yet. This event is commonly used to provide visual feedback to the user
        /// </summary>
        /// <param name="downMotion">The down motion event</param>
        public virtual void OnShowPress(MotionEvent downMotion)
        {
        }

        public virtual bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }
        #endregion IOnGestureDectector Implementation

        #region IOnTouchListener implementation
        public bool OnTouch(View v, MotionEvent e)
        {
            if (v.GetType() == ContentView?.GetType())
            {
                if (!_gestureDetector.OnTouchEvent(e))
                {
                    if (e.Action == MotionEventActions.Up || e.Action == MotionEventActions.Cancel)
                    {
                        bool swipingRight = ContentView.TranslationX > 0;
                        bool swipingLeft = ContentView.TranslationX < 0;
                        if (swipingLeft && ContentView.TranslationX > RightActionButtonLeftX())
                        {
                            float centerXPos = RightActionButtonLeftX() / 2;
                            if (ContentView.TranslationX > centerXPos)
                            {
                                ResetView();
                            }
                            else
                            {
                                SwipeToUncoverRightActionButton();
                            }
                        }
                        else if (swipingRight && ContentView.TranslationX < LeftActionButtonRightX())
                        {
                            float centerXPos = LeftActionButtonRightX() / 2;
                            if (ContentView.TranslationX > centerXPos)
                            {
                                SwipeToUncoverLeftActionButton();
                            }
                            else
                            {
                                ResetView();
                            }
                        }
                        else if (ContentView.TranslationX < RightActionButtonLeftX())
                        {
                            if (SwipeToActionEnabled && FullSwipeLeftThresholdMet(ContentView.TranslationX))
                            {
                                SwipeFullLeft();
                            }
                            else
                            {
                                SwipeToUncoverRightActionButton();
                            }
                        }
                        else if (ContentView.TranslationX > LeftActionButtonRightX())
                        {
                            if (SwipeToActionEnabled && FullSwipeRightThresholdMet(ContentView.TranslationX))
                            {
                                SwipeFullRight();
                            }
                            else
                            {
                                SwipeToUncoverLeftActionButton();
                            }
                        }
                        else if (swipingRight && LeftActionButton == null)
                        {
                            ResetView();
                        }
                        else if (swipingLeft && RightActionButton == null)
                        {
                            ResetView();
                        }
                    }
                }
                return true;
            }

            return false;
        }
        #endregion IOnTouchListener implementation

        public abstract void OnFullSwipeLeft();

        public abstract void OnFullSwipeRight();

        #region Swiping Methods
        private void AnimateTranslation(float endX, bool fullSwipe = false)
        {
            int alpha = fullSwipe ? 0 : 1;

            bool swipingLeft = endX < 0;
            bool swipingRight = endX > 0;

            ContentView.Animate().SetDuration(SwipeAnimationDuration)
                .Alpha(alpha)
                .TranslationX(endX)
                .WithEndAction(new Runnable(() =>
                {
                    if (fullSwipe)
                    {
                        if (swipingLeft)
                        {
                            OnFullSwipeLeft();
                            ResetView();
                        }
                        else if (swipingRight)
                        {
                            OnFullSwipeRight();
                            ResetView();
                        }
                    }
                }));

            this.Animate().SetDuration(SwipeAnimationDuration).Alpha(alpha);
        }

        private void SwipeFullLeft()
        {
            AnimateTranslation(RightActionButtonLeftX(), fullSwipe: true);
        }

        private void SwipeFullRight()
        {
            AnimateTranslation(LeftActionButtonRightX(), fullSwipe: true);
        }

        private void SwipeToUncoverRightActionButton()
        {
            AnimateTranslation(RightActionButtonLeftX(), false);
        }

        private void SwipeToUncoverLeftActionButton()
        {
            AnimateTranslation(LeftActionButtonRightX(), false);
        }

        protected void ResetView()
        {
            AnimateTranslation(0);
            this.Background = new ColorDrawable(Color.Transparent);
            if (LeftActionButton != null)
            {
                LeftActionButton.Visibility = ViewStates.Visible;
            }
            if (RightActionButton != null)
            {
                RightActionButton.Visibility = ViewStates.Visible;
            }
        }

        private bool FullSwipeLeftThresholdMet(float dX)
        {
            float activationX = Width * -0.4f;
            return dX < activationX;
        }

        private bool FullSwipeRightThresholdMet(float dX)
        {
            float activationX = Width * 0.4f;
            return dX > activationX;
        }

        private float RightActionButtonLeftX()
        {
            return (RightActionButton?.Width ?? Width) * -1;
        }

        private float LeftActionButtonRightX()
        {
            return LeftActionButton?.Width ?? 0;
        }

        private bool AllowSwiping(float dX)
        {
            SwipingDirection swipingDirection = GetSwipingDirection(dX);

            if (System.Math.Abs(dX) < _swipeSlop)
                return false;

            float centerX = 0;
            SetAppearanceForSwipe(swipingDirection);

            switch (swipingDirection)
            {
                case SwipingDirection.Left:
                    centerX = Width * -0.5f;
                    return (RightActionButton != null || ContentView.TranslationX > 0) && ContentView.TranslationX >= centerX;
                case SwipingDirection.Right:
                    centerX = Width * 0.5f;
                    return (LeftActionButton != null || ContentView.TranslationX < 0) && ContentView.TranslationX <= centerX;
                default:
                    return false;
            }
        }

        private void SetAppearanceForSwipe(SwipingDirection swipingDirection)
        {
            if (swipingDirection == SwipingDirection.Left && ContentView.TranslationX < 0)
            {
                this.Background = new ColorDrawable(_rightActionColor);

                if (LeftActionButton != null)
                {
                    LeftActionButton.Visibility = ViewStates.Invisible;
                }
                if (RightActionButton != null)
                {
                    RightActionButton.Visibility = ViewStates.Visible;
                }
            }
            else if (swipingDirection == SwipingDirection.Right && ContentView.TranslationX > 0)
            {
                this.Background = new ColorDrawable(_leftActionColor);

                if (RightActionButton != null)
                {
                    RightActionButton.Visibility = ViewStates.Invisible;
                }
                if (LeftActionButton != null)
                {
                    LeftActionButton.Visibility = ViewStates.Visible;
                }
            }
            else if (swipingDirection == SwipingDirection.None)
            {
                this.Background = new ColorDrawable(Color.Transparent);

                if (RightActionButton != null)
                {
                    RightActionButton.Visibility = ViewStates.Visible;
                }
                if (LeftActionButton != null)
                {
                    LeftActionButton.Visibility = ViewStates.Visible;
                }
            }
        }

        private SwipingDirection GetSwipingDirection(float dX)
        {
            if (dX == 0)
            {
                return SwipingDirection.None;
            }

            return dX > 0 ? SwipingDirection.Right : SwipingDirection.Left;
        }
        #endregion Swiping methods
    }
}