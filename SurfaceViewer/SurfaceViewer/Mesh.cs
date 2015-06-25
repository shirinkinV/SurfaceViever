using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurfaceViewer.Functions;
using SharpGL;

namespace SurfaceViewer
{

    class Mesh
    {

        Surface surface;

        double seed;
        double left;
        double right;
        double bottom;
        double top;
        bool doubleSurface;
        bool net;

        private int vertexCount;
        private int stripCount;
        private List<int[]> strips;
        private List<int> lengths;
        private float[] vertices;
        private float[] normals;
        private float[] doubleNormals;
        private int[] colors;
        private float[] textCoords;



        public Mesh(Surface surface, double seed, double left, double right, double bottom, double top, bool doubleSurface, bool net)
        {
            this.surface = surface;
            this.seed = seed;
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;
            this.doubleSurface = doubleSurface;
            this.net = net;
        }

        public void computeMesh()
        {
            int verticalCount = (int)((top - bottom) / seed);
            int horizontalCount = (int)((right - left) / seed);


            vertexCount=(verticalCount + 2) * (horizontalCount + 2);
            stripCount= verticalCount + 1;

            for (int i = 0; i <= verticalCount; i++)
            {
                double v = bottom + seed * i;
                for (int j = 0; j <= horizontalCount; j++)
                {
                    double u = left + seed * j;
                    putVertices(surface.getVertex(u, v));
                    putColors(surface.getColor(u, v));
                    putNormals(surface.getNormal(u, v));
                    vertexIndex++;
                }
                putVertices(surface.getVertex(right, v));
                putColors(surface.getColor(right, v));
                putNormals(surface.getNormal(right, v));
                vertexIndex++;
            }
            
            double vv = top;
            for (int j = 0; j <= horizontalCount; j++)
            {
                double u = left + seed * j;
                putVertices(surface.getVertex(u, vv));
                putColors(surface.getColor(u, vv));
                putNormals(surface.getNormal(u, vv));
                vertexIndex++;
            }
            putVertices(surface.getVertex(right, vv));
            putColors(surface.getColor(right, vv));
            putNormals(surface.getNormal(right, vv));
            vertexIndex++;

            for (int i = 0; i <= verticalCount; i++)
            {
                int[] strip = new int[(horizontalCount + 2) * 2];

                for (int j = 0; j <= horizontalCount + 1; j++)
                {
                    if (net)
                    {
                        strip[j * 2 + 1] = ((i + 1) * (horizontalCount + 2) + j);
                        strip[j * 2] = ((i) * (horizontalCount + 2) + j);
                    }
                    else
                    {
                        strip[j * 2] = ((i + 1) * (horizontalCount + 2) + j);
                        strip[j * 2 + 1] = ((i) * (horizontalCount + 2) + j);
                    }
                }

                putStrip(strip);
            }

        }

        int vertexIndex;

        void putStrip(int[] indices)
        {
            if (strips == null) strips = new List<int[]>();
            strips.Add(indices);
            if (lengths == null) lengths = new List<int>();
            lengths.Add(indices.Length);
        }

        public void putVertices(float[] vertices)
        {
            int index = vertexIndex * 3;
            if (this.vertices == null)
            {
                this.vertices = new float[vertexCount * 3];
                this.vertices[index] = vertices[0];
                this.vertices[index + 1] = vertices[1];
                this.vertices[index + 2] = vertices[2];
            }
            this.vertices[index] = vertices[0];
            this.vertices[index + 1] = vertices[1];
            this.vertices[index + 2] = vertices[2];
        }

        public void putNormals(float[] normals)
        {
            int index = vertexIndex * 3;
            if (this.normals == null)
            {
                this.normals = new float[vertexCount * 3];
            }
            this.normals[index] = normals[0];
            this.normals[index + 1] = normals[1];
            this.normals[index + 2] = normals[2];
            if (doubleSurface)
            {
                if (doubleNormals == null)
                {
                    doubleNormals = new float[vertexCount * 3];
                }
                doubleNormals[index] = -normals[0];
                doubleNormals[index + 1] = -normals[1];
                doubleNormals[index + 2] = -normals[2];
            }
        }

        public void putColors(int[] colors)
        {
            int index = vertexIndex * 3;
            if (this.colors == null)
            {
                this.colors = new int[vertexCount * 3];
                this.colors[index] = colors[0];
                this.colors[index + 1] = colors[1];
                this.colors[index + 2] = colors[2];
            }
            this.colors[index] = colors[0];
            this.colors[index + 1] = colors[1];
            this.colors[index + 2] = colors[2];
        }

        public void putTextCoords(float[] textCoords)
        {
            int index = vertexIndex * 2;
            if (this.textCoords == null)
            {
                this.textCoords = new float[vertexCount * 2];
                this.textCoords[index] = textCoords[0];
                this.textCoords[index + 1] = textCoords[1];
            }
            this.textCoords[index] = textCoords[0];
            this.textCoords[index + 1] = textCoords[1];
        }

        public void draw(OpenGL gl)
        {
            if (net)
                gl.CullFace(OpenGL.GL_FRONT);
            else
                gl.CullFace(OpenGL.GL_BACK);
            if (vertices != null)
            {
                gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
                gl.VertexPointer(3, OpenGL.GL_FLOAT, 0, vertices);
            }
            if (normals != null)
            {
                gl.EnableClientState(OpenGL.GL_NORMAL_ARRAY);
                gl.NormalPointer(OpenGL.GL_FLOAT, 0, normals);
            }
            if (colors != null)
            {
                gl.EnableClientState(OpenGL.GL_COLOR_ARRAY);
                gl.ColorPointer(3, OpenGL.GL_UNSIGNED_BYTE, 0, colors);
            }
            if (textCoords != null)
            {
                gl.EnableClientState(OpenGL.GL_TEXTURE_COORD_ARRAY);
                gl.TexCoordPointer(2, OpenGL.GL_FLOAT, 0, textCoords);
            }
            gl.FrontFace(OpenGL.GL_CCW);
            if (lengths != null && strips != null)
                for (int i = 0; i < strips.Count; i++)
                {
                    gl.DrawElements(OpenGL.GL_TRIANGLE_STRIP, lengths[i], OpenGL.GL_UNSIGNED_INT, strips[i]);
                }
            if (doubleSurface)
            {
                if (doubleNormals != null)
                {
                    gl.NormalPointer(OpenGL.GL_FLOAT, 0, doubleNormals);
                }
                gl.FrontFace(OpenGL.GL_CW);

                for (int i = 0; i < strips.Count; i++)
                {
                    gl.DrawElements(OpenGL.GL_TRIANGLE_STRIP, lengths[i], OpenGL.GL_UNSIGNED_INT, strips[i]);
                }
            }

            gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.DisableClientState(OpenGL.GL_COLOR_ARRAY);
            gl.DisableClientState(OpenGL.GL_NORMAL_ARRAY);
            gl.DisableClientState(OpenGL.GL_TEXTURE_COORD_ARRAY);
        }
    }
}
