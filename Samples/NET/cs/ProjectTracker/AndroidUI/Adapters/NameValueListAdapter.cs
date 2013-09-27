using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Csla;

namespace ProjectTracker.AndroidUI.Adapters
{
    public class NameValueListAdapter<N, V> : BaseAdapter<string>
    {
        private readonly NameValueListBase<N, V> items;
        private readonly Activity context;

        private NameValueListAdapter()
        {
            
        }
        
        public NameValueListAdapter(Activity context, NameValueListBase<N, V> items)
        {
            if (!this.IsAcceptableType())
                throw new NotImplementedException();
            this.context = context;
            this.items = items;
        }

        private bool IsAcceptableType()
        {
            var myType = typeof(N);
            if (myType == typeof(int) || myType == typeof(uint) || myType == typeof(long) || myType == typeof(ulong) || myType == typeof(short) || myType == typeof(ushort))
            {
                return true;
            }
            return false;
        }

        public override long GetItemId(int position)
        {
            return long.Parse(this.items[position].Key.ToString());
        }

        public override string this[int position]
        {
            get { return items[position].Value.ToString(); }
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].Value.ToString();
            return view;
        }
    }
}