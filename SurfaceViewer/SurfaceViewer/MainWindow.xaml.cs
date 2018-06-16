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
using SharpGL;
using SharpGL.WPF;
using SurfaceViewer.Functions;
using Symbolic.Functions;
using Symbolic;

namespace SurfaceViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Surface surface;
        Mesh mesh;
        float rotate;
        private DateTime begin;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openGL_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0f, (float)gl.RenderContextProvider.Width /
                (float)gl.RenderContextProvider.Height,
                0.1f, 100.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void openGL_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -3.0f);
            rotate = (float)((DateTime.Now - begin).TotalSeconds) * 10;
            gl.Rotate(rotate, 0.0f, 1.0f, 0.0f);
            if (mesh != null)
                mesh.draw(gl);
            gl.Flush();
        }

        private void openGL_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.ClearColor(0.5f, 0.5f, 0.5f, 1);

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            gl.PolygonMode(OpenGL.GL_BACK, OpenGL.GL_LINE);
            gl.Enable(OpenGL.GL_CULL_FACE);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, new float[] { 1, 1, 1, 1 });
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, new float[] { 1, 1, 1, 1 });
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, new float[] { 0, 0, 0, 1 });

            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new float[] { .8f, 0, 0, 1 });
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new float[] { .3f, 0, 0, 1 });
            gl.Material(OpenGL.GL_BACK, OpenGL.GL_DIFFUSE, new float[] { 0, 0, 1, 1 });
            gl.Material(OpenGL.GL_BACK, OpenGL.GL_SPECULAR, new float[] { .2f, 0, 0, 1 });
            enterData();
            begin = DateTime.Now;
        }

        private void solid_Checked(object sender, RoutedEventArgs e)
        {
            if (mesh != null)
            {
                mesh.setSolid(true);
            }
        }

        private void net_Checked(object sender, RoutedEventArgs e)
        {
            if (mesh != null)
            {
                mesh.setSolid(false);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            enterData();
        }

        public void enterData()
        {
            surface = new Surface(x.Text, y.Text, z.Text, "s", "t", (bool)(analitics.IsChecked));
            double left = ((CommonFunction)(this.left.Text + ":null")).Invoke(null);
            double right = ((CommonFunction)(this.right.Text + ":null")).Invoke(null);
            double bottom = ((CommonFunction)(this.bottom.Text + ":null")).Invoke(null);
            double top = ((CommonFunction)(this.top.Text + ":null")).Invoke(null);
            double seed = ((CommonFunction)(this.seed.Text + ":null")).Invoke(null);
            mesh = new Mesh(surface, seed, left, right, bottom, top, true, (bool)this.net.IsChecked);
            mesh.computeMesh();
        }

        private void sphere_Click(object sender, RoutedEventArgs e)
        {
            x.Text = "cos(s)*cos(t)";
            y.Text = "cos(s)*sin(t)";
            z.Text = "sin(s)";
            left.Text = "dtr(-90)";
            right.Text = "dtr(90)";
            bottom.Text = "dtr(-180)";
            top.Text = "dtr(180)";
        }

        private void helicoid_Click(object sender, RoutedEventArgs e)
        {
            x.Text = "s*cos(t)";
            y.Text = "t*0.5";
            z.Text = "s*sin(t)";
            left.Text = "-0.7";
            right.Text = "0.7";
            bottom.Text = "-3";
            top.Text = "3";
        }

        private void parabol_Click(object sender, RoutedEventArgs e)
        {
            x.Text = "s";
            y.Text = "s^2+t^2-1";
            z.Text = "t";
            left.Text = "-0.5";
            right.Text = "0.5";
            bottom.Text = "-1";
            top.Text = "1";
        }

        private void monkey_Click(object sender, RoutedEventArgs e)
        {
            x.Text = "s";
            z.Text = "t";
            y.Text = "s^3-3*s*t^2";
            left.Text = "-0.7";
            right.Text = "0.7";
            bottom.Text = "-0.7";
            top.Text = "0.7";
        }

        private void hyperbol_Click(object sender, RoutedEventArgs e)
        {
            x.Text = "s";
            y.Text = "0.7*s^2-0.7*t^2";
            z.Text = "t";
            left.Text = "-0.5";
            right.Text = "0.5";
            bottom.Text = "-1";
            top.Text = "1";
        }

        private void thorus_Click(object sender, RoutedEventArgs e)
        {
            x.Text = "(0.7+0.2*cos(s))*cos(t)";
            y.Text = "(0.7+0.2*cos(s))*sin(t)";
            z.Text = "0.2*sin(s)";
            left.Text = "dtr(-180)";
            right.Text = "dtr(180)";
            bottom.Text = "dtr(-180)";
            top.Text = "dtr(180)";
        }

        private void thorus_broken_Click(object sender, RoutedEventArgs e)
        {
            x.Text = "(0.5+0.5*cos(s))*cos(t)";
            y.Text = "(0.5+0.5*cos(s))*sin(t)";
            z.Text = "0.5*sin(s)";
            left.Text = "dtr(-150)";
            right.Text = "dtr(150)";
            bottom.Text = "dtr(-90)";
            top.Text = "dtr(90)";
        }
    }
}
