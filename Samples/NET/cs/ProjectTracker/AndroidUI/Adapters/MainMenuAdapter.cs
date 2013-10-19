using Android.App;
using Android.Content.Res;
using Android.Views;
using Android.Widget;

namespace ProjectTracker.AndroidUI.Adapters
{
    public class MainMenuAdapter : BaseAdapter<string>
    {
        Activity context;
        public MainMenuAdapter(Activity context)
        {
            this.context = context;
        }

        public override long GetItemId(int position)
        {
            long returnValue = 0;
            switch (position)
            {
                case 0:
                    returnValue = Resource.String.MenuProjects;
                    break;
                case 1:
                    returnValue = Resource.String.MenuResources;
                    break;
            }
            return returnValue;
        }

        public override string this[int position]
        {
            get
            {
                return GetValue(position);
            }
        }

        public override int Count
        {
            get { return 2; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = GetValue(position);
            return view;
        }

        private string GetValue(int position)
        {
            string returnValue = "";
            switch (position)
            {
                case 0:
                    returnValue = context.Resources.GetString(Resource.String.MenuProjects);
                    break;
                case 1:
                    returnValue = context.Resources.GetString(Resource.String.MenuResources);
                    break;
            }
            return returnValue;
        }
    }
}