using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace StudySpace
{
    public partial class Form2 : Form
    {
        double lastMouseX, lastAngleY, lastMouseY, lastPosX, lastPosY;
        bool pressed = false;
        double AngleY = 0;
        double AngleX = 0;
        double AngleZ = 0;
        private int texture;
        Timer timer;
        double fi = 3;//скорость вокруг оси марса
        double fi2 = 3;//скорость вокруг оси деймоса
        const double Speed_Deimos = 0.009;//скорость движения по орбите деймоса
        const double Speed_Mars = 0.025;//скорость движения по орбите марса

        public struct PointFloat//начальное положение отрисовки сферы
        {
            public double X;
            public double Y;
        }
  
        const double Step = 0.3; // шаг перемещения наблюдателя в лабиринте
        const double hdl = 30;   
        const double AngleDl = 5;

        PointFloat Pos;

        Bitmap Space;
        Bitmap Mars;
        Bitmap FIO;
        Bitmap Deimos;
     
        public Form2()
        {
            InitializeComponent();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {

            GL.ClearColor(0f, 0.5f, 0.75f, 1.0f); // цвет фона
            // очистка буферов цвета и глубины
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // поворот изображения
            GL.LoadIdentity();

            GL.Rotate(AngleY, 0.0, 1.0, 0.0);
            GL.Translate(Pos.X, 0, Pos.Y);

            //Космическое пространство
            GLTexture.LoadTexture(Space);
            GL.Enable(EnableCap.Texture2D);
            GL.PushMatrix();
            GL.Rotate(120, 1.0, -1.0, 1.0);
            paint_Space(16, 40, 40, 0, 0, 0, false);
            GL.PopMatrix();
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);

            //Планета Марс
            GLTexture.LoadTexture(Mars);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);
            GL.PushMatrix();
            GL.Rotate(-50, 1.0, -1.0, 1.0);
            paint_Mars(2, 20, 20, 0, 0, 0, true);
            GL.PopMatrix();
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);

            //Спутник Деймос
            GLTexture.LoadTexture(Deimos);
            GL.Enable(EnableCap.Texture2D);
            GL.PushMatrix();
            GL.Rotate(-45, 1.0, -1.0, 1.0);
            paint_Deimos(0.6, 15, 15, Math.Sin(fi) * 6, Math.Cos(fi) * 6, 0, true);// радиус,растяжение объекта по x,y,траектория движения,вращение вокруг своей оси
            GL.PopMatrix();
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            //Табличка с ФИО
           GLTexture.LoadTexture(FIO);
            GL.Enable(EnableCap.Texture2D);          
            GL.PushMatrix();
            GL.Rotate(0, 1.0, -1.0, 1.0);
            GL.Translate(-2, -1.5, 4);
            GL.Begin(PrimitiveType.QuadStrip);
            GL.TexCoord2(0, 1);
            GL.Vertex3(0.2, -0.25, 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(1.2, -0.25, 2);
            GL.TexCoord2(0, 0);
            GL.Vertex3(0.2, 0.25, 2);
            GL.TexCoord2(1, 0);
            GL.Vertex3(1.2, 0.25, 2);
            GL.End();
            GL.PopMatrix();           
            GL.Disable(EnableCap.Texture2D);





            GL.Flush();
            GL.Finish();
            glControl1.SwapBuffers();

        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);//глубина теста,чтобы светилось нормально
            GL.Enable(EnableCap.Light0);
            Space = new Bitmap(@"C:\Users\devil\Desktop\Институт\Курсовые работы\3 семестр\Технологии пространственного моделирования\Mars-with-Deimos\StudySpace\Textures\Spacebmp.bmp");      
            Mars = new Bitmap(@"C:\Users\devil\Desktop\Институт\Курсовые работы\3 семестр\Технологии пространственного моделирования\Mars-with-Deimos\StudySpace\Textures\Mars.bmp");
           FIO = new Bitmap(@"C:\Users\devil\Desktop\Институт\Курсовые работы\3 семестр\Технологии пространственного моделирования\Mars-with-Deimos\StudySpace\Textures\ФИО.bmp");
            Deimos = new Bitmap(@"C:\Users\devil\Desktop\Институт\Курсовые работы\3 семестр\Технологии пространственного моделирования\Mars-with-Deimos\StudySpace\Textures\deimos2k.bmp");

            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.ProxyTexture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
          
            Pos.Y = -8;//начальное положение всех текстур

            timer = new Timer();
            timer.Interval = 1;
            timer.Tick += movesatellite;
            timer.Enabled = true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        } 
        private void movesatellite(object sender, EventArgs e)
        {
            fi += Speed_Deimos;
            fi2 += Speed_Mars;
            glControl1.Invalidate();
        }
        void paint_Mars(double r, int nx, int ny, double sx, double sy, double sz, bool rotate_texture = false)
        {
            int ix, iy;
            double x, y, z, tex_x, tex_y;


            for (iy = 0; iy < ny; ++iy)
            {
                tex_y = (double)iy / (double)ny;

                GL.Begin(PrimitiveType.QuadStrip);
                for (ix = 0; ix <= nx; ++ix)
                {
                    tex_x = (double)ix / (double)nx + (rotate_texture ? fi2 / 48 - Math.Floor(fi2) : 0);//поворот текстуры

                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + sy;
                    z = r * Math.Cos(iy * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);//нормаль направлена от центра
                    GL.TexCoord2(tex_x, tex_y);
                    GL.Vertex3(x, y, z);

                    

                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + sy;
                    z = r * Math.Cos((iy + 1) * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tex_x, tex_y + 1.0 / (double)ny);
                    GL.Vertex3(x, y, z);
                }

                GL.End();
            }
        }
        void paint_Space(double r, int nx, int ny, double sx, double sy, double sz, bool rotate_texture = false)
        {
            int ix, iy;
            double x, y, z, tex_x, tex_y;


            for (iy = 0; iy < ny; ++iy)
            {
                tex_y = (double)iy / (double)ny;

                GL.Begin(PrimitiveType.QuadStrip);
                for (ix = 0; ix <= nx; ++ix)
                {
                    tex_x = (double)ix / (double)nx + (rotate_texture ? fi  - Math.Floor(fi) : 0);

                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + sy;
                    z = r * Math.Cos(iy * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z); //нормаль направлена от центра
                    GL.TexCoord2(tex_x, tex_y);
                    GL.Vertex3(x, y, z);
                                   
                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + sy;
                    z = r * Math.Cos((iy + 1) * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tex_x, tex_y + 1.0 / (double)ny);
                    GL.Vertex3(x, y, z);
                }

                GL.End();
            }
        }
        void paint_Deimos(double r, int nx, int ny, double sx, double sy, double sz, bool rotate_texture = false)
        {
            int ix, iy;
            double x, y, z, tex_x, tex_y;


            for (iy = 0; iy < ny; ++iy)
            {
                tex_y = (double)iy / (double)ny;

                GL.Begin(PrimitiveType.QuadStrip);
                for (ix = 0; ix <= nx; ++ix)
                {
                    tex_x = (double)ix / (double)nx + (rotate_texture ? fi/3  - Math.Floor(fi) : 0);

                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + sy;
                    z = r * Math.Cos(iy * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);//нормаль направлена от центра
                    GL.TexCoord2(tex_x, tex_y);
                    GL.Vertex3(x, y, z);

                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + sy;
                    z = r * Math.Cos((iy + 1) * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tex_x, tex_y +1.0 / (double)ny);
                    GL.Vertex3(x, y, z);
                }

                GL.End();
            }
        }
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.A:
                    AngleY += -AngleDl;
                    break;
                case Keys.D:
                    AngleY -= -AngleDl;
                    break;
                case Keys.W:
                    Pos.X = Pos.X - Step * Math.Sin(AngleY * Math.PI / 90);
                    Pos.Y = Pos.Y + Step * Math.Cos(AngleY * Math.PI / 90);
                    break;
                case Keys.S:
                    Pos.X = Pos.X + Step * Math.Sin(AngleY * Math.PI / 90);
                    Pos.Y = Pos.Y - Step * Math.Cos(AngleY * Math.PI / 90);
                    break;
                case Keys.F5:
              
                    Form3 newForm = new Form3();
                    newForm.Show();
                    
                    break;


            }

            glControl1.Invalidate();

        }

        private void glControl1_Resize(object sender, EventArgs e)
        {

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Frustum(-0.5, 0.5, -0.5, 0.5, 0.5, 50);
            GL.MatrixMode(MatrixMode.Modelview);
            glControl1.Invalidate();

        }
        private void glControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            pressed = true;
            lastMouseX = e.X;
            lastMouseY = e.Y;
            lastAngleY = AngleY;
            lastPosX = Pos.X;
            lastPosY = Pos.Y;
        
            
        }
        private void glControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (pressed)
            {
                AngleY = lastAngleY - (e.X - lastMouseX) / 5;
                Pos.X = lastPosX + (e.Y - lastMouseY) / 50 * Math.Sin(AngleY * Math.PI / 180);
                Pos.Y = lastPosY - (e.Y - lastMouseY) / 50 * Math.Cos(AngleY * Math.PI / 180);
                glControl1.Invalidate();
            }


        }
        private void glControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            pressed = false;
        }


    }

}

