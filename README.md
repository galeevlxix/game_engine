### Разработка графического программного обеспечения для визуализации и работы с трехмерными объектами
ст. Галеев Тимур, гр. 3530203/00102 (летняя практика)
# Оглавление
[Первые шаги и треугольник](#s1)

[Индексная отрисовка](#s3)

[Интерполяционный цвет](#s4)

[Вращение, перемещение, масштаб](#s5)

[Добавим класс GameObj](#s6)

[Разные форматы файлов](#s7)

[Камера](#s8)

<a name="s1"></a>
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
<a name="s3"></a> 
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
<a name="s4"></a> 
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
<a name="s5"></a> 
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
            m[0, 0] = 1.0f;   m[0, 1] = 0.0f;   m[0, 2] = 0.0f;   m[0, 3] = 0.0f;
            m[1, 0] = 0.0f;   m[1, 1] = 1.0f;   m[1, 2] = 0.0f;   m[1, 3] = 0.0f;
            m[2, 0] = 0.0f;   m[2, 1] = 0.0f;   m[2, 2] = 1.0f;   m[2, 3] = 0.0f;
            m[3, 0] = x;      m[3, 1] = y;      m[3, 2] = z;      m[3, 3] = 1.0f;
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
Матрица проекции перспективы:
```c#
        private static Matrix4 InitPersProjTransform(float FOV, float Width, float Height, float zNear, float zFar)
        {
            Matrix4 m = new Matrix4();
            float ar = Width / Height;            //Соотношение сторон
            float zRange = zNear - zFar;          //Разница расстояний до ближней и дальней плоскостей отсечения
            float tanHalfFOV = math3d.tan(math3d.ToRadian(FOV) * 0.5f);  //Угол обзора в радианах

            m[0, 0] = 1.0f / (tanHalfFOV * ar);     m[0, 1] = 0.0f;                 m[0, 2] = 0.0f;                         m[0, 3] = 0;
            m[1, 0] = 0.0f;                         m[1, 1] = 1.0f / tanHalfFOV;    m[1, 2] = 0.0f;                         m[1, 3] = 0;
            m[2, 0] = 0.0f;                         m[2, 1] = 0.0f;                 m[2, 2] = -(-zNear - zFar) / zRange;     m[2, 3] = -1.0f;
            m[3, 0] = 0.0f;                         m[3, 1] = 0.0f;                 m[3, 2] = 2.0f * zFar * zNear / zRange; m[3, 3] = 0;
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
Мы теперь можем в реальном времени изменять свойства объекта: вращать, изменять размер, перемещать. Перед отрисовкой объекта загружаем полученную матрицу `mvp` в шейдер. 
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
<a name="s6"></a>
# Добавим класс GameObj
Чтобы изолировать или отделить всю логику объекта (VBO, VAO, IBO, mvp...) от основного класса движка добавим класс для каждого конкретного объекта. Все объекты будут отныне отдельно друг от друга вращаться, перемещаться, отрисовываться и тд. Заодно избавим класс движка от многотонного кода. Ничего нового мы не добавили, а просто переместили код в отдельные классы.
### Добавим класс Storage
Где в будущем мы просто будем хранить всякие шейдеры, вершины и индексы.
```c#
    public class Storage
    {
        public string vertexShader { get; }
        public string fragmentShader { get; }

        public float[] cubeVertices { get; }
        public int[] cubeIndices { get; }

        private mPersProj mPersProj;

        public mPersProj GetPersProj { get => mPersProj; }

        public Storage()
        {
            vertexShader =
            "#version 330                                           \n" +
            "layout (location = 0) in vec3 aPosition;               \n" +
            "out vec4 vertexColor;                                  \n" +
            "uniform mat4 mvp;                                      \n" +
            "void main()                                            \n" +
            "{                                                      \n" +
            "   gl_Position = vec4(aPosition, 1.0) * mvp;           \n" +
            "   vertexColor = vec4(clamp(aPosition, 0.0, 1.0), 1.0);\n" +
            "}";

            fragmentShader =
            "#version 330                                           \n" +
            "in vec4 vertexColor;                                   \n" +
            "void main() { gl_FragColor = vertexColor; }            \n";

            cubeVertices = new float[]{    //куб
                0.5f, -0.5f, -0.5f,
              0.5f, -0.5f,  0.5f,
             -0.5f, -0.5f,  0.5f,
             -0.5f, -0.5f, -0.5f,
              0.5f,  0.5f, -0.5f,
              0.5f,  0.5f,  0.5f,
             -0.5f,  0.5f,  0.5f,
             -0.5f,  0.5f, -0.5f
            };
            cubeIndices = new int[]{
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
                6,7,3
            };

            mPersProj = new mPersProj();
            mPersProj.FOV = 50;
            mPersProj.width = 1920;
            mPersProj.height = 1080;
            mPersProj.zNear = 0.1f;
            mPersProj.zFar = 100;
        }
    }
```
### Добавим класс Mesh
Тут будут создаваться и связываться VBO, VAO, IBO каждого нашего объекта.
```c#
    public class Mesh : IDisposable
    {
        private int VBO { get; set; }
        private int VAO { get; set; }
        private int IBO { get; set; }

        private float[] Vertices { get; set; }
        private int[] Indices { get; set; }

        public Mesh()
        {
            Vertices = new Storage().cubeVertices;
            Indices = new Storage().cubeIndices;

            Load();
        }

        public Mesh(string file_name)
        {

        }

        private void Load()
        {
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            IBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(0);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
        }
    }
```
### Добавим класс GameObj
У всех объектов будет свой Mesh, Shader и mvp на экране. Пока что GameObj выглядит так, но в будущем мы его дополним таким образом, чтобы можно было создавать модели любых форматов.
```c#
    public class GameObj
    {
        private Mesh mesh;
        private Shader shader;
        private Pipeline pipeline;

        public GameObj()
        {
            Storage stor = new Storage();
            mesh = new Mesh();
            shader = new Shader(stor.vertexShader, stor.fragmentShader);
            pipeline = new Pipeline();
            Position(0, 0, -2);
            Rotate(0, 0, 0);
            Scale(0, 0, 0);
            pipeline.mPersProj = stor.GetPersProj;
        }

        public void Draw()
        {
            shader.setMatrix(pipeline.getMVP());
            shader.Use();
            mesh.Draw();
        }

        public void Rotate(float x, float y, float z)
        {
            pipeline.Rotate(x, y, z);
        }

        public void Position(float x, float y, float z)
        {
            pipeline.Position(x, y, z);
        }

        public void Scale(float x, float y, float z)
        {
            pipeline.Scale(x, y, z);
        }
    }
```
### Как теперь выглядит отрисовка
В функциии рендеринга мы просто задаем свойства объекта и отрисовываем его.
```c#
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            timer++;

            gameObj.Scale(0.5f, 0.5f, 0.5f);
            gameObj.Position(0, math3d.abs(math3d.sin((float)timer / 500) / 2) - 0.25f, -2);
            gameObj.Rotate(math3d.sin((float)timer / 500) * 50, (float)timer / 50, 0);

            gameObj2.Scale(0.5f, 0.5f, 0.5f);
            gameObj2.Position(2, 0, -4);
            gameObj2.Rotate(0 , (float)timer / 25, 0);

            gameObj.Draw();
            gameObj2.Draw();

            SwapBuffers();
        }
```
## Результат:
![2obj](https://github.com/galeevlxix/game_engine/blob/master/screens/bandicam%202023-06-28%2016-01-06-972.gif)
<a name="s7"></a>
# Разные форматы 3D-моделей
Расширим наш класс `Mesh`. Теперь мы можем загружать 3D-модели разных форматов, а не только кубики. Все потому что эти файлы внутри уже содержат вершины и индексы, которые необходиы нам для отрисовки объекта.
Добавим еще один конструктор `Mesh`, который будет проверять, какого формата объект, и загружать его в буфер.
```c#
        public Mesh(string file_name)
        {
            if (regOBJ.IsMatch(file_name))
            {
                LoadFromObj(new StreamReader(file_name));
                Console.WriteLine("+obj");
            } 
            else if(regFBX.IsMatch(file_name))
            {
                LoadFromFbx(new StreamReader(file_name));
                Console.WriteLine("+fbx");
            }
            else if (regDAE.IsMatch(file_name))  //не работает
            {
                LoadFromDae(new StreamReader(file_name));
                Console.WriteLine("+dae");
            }
            else if (regPLY.IsMatch(file_name))
            {
                LoadFromPly(new StreamReader(file_name));
                Console.WriteLine("+ply");
            }
            else
            {
                Vertices = new Storage().cubeVertices;
                Indices = new Storage().cubeIndices;
                Console.WriteLine("Unknown file format");
            }
            Load();
        }
```
Каждый из этих классов читает файл построчно и высасывает из них вершины и индексы объекта.
```c#
        private void LoadFromObj(TextReader tr)
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();

            vertices.Add(0.0f);
            vertices.Add(0.0f);
            vertices.Add(0.0f);

            string line;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace("  ", " ");
                var parts = line.Split(' ');

                if (parts.Length == 0) continue;
                switch (parts[0])
                {
                    case "v":
                        vertices.Add(float.Parse(parts[1], CultureInfo.InvariantCulture));
                        vertices.Add(float.Parse(parts[2], CultureInfo.InvariantCulture));
                        vertices.Add(float.Parse(parts[3], CultureInfo.InvariantCulture));
                        break;
                    case "f":
                        if (parts.Length == 4) 
                        {
                            foreach (string v in parts)
                            {
                                if (v != "f")
                                {
                                    var w = v.Split('/');
                                    fig.Add(int.Parse(w[0]));
                                }
                            }
                        }
                        if (parts.Length == 5)
                        {
                            var temp = new List<int>();

                            foreach (string v in parts)
                            {
                                if (v != "f")
                                {
                                    var w = v.Split('/');
                                    if (w[0] != "")
                                        temp.Add(int.Parse(w[0]));
                                }
                            }

                            fig.Add(temp[0]);
                            fig.Add(temp[2]);
                            fig.Add(temp[3]);

                            fig.Add(temp[0]);
                            fig.Add(temp[1]);
                            fig.Add(temp[2]);
                        }
                        break;
                }
            }
            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }

        private void LoadFromFbx(TextReader tr)
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();
            bool ver = false, frag = false;
            Regex r1 = new Regex(@"^Vertices:\w*");
            Regex r2 = new Regex(@"^a:\w*");
            Regex r3 = new Regex(@"^PolygonVertexIndex:\w*");

            string line;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace(" ", "");
                line = line.Replace("\t", "");

                if (r1.IsMatch(line))
                {
                    ver = true;
                }
                else if (r3.IsMatch(line))
                {
                    frag = true;
                }
                else if (ver)
                {
                    if(line == "}")
                    {
                        ver = false;
                        continue;
                    }
                    if (r2.IsMatch(line)) line = line.Replace("a:", "");
                    line = line.Trim(',');
                    var w = line.Split(',');
                    foreach (string s in w)
                        if (s != "" && s != null)
                            vertices.Add(float.Parse(s, CultureInfo.InvariantCulture));
                }
                else if (frag)
                {
                    if (line == "}")
                    {
                        frag = false;
                        continue;
                    }
                    if (r2.IsMatch(line)) line = line.Replace("a:", "");
                    line = line.Trim(',');
                    var w = line.Split(',');
                    var temp = new List<int>();
                    foreach (string s in w)
                    {
                        if (s != "" && s != null)
                            temp.Add(int.Parse(s));
                        if (temp.Count == 4)
                        {
                            fig.Add(temp[0]);
                            fig.Add(temp[2]);
                            fig.Add(temp[3]);

                            fig.Add(temp[0]);
                            fig.Add(temp[1]);
                            fig.Add(temp[2]);
                            temp.Clear();
                        }
                    }
                }
            }

            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }

        private void LoadFromDae(TextReader tr)  //не работает
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();
            bool ver = false, frag = false, msh = false;

            Regex r = new Regex(@"<\w*>");
            Regex r1 = new Regex(@"^<p>\w*");
            char[] fdd = new char[] { '>', '<' };

            string line;

            while((line = tr.ReadLine()) != null)
            {
                line = line.Trim(' ');
                line = line.Replace("\t", "");
                if (line == "</mesh>")
                {
                    return;
                }
                else if (line == "<source id=\"TopN-mesh-positions\">")
                {
                    ver = true;
                }
                else if (r1.IsMatch(line))
                {
                    var t = line.Split(fdd);
                    var w = t[2].Split(' ');
                    foreach (string v in w)
                    {
                        if (v != "" && v != null)
                            fig.Add(int.Parse(v));
                    }
                }
                else if (ver)
                {
                    ver = false;
                    var t = line.Split(fdd);
                    var w = t[2].Split(' ');
                    foreach(string v in w)
                    {
                        if (v != "" && v != null)
                            vertices.Add(float.Parse(v, CultureInfo.InvariantCulture));
                    }
                }
            }
            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }

        private void LoadFromPly(TextReader tr)
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();
            string line;
            bool a = false;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Trim(' ');

                if (line == "end_header")
                {
                    a = true;
                }
                else if (a)
                {
                    var w = line.Split(' ');

                    if (w.Length == 8)
                    {
                        vertices.Add(float.Parse(w[0], CultureInfo.InvariantCulture));
                        vertices.Add(float.Parse(w[1], CultureInfo.InvariantCulture));
                        vertices.Add(float.Parse(w[2], CultureInfo.InvariantCulture));
                    }
                    else if(w.Length == 4)
                    {
                        if (w[0] == "3")
                        {
                            fig.Add(int.Parse(w[1]));
                            fig.Add(int.Parse(w[2]));
                            fig.Add(int.Parse(w[3]));
                        }
                        if (w[0] == "4")
                        {
                            var temp = new List<int>();

                            temp.Add(int.Parse(w[1]));
                            temp.Add(int.Parse(w[2]));
                            temp.Add(int.Parse(w[3]));
                            temp.Add(int.Parse(w[4]));

                            fig.Add(temp[0]);
                            fig.Add(temp[2]);
                            fig.Add(temp[3]);

                            fig.Add(temp[0]);
                            fig.Add(temp[1]);
                            fig.Add(temp[2]);
                        }
                    }
                }

            }
            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }
