using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Csla.Axml.Binding;
using Csla.Serialization.Mobile;

namespace Csla.Axml
{
    public class ActivityBase<T, Z> : Activity where T : ViewModel<Z>
    {
        protected T viewModel = null;
        protected BindingManager Bindings = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Bindings = new BindingManager(this);
        }

        protected byte[] SerilizeModelForParameter()
        {
            return this.SerilizeModelForParameter(this.viewModel.Model);
        }

        protected byte[] SerilizeModelForParameter(object model)
        {
            return MobileFormatter.Serialize(model);
        }

        protected object DeserializeFromParameter(byte[] parameter)
        {
            return MobileFormatter.Deserialize(parameter);
        }
    }
}