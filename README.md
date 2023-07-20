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
**Vertex Buffer Object (VBO)** может хранить большое количество вершин в памяти графического процессора. Преимущество использования VBO заключается в том, что мы можем отправлять одновременно большие пакеты данных на видеокарту без необходимости отправлять данные по одной вершине за раз.

**Vertex Arrays Object (VAO)** говорит OpenGL, какую часть VBO следует использовать в последующих командах. Преимущество VAO заключается в том, что при настройке указателей атрибутов вершин вам нужно выполнить эти вызовы только один раз, и всякий раз, когда мы хотим нарисовать объект, мы можем просто привязать соответствующий VAO. Это упрощает переключение между различными конфигурациями данных вершин и атрибутов так же, как привязку к другому VAO. Все состояния, которые мы только что установили, хранятся внутри VAO.

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
Создадим и привяжем VAO и VBO. 

`GL.BufferData` - это функция, специально предназначенная для копирования данных в текущий связанный буфер.

В `GL.VertexAttribPointer` мы говорим `OpenGL`, как она должен интерпритировать данные вершины (для каждого атрибута вершины).

Теперь, когда мы указали, как `OpenGL` должен интерпретировать данные вершины, мы должны включить атрибут вершины в `GL.EnableVertexAttribArray`, указав местоположение атрибута вершины в качестве аргумента.
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
            mesh.Draw();
            shader.setMatrix(pipeline.getMVP());
            shader.Use();
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
## Класс `Camera`
Наша камера будет хранить в себе три основных `vector3f` свойства: позиция `Pos`, направление `Target` и вверх `Up`. Вектор `Left` - вспомогательный, он высчитывается путем перекрестного произведения векторов `Up` и `Target`. 

`angle_h` - угол горизонтального поворота. `angle_v` - вертикального.

`velocity` - ускорение перемещения камеры. `sensitivity` - чувствительность мышки (ускорение поворота камеры).

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
Это объявление класса камеры. Он хранит 3 свойства, которые характеризуют камеру - позиция, направление и верхний вектор. По умолчанию просто располагает камеру в начале координат, направляет ее в сторону уменьшения Z, а верхний вектор устремляет в "небо" (0,1,0). Но есть возможность создать камеру с указанием значений атрибутов.
```c#
        public Camera()
        {
            Pos = vector3f.Zero;
            Target = vector3f.Ford;
            Target.Normalize();
            Up = vector3f.Up;
            Left = new vector3f();

            Init();
        }

        public Camera(vector3f cameraPos, vector3f cameraTarget, vector3f cameraUp)
        {
            Pos = cameraPos;
            Target = cameraTarget;
            Target.Normalize();
            Up = cameraUp;
            Up.Normalize();
            Left = new vector3f();

            Init();
        }
```
В функции Init() мы начинаем с вычисления горизонтального угла. Мы создаем новый вектор, названый HTarget (направление по горизонтали), который является проекцией исходного вектора направления на плоскость XZ. Затем мы его нормируем (так как для выводов выше требуется единичный вектор на плоскости XZ). Затем мы проверяем какой кватернион соответствует вектору для конечного подсчета значения координаты Z. Далее мы подсчитываем вертикальный угол; сделать это гораздо проще.Скорости перемещения и поворота камеры изначально равны 0. 
```c#
        private void Init()
        {
            vector3f HTarget = new vector3f(Target.x, 0, Target.z);
            HTarget.Normalize();

            if (HTarget.z >= 0.0f)
            {
                if (HTarget.x >= 0.0f)
                {
                    angle_h = 360.0f - math3d.ToDegree(math3d.asin(HTarget.z));
                }
                else
                {
                    angle_h = 180.0f + math3d.ToDegree(math3d.asin(HTarget.z));
                }
            }
            else
            {
                if (HTarget.x >= 0.0f)
                {
                    angle_h = math3d.ToDegree(math3d.asin(-HTarget.z));
                }
                else
                {
                    angle_h = 90.0f + math3d.ToDegree(math3d.asin(-HTarget.z));
                }
            }

            angle_v = -math3d.ToDegree(math3d.asin(Target.y));

            speedX = 0.00f;
            speedY = 0.00f;
            speedZ = 0.00f;
            angularX = 0;
            angularY = 0;
        }
```
В функцию `OnKeyboard` мы задаем скорость камеры в направлении, выбранном игроком. А в `OnMouse` - скорость вращения камеры. Это функции управления камерой, которые мы будем вызывать в движке.
```c#
        public void OnKeyboard(KeyboardState key)
        {
            if (!key.IsAnyKeyDown) return;
            if (key.IsKeyDown(Keys.W))
            {
                speedZ -= velocity;
            }
            if (key.IsKeyDown(Keys.S))
            {
                speedZ += velocity;
            }
            if (key.IsKeyDown(Keys.A))
            {
                speedX += velocity;
            }
            if (key.IsKeyDown(Keys.D))
            {
                speedX -= velocity;
            }
            if (key.IsKeyDown(Keys.Space))
            {
                speedY += velocity;
            }
            if (key.IsKeyDown(Keys.LeftShift))
            {
                speedY -= velocity;
            }
        }

        public void OnMouse(float DeltaX, float DeltaY)       //сюда реальные координаты мыши, а не дельта
        {
            angularX += DeltaX * sensitivity;
            angularY += DeltaY * sensitivity;
        }
```
Функция рендера `OnRender` тоже вызывается из движка. Но тут сначала высчитываются скорости камеры в данный момент времени. Сначала замедляем нашу камеру, а затем, если скорости камеры слишком малы, просто зануляем их, чтобы не мучать GPU. Далее считаются вектора `Pos`, `Target`, `Up`. 

