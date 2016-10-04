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
    /// Interaction logic for ImageTextButton.xaml
    /// </summary>
    public partial class ImageTextButton : Button
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
          DependencyProperty.Register(nameof(Text), typeof(string), typeof(ImageTextButton), new UIPropertyMetadata(""));

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
           DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(ImageTextButton), new UIPropertyMetadata(null));

        public ImageTextButton()
        {
            InitializeComponent();
        }
    }
}
