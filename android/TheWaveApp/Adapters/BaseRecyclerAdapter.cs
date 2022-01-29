    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using AndroidX.RecyclerView.Widget;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;

    namespace TheWaveApp.Adapters
	{
        public abstract class BaseRecyclerAdapter<T> : RecyclerView.Adapter
		{
			private readonly List<T> items;
			private readonly int? layoutId;
			private readonly RecyclerView recyclerView;

			public event EventHandler<T> ItemSelected;
			protected virtual void OnItemSelected(T e) => this.ItemSelected?.Invoke(this, e);

			public event EventHandler<T> LongItemSelected;
			protected virtual void OnLongItemSelected(T e) => this.LongItemSelected?.Invoke(this, e);

			protected BaseRecyclerAdapter(RecyclerView recyclerView, List<T> items, int? layoutId = null)
			{
				this.items = items;
				//items.CollectionChanged += this.OnCollectionChanged;
				this.layoutId = layoutId;
				this.recyclerView = recyclerView;
				if (recyclerView != null)
				{
					this.recyclerView.SetAdapter(this);
					this.recyclerView.AddOnChildAttachStateChangeListener(new AttachStateChangeListener(this));
				}
			}

			public override int ItemCount => (this.items != null) ? this.items.Count : 0;

			public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
			{
				BaseViewHolder baseViewHolder = (BaseViewHolder)holder;
				this.OnUpdateView(baseViewHolder, this.items[position]);
			}

			public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			{
				int viewId = this.OnGetViewId(viewType);
				View layout = LayoutInflater.From(parent.Context).Inflate(viewId, parent, false);


				BaseViewHolder genericViewHolder = new BaseViewHolder(layout);

				this.OnLookupViewItems(layout, genericViewHolder);
				return genericViewHolder;
			}

			public override int GetItemViewType(int position)
			{
				return this.GetViewIdForType(this.items[position]);
			}

			protected virtual int OnGetViewId(int viewType)
			{
				if (this.layoutId == null)
				{
					throw new InvalidOperationException("No layoutId provided on adapter constructor, you need to override OnGetViewId and provide a valid resource is for this viewType");
				}

				return this.layoutId.Value;
			}

			protected abstract void OnLookupViewItems(View layout, BaseViewHolder viewHolder);

			protected abstract void OnUpdateView(BaseViewHolder viewHolder, T item);

			protected virtual int GetViewIdForType(T item)
			{
				return 0;
			}

			private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						this.NotifyItemInserted(e.NewStartingIndex);
						break;
					case NotifyCollectionChangedAction.Remove:
						this.NotifyItemRemoved(e.OldStartingIndex);
						break;
					case NotifyCollectionChangedAction.Replace:
						this.NotifyItemChanged(e.OldStartingIndex);
						this.NotifyItemChanged(e.NewStartingIndex);
						break;
					case NotifyCollectionChangedAction.Move:
						this.NotifyItemRemoved(e.OldStartingIndex);
						this.NotifyItemRemoved(e.NewStartingIndex);
						break;
					case NotifyCollectionChangedAction.Reset:
						this.NotifyDataSetChanged();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			public void RemoveItem(int position)
			{
				items.RemoveAt(position);
				// ?? NotifyDataSetChanged();
				// ?? NotifyItemChanged(position);
			}

			/// <summary>
			/// Subscribes to view click so that we can have ItemSelected w/o any custom code
			/// </summary>
			/// <seealso cref="Android.Support.V7.Widget.RecyclerView.Adapter" />
			internal class AttachStateChangeListener : Java.Lang.Object, RecyclerView.IOnChildAttachStateChangeListener
			{
				private readonly BaseRecyclerAdapter<T> parentAdapter;

				public AttachStateChangeListener(BaseRecyclerAdapter<T> parentAdapter)
					: base()
				{
					this.parentAdapter = parentAdapter;
				}

				public void OnChildViewAttachedToWindow(View view)
				{
					view.Click += this.View_Click;
					view.LongClick += this.View_LongClick;
				}

				public void OnChildViewDetachedFromWindow(View view)
				{
					view.Click -= this.View_Click;
					view.LongClick -= this.View_LongClick;
				}

				private void View_Click(object sender, EventArgs e)
				{
					BaseViewHolder holder = (BaseViewHolder)this.parentAdapter.recyclerView.GetChildViewHolder(((View)sender));
					int clickedPosition = holder.AdapterPosition;
					this.parentAdapter.OnItemSelected(this.parentAdapter.items[clickedPosition]);
				}

				private void View_LongClick(object sender, EventArgs e)
				{
					BaseViewHolder holder = (BaseViewHolder)this.parentAdapter.recyclerView.GetChildViewHolder(((View)sender));
					int clickedPosition = holder.AdapterPosition;
					this.parentAdapter.OnLongItemSelected(this.parentAdapter.items[clickedPosition]);
				}
			}
		}
	}
