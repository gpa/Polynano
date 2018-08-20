using System.Windows.Forms;

namespace Polynano.Startup.Views
{
    public partial class LoadingForm : Form
    {
        public int StatusBar
        {
            get => statusBar.Value;
            set => statusBar.Value = value;
        }

        public string StatusText
        {
            get => Text;
            set => Text = value;
        }

        public ProgressBarStyle Style
        {
            set
            {
                statusBar.Style = value;
                statusBar.MarqueeAnimationSpeed = 15;
            }
        }

        public string StatusBarText
        {
            get => statusBarText.Text;
            set
            {
                statusBarText.Text = value;
                statusBarText.Visible = true;
                statusBar.Visible = false;
            }
        }

        public LoadingForm()
        {
            InitializeComponent();
        }
    }
}
