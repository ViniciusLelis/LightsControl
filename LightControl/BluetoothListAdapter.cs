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
using Android.Bluetooth;
using Android.Graphics;

namespace LightControl
{
    class BluetoothListAdapter : BaseAdapter<BluetoothDevice>
    {
        Activity context;
        List<BluetoothDevice> list;

        public BluetoothListAdapter(Activity _context, List<BluetoothDevice> _list) : base()
        {
            this.context = _context;
            this.list = new List<BluetoothDevice>(_list);
        }

        public override BluetoothDevice this[int position]
        {
            get { return list[position]; }
        }

        public override int Count
        {
            get { return list.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.custom_listview, null);
            }
            Typeface font = Typeface.CreateFromAsset(Application.Context.Assets, "NexaLight.otf");
            TextView textName = (TextView)convertView.FindViewById(Resource.Id.textName);
            textName.Typeface = font;
            TextView textAddress = (TextView)convertView.FindViewById(Resource.Id.textAddress);
            textAddress.Typeface = font;

            TextView nomeLabel = (TextView)convertView.FindViewById(Resource.Id.nomeLabel);
            nomeLabel.Typeface = font;
            TextView enderecoLabel = (TextView)convertView.FindViewById(Resource.Id.enderecoLabel);
            enderecoLabel.Typeface = font;

            BluetoothDevice dispositivo = (BluetoothDevice)GetItem(position);

            textName.Text = dispositivo.Name;
            textAddress.Text = dispositivo.Address;
            return convertView;
        }
    }
}