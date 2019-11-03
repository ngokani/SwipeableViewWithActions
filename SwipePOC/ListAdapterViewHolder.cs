using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace SwipePOC
{
    public class ListAdapterViewHolder : SwipeableViewWithActions
    {
        private readonly TextView _textView;
        public string Text
        {
            get => _textView?.Text;
            set => _textView.Text = value;
        }

        public int ItemPosition { get; }

        public event EventHandler<int> OnSwipedLeft;
        public event EventHandler<int> OnSwipedRight;

        public ListAdapterViewHolder(Context context, int itemPosition)
            : base(context)
        {
            ItemPosition = itemPosition;
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            SetMinimumHeight(200);
            SwipeToActionEnabled = true;

            var favoriteButton = new Button(context);
            favoriteButton.Background = new ColorDrawable(Color.Blue);
            favoriteButton.Text = "Favorite";

            var forgetButton = new Button(context);
            forgetButton.Background = new ColorDrawable(Color.Red);
            forgetButton.Text = "Forget";

            FrameLayout frameLayout = new FrameLayout(context);
            frameLayout.Background = new ColorDrawable(Color.White);
            _textView = new TextView(context);
            _textView.TextSize = 25;
            _textView.Gravity = GravityFlags.Center;
            _textView.SetLines(1);
            _textView.SetTextColor(Color.Black);
            _textView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
            frameLayout.AddView(_textView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

            AddContentView(frameLayout);
            AddLeftActionButton(forgetButton);
            AddRightActionButton(favoriteButton);
        }

        public void PrepareForReuse()
        {
            base.ResetView();
            Text = string.Empty;
        }

        #region Abstract methods
        public override void OnFullSwipeLeft()
        {
            OnSwipedLeft?.Invoke(this, ItemPosition);
            Toast.MakeText(Context, "Full swipe left", ToastLength.Short).Show();
        }

        public override void OnFullSwipeRight()
        {
            OnSwipedRight?.Invoke(this, ItemPosition);
            Toast.MakeText(Context, "Full swipe right", ToastLength.Short).Show();
        }
        #endregion abstract methods
    }
}