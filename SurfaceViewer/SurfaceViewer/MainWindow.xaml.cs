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
            gl.Translate(0.0f, 0.0f, -3.0f);
            gl.Rotate(rotate, 0.0f, 1.0f, 0.0f);
            mesh.draw(gl);
            gl.Flush();
            rotate += 0.3f;
        }

        private void openGL_OpenGLInitialized(object sender, SharpGL.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.ClearColor(0.5f, 0.5f, 0.5f, 1);
            List<CommonFunction> coordinates = new List<CommonFunction>();
            coordinates.Add(MathParserObjective.ParseExpressionObject("cos(phi)*cos(psi)", new string[] { "phi", "psi" }));
            coordinates.Add(MathParserObjective.ParseExpressionObject("cos(phi)*sin(psi)", new string[] { "phi", "psi" }));
            coordinates.Add(MathParserObjective.ParseExpressionObject("sin(phi)", new string[] { "phi", "psi" }));
            VectorFunction r = new VectorFunction(coordinates);
            surface = new Surface(r);
            mesh = new Mesh(surface, 0.05, -Math.PI / 4, Math.PI / 2, -Math.PI / 2, Math.PI / 4, true, true);
            mesh.computeMesh();
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            gl.PolygonMode(OpenGL.GL_BACK, OpenGL.GL_LINE);
            gl.Enable(OpenGL.GL_CULL_FACE);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, new float[] { 1, 1, 1, 1 });
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, new float[] { 0, 0, 0, 1 });

            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new float[] { 1, 0, 0, 1 });
            gl.Material(OpenGL.GL_BACK, OpenGL.GL_DIFFUSE, new float[] { 0, 0, 1, 1 });
        }
    }
}
