using Android.App;
using Android.Widget;
using Android.OS;

namespace SwipePOC
{
    [Activity(Label = "SwipePOC", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ListView _listView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            _listView = FindViewById<ListView>(Resource.Id.ListView);
            _listView.Adapter = new ListAdapter(this);
        }


    }
}

