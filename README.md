### Разработка графического программного обеспечения для визуализации и работы с трехмерными объектами
ст. Галеев Тимур, гр. 3530203/00102 (летняя практика)

# Первые шаги и треугольник
Для реализации графического приложения я использую библиотеку `OpenTK`. Она предоставляет нам большой набор функций, которые мы можем использовать для управления графикой, и упрощает работу с OpenGL. OpenTK можно использовать для игр, научных приложений или других проектов, требующих трехмерной графики, аудио или вычислительной функциональности.  
## Создание окна
Первым делом нужно создать класс нашего движка `GameEngine`, базовым классом которого является класс `GameWindow` библиотеки OpenTK.
```c#
public class GameEngine : GameWindow {
  public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }
```
Добавим функцию инициализации. 
```c#
        public void Init()
        {
            GLFWBindingsContext binding = new GLFWBindingsContext();
            GL.LoadBindings(binding);
            if (GLFW.Init())
            {
                Console.WriteLine("Успешная инициализация!");
            }
        }
```
Готово! Теперь, когда в `Main` мы объявим наш класс, проинициализируем и запустим функцией `Run()`, у нас будет пустое белое окно.  
```c#
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace game_2
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings settings = GameWindowSettings.Default;
            NativeWindowSettings windowSettings = NativeWindowSettings.Default;

            windowSettings.WindowState = WindowState.Maximized;
            windowSettings.Title = "Game";

            GameEngine engine = new GameEngine(settings, windowSettings);
            engine.Init();
            engine.Run();
        }
    }
}
```
### Результат:
![st1](https://github.com/galeevlxix/game_engine/blob/master/screens/st1.png)
## Треугольник 
### Шейдеры
Напишем сами шейдеры: вершинный шейдер и фрагментный шейдер.
```c#
        string vertexShader =
                "#version 330 core\n" +
            "layout (location = 0) in vec3 aPosition;       \n" +
            "void main()                                    \n" +
            "{                                              \n" +
            "    gl_Position = vec4(aPosition, 1.0);        \n" +
            "}";

        string fragmentShader =
            "#version 330                                               \n" +
            "void main() { gl_FragColor = vec4(0.8, 0.2, 1.0, 1.0); }   \n";
```
Шейдер — это программа, которая выполняет графические вычисления. В нашем случае преобразование вершин или определение цвета пикселя. Обычно эти программы выполняются в графическом процессоре.
### Класс шейдеров
Создадим класс шейдеров.
```c#
public class Shader : IDisposable
    {
        int Handle;
        ...
```
На вход конструктора подается исходный код шейдеров. Далее мы генерируем наши шейдеры и привязываем исходный код к дескрипторам. Затем мы компилируем шейдеры и проверяем на наличие ошибок. 

Наши отдельные шейдеры скомпилированы, но чтобы их действительно использовать, мы должны связать их вместе в программу, которая может быть запущена на графическом процессоре. Это то, что мы имеем в виду, когда с этого момента говорим о "шейдере".

Это все, что нужно! `Handle` - это теперь полезная шейдерная программа.

Прежде чем мы покинем конструктор, нам следует произвести небольшую очистку. Отдельные вершинные и фрагментные шейдеры теперь бесполезны, поскольку они были связаны. 
```c#
        public Shader(string vs, string fs)
        {
            //дескрипторы шейдеров 
            int VertexShader;
            int FragmentShader;

            //создание и привязка к дескрипторам
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, vs);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fs);
            
            //компиляция шейдеров
            CompileShaders(VertexShader, FragmentShader);

            //связываем шейдеры 
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infolog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infolog);
            }

            //очистка
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }
```
```c#
        public void CompileShaders(int VertexShader, int FragmentShader)
        {
            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int success2);
            if (success2 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }
        }
```
Теперь у нас есть действующий шейдер, так что давайте добавим функцию его использования.
```c#
        public void Use()
        {
            GL.UseProgram(Handle);
        }
```
### VBO и VAO
<sub>**Vertex Buffer Object (VBO)** может хранить большое количество вершин в памяти графического процессора. Преимущество использования VBO заключается в том, что мы можем отправлять одновременно большие пакеты данных на видеокарту без необходимости отправлять данные по одной вершине за раз.
**Vertex Arrays Object (VAO)** говорит OpenGL, какую часть VBO следует использовать в последующих командах. Преимущество VAO заключается в том, что при настройке указателей атрибутов вершин вам нужно выполнить эти вызовы только один раз, и всякий раз, когда мы хотим нарисовать объект, мы можем просто привязать соответствующий VAO. Это упрощает переключение между различными конфигурациями данных вершин и атрибутов так же, как привязку к другому VAO. Все состояния, которые мы только что установили, хранятся внутри VAO.</sub>

Напишем дескрипторы VAO и VBO:
```c#
        int VBO;
        int VAO;
```
В функции `OnLoad` движка:
```c#
            VBO = GL.GenBuffer();            
            VAO = GL.GenVertexArray();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
```
<sub>Создадим и привяжем VAO и VBO. `GL.BufferData` - это функция, специально предназначенная для копирования данных в текущий связанный буфер.
В `GL.VertexAttribPointer` мы говорим `OpenGL`, как она должен интерпритировать данные вершины (для каждого атрибута вершины).
Теперь, когда мы указали, как `OpenGL` должен интерпретировать данные вершины, мы должны включить атрибут вершины в `GL.EnableVertexAttribArray`, указав местоположение атрибута вершины в качестве аргумента. </sub>
### Создадим вершины нашего треугольника 
```c#
        float[] vertices = {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
             0.5f, -0.5f, 0.0f, //Bottom-right vertex
             0.0f,  0.5f, 0.0f  //Top vertex
        };
```
###  Отрисуем наш объект
В `OnRenderFrame`:
```c#
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shader.Use();
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            
            SwapBuffers();
        }
```
### Результат:
![st2](https://github.com/galeevlxix/game_engine/blob/master/screens/tria.png)
# Индексная отрисовка
Вершины и индексы наших треугольников 
```
        float[] vertices = {    //вершины
            -0.5f, 0.5f, 0.0f,
            -1.0f, -0.5f, 0.0f,
            0.0f, -0.5f, 0.0f,
            0.5f, 0.5f, 0.0f,
            1.0f, -0.5f, 0.0f,
        };

        int[] indices = {  
            0, 1, 2,   // первый треугольник
            2, 3, 4    // второй треугольник
        };
```
В `OnLoad` добавим код:
```c#
        IBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
```
В `OnRenderFrame` поменяем функцию отрисовки на:
```c#
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
```
## Результат:
![st3](https://github.com/galeevlxix/game_engine/blob/master/screens/2tria.png)
# Интерполяционный цвет
Добавим в вершинный шейдер выходной параметр vertexColor, который будет входным параметром в фрагментном шейдере, где gl_FragColor будет получать его значение.
```c#
        string vertexShader =
                "#version 330                                       \n" +
            "layout (location = 0) in vec3 aPosition;               \n" +
            "out vec4 vertexColor;                                  \n" +
            "void main()                                            \n" +
            "{                                                      \n" +
            "    gl_Position = vec4(aPosition, 1.0);                \n" +
            "   vertexColor = vec4(clamp(aPosition, 0.0, 1.0), 1.0);\n" +
            "}";

        string fragmentShader =
            "#version 330                                             \n" +
            "in vec4 vertexColor;                                     \n"+
            "void main() { gl_FragColor = vertexColor; }              \n";
```
### Результат:
![st4](https://github.com/galeevlxix/game_engine/blob/master/screens/interp.png)
# Вращение, перемещение, масштаб
## Создание класса Pipeline
Pipeline будет вспомогательным классом для создания и работы с матрицами масштабирвания, вращения, сдвига (относительно осей координат), а также проекции перспективы.
### Ввод значений свойств объектов
У каждого объекта будет по 3 вектора: вращения, перемещения, масштаба. И одна структура mPersProj для настройки перспективы.
```c#
        public vector3 RotateVector;
        public vector3 ScaleVector;
        public vector3 PositionVector;
        public mPersProj mPersProj;
        public Pipeline()
        {
            RotateVector = new vector3 { x = 0, y = 0, z = 0 };
            ScaleVector = new vector3 { x = 1, y = 1, z = 1 };
            PositionVector = new vector3 { x = 0, y = 0, z = 0 };
            mPersProj = new mPersProj();
        }
```
```c#
    public struct mPersProj
    {
        public float FOV;
        public float width;
        public float height;
        public float zNear;
        public float zFar;
    }
```
Для ввода значений этих векторов напишем специальные public функции.
```c#
        public void Rotate(float angleX, float angleY, float angleZ)
        {
            RotateVector.x = angleX;
            RotateVector.y = angleY;
            RotateVector.z = angleZ;
        }

        public void Scale(float ScaleX, float ScaleY, float ScaleZ)
        {
            ScaleVector.x = ScaleX;
            ScaleVector.y = ScaleY;
            ScaleVector.z = ScaleZ;
        }

        public void Position(float PosX, float PosY, float PosZ)
        {
            PositionVector.x = PosX;
            PositionVector.y = PosY;
            PositionVector.z = PosZ;
        }

        public void PersProj(float FOV, float width, float height, float zNear, float zFar)
        {
            mPersProj.FOV = FOV;
            mPersProj.width = width;
            mPersProj.height = height;
            mPersProj.zNear = zNear;
            mPersProj.zFar = zFar;
        }
```
### Получение матриц
Матрица вращения:
```c#
        private static Matrix4 InitRotateTransform(float RotateX, float RotateY, float RotateZ)
        {
            Matrix4 rx = new Matrix4(), ry = new Matrix4(), rz = new Matrix4();
            float x = math3d.ToRadian(RotateX);
            float y = math3d.ToRadian(RotateY);
            float z = math3d.ToRadian(RotateZ);

            rx[0, 0] = 1.0f;              rx[0, 1] = 0.0f;              rx[0, 2] = 0.0f;              rx[0, 3] = 0.0f;
            rx[1, 0] = 0.0f;              rx[1, 1] = math3d.cos(x);     rx[1, 2] = -math3d.sin(x);    rx[1, 3] = 0.0f;
            rx[2, 0] = 0.0f;              rx[2, 1] = math3d.sin(x);     rx[2, 2] = math3d.cos(x);     rx[2, 3] = 0.0f;
            rx[3, 0] = 0.0f;              rx[3, 1] = 0.0f;              rx[3, 2] = 0.0f;              rx[3, 3] = 1.0f;

            ry[0, 0] = math3d.cos(y);     ry[0, 1] = 0.0f;              ry[0, 2] = -math3d.sin(y);    ry[0, 3] = 0.0f;
            ry[1, 0] = 0.0f;              ry[1, 1] = 1.0f;              ry[1, 2] = 0.0f;              ry[1, 3] = 0.0f;
            ry[2, 0] = math3d.sin(y);     ry[2, 1] = 0.0f;              ry[2, 2] = math3d.cos(y);     ry[2, 3] = 0.0f;
            ry[3, 0] = 0.0f;              ry[3, 1] = 0.0f;              ry[3, 2] = 0.0f;              ry[3, 3] = 1.0f;

            rz[0, 0] = math3d.cos(z);     rz[0, 1] = -math3d.sin(z);    rz[0, 2] = 0.0f;              rz[0, 3] = 0.0f;
            rz[1, 0] = math3d.sin(z);     rz[1, 1] = math3d.cos(z);     rz[1, 2] = 0.0f;              rz[1, 3] = 0.0f;
            rz[2, 0] = 0.0f;              rz[2, 1] = 0.0f;              rz[2, 2] = 1.0f;              rz[2, 3] = 0.0f;
            rz[3, 0] = 0.0f;              rz[3, 1] = 0.0f;              rz[3, 2] = 0.0f;              rz[3, 3] = 1.0f;

            return rz * ry * rx;
        }
```
Матрица перемещения:
```c#
        private static Matrix4 InitTranslationTransform(float x, float y, float z)
        {
            Matrix4 m = new Matrix4();
            m[0, 0] = 1.0f; m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = 1.0f; m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = 1.0f; m[2, 3] = 0.0f;
            m[3, 0] = x; m[3, 1] = y; m[3, 2] = z; m[3, 3] = 1.0f;
            return m;
        }
```
Матрица масштаба:
```c#
        private static Matrix4 InitScaleTransform(float x, float y, float z)
        {
            Matrix4 m = new Matrix4();
            m[0, 0] = x;    m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = y;    m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = z;    m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
            return m;
        }
```
Но все эти матрицы мы можем получить только внутри класса, когда хотим получить матрицу `mvp` (проекция вида модели). Она нам еще очень понадобится.
```c#
        public Matrix4 getMVP()
        {
            return  InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z) *
                    InitRotateTransform(RotateVector.x, RotateVector.y, RotateVector.z) *
                    InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z) *
                    InitPersProjTransform(mPersProj.FOV, mPersProj.width, mPersProj.height, mPersProj.zNear, mPersProj.zFar);                    
        }
```
## Изменения в шейдере
При инициализации шейдера объявляем его входным параметром матрицу `mvp` в шейдере.
```c#
        MVPID = GL.GetUniformLocation(Handle, "mvp");
```
Перед отрисовкой объекта загружаем его `mvp` в объект шейдера и свзяываем с дескриптором `MVPID`.
```c#
        public void setMatrix(Matrix4 m)
        {
            GL.UniformMatrix4(MVPID, true, ref m);
        }
```
## Изменения в классе `GameEngine`
### Вершинный шейдер
Эту `mvp` мы засунем в вершинный шейдер, чтобы умножить ее на все вершины объекта, тем самым преобразовав вид объекта.
```c#
        string vertexShader =
            "#version 330                                           \n" +
            "layout (location = 0) in vec3 aPosition;               \n" +
            "out vec4 vertexColor;                                  \n" +
            "uniform mat4 mvp;                                      \n" +
            "void main()                                            \n" +
            "{                                                      \n" +
            "   gl_Position = vec4(aPosition, 1.0) * mvp;           \n" +
            "   vertexColor = vec4(clamp(aPosition, 0.0, 1.0), 1.0);\n" +
            "}";
```
### Теперь работаем с кубом
Для демонстрации визуализации именно 3D объектов создадим вершины и индексы куба.
```c#
        float[] vertices = {    //куб
              0.5f, -0.5f, -0.5f,
              0.5f, -0.5f,  0.5f,
             -0.5f, -0.5f,  0.5f,
             -0.5f, -0.5f, -0.5f,
              0.5f,  0.5f, -0.5f,
              0.5f,  0.5f,  0.5f,
             -0.5f,  0.5f,  0.5f,
             -0.5f,  0.5f, -0.5f
        };

        int[] indices = {  
                0,1,2, // передняя сторона
                2,3,0,

                6,5,4, // задняя сторона
                4,7,6,

                4,0,3, // левый бок
                3,7,4,

                1,5,6, // правый бок
                6,2,1,

                4,5,1, // вверх
                1,0,4,

                3,2,6, // низ
                6,7,3,
        };
```
### При загрузке окна
Проинициализируем наш `Pipeline` и установим проекцию перспективы. Установим таймер игры на 0.
```c#
            p = new Pipeline();
            p.PersProj(50.0f, 1920, 1080, 0.1f, 100.0f);
            timer = 0;
```
### В функции рендеринга
Мы теперь можем в реальном времени изменять его свойства: вращать, изменять размер, перемещать. Перед отрисовкой объекта загружаем полученную матрицу `mvp` в шейдер. 
```c#
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            timer++;

            p.Scale(0.5f, 0.5f, 0.5f);
            p.Position(0, math3d.abs(math3d.sin((float)timer / 500) / 2), -3);
            p.Rotate(math3d.sin((float)timer / 500) * 50, (float)timer / 50, 0);

            shader.setMatrix(p.getMVP());
            shader.Use();

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }
```
## Результат:
![cube](https://github.com/galeevlxix/game_engine/blob/master/screens/anim.gif)
