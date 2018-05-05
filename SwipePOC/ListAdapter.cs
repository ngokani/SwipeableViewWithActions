using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SwipePOC
{
    public class ListAdapter : BaseAdapter
    {

        private Context _context;
        private List<string> _items = new List<string>();

        public ListAdapter(Context context)
        {
            this._context = context;
            _items.Add("Hello world one two three four 1");
            _items.Add("Hello world one two three four 2");
            _items.Add("Hello world one two three four 3");
            _items.Add("Hello world one two three four 4");
            _items.Add("Hello world one two three four 5");
            _items.Add("Hello world one two three four 6");
            _items.Add("Hello world one two three four 7");
            _items.Add("Hello world one two three four 8");
        }


        public override Java.Lang.Object GetItem(int position)
        {
            if (position < 0 || position > _items.Count)
                return null;

            return _items[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ListAdapterViewHolder holder = convertView as ListAdapterViewHolder;

            if (holder == null)
            {
                holder = new ListAdapterViewHolder(_context, position);
                holder.OnSwipedLeft += (sender, itemPosition) =>
                {
                    Toast.MakeText(_context, "Swiping left...Adapter method!", ToastLength.Short).Show();
                };

                holder.OnSwipedRight += (sender, itemPosition) =>
                {
                    _items.RemoveAt(itemPosition);
                    this.NotifyDataSetChanged();
                };
            }

            holder.PrepareForReuse();
            holder.Text = GetItem(position).ToString();
            return holder;
        }

        //Fill in cound here, currently 0
        public override int Count => _items.Count;

    }
}