Перемещение считается легко. Просто нужно прибавить к вектору `Pos` соответствующие скорости в заданных направлениях. А вот с другими двумя мы разберемся в функции `Update`.
```c#
        public void OnRender()
        {
            float m_speedX = speedX * (1 - brakingKeyBo);
            float m_speedY = speedY * (1 - brakingKeyBo);
            float m_speedZ = speedZ * (1 - brakingKeyBo);

            float m_angularX = angularX * (1 - brakingMouse);
            float m_angularY = angularY * (1 - brakingMouse);

            if (m_speedX < zeroLimit && m_speedX > -zeroLimit)
            {
                speedX = 0;
            }
            else
            {
                speedX = m_speedX;
            }

            if (m_speedY < zeroLimit && m_speedY > -zeroLimit)
            {
                speedY = 0;
            }
            else
            {
                speedY = m_speedY;
            }

            if (m_speedZ < zeroLimit && m_speedZ > -zeroLimit)
            {
                speedZ = 0;
            }
            else
            {
                speedZ = m_speedZ;
            }

            if (m_angularX < zeroLimit && m_angularX > -zeroLimit)
            {
                angularX = 0;
            }
            else
            {
                angularX = m_angularX;
            }

            if (m_angularY < zeroLimit && m_angularY > -zeroLimit)
            {
                angularY = 0;
            }
            else
            {
                angularY = m_angularY;
            }

            Pos += Target * speedZ;

            Left = vector3f.Cross(Target, Up);
            Left.Normalize();
            Pos += Left * speedX;

            Pos += vector3f.Up * speedY;

            angle_h += angularX;

            if (angle_v + angularY < 90 && angle_v + angularY > -90)
                angle_v += angularY;

            Update();
        }
```
Функция `Update` вызывается из `OnRender`. Эта функция обновляет значения векторов `Target` и `Up` согласно горизонтальному и вертикальному углам. Мы начинаем с вектором обзора в "сброшенном" состоянии. Это значит, что он параллелен земле (вертикальный угол равен 0) и смотрит направо (горизонтальный угол равен 0 - смотри диаграмму выше). Мы устанавливаем вертикальную ось прямо вверх и вращаем вектор направления на горизонтальный угол относительно нее. В результате получаем вектор, который, в общем то, соответствует искомому, но имеет не правильную высоту (т.к. он принадлежит плоскости XZ). Совершив векторное произведение между этим вектором и вертикальной осью, мы получаем еще один вектор на плоскости XZ, но он будет перпендикулярен плоскости, образованной вертикальным вектором и вектором направления. Это наша новая горизонтальная ось, и настал момент вращать вектор вокруг нее на вертикальный угол. Результат - итоговый вектор направления, и мы записываем его в соответствующее место в классе. Теперь нам нужно исправить вектор вверх. Например, если камера смотрит поворачивается вверх, то вектор будет откланяться назад (он обязан быть под углом в 90 градусов с вектором направления). Это схоже с тем, как вы наклоняете голову, когда смотрите на небо. Новый вектор подсчитывается просто векторным произведением итоговым вектором направления и новым вектором вправо. Если вертикальный угол снова 0, тогда вектор направления возвращается на плоскость XZ, и вектор вверх обратно (0,1,0). Если вектор направления движется вверх или вниз, то вектор вверх наклоняется вперед / назад соответственно.
```c#
        private void Update()
        {
            vector3f Vaxis = vector3f.Up;

            // Поворачиваем вектор вида на горизонтальный угол вокруг вертикальной оси
            vector3f View = vector3f.Right;
            View.Rotate(angle_h, Vaxis);
            View.Normalize();

            // Поворачиваем вектор вида на вертикальный угол вокруг горизонтальной оси
            vector3f Haxis = vector3f.Cross(Vaxis, View);
            Haxis.Normalize();
            View.Rotate(angle_v, Haxis);

            Target = View;
            Target.Normalize();

            Up = vector3f.Cross(Target, Haxis);
            Up.Normalize();
        }
```
## Вектор3
Это вектор состоящий из 3 переменных: x, y, z. 

