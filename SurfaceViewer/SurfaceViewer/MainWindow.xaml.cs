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
using SurfaceViewer.Parsing;

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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openGL_Resized(object sender, SharpGL.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0f, (float)gl.RenderContextProvider.Width /
                (float)gl.RenderContextProvider.Height,
                0.1f, 100.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void openGL_OpenGLDraw(object sender, SharpGL.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -6.0f);
            gl.Rotate(rotate, 0.0f, 1.0f, 0.0f);
            mesh.draw(gl);
            gl.Flush();
            rotate += 3;
        }

        private void openGL_OpenGLInitialized(object sender, SharpGL.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            List<CommonFunction> coordinates = new List<CommonFunction>();
            coordinates.Add(MathParserObjective.ParseExpressionObject("u", new string[] { "u", "v" }));
            coordinates.Add(MathParserObjective.ParseExpressionObject("v", new string[] { "u", "v" }));
            coordinates.Add(MathParserObjective.ParseExpressionObject("sqrt(4-u^2-v^2)", new string[] { "u", "v" }));
            VectorFunction r = new VectorFunction(coordinates);
            surface = new Surface(r);
            mesh = new Mesh(surface, 0.1, -1, 1, -1, 1, true, false);
            mesh.computeMesh();
            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }
    }
}
