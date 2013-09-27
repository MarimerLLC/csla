using Android.App;
using Android.Views;
using Android.Widget;
using ProjectTracker.Library;

namespace ProjectTracker.AndroidUI.Adapters
{
    public class ProjectListAdapter : BaseAdapter<string>
    {
        Library.ProjectList items;
        Activity context;
        public ProjectListAdapter(Activity context, Library.ProjectList items)
            : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return this.items[position].Id;
        }

        public override string this[int position]
        {
            get { return items[position].Name; }
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
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].Name;
            return view;
        }
    }
}