```
## Результаты:
### .obj
Тут сразу 3 объекта на экране (мужик, машина и пол это все один объект)
![objform](https://github.com/galeevlxix/game_engine/blob/master/screens/scene1.gif)
### .fbx
![fbxform](https://github.com/galeevlxix/game_engine/blob/master/screens/fbxfi.png)
### .ply
![plyfor](https://github.com/galeevlxix/game_engine/blob/master/screens/ply.png)
На этом моменте моя работа в рамках летней практики заканчивается. Но я намерен дальше развивать проект и добавлять в него кучу всего.
<a name = "s8"></a>
# Камера
Как реализовать камеру? Вернее, как ей управлять? 
Когда мы ходим в играх вперед/назад, влево/враво или летаем вверх/вниз, это не мы перемещаемся относительно системы координат, в которой находятся объекты, а, наоборот, мы перемещаем с.к. относительно камеры, но перемещаем в другую сторону. То же самое, когда мы вертим мышкой камеру. Мы вертим с.к. с объектами вокруг себя.
## Класс `Quaternion`
Кватернион представляет ось в 3D-пространстве и поворот вокруг этой оси. Он нужен нам для вращения камеры мышкой.
```c#
    public class Quaternion
    {
        public float x, y, z, w;

        public Quaternion(float _x, float _y, float _z, float _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }

        public void Normalize()
        {
            float Length = math3d.sqrt(x * x + y * y + z * z + w * w);

            x /= Length;
            y /= Length;
            z /= Length;
            w /= Length;
        }

        public Quaternion Conjugate()
        {
            return new Quaternion(-x, -y, -z, w);
        }

        public static Quaternion operator *(Quaternion l, Quaternion r)
        {
            float w = (l.w * r.w) - (l.x * r.x) - (l.y * r.y) - (l.z * r.z);
            float x = (l.x * r.w) + (l.w * r.x) + (l.y * r.z) - (l.z * r.y);
            float y = (l.y * r.w) + (l.w * r.y) + (l.z * r.x) - (l.x * r.z);
            float z = (l.z * r.w) + (l.w * r.z) + (l.x * r.y) - (l.y * r.x);

            return new Quaternion(x, y, z, w);
        }

        public static Quaternion operator *(Quaternion q, vector3f v)
        {
            float w = -(q.x * v.x) - (q.y * v.y) - (q.z * v.z);
            float x = (q.w * v.x) + (q.y * v.z) - (q.z * v.y);
            float y = (q.w * v.y) + (q.z * v.x) - (q.x * v.z);
            float z = (q.w * v.z) + (q.x * v.y) - (q.y * v.x);

            return new Quaternion(x, y, z, w);
        }
    }
