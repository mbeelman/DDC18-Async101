using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public class Foo : INotifyCompletion
    {
        private Button _button;
        public Foo(Button button) { _button = button; }
        private Action _continuation;
        public bool IsCompleted  => false;

        // called when await is starting
        public void OnCompleted(Action continuation)
        {
            _button.Click += OnButtonClick;
            // cache the continuation
            _continuation = continuation;
            _button.Enabled = true;
        }
        // called when the button is clicked
        private void OnButtonClick(object sender, EventArgs e)
        {
            _button.Click -= OnButtonClick;
            _button.Text = "Done";
            _button.Enabled = false;
            // start the excution of the continuation after the await call
            _continuation();
        }

        public void GetResult() { }

    }


    public static class Ext
    {
        /// Extension method that returns the awaiter when the button
        /// is called with await
        public static Foo GetAwaiter(this Button _btn)
        {
            // return a new instance of class foo which implements
            // the INotifyCompletion interface
            return new Foo(_btn);
        }

    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Setup();
        }

        private async void Setup()
        {
            button2.Enabled = false;
            await button1;
            await button2;
            MessageBox.Show("All done");

        }
    }
}
