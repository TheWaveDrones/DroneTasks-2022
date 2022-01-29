namespace TheWaveApp.Adapters
{
    using Android.Runtime;
    using Android.Views;
    using AndroidX.RecyclerView.Widget;
    using System;
    using System.Collections.Generic;

    public class BaseViewHolder : RecyclerView.ViewHolder
	{
		private readonly Dictionary<string, View> viewCollection = new Dictionary<string, View>();

		public BaseViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
		}

		public BaseViewHolder(View itemView)
			: base(itemView)
		{
		}

		public void AddView(View view, string id)
		{
			this.viewCollection.Add(id, view);
		}

		public T GetView<T>(string id) where T : View
		{
			return (T)this.viewCollection[id];
		}
	}
}