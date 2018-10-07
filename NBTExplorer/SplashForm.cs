using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NBTExplorer
{
    public partial class SplashForm : Form
    {
        //Splashフォーム
        private static SplashForm _form = null;
        //メインフォーム
        private static Form _mainForm = null;
        //Splashを表示するスレッド
        private static System.Threading.Thread _thread = null;
        //lock用のオブジェクト
        private static readonly object syncObject = new object();
        //Splashが表示されるまで待機するための待機ハンドル
        private static System.Threading.ManualResetEvent splashShownEvent = null;

        /// <summary>
        /// Splashフォーム
        /// </summary>
        public static SplashForm Form
        {
            get { return _form; }
        }

        /// <summary>
        /// Splashフォームを表示する
        /// </summary>
        /// <param name="mainForm">メインフォーム</param>
        public static void ShowSplash(Form mainForm)
        {
            lock (syncObject)
            {
                if (_form != null || _thread != null)
                {
                    return;
                }

                _mainForm = mainForm;
                //メインフォームのActivatedイベントでSplashフォームを消す
                if (_mainForm != null)
                {
                    _mainForm.Activated += new EventHandler(_mainForm_Activated);
                }

                //待機ハンドルの作成
                splashShownEvent = new System.Threading.ManualResetEvent(false);

                //スレッドの作成
                _thread = new System.Threading.Thread(
                    new System.Threading.ThreadStart(StartThread));
                _thread.Name = "SplashForm";
                _thread.IsBackground = true;
                _thread.SetApartmentState(System.Threading.ApartmentState.STA);
                //.NET Framework 1.1以前では、以下のようにする
                //_thread.ApartmentState = System.Threading.ApartmentState.STA;
                //スレッドの開始
                _thread.Start();
            }
        }

        /// <summary>
        /// Splashフォームを表示する
        /// </summary>
        public static void ShowSplash()
        {
            ShowSplash(null);
        }

        /// <summary>
        /// Splashフォームを消す
        /// </summary>
        public static void CloseSplash()
        {
            lock (syncObject)
            {
                if (_thread == null)
                {
                    return;
                }

                if (_mainForm != null)
                {
                    _mainForm.Activated -= new EventHandler(_mainForm_Activated);
                }

                //Splashが表示されるまで待機する
                if (splashShownEvent != null)
                {
                    splashShownEvent.WaitOne();
                    splashShownEvent.Close();
                    splashShownEvent = null;
                }

                //Splashフォームを閉じる
                //Invokeが必要か調べる
                if (_form != null)
                {
                    if (_form.InvokeRequired)
                    {
                        _form.Invoke(new MethodInvoker(CloseSplashForm));
                    }
                    else
                    {
                        CloseSplashForm();
                    }
                }

                //メインフォームをアクティブにする
                if (_mainForm != null)
                {
                    if (_mainForm.InvokeRequired)
                    {
                        _mainForm.Invoke(new MethodInvoker(ActivateMainForm));
                    }
                    else
                    {
                        ActivateMainForm();
                    }
                }

                _form = null;
                _thread = null;
                _mainForm = null;
            }
        }

        //スレッドで開始するメソッド
        private static void StartThread()
        {
            //Splashフォームを作成
            _form = new SplashForm();
            //Splashフォームをクリックして閉じられるようにする
            _form.Click += new EventHandler(_form_Click);
            //Splashが表示されるまでCloseSplashメソッドをブロックする
            _form.Activated += new EventHandler(_form_Activated);
            //Splashフォームを表示する
            Application.Run(_form);
        }

        //SplashのCloseメソッドを呼び出す
        private static void CloseSplashForm()
        {
            if (!_form.IsDisposed)
            {
                _form.Close();
            }
        }

        //メインフォームのActivateメソッドを呼び出す
        private static void ActivateMainForm()
        {
            if (!_mainForm.IsDisposed)
            {
                _mainForm.Activate();
            }
        }

        //Splashフォームがクリックされた時
        private static void _form_Click(object sender, EventArgs e)
        {
            //Splashフォームを閉じる
            CloseSplash();
        }

        //メインフォームがアクティブになった時
        private static void _mainForm_Activated(object sender, EventArgs e)
        {
            //Splashフォームを閉じる
            CloseSplash();
        }

        //Splashフォームが表示された時
        private static void _form_Activated(object sender, EventArgs e)
        {
            _form.Activated -= new EventHandler(_form_Activated);
            //CloseSplashメソッドの待機を解除
            if (splashShownEvent != null)
            {
                splashShownEvent.Set();
            }
        }
        public SplashForm()
        {
            InitializeComponent();
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

}
