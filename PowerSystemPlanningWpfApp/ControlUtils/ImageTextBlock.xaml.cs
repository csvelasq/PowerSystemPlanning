using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerSystemPlanningWpfApp.ControlUtils
{
    /// <summary>
    /// Interaction logic for ImageTextBlock.xaml
    /// </summary>
    public partial class ImageTextBlock : UserControl
    {
        /// <summary>
        /// The message shown in the textblock
        /// </summary>
        public string MessageForTextBlock
        {
            set { tbMsg.Text = value; }
        }
        /// <summary>
        /// The name of the image to show left of the textblock (must be an image within the project's resources).
        /// </summary>
        public ImageSource ImageSource
        {
            get { return imgLeft.Source; }
            set { imgLeft.Source = value; }
        }

        public ImageTextBlock()
        {
            InitializeComponent();
        }
    }
}
