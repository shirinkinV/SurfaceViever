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

namespace SurfaceViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void openGL_Resized(object sender, SharpGL.OpenGLEventArgs args)
        {

        }

        private void openGL_OpenGLDraw(object sender, SharpGL.OpenGLEventArgs args)
        {

        }

        private void openGL_OpenGLInitialized(object sender, SharpGL.OpenGLEventArgs args)
        {

        }
    }
}
