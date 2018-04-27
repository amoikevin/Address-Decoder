#region

using System.Net;
using System.Reactive.Linq;
using System.Text;
using MetroFramework.Controls;
using MetroFramework.Forms;
using static System.StringComparison;

#endregion

namespace System
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();

            Observable.FromEventPattern(h => tbSrc.GotFocus += h, h => tbSrc.GotFocus -= h)
                .Throttle(TimeSpan.FromMilliseconds(300)).ObserveOn(this).Subscribe(obj => tbSrc.SelectAll());
            Observable.FromEventPattern(h => tbTrue.GotFocus += h, h => tbTrue.GotFocus -= h)
                .Throttle(TimeSpan.FromMilliseconds(800)).ObserveOn(this).Subscribe(obj => SelectAll(tbTrue));
            Observable.FromEventPattern(h => tbThunber.GotFocus += h, h => tbThunber.GotFocus -= h)
                .Throttle(TimeSpan.FromMilliseconds(800)).ObserveOn(this).Subscribe(obj => SelectAll(tbThunber));
            Observable.FromEventPattern(h => tbQQ.GotFocus += h, h => tbQQ.GotFocus -= h)
                .Throttle(TimeSpan.FromMilliseconds(800)).ObserveOn(this).Subscribe(obj => SelectAll(tbQQ));
            Observable.FromEventPattern(h => tbFlash.GotFocus += h, h => tbFlash.GotFocus -= h)
                .Throttle(TimeSpan.FromMilliseconds(800)).ObserveOn(this).Subscribe(obj => SelectAll(tbFlash));
            Observable.FromEventPattern(h => tbRayFile.GotFocus += h, h => tbRayFile.GotFocus -= h)
                .Throttle(TimeSpan.FromMilliseconds(800)).ObserveOn(this).Subscribe(obj => SelectAll(tbRayFile));

            GotFocus += (sender, e) => tbSrc.Focus();

            Observable.FromEventPattern(h => tbSrc.TextChanged += h, h => tbSrc.TextChanged -= h)
                .Throttle(TimeSpan.FromMilliseconds(800)).Select(obj => tbSrc.Text).DistinctUntilChanged().ObserveOn(this).Subscribe(OnAddressChanged);
        }

        private void Clear(string value = default(string))
        {
            tbTrue.Text = value;
            tbThunber.Text = value;
            tbQQ.Text = value;
            tbFlash.Text = value;
            tbRayFile.Text = value;
        }

        private void Decode4Flashget(string address)
        {
            tbFlash.Text = address;
            try
            {
                address = address.Substring(11);
                var base64 = Convert.FromBase64String(address);
                address = Encoding.Default.GetString(base64);
                var index = address.IndexOf('&');
                if (index > -1)
                {
                    address = address.Substring(0, index);
                }

                if (address.StartsWith("[FLASHGET]") && address.EndsWith("[FLASHGET]"))
                {
                    address = address.Substring(10, address.Length - 20);
                    tbTrue.Text = address;
                    tbThunber.Text = Encode4Thunder(address);
                    tbQQ.Text = Encode4QQ(address);
                    tbRayFile.Text = Encode4RayFile(address);
                }

                return;
            }
            catch
            {
            }

            Clear("无效的快车地址");
        }

        private void Decode4QQ(string address)
        {
            tbQQ.Text = address;
            try
            {
                address = address.Substring(7);
                var base64 = Convert.FromBase64String(address);
                address = Encoding.Default.GetString(base64);
                tbTrue.Text = address;
                tbThunber.Text = Encode4Thunder(address);
                tbFlash.Text = Encode4Flashget(address);
                tbRayFile.Text = Encode4RayFile(address);

                return;
            }
            catch
            {
            }

            Clear("无效的旋风地址");
        }

        private void Decode4RayFile(string address)
        {
            tbRayFile.Text = address;
            try
            {
                address = address.Substring(9);
                var base64 = Convert.FromBase64String(address);
                address = Encoding.Default.GetString(base64);
                var index = address.IndexOf('|');
                if (index > -1)
                {
                    address = address.Substring(0, index);
                }

                address = WebUtility.UrlDecode(address);
                index = address.IndexOf("://", InvariantCultureIgnoreCase);
                if (index == -1)
                    address = $"http://{address}";

                tbTrue.Text = address;
                tbThunber.Text = Encode4Thunder(address);
                tbQQ.Text = Encode4QQ(address);
                tbFlash.Text = Encode4Flashget(address);


                return;
            }
            catch
            {
            }

            Clear("无效的RayFile 地址");
        }

        private void Decode4Thunder(string address)
        {
            tbThunber.Text = address;
            try
            {
                address = address.Substring(10);
                var base64 = Convert.FromBase64String(address);
                address = Encoding.Default.GetString(base64);
                if (address.StartsWith("AA") && address.EndsWith("ZZ"))
                {
                    address = address.Substring(2, address.Length - 4);
                    tbTrue.Text = address;
                    tbQQ.Text = Encode4QQ(address);
                    tbFlash.Text = Encode4Flashget(address);
                    tbRayFile.Text = Encode4RayFile(address);
                }

                return;
            }
            catch
            {
            }

            Clear("无效的迅雷地址");
        }

        private string Encode4Flashget(string address)
        {
            address = $"[FLASHGET]{address}[FLASHGET]";
            address = $"flashget://{Convert.ToBase64String(Encoding.Default.GetBytes(address))}";
            return address;
        }

        private string Encode4QQ(string address)
        {
            address = $"qqdl://{Convert.ToBase64String(Encoding.Default.GetBytes(address))}";
            return address;
        }

        private string Encode4RayFile(string address)
        {
            address = $"fs2you://{Convert.ToBase64String(Encoding.Default.GetBytes(address))}";
            return address;
        }

        private string Encode4Thunder(string address)
        {
            address = $"AA{address}ZZ";
            address = $"thunder://{Convert.ToBase64String(Encoding.Default.GetBytes(address))}";
            return address;
        }

        private void OnAddressChanged(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                Clear();
                return;
            }

            address = WebUtility.UrlDecode(address);
            if (address.StartsWith("https://", InvariantCultureIgnoreCase) 
                || address.StartsWith("http://", InvariantCultureIgnoreCase)
                || address.StartsWith("ftp://", InvariantCultureIgnoreCase)
                || address.StartsWith("magnet:?", InvariantCultureIgnoreCase)
                || address.StartsWith("ed2k://", InvariantCultureIgnoreCase))
            {
                tbTrue.Text = address;
                tbThunber.Text = Encode4Thunder(address);
                tbQQ.Text = Encode4QQ(address);
                tbFlash.Text = Encode4Flashget(address);
                tbRayFile.Text = Encode4RayFile(address);
                return;
            }

            if (address.StartsWith("thunder://", InvariantCultureIgnoreCase))
            {
                Decode4Thunder(address);
                return;
            }

            if (address.StartsWith("qqdl://", InvariantCultureIgnoreCase))
            {
                Decode4QQ(address);
                return;
            }

            if (address.StartsWith("flashget://", InvariantCultureIgnoreCase))
            {
                Decode4Flashget(address);
                return;
            }

            if (address.StartsWith("fs2you://", InvariantCultureIgnoreCase))
            {
                Decode4RayFile(address);
                return;
            }

            Clear("无效的地址");
        }

        private void SelectAll(MetroTextBox tb)
        {
            if (string.IsNullOrEmpty(tb.Text)) return;
            tb.SelectAll();
//            try
//            {
//                Clipboard.SetText(tb.Text, TextDataFormat.UnicodeText);
//            }
//            catch
//            {
//            }
        }
    }
}