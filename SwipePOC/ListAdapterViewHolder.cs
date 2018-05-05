using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SwipePOC
{
    public class ListAdapterViewHolder : SwipeableViewWithActions
    {
        private TextView _textView;
        public string Text
        {
            get => _textView?.Text;
            set
            {
                _textView.Text = value;
            }
        }

        public int ItemPosition { get; private set; }

        private Button _favoriteButton;
        private Button _forgetButton;
        public event EventHandler<int> OnSwipedLeft;
        public event EventHandler<int> OnSwipedRight;

        public ListAdapterViewHolder(Context context, int itemPosition)
            : base(context)
        {
            ItemPosition = itemPosition;
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            SetMinimumHeight(200);
            SwipeToActionEnabled = true;

            _favoriteButton = new Button(context);
            _favoriteButton.Background = new ColorDrawable(Color.Blue);
            _favoriteButton.Text = "Favorite";

            _forgetButton = new Button(context);
            _forgetButton.Background = new ColorDrawable(Color.Red);
            _forgetButton.Text = "Forget";

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
            AddLeftActionButton(_forgetButton);
            AddRightActionButton(_favoriteButton);
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