`Cross` - метод для векторного умножения между 2 векторами. Такая операция возвращает вектор, перпендикулярный плоскости, определяемой исходными векторами. 

Для генерации матрицы UVN мы должны сделать вектора единичной длины. Метод `Normalize` заключается в том, что все компоненты вектора делятся на его длину.


```c#
    public class vector3f
    {
        public float x;
        public float y;
        public float z;

        public vector3f()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public vector3f(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static vector3f operator +(vector3f l, vector3f r)
        {
            return new vector3f(l.x + r.x, l.y + r.y, l.z + r.z);
        }

        public static vector3f operator -(vector3f l, vector3f r)
        {
            return new vector3f(l.x - r.x, l.y - r.y, l.z - r.z);
        }

        public static vector3f operator -(vector3f r)
        {
            return new vector3f(-r.x, -r.y, -r.z);
        }

        public static vector3f operator *(vector3f l, float f)
        {
            return new vector3f(l.x * f, l.y * f, l.z * f);
        }

        public static vector3f operator /(float l, vector3f f)
        {
            return new vector3f(f.x / l, f.y / l, f.z / l);
        }

        public static vector3f Transform(vector3f v, Matrix4 m)
        {
            float x = v.x * m[0, 0] + v.y * m[1, 0] + v.z * m[2, 0] + m[3, 0];
            float y = v.y * m[0, 1] + v.y * m[1, 1] + v.z * m[2, 1] + m[3, 1];
            float z = v.z * m[0, 2] + v.y * m[1, 2] + v.z * m[2, 2] + m[3, 2];
            return new vector3f(x, y, z);
        }

        public static vector3f Cross(vector3f b, vector3f v)
        {
            float _x = b.y * v.z - b.z * v.y;
            float _y = b.z * v.x - b.x * v.z;
            float _z = b.x * v.y - b.y * v.x;
            return new vector3f(_x, _y, _z);
        }

        public static vector3f Normalize(vector3f v)
        {
            float Length = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);

            v.x /= Length;
            v.y /= Length;
            v.z /= Length;

            return v;
        }

        public void Normalize()
        {
            float Length = (float)Math.Sqrt(x * x + y * y + z * z);

            x /= Length;
            y /= Length;
            z /= Length;
        }

        public void Rotate(float Angle, vector3f Axe)
        {
            float SinHalfAngle = (float)Math.Sin(math3d.ToRadian(Angle / 2));
            float CosHalfAngle = math3d.cos(math3d.ToRadian(Angle / 2));

            float Rx = Axe.x * SinHalfAngle;
            float Ry = Axe.y * SinHalfAngle;
            float Rz = Axe.z * SinHalfAngle;
            float Rw = CosHalfAngle;
            Quaternion RotationQ = new Quaternion(Rx, Ry, Rz, Rw);
            Quaternion ConjugateQ = RotationQ.Conjugate();
            Quaternion W = RotationQ * (this) * ConjugateQ;

            x = W.x;
            y = W.y;
            z = W.z;
        }

        public static vector3f Zero
        {
            get
            {
                return new vector3f { x = 0, y = 0, z = 0 };
            }
        }

        public static vector3f One
        {
            get
            {
                return new vector3f { x = 1, y = 1, z = 1 };
            }
        }

        public static vector3f Right
        {
            get
            {
                return new vector3f { x = 1, y = 0, z = 0 };
            }
        }

        public static vector3f Up
        {
            get
            {
                return new vector3f { x = 0, y = 1, z = 0 };
            }
        }

        public static vector3f Ford
        {
            get
            {
                return new vector3f { x = 0, y = 0, z = 1 };
            }
        }
  }
```
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
## UVN 
Эта функция `matrix4f` генерирует преобразования камеры, которые позднее будут использованы конвейером. Векторы U,V и N высчитываются и заносятся в ряды матрицы. Так как вектор позиции будет умножаться справа (в виде столбца), то мы получим скалярное произведение между этим вектором и векторами U,V и N. Это вычислит значения 3 скалярных проекций, которые станут XYZ значениями позиции в пространстве экрана.

Функция получает вектор направления и верхний вектор. Вектор вправо вычисляется как их векторное произведение. Заметим, что мы хотим нормировать векторы в любом случае, даже если они уже единичной длины. После генерации вектор вверх пересчитывается как векторное произведение между векторами направления и вектором вправо. Причина станет ясна позднее, когда мы начнем двигать камеру. Проще обновить только вектор направления, но тогда угол между направлением и вектором вверх не будет равен 90 градусам, что нарушит линейность системы координат. После подсчета вектора вправо и затем векторно умножив его на вектор направления, мы получим обратно вектор вверх, тем самым мы получаем систему координат, у которой угол между любыми 2 осями равен 90 градусов.
```c#
        public void InitCameraTransform(vector3f Target, vector3f Up)
        {
            vector3f N = Target;
            N = vector3f.Normalize(N);
            vector3f U = Up;
            U = vector3f.Normalize(U);
            U = vector3f.Cross(U, N);
            vector3f V = vector3f.Cross(N, U);

            m[0, 0] = U.x;      m[0, 1] = U.y;      m[0, 2] = U.z;      m[0, 3] = 0.0f;
            m[1, 0] = V.x;      m[1, 1] = V.y;      m[1, 2] = V.z;      m[1, 3] = 0.0f;
            m[2, 0] = N.x;      m[2, 1] = N.y;      m[2, 2] = N.z;      m[2, 3] = 0.0f;
            m[3, 0] = 0.0f;     m[3, 1] = 0.0f;     m[3, 2] = 0.0f;     m[3, 3] = 1.0f;

            this.Trans();
        }
```
## Pipeline
Давайте обновим функцию, генерирующую итоговую матрицу преобразований объектов. Она станет немного сложнее с 2 новыми матрицами, характеризующими участие камеры. После завершения мировых преобразований (комбинация масштабирования, вращения и перемещения объекта), мы начинаем преобразования камеры 'движением' ее обратно в начало координат. Это делается смещением на обратный вектор позиции камеры. Поэтому если камера находится в точке (1,2,3), мы двигаем объекты на (-1,-2,-3). После этого мы генерируем вращение камеры, основываясь на направлении камеры и ее векторе вверх. На этом участие камеры завершено.
```c#
        public matrix4f getMVP()
        {
            matrix4f scaleTrans = new matrix4f();
            matrix4f rotateTrans = new matrix4f();
            matrix4f translationTrans = new matrix4f();
            matrix4f PersProjTrans = new matrix4f();
            matrix4f CameraTranslation = new matrix4f();
            matrix4f CameraRotate = new matrix4f();

            scaleTrans.InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z);
            rotateTrans.Rotate(RotateVector.x, RotateVector.y, RotateVector.z);
            translationTrans.InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z);
            PersProjTrans.InitPersProjTransform(mPersProj.FOV, mPersProj.width, mPersProj.height, mPersProj.zNear, mPersProj.zFar);

            CameraTranslation.InitTranslationTransform(-CameraInfo.Pos.x, -CameraInfo.Pos.y, -CameraInfo.Pos.z);
            CameraRotate.InitCameraTransform(CameraInfo.Target, CameraInfo.Up);                       

            Transformation = scaleTrans * rotateTrans * translationTrans * CameraTranslation * CameraRotate * PersProjTrans;
            return Transformation;
        }
```
## Применение камеры
Осталось только добавить камеру в движок. Для начала объявим ее и зададим начальные координаты.
```c#
        Camera cam;
```
```c#
        cam = new Camera();
        cam.Pos = new vector3f(0, 3, 4);
```
В функции `OnUpdateFrame` мы можем отправлять камере состояния клавиатуры и мышки для управления ей. Знак `-` перед Delta, потому что с.к. вращается в противоположную сторону относительно движения мышки. 
```c#
        cam.OnMouse(-MouseState.Delta.X, -MouseState.Delta.Y);
        cam.OnKeyboard(KeyboardState);
```
В функции `OnRenderFrame` рендерим нашу камеру.
```
        cam.OnRender();
```
## Результат:
![camera](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/camera%20(1).gif)