```
## Класс `Camera`
Наша камера будет хранить в себе три основных `vector3f` свойства: позиция `Pos`, направление `Target` и вверх `Up`. Вектор `Left` - вспомогательный, он высчитывается путем перекрестного произведения векторов `Up` и `Target`. 

`angle_h` - угол горизонтального поворота. `angle_v` - вертикального.

`velocity` - скорость перемещения камеры. `sensitivity` - чувствительность мышки.

`brakingKeyBo` и `brakingMouse` отвечают за торможение перемещения и вращения камеры. Здесь реализовано плавное управление камерой. Это необязательно, но прикольно.

`zeroLimit` - это такое маленькое число, что, достигая при торможении скорости его значения и ниже, мы задаем значение скорости 0.
```c#
        public vector3f Pos { get; private set; }
        public vector3f Target { get; private set; }
        public vector3f Up { get; private set; }
        private vector3f Left;

        private float angle_h; 
        private float angle_v;

        private float velocity = 0.00015f;
        private float sensitivity = 0.002f;
        private float brakingKeyBo = 0.01f;
        private float brakingMouse = 0.03f;

        private float zeroLimit = 0.0001f;
```
Скорости перемещения и поворота камерой:
```c#
        private float speedX;
        private float speedY;
        private float speedZ;

        private float angularX;
        private float angularY;
```
