### Разработка графического программного обеспечения для визуализации и работы с трехмерными объектами
ст. Галеев Тимур, гр. 3530203/00102 (летняя практика + НИР1 + НИР2 + преддипломная практика + ВКР)
# Оглавление
[1. Первые шаги и треугольник](#s1)  
[2. Индексная отрисовка](#s3)  
[3. Интерполяционный цвет](#s4)  
[4. Вращение, перемещение, масштаб](#s5)  
[5. Добавим класс GameObj](#s6)  
[6. Разные форматы файлов](#s7)  
[7. Камера](#s8)  
[8. Текстуры](#s9)  
[9. Нормали](#s10)  
[10. Извлечение текстур и нормалей из obj](#s11)  
[11. DeltaTime](#s12)  
[12. Скайбокс и прицел](#s13)  
[13. Загрузка моделей через Assimp](#s14)  
[14. Информационная панель](#s15)  
[15. Свет: окружающее освещение, рассеянное освещение, отраженный свет](#s16)  
[16. Свет: Point Light](#s17)  
[17. Свет: Spot Light](#s18)  
[18. Карты нормали (Normal Map)](#s19)  
[19. Карта отражения (Specular Map)](#s20)  
[20. Тени (Shadow Mapping)](#s21)  
[21. PCF](#s22)  
[22. Shadow Mapping для нескольких источников света](#s23)  
[23. 3D выбор](#s24)  
[24. Компилятор](#s25)  

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
<a name = "s9"></a>
# Текстуры
Diffuse map – карта диффузного цвета пикселей трехмерного объекта. Нормированные координаты текстур UV каждой вершины указывают позицию XY текселя на изображении текстуры. То есть, например, для вершины с координатой текстуры UV = (0,5; 0,5) позиция текселя на изображении текстуры размером 1024х2048 будет равна XY = (0,5 * 1024; 0,5 * 2048) = (512; 1024). Координаты UV всегда находятся в диапазоне от 0 до 1 и связаны не только с диффузной картой, но и с картой нормали и картой отражения.  
Некоторые библиотеки загружают изображения, начиная с верхнего левого пикселя, в то время как OpenGL загружает, начиная с нижнего левого. Во время работы с текстурами следует это учесть, иначе они переворачиваются по вертикали.  
Во время преобразований объекта в мировом пространстве координаты его текстур остаются неизменными и прикреплены к вершинам.  
![image](https://github.com/user-attachments/assets/3d4ceea3-a23e-4a7a-aa48-6e597703c557)  
![image](https://github.com/user-attachments/assets/d467fffc-16a1-4a11-bd00-5c7ae669c5de)
Во время рендеринга может возникнуть ситуация, когда координаты текстуры UV указывают на тексел в XY = (223,23; 65,8). Метод, который выбирает итоговый тексел, является фильтрацией. Два самых распространенных метода фильтрации являются ближайшая фильтрация и линейная фильтрация. При ближайшей фильтрации координаты текселя округляются: (223; 66). А при линейной берутся соседние тексели ((223, 66), (223, 65), (224, 66) и (224, 65)) и над ними совершается линейная интерполяция между их цветами, сохраняя относительное расстояние между исходным текселем и каждым соседним текселем. Разница между двумя подходами заключается в более мягком отображении текстур.  
![image](https://github.com/user-attachments/assets/0ec1c1e7-e4f9-40ec-b42f-2a889de98758)
Итоговый цвет пикселя равен цвету текселя умноженному на результирующий цвет освещения.  
![image](https://github.com/user-attachments/assets/8806a11a-51f2-4e2a-88f5-6eecc96034a6)
## Класс Texture
В методе загрузки текстуры `Load` первым делом создаем пустую текстуру `Handle` для нашего использования.  
`stb_image` загружается с верхнего левого пикселя, в то время как OpenGL загружается с нижнего левого, в результате чего текстура переворачивается по вертикали. Функция `stbi_set_flip_vertically_on_load` исправит это недоразумение, заставив текстуру отображаться должным образом.  
Далее мы читаем файл, загружаем изображение и вводим значения параметров для `TexImage2D`.  
Потом с помощью функции `TexParameter` мы настраеваем параметры текстуры: перенос и фильтрацию.  
После того, как текстура была создана, генерируем коллекцию этой текстуры в формате mipmapped с помощью функции `GenerateMipmap`. Mipmaps используются при изменении расстояния до объектов. MIP-карта с более высоким разрешением используется для объектов, которые находятся ближе, а MIP-карта с более низким разрешением используется для объектов, которые находятся дальше. Коллекция начинается с разрешения изображения текстуры и уменьшает разрешение вдвое, пока не будет создано изображение текстуры размера 1x1.
```c#
        private readonly int Handle;

        private Texture(int glHandle)
        {
            Handle = glHandle;
        }

        public static Texture Load(string file_name)
        {
            int handle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, handle);

            StbImage.stbi_set_flip_vertically_on_load(1);

            using (Stream stream = File.OpenRead(file_name))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(
                    TextureTarget.Texture2D,    //Тип создаваемой текстуры
                    0,                          //Уровень детализации
                    PixelInternalFormat.Rgba,   //Формат для хранения пикселей на графическом процессоре
                    image.Width,                //Ширина изображения
                    image.Height,               //Высота изображения
                    0,                          //Граница изображения
                    PixelFormat.Rgba,           //Формат байтов   
                    PixelType.UnsignedByte,     //Тип пикселей
                    image.Data);                //Массив пикселей
            }

            //Фильтрация
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);    
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Перенос
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //генерируем коллекцию mipmapped
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new Texture(handle);
        }
```
Функция применения текстуры:  
```c#
        public void Use(TextureUnit TextureUnit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(TextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
```
## Добавление текстуры в Mesh
Здесь нам нужно будет изменить положения атрибутов вершин, чтобы отправлять координаты текстуры шейдерам. В функции `Load` изменим код на:  
```c#
        private void Load()
        {
            // Генерация и привязка VAO и VBO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            // Привязываем данные вершины к текущему буферу по умолчанию
            // Static Draw, потому что наши данные о вершинах в буфере не меняются
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            // Element Buffer
            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(int), Indices, BufferUsageHint.StaticDraw);

            // Шейдеры
            shader = new Shader(ShaderLoader.LoadVertexShader(), ShaderLoader.LoadFragmentShader());

            // Устанавливаем указатели атрибутов вершины
            var location = shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            var texCordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 5  * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCordLocation);

            // Текстуры
            texture = Texture.Load(texPath);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.DepthTest);
        }
```
Также в функцию `Draw` добавим:
```c#
            texture.Use(TextureUnit.Texture0);
```
## Шейдеры
В вершинный шейдер поступают еше и координаты текстур для каждой вершины:
```hlsl
#version 330                                           
layout (location = 0) in vec3 aPosition; 
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 mvp;         

void main()                                            
{        
	texCoord = aTexCoord;
	gl_Position = vec4(aPosition, 1.0) * mvp;           
}
```
И тут же отправляются во фрагментный шейдер:
```hlsl
#version 330
out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main() 
{ 
	outputColor = texture(texture0, texCoord);
}
```
## Модели
Чуть позже я изменю загрузчик 3D-моделей, чтобы вытягивать из них координаты текстур и нормалей. А пока создадим свои простые отечественные модели: пол и ящик. В них будут координаты вершин, индексы и текстуры.
```c#
    public static class Box
    {
        public static readonly float[] Vertices = new float[]
        {           //cords                 //textures
                    -1.0f, 1.0f, 1.0f,      0.0f, 1.0f,
                    1.0f, 1.0f, 1.0f,       1.0f, 1.0f,
                    -1.0f, -1.0f, 1.0f,     0.0f, 0.0f,
                    1.0f, -1.0f, 1.0f,      1.0f, 0.0f,

                    1.0f, 1.0f, 1.0f,       0.0f, 1.0f,
                    1.0f, 1.0f, -1.0f,      1.0f, 1.0f,
                    1.0f, -1.0f, 1.0f,      0.0f, 0.0f,
                    1.0f, -1.0f, -1.0f,     1.0f, 0.0f,

                    1.0f, 1.0f, -1.0f,      0.0f, 1.0f,
                    -1.0f, 1.0f, -1.0f,     1.0f, 1.0f,
                    1.0f, -1.0f, -1.0f,     0.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,    1.0f, 0.0f,

                    -1.0f, 1.0f, -1.0f,     0.0f, 1.0f,
                    -1.0f, 1.0f, 1.0f,      1.0f, 1.0f,
                    -1.0f, -1.0f, -1.0f,    0.0f, 0.0f,
                    -1.0f, -1.0f, 1.0f,     1.0f, 0.0f,

                    -1.0f, 1.0f, -1.0f,     0.0f, 1.0f,
                    1.0f, 1.0f, -1.0f,      1.0f, 1.0f,
                    -1.0f, 1.0f, 1.0f,      0.0f, 0.0f,
                    1.0f, 1.0f, 1.0f,       1.0f, 0.0f,

                    -1.0f, -1.0f, 1.0f,     0.0f, 1.0f,
                    1.0f, -1.0f, 1.0f,      1.0f, 1.0f,
                    -1.0f, -1.0f, -1.0f,    0.0f, 0.0f,
                    1.0f, -1.0f, -1.0f,     1.0f, 0.0f,
        };

        public static readonly int[] Indices = new int[]
        {
                        9, 11, 8,
                        8, 11, 10,

                        1, 3, 0,
                        0, 3, 2,

                        5, 7, 4,
                        4, 7, 6,

                        13, 15, 12,
                        12, 15, 14,

                        22, 20, 23,
                        23, 20, 21,

                        17, 19, 16,
                        16, 19, 18
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\container.png";
    }
```
```c#
    public class Floor
    {
        public static readonly float[] Vertices = new float[]
        {   //cords    //textures
            -3, 0, -3,  0,  0,
             3, 0, -3,  1,  0,
             3, 0,  3,  1,  1,
            -3, 0,  3,  0,  1
        };

        public static readonly int[] Indices = new int[]
        {
            0, 1, 2,
            0, 2, 3
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\grass.png";
    }
```
## Результат
![tex](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/textures.png)

<a name = "s10"></a>
# Нормали
Если заглянуть через блокнот в файл 3D-модели формата `.obj`, то мы увидим, что, помимо координат вершин `v` и текстур `vt`, у объекта есть еще и вектора нормалей `vn`. Нормали - это такие нормализованные векторы, перпендикулярные плоскости примитивных треугольников.  
Файл модели куба:
```obj
v -0.500000 0.500000 0.500000
v -0.500000 -0.500000 0.500000
v -0.500000 0.500000 -0.500000
...
v 0.500000 0.500000 0.500000
v -0.500000 0.500000 -0.500000
v 0.500000 0.500000 -0.500000
vt 0.056302 0.934555
vt 0.308284 0.682573
vt 0.308284 0.934555
...
vt 0.373878 0.929480
vt 0.356402 0.304927
vt 0.696564 0.671249
vn -1.0000 0.0000 0.0000
vn 1.0000 0.0000 0.0000
vn 0.0000 -0.0000 1.0000
vn 0.0000 0.0000 -1.0000
vn 0.0000 -1.0000 -0.0000
vn 0.0000 1.0000 0.0000
s off
f 3/1/1 2/2/1 1/3/1
f 6/4/2 7/5/2 5/6/2
f 10/7/3 11/8/3 9/9/3
...
f 15/10/4 16/22/4 14/11/4
f 19/13/5 20/23/5 18/14/5
f 22/16/6 24/24/6 23/17/6
```
Если мы взгялнем на поверхности (строки, начинающиеся с `f`), то мы увидим, что у каждого примитива (в нашем случае треугольника) есть по три вершины, а у каждой вершины есть по 3 индекса: _координата вершины_/_координата текстуры_/_вектор нормали_. Нормаль есть у каждой вершины. Она перпендикулярна примитиву и нормализована, то есть длина вектора нормали равна 1.  
Когда свет попадает на полигон, угол светового луча сравнивается с нормалью. При отражении используется тот же угол относительно нормали.
## Добавление нормалей в модели
Давайте добавим эти нормальные векторы к ящику и полу, которые мы создал в прошлом разделе. Индексы мы оставим без измнения.
### Ящик:
```c#
        public static readonly float[] Vertices = new float[]
        {           //cords                 //textures      //normals
                    -1.0f, 1.0f, 1.0f,      0.0f, 1.0f,     0.0f, 0.0f, 1.0f,
                    1.0f, 1.0f, 1.0f,       1.0f, 1.0f,     0.0f, 0.0f, 1.0f,
                    -1.0f, -1.0f, 1.0f,     0.0f, 0.0f,     0.0f, 0.0f, 1.0f,
                    1.0f, -1.0f, 1.0f,      1.0f, 0.0f,     0.0f, 0.0f, 1.0f,

                    1.0f, 1.0f, 1.0f,       0.0f, 1.0f,     1.0f, 0.0f, 0.0f,
                    1.0f, 1.0f, -1.0f,      1.0f, 1.0f,     1.0f, 0.0f, 0.0f,
                    1.0f, -1.0f, 1.0f,      0.0f, 0.0f,     1.0f, 0.0f, 0.0f,
                    1.0f, -1.0f, -1.0f,     1.0f, 0.0f,     1.0f, 0.0f, 0.0f,

                    1.0f, 1.0f, -1.0f,      0.0f, 1.0f,     0.0f, 0.0f, -1.0f,
                    -1.0f, 1.0f, -1.0f,     1.0f, 1.0f,     0.0f, 0.0f, -1.0f,
                    1.0f, -1.0f, -1.0f,     0.0f, 0.0f,     0.0f, 0.0f, -1.0f,
                    -1.0f, -1.0f, -1.0f,    1.0f, 0.0f,     0.0f, 0.0f, -1.0f,

                    -1.0f, 1.0f, -1.0f,     0.0f, 1.0f,     -1.0f, 0.0f, 0.0f,
                    -1.0f, 1.0f, 1.0f,      1.0f, 1.0f,     -1.0f, 0.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,    0.0f, 0.0f,     -1.0f, 0.0f, 0.0f,
                    -1.0f, -1.0f, 1.0f,     1.0f, 0.0f,     -1.0f, 0.0f, 0.0f,

                    -1.0f, 1.0f, -1.0f,     0.0f, 1.0f,     0.0f, 1.0f, 0.0f,
                    1.0f, 1.0f, -1.0f,      1.0f, 1.0f,     0.0f, 1.0f, 0.0f,
                    -1.0f, 1.0f, 1.0f,      0.0f, 0.0f,     0.0f, 1.0f, 0.0f,
                    1.0f, 1.0f, 1.0f,       1.0f, 0.0f,     0.0f, 1.0f, 0.0f,

                    -1.0f, -1.0f, 1.0f,     0.0f, 1.0f,     0.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, 1.0f,      1.0f, 1.0f,     0.0f, -1.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,    0.0f, 0.0f,     0.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, -1.0f,     1.0f, 0.0f,     0.0f, -1.0f, 0.0f,
        };
```
### Пол:
```c#
        public static readonly float[] Vertices = new float[]
        {   //cords     	//textures  	//normals
            -3, 0, -3,  	0,  0,      	0, 1, 0,
             3, 0, -3,  	1,  0,      	0, 1, 0,
             3, 0,  3,  	1,  1,      	0, 1, 0,
            -3, 0,  3,  	0,  1,      	0, 1, 0,
        };
```
## Mesh
Теперь изменим код в функции `Init` класса `Mesh` на:
```c#
            // Устанавливаем указатели атрибутов вершины
            var location = shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            var texCordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 8  * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCordLocation);

            var normCordLocation = shader.GetAttribLocation("aNormal");
            GL.VertexAttribPointer(normCordLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(normCordLocation);
```
Здесь мы отправляем наши координаты вершин, текстур и нормали в шейдеры.
## Шейдеры
### Вершинный шейдер
В вершинный шейдер теперь попадают нормали, и перед тем, как отправиться во фрагментный шейдер, они умножаются на `mvp`, потому что наши объекты не стоят на месте, а масштабируются, крутятся и перемещаются.
```hlsl
#version 330                                           
layout (location = 0) in vec3 aPosition; 
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
 
out vec2 texCoord;
out vec3 vNormal;

uniform mat4 mvp;
uniform mat4 campos;
uniform mat4 camrot;
uniform mat4 pers;

void main()                                            
{        
	texCoord = aTexCoord;
	gl_Position = vec4(aPosition, 1.0) * mvp * campos * camrot * pers;
	vNormal = (vec4(aNormal, 0.0) * mvp).xyz;
}
```
### Фрашментный шейдер
Сюда мы добавим окружающий свет с интенсивностью `ambientLightIntensity` и свет от "солнца" с интенсивностью `sunLightIntensity` и направлением `sunLightDirection`. Используя наши нормали и параметры света, мы можем определить насколько темным или светлым будет пиксель в данной точке у объекта. Результирующее значение интенсивности света `lightIntensity` мы умножаем на `texel.rgb`.  
```hlsl
#version 330
out vec4 outputColor;

in vec2 texCoord;
in vec3 vNormal;

uniform sampler2D texture0;

void main() 
{ 
    	vec3 ambientLightIntensity = vec3(0.3, 0.3, 0.3);
    	vec3 sunLightIntensity = vec3(1, 1, 1);
    	vec3 sunLightDirection = normalize(vec3(-20, 20, 20.0));

    	vec4 texel = texture(texture0, texCoord);

    	vec3 lightIntensity = ambientLightIntensity + sunLightIntensity * max(dot(vNormal, sunLightDirection), 0.0f);

	outputColor = vec4(texel.rgb * lightIntensity, texel.a);
}             
```
Это самая простая реализация света, но в будущем мы займемся им более основательно.
## Результат
![normals](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/bandicam%202023-07-21%2002-51-44-481.gif)
<a name="s11"></a>
# Извлечение текстур и нормалей из obj
## Изменение ModelLoader
Теперь в функции извлечения вершин из obj мы сами собираем массивы `_modelVerts` и `_modelInd`.
```c#
        private static void LoadFromObj(TextReader tr, ref float[] Vertices, ref int[] Indices)
        {
            List<float> _modelVerts = new List<float>();
            List<int> _modelInd = new List<int>();
            int _iter = 0;

            List<List<float>> vertCords = new List<List<float>>();

            List<List<float>> textCords = new List<List<float>>();

            List<List<float>> normCords = new List<List<float>>();

            int[] vertInd, texInd, normInd;

            string line;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace("  ", " ");
                var parts = line.Split(' ');

                if (parts.Length == 0) continue;
                switch (parts[0])
                {
                    case "v":
                        vertCords.Add(new List<float>
                        {
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        });
                        break;
                    case "vt":
                        textCords.Add(new List<float>
                        {
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture)
                        });
                        break;
                    case "vn":
                        normCords.Add(new List<float>
                        {
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        });
                        break;
                    case "f":
                        if (vertCords.Count == 0 || textCords.Count == 0 || normCords.Count == 0)
                        {
                            Console.WriteLine("Wrong model");
                            return;
                        }

                        List<string> fString = new List<string>();
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (parts[i] != "" && parts[i] != null && parts[i] != "f") fString.Add(parts[i]);
                        }

                        if (fString.Count == 3)
                        {
                            vertInd = new int[3];
                            texInd = new int[3];
                            normInd = new int[3];
                            int _i = 0;

                            foreach (string v in fString)
                            {
                                string[] w = v.Split('/');

                                vertInd[_i] = int.Parse(w[0]) - 1;
                                texInd[_i] = int.Parse(w[1]) - 1;
                                normInd[_i] = int.Parse(w[2]) - 1;
                                _i++;
                            }

                            for (int i = 0; i < 3; i++)
                            {
                                foreach (float v in vertCords[vertInd[i]])
                                {
                                    _modelVerts.Add(v);
                                }
                                foreach (float t in textCords[texInd[i]])
                                {
                                    _modelVerts.Add(t);
                                }
                                foreach (float n in normCords[normInd[i]])
                                {
                                    _modelVerts.Add(n);
                                }
                                _modelInd.Add(_iter++);
                            }                            
                        }
                        if (fString.Count == 4)
                        {
                            vertInd = new int[4];
                            texInd = new int[4];
                            normInd = new int[4];
                            int _i = 0;

                            foreach (string v in parts)
                            {
                                string[] w = v.Split('/');

                                vertInd[_i] = int.Parse(w[0]) - 1;
                                texInd[_i] = int.Parse(w[1]) - 1;
                                normInd[_i] = int.Parse(w[2]) - 1;
                                _i++;
                            }

                            for (int i = 0; i < 4; i++)
                            {
                                foreach (float v in vertCords[vertInd[i]])
                                {
                                    _modelVerts.Add(v);
                                }
                                foreach (float t in textCords[texInd[i]])
                                {
                                    _modelVerts.Add(t);
                                }
                                foreach (float n in normCords[normInd[i]])
                                {
                                    _modelVerts.Add(n);
                                }
                            }

                            int i0 = _iter++;
                            int i1 = _iter++;
                            int i2 = _iter++;
                            int i3 = _iter++;

                            _modelInd.Add(i0);
                            _modelInd.Add(i2);
                            _modelInd.Add(i3);

                            _modelInd.Add(i0);
                            _modelInd.Add(i1);
                            _modelInd.Add(i2);
                        }
                        break;
                }
            }

            Vertices = _modelVerts.ToArray();
            Indices = _modelInd.ToArray();
        }
```
## Результат
![monkey](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/обезьяна.gif)
<a name="s12"></a>
# DeltaTime
Когда в нашей игре стабильно 60 FPS, у нас за секунду обновляются 60 кадров, то есть 60 раз в секунду вызываются функции `OnUpdateFrame` и `OnRenderFrame`. Если мы с 60 FPS в функции рендера будем вызывать метод `obj.Move(0.1f, 0.0f, 0.0f)`, отвечающий за передвижение по оси X какого-то объекта `obj` на 0.1f, то за 60 кадров в секунду объект переместится на 6 блоков.  
Перемещение на 6 блоков в секунду выполняется при условии, что у нас стабильно 60 FPS. Но что если наш FPS упадет до 30? Тогда наш объект будет перемещаться только на 3 блока в секунду. Учитывая, что в играх FPS может постоянно скакать, предсказать перемещение объекта будет невозможно.  
В OpenTK есть значение DeltaTime, которое является аргументом функций `OnUpdateFrame` и `OnRenderFrame`. DeltaTime - это интервал в секундах от последнего кадра до текущего. Его мы и будем использовать для решения этой проблемы.  
Чтобы наш объект передвигался со стабильной скоростью 10 блоков в секунду, мы просто будем в функции рендера двигать его на `ObjectSpeed * DeltaTime`. Тогда, сколько бы у нас не было FPS, объект будет двигаться со стабильной скоростью ObjectSpeed = 10 блоков/с. 
## В Pipeline
Добавим функции, в которых к текущим значениям прибавляются скорости, помноженные на время.
```c#
        // Вращать

        public void Rotate(float speedX, float speedY, float speedZ, float time)
        {
            RotateVector.x += speedX * time;
            RotateVector.y += speedY * time;
            RotateVector.z += speedZ * time;
        }

        // Передвижение

        public void Move(float speedX, float speedY, float speedZ, float time)
        {
            PositionVector.x += speedX * time;
            PositionVector.y += speedY * time;
            PositionVector.z += speedZ * time;
        }

        public void MoveX(float speedX, float time)
        {
            PositionVector.x += speedX * time;
        }

        public void MoveY(float speedY, float time)
        {
            PositionVector.y += speedY * time;
        }

        public void MoveZ(float speedZ, float time)
        {
            PositionVector.z += speedZ * time;
        }

        // Увеличение

        public void Expand(float speedX, float speedY, float speedZ, float time)
        {
            ScaleVector.x += speedX * time;
            ScaleVector.y += speedY * time;
            ScaleVector.z += speedZ * time;
        }

        public void Expand(float speed, float time)
        {
            ScaleVector.x += speed * time;
            ScaleVector.y += speed * time;
            ScaleVector.z += speed * time;
        }
```
## В Camera
Приращение скоростей передвижения и вращения камеры и их постепенное торможение тоже теперь зависят от DeltaTime.
```c#
        public static void OnRender(float deltaTime)
        {
            if (!inited) return;
            Braking(deltaTime);

            Pos += Target * speedZ * deltaTime;

            Left = vector3f.Cross(Target, Up);
            Left.Normalize();
            Pos += Left * speedX * deltaTime;

            Pos += vector3f.Up * speedY * deltaTime;

            angle_h += angularX * deltaTime;

            if (angle_v + angularY * deltaTime < 90 && angle_v + angularY * deltaTime > -90)
                angle_v += angularY * deltaTime;

            Update();

            CameraTranslation.InitTranslationTransform(-Pos.x, -Pos.y, -Pos.z);
            CameraRotation.InitCameraTransform(Target, Up);
        }

        private static void Braking(float deltaTime)
        {
            float m_speedX = speedX * (1 - brakingKeyBo * deltaTime);
            float m_speedY = speedY * (1 - brakingKeyBo * deltaTime);
            float m_speedZ = speedZ * (1 - brakingKeyBo * deltaTime);

            float m_angularX = angularX * (1 - brakingMouse * deltaTime);
            float m_angularY = angularY * (1 - brakingMouse * deltaTime);

            if (m_speedX < min_speed && m_speedX > -min_speed)
            {
                speedX = 0;
            }
            else
            {
                speedX = m_speedX;
            }

            if (m_speedY < min_speed && m_speedY > -min_speed)
            {
                speedY = 0;
            }
            else
            {
                speedY = m_speedY;
            }

            if (m_speedZ < min_speed && m_speedZ > -min_speed)
            {
                speedZ = 0;
            }
            else
            {
                speedZ = m_speedZ;
            }

            if (m_angularX < min_speed && m_angularX > -min_speed)
            {
                angularX = 0;
            }
            else
            {
                angularX = m_angularX;
            }

            if (m_angularY < min_speed && m_angularY > -min_speed)
            {
                angularY = 0;
            }
            else
            {
                angularY = m_angularY;
            }
        }
```
## Объекты
Чтобы теперь двигать, вращать и увеличивать объекты, зададим сначала их постоянные скорости. А потом в методе рендера класса массива объектов будем вызывать уже знакомые функции - вращение обезьянки `this[0]` со скоростью 90 градусов в секунду, перемещение кубика `this[10]` со скоростью 10 блоков в секунду и увеличение мужика `this[11]` со скоростью, зависящей от времени.
```c#
        float cube_speed = 10;
        float monkey_rotationSpeed = 90;

        public void OnRender(float deltaTime)
        {
            this[0].pipeline.Rotate(0, monkey_rotationSpeed, 0, deltaTime);

            if (this[10].pipeline.PosZ == -6f && this[10].pipeline.PosX + 0.01f <= 6f)
            {
                this[10].pipeline.MoveX(cube_speed, deltaTime);
                if (this[10].pipeline.PosX >= 5.9f && this[10].pipeline.PosX <= 6f)
                    this[10].pipeline.SetPositionX(6f);
            }
            else if (this[10].pipeline.PosX == 6f && this[10].pipeline.PosZ + 0.01f <= 6f)
            {
                this[10].pipeline.MoveZ(cube_speed, deltaTime);
                if (this[10].pipeline.PosZ >= 5.9f && this[10].pipeline.PosZ <= 6f) 
                    this[10].pipeline.SetPositionZ(6f);
            }
            else if (this[10].pipeline.PosZ == 6f && this[10].pipeline.PosX - 0.01f >= -6f)
            {
                this[10].pipeline.MoveX(-cube_speed, deltaTime);
                if (this[10].pipeline.PosX <= -5.9f && this[10].pipeline.PosX >= -6f)
                    this[10].pipeline.SetPositionX(-6f);
            }
            else if (this[10].pipeline.PosX == -6f && this[10].pipeline.PosZ - 0.01f >= -6f)
            {
                this[10].pipeline.MoveZ(-cube_speed, deltaTime);
                if (this[10].pipeline.PosZ <= -5.9f && this[10].pipeline.PosZ >= -6f)
                    this[10].pipeline.SetPositionZ(-6f);
            }

            counter += deltaTime;
            this[11].pipeline.Expand(math3d.sin((float)counter), deltaTime);
        }
```
## При обновлении кадра
В `OnRenderFrame`:
```c#
            Camera.OnRender((float)args.Time);
            Models.OnRender((float)args.Time);
```
## Результат
![dt](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/DeltaTimeScene%20(online-video-cutter.com).gif)
<a name="s13"></a>
# Скайбокс и прицел
## FixBugs
Добавляя в проект скайбокс и прицел (и отдельные шейдеры для них), я столкнулся с проблемой прорисовки объектов. Дело в том, что несколько объектов, отрисовываясь, могут поменяться местами, текстурами, шейдерами или вообще не отрисоваться. Чтобы исправить этот баг, нужно изменить функцию отрисовки объектов, то есть соблюсти порядок вызовов методов в ней.  
Вот в таком порядке (ну или не совсем) вы должны отрисовывать объект:  

_BindTexture должен быть перед DrawElements:_  
`ActiveTexture` -> `BindTexture` ->  
_BindVertexArray должен быть перед DrawElements:_  
-> `BindVertexArray` ->   
_UseProgram и передача юниформов перед DrawElements:_  
-> `UseProgram` -> `UniformMatrix4` ->  
_DrawElements должен быть в конце:_  
-> `DrawElements`  
## Скайбокс
**Скайбокс** - это такой же объект в форме большого куба, позиция которого всегда равна позиции камеры.
```c#
    public class Skybox : GameObj
    {
        public Skybox()
        {
            mesh = new SkyboxMesh();
            pipeline = new Pipeline();

            pipeline.SetPosition(Camera.Pos.x, Camera.Pos.y, Camera.Pos.z);
            pipeline.SetAngle(0, 0, 0);
            pipeline.SetScale(55);
        }

        public override void Draw()
        {
            pipeline.SetPosition(Camera.Pos.x, Camera.Pos.y, Camera.Pos.z);
            mesh.Draw(pipeline.getMVP().ToOpenTK());
        }
    }
```
Для скайбокса создадим для него еще один mesh-потомок от обычного mesh. Mesh скайбокса не имеет нормалей и имеет свои шейдеры, более простые, которые мы создвали ранее.
```c#
    public class SkyboxMesh : Mesh
    {
        public SkyboxMesh()
        {
            Vertices = SkyboxVertices.Vertices;
            Indices = SkyboxVertices.Indices;
            texture_file_name = SkyboxVertices.TexturePath;

            Load();
        }

        protected override void Load()
        {
            // Генерация и привязка VAO и VBO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            // Привязываем данные вершины к текущему буферу по умолчанию
            // Static Draw, потому что наши данные о вершинах в буфере не меняются
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            // Element Buffer
            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(int), Indices, BufferUsageHint.StaticDraw);

            // Шейдеры
            shader = new Shader(
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\Skybox\\SkyboxVetexShader.hlsl"), 
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\Skybox\\SkyboxFragShader.hlsl"));

            // Устанавливаем указатели атрибутов вершины
            var location = shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            var texCordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCordLocation);

            texture = Texture.Load(texture_file_name);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.DepthTest);
        }

        public override void Draw(Matrix4 matrix)
        {
            base.Draw(matrix);
        }
    }
```
```hlsl
#version 330                                           
layout (location = 0) in vec3 aPosition; 
layout (location = 1) in vec2 aTexCoord;
 
out vec2 texCoord;

uniform mat4 mvp;
uniform mat4 campos;
uniform mat4 camrot;
uniform mat4 pers;

void main()                                            
{        
	texCoord = aTexCoord;
	gl_Position = vec4(aPosition, 1.0) * mvp * campos * camrot * pers;
}
```
```hlsl
#version 330
out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main() 
{ 
	vec4 tex = texture(texture0, texCoord);
	vec3 light = vec3(0.8, 0.8, 0.8);
	outputColor = vec4(tex.rgb * light, tex.a);
}
```
В векторе `light` мы можем настроить яркость скайбокса. 
```c#
    public static class SkyboxVertices
    {
        public static readonly float[] Vertices = new float[]
        {           //cords                 //textures    
                    -1.0f,  1.0f,  1.0f,     1.0f,  2f / 3f,        //ближний!
                     1.0f,  1.0f,  1.0f,     0.75f, 2f / 3f,  
                    -1.0f, -1.0f,  1.0f,     1.0f,  1f / 3f, 
                     1.0f, -1.0f,  1.0f,     0.75f, 1f / 3f,

                     1.0f,  1.0f,  1.0f,     0.75f, 2f / 3f,       //правый!
                     1.0f,  1.0f, -1.0f,     0.5f,  2f / 3f,
                     1.0f, -1.0f,  1.0f,     0.75f, 1f / 3f, 
                     1.0f, -1.0f, -1.0f,     0.5f,  1f / 3f,

                     1.0f,  1.0f, -1.0f,     0.5f,  2f / 3f,        //дальний!
                    -1.0f,  1.0f, -1.0f,     0.25f, 2f / 3f, 
                     1.0f, -1.0f, -1.0f,     0.5f,  1f / 3f, 
                    -1.0f, -1.0f, -1.0f,     0.25f, 1f / 3f,

                    -1.0f,  1.0f, -1.0f,     0.25f, 2f / 3f,       //левый!
                    -1.0f,  1.0f,  1.0f,     0.0f,  2f / 3f, 
                    -1.0f, -1.0f, -1.0f,     0.25f, 1f / 3f, 
                    -1.0f, -1.0f,  1.0f,     0.0f,  1f / 3f, 

                    -1.0f,  1.0f, -1.0f,     0.25f, 2f / 3f,       //верхний!
                     1.0f,  1.0f, -1.0f,     0.5f,  2f / 3f,
                    -1.0f,  1.0f,  1.0f,     0.25f, 1.0f,
                     1.0f,  1.0f,  1.0f,     0.5f,  1.0f,   

                    -1.0f, -1.0f,  1.0f,     0.25f, 0.0f,            //нижний!
                     1.0f, -1.0f,  1.0f,     0.5f,  0.0f,  
                    -1.0f, -1.0f, -1.0f,     0.25f, 1f / 3f, 
                     1.0f, -1.0f, -1.0f,     0.5f,  1f / 3f, 
        };

        public static readonly int[] Indices = new int[]
        {
                        9, 11, 8,
                        8, 11, 10,

                        1, 3, 0,
                        0, 3, 2,

                        5, 7, 4,
                        4, 7, 6,

                        13, 15, 12,
                        12, 15, 14,

                        22, 20, 23,
                        23, 20, 21,

                        17, 19, 16,
                        16, 19, 18
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\sky1.jpeg";
    }
```
## Прицел
**Прицел** - это тоже обычный 2D объект, который просто всегда находится в центре экрана. То есть перемещение и вращение камеры на него не влияют.
```с#
    public class Aim : GameObj
    {
        public Aim()
        {
            mesh = new AimMesh();
            pipeline = new Pipeline();

            pipeline.SetPosition(0, 0, -1);
            pipeline.SetScale(0.02f);
        }
    }
```
Шейдеры и функция загрузки Mesh прицела такие же, как у скайбокса, потому что ему не нужны нормали. Но проекция перспетивы для прицела другая: прицел не должен никогда пропадать за текстурами других объектов.  
В `Draw` вместо матриц камеры мы передаем в шейдеры единичные матрицы.
```c#
    public class AimMesh : SkyboxMesh
    {
        public AimMesh()
        {
            Vertices = AimVertices.Vertices;
            Indices = AimVertices.Indices;
            texture_file_name =  AimVertices.TexturePath ;
            pers_proj = pers_mat();

            Load();
        }

        public override void Draw(Matrix4 matrix)
        {
	    texture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            shader.setMatrices(matrix, Matrix4.Identity, Matrix4.Identity, pers_proj);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private Matrix4 pers_proj;

        private Matrix4 pers_mat()
        {
            float FOV = 50;
            float width = 1920;
            float height = 1080;
            float zNear = 1f;
            float zFar = 200;

            matrix4f pers = new matrix4f();
            pers.InitPersProjTransform(FOV, width, height, zNear, zFar);

            return pers.ToOpenTK();
        }
    }
```
```c#
    public static class AimVertices
    {
        public static readonly float[] Vertices = new float[]
        {   //cords             //textures
            -0.1f,  0.5f, 0,    0, 1,   
             0.1f,  0.5f, 0,    1, 1,
             0.1f, -0.5f, 0,    1, 0,
            -0.1f, -0.5f, 0,    0, 0,

             0.5f,  0.1f, 0,    0, 1,
             0.5f, -0.1f, 0,    1, 1,
            -0.5f, -0.1f, 0,    1, 0,
            -0.5f,  0.1f, 0,    0, 0,
        };

        public static readonly int[] Indices = new int[]
        {
            0, 1, 3,
            1, 2, 3,

            4, 5, 7,
            5, 6, 7
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\ain.png";
    }
```
## Результат
![skb](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/skybox.gif)
<a name="s14"></a>
# Загрузка моделей через Assimp
До сих пор мы загружали 3D-объекты "вручную", строчка за строчкой читая файлы моделей. Для каждого поддерживаемого формата мы делали собственный загрузчик. Однако это не очень оптимально, потому что, если мы захотим добавить объект нового формата, нам надо будет написать новый загрузчик для него. Также, загружая объекты не с одной, а с несколькими текстурами, нам нужно разбивать модель на мини-модели, каждой из которых присваивать нужную текстуру. 

*Разработка своего загрузчика может занять довольно много времени. Если вы хотите, что бы была возможность загружать модели из различных источников, то потребуется изучить каждый формат и написать для каждого свой загрузчик. Некоторые форматы простые, но от некоторых идет пар из ушей, и на них уйдет масса времени, причем это не является целью 3D программирования. (c)OGLDev*

Теперь новый метод загрузки моделей - это использование внешней библиотеки для разбора и загрузки моделей из файла.

## Vertex
Вершина будет представлять из себя структуру, состоящую из позиции, нормали, текстуры и тд. 
```c#
    public struct Vertex
    {
        public Vector3 Pos;
        public Vector3 Normal;
        public Vector2 Tex;
        public Vector3 Tangent;
        public Vector3 Bitangent;
    }
```
##  MeshEntry
Создадим класс мини-моделей, на которые наша модель будет разбираться. Она будет хранить в себе VBO, VAO и IBO, а также пути к текстурам. В конструкторе класса объект будет инициализироваться, а в функции рендера вырисовываться.
```c#
    public class MeshEntry : IDisposable
    {
        private int _IndicesCount;
        public ModelTexturePaths _Paths;

        private VertexArrayObject VAO;
        private BufferObject<Vertex> VBO;
        private BufferObject<int> IBO;

        public unsafe MeshEntry(List<Vertex> Vertices, List<int> Indices, ModelTexturePaths Paths)
        {
            _IndicesCount = Indices.Count;
            _Paths = Paths;

            VAO = new VertexArrayObject();
            VBO = new BufferObject<Vertex>(Vertices.ToArray(), BufferTarget.ArrayBuffer);
            VAO.LinkBufferObject(ref VBO);

            VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, sizeof(Vertex), IntPtr.Zero);
            VAO.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Normal"));
            VAO.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Tex"));
            VAO.VertexAttributePointer(3, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Tangent"));
            VAO.VertexAttributePointer(4, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Bitangent"));

            IBO = new BufferObject<int>(Indices.ToArray(), BufferTarget.ElementArrayBuffer);
            VAO.LinkBufferObject(ref IBO);

            Vertices.Clear();
            Indices.Clear();
        }
        
        public void Render()
        {
            VAO.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _IndicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose() => VAO.Dispose();
    }
```
## Assimp Mesh
После того как сцена (т.е. наш объект) импортировался, рекурсивно инициализируются узлы объекта (от родителя). Каждый из узлов представляет из себя `Mesh`.
```c#
    public class AssimpMesh : IDisposable
    {
        private Scene scene;
        public List<MeshEntry> meshes { get; }
        public MeshEntry FirstMesh => meshes[0];
        public string PathModel;

        public AssimpMesh(string pathModel, bool flipuvs)
        {
            if (!File.Exists(pathModel)) 
                throw new Exception("error: file not exists " +  pathModel);

            PathModel = Path.GetDirectoryName(pathModel);

            scene = new Scene();
            meshes = new List<MeshEntry>();

            using(var importer = new AssimpContext())
            {
                if (flipuvs)
                    scene = importer.ImportFile(
                        pathModel,
                        PostProcessSteps.Triangulate |
                        PostProcessSteps.GenerateSmoothNormals |
                        PostProcessSteps.FlipUVs |
                        PostProcessSteps.CalculateTangentSpace);
                else
                    scene = importer.ImportFile(
                        pathModel,
                        PostProcessSteps.Triangulate |
                        PostProcessSteps.GenerateSmoothNormals |
                        PostProcessSteps.CalculateTangentSpace);
            }

            ProcessNodes(scene.RootNode);
        }

        private void ProcessNodes(Node node)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                ProcessMesh(scene.Meshes[node.MeshIndices[i]]);
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNodes(node.Children[i]);
            }
        }

        private void ProcessMesh(Mesh mesh)
        {
            var vertices = new List<Vertex>();
            var indices = new List<int>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var packed = new Vertex();

                packed.Pos = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
                packed.Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
                if (mesh.HasTextureCoords(0))
                {
                    packed.Tex = new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y);
                }
                else
                {
                    packed.Tex = new Vector2(0.0f, 0.0f);
                }
                packed.Tangent = new Vector3(mesh.Tangents[i].X, mesh.Tangents[i].Y, mesh.Tangents[i].Z);
                packed.Bitangent = new Vector3(mesh.BiTangents[i].X, mesh.BiTangents[i].Y, mesh.BiTangents[i].Z);

                vertices.Add(packed);
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add((ushort)face.Indices[j]);
                }
            }

            ModelTexturePaths texturesPaths = new ModelTexturePaths();

            if (mesh.MaterialIndex >= 0)
            {
                // Texturas
                Material material = scene.Materials[mesh.MaterialIndex];
                texturesPaths = ProcessTextures(material.GetAllMaterialTextures());
            }

            meshes.Add(new MeshEntry(vertices, indices, texturesPaths));
        }

        private ModelTexturePaths ProcessTextures(TextureSlot[] slot)
        {
            ModelTexturePaths texturesPath = new ModelTexturePaths();

            foreach (var item in slot)
            {
                if (item.FilePath != null)
                {
                    if (item.TextureType == TextureType.Diffuse)
                    {
                        texturesPath._DiffusePath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Specular)
                    {
                        texturesPath._SpecularPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Normals)
                    {
                        texturesPath._NormalPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Height)
                    {
                        texturesPath._HeightPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Metalness)
                    {
                        texturesPath._MetallicPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Roughness)
                    {
                        texturesPath._RoughnnesPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Lightmap)
                    {
                        texturesPath._LightMap = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Emissive)
                    {
                        texturesPath._EmissivePath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.AmbientOcclusion)
                    {
                        texturesPath._AmbientOcclusionPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                }
            }

            return texturesPath;
        }

        public void Dispose()
        {
            scene.Clear();
        }
    }
```
## Assimp Object
```c#
    public class AssimpObject : GameObj
    {
        private AssimpMesh assimpModel;
        private List<MeshEntry> meshes;
        private Shader shader;
        private Dictionary<string, Texture> TextureMap = new Dictionary<string, Texture>();

        public AssimpObject(string modelPath)
        {
            assimpModel = new AssimpMesh(modelPath, false);
            meshes = new List<MeshEntry>(assimpModel.meshes);
            pipeline = new Pipeline();

            shader = new Shader(ShaderLoader.LoadVertexShader(), ShaderLoader.LoadFragmentShader());

            foreach(MeshEntry m in meshes)
            {
                LoadTextures(m._Paths._DiffusePath, 		PixelInternalFormat.Rgba, 	TextureUnit.Texture0);
                LoadTextures(m._Paths._NormalPath, 		PixelInternalFormat.Rgba, 	TextureUnit.Texture1);
                LoadTextures(m._Paths._LightMap, 		PixelInternalFormat.Rgba, 	TextureUnit.Texture2);
                LoadTextures(m._Paths._EmissivePath, 		PixelInternalFormat.SrgbAlpha, 	TextureUnit.Texture3);
                LoadTextures(m._Paths._SpecularPath, 		PixelInternalFormat.Rgba, 	TextureUnit.Texture4);
                LoadTextures(m._Paths._HeightPath, 		PixelInternalFormat.Rgba, 	TextureUnit.Texture5);
                LoadTextures(m._Paths._MetallicPath, 		PixelInternalFormat.Rgba, 	TextureUnit.Texture6);
                LoadTextures(m._Paths._RoughnnesPath, 		PixelInternalFormat.Rgba, 	TextureUnit.Texture7);
                LoadTextures(m._Paths._AmbientOcclusionPath, 	PixelInternalFormat.Rgba, 	TextureUnit.Texture8);
            }

            GL.Enable(EnableCap.DepthTest);
        }

        public override void Draw()
        {
            shader.setMatrices(pipeline.getMVP().ToOpenTK());

            foreach (var item in meshes)
            {
                TextureMap[item._Paths._DiffusePath].Use();
                item.Render();
            }
        }

        private void LoadTextures(string tex_path, PixelInternalFormat pixelFormat, TextureUnit unit)
        {
            if (!TextureMap.ContainsKey(tex_path))
            {
                if (tex_path != string.Empty)
                {
                    Texture _texture_map = Texture.Load(tex_path, pixelFormat, unit);
                    TextureMap.Add(tex_path, _texture_map);
                }
            }
        }

        public override void OnDelete()
        {
            for (int i = 0; i < meshes.Count; i++)
                meshes[i].Dispose();

            foreach (var index in TextureMap.Keys)
                TextureMap[index].Dispose();

            shader.Dispose();
        }
    }
```
## Результат: 
До библиотеки Assimp мы могли загрузить только одну текстуру на модель, в результате чего некоторые области модели выглядили нелепо, потому что тектура находилась не на своем месте.

Результат:
![warr](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/warr_man.png)

<a name="s15"></a>
# Информационная панель
Кто помнит, в minecraft при нажатии на клавишу `f3` открывается экран отладки, в котором можно увидеть координаты игрока, FPS и тд. Мы с вами уже сделали прицел, который статично находится посередине экрана. Так почему бы не сделать вывод какой-то информации на экран, которая постоянно будет перед нашими глазами так же, как и прицел.
## Шрифт
Для начала подготовим картинку с нашим шрифтом, которую мы будем разбивать на символы. Не обращайте внимание на то, что некоторые буквы не в алфавитном порядке. Это для нас не так важно. А что важно, так это то, что сначала идут английские символы, потом цифры, пунктуация и русские символы. Такой порядок нужен при инициализации экрана отладки. То есть, если нам не будут нужны русские символы, то мы просто остановимся на пунктуации.  

### `font.png`:
![font](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/font.png)

## Класс InfoPanel
Этот класс будет содержать и регулировать символы на экране. При инициализации мы объявляем необходимое количество символов. `FontType.English` означает, что необходимо только 26 английских символов, `FontType.EnglishWithNumbers` - еще 10 цифр и тд. Все символы инициализируются, как игровые объекты, и хранятся для быстрой отрисовки в `_symbols`.  

`English`: *Только английские буквы (первые 26 символов)*  
`EnglishWithNumbers`: *English + цифры от 0 до 9 (первые 36 символов)*  
`EnglishWithNumbersAndPunctuation`: *EnglishWithNumbers + знаки пунктуации, включая пробел (первые 67 символов)*  
`FullSet`: *Все 100 символов, EnglishWithNumbersAndPunctuation + русские символы*  

В функции `PutLineAndDraw(string line)` входящая строка разбивается на символы, и для каждого символа функция `GetSymbolNumber(char c)` возвращает свой номер в соответствии с его положением в `font.png`. Этот номер нам нужен, чтобы найти нужный символ в массиве `_symbols` и отрисовать его. Отступы между выводимыми символами равны `step_x` и `step_y`.

```c#
    public class InfoPanel 
    {
        private List<GameObj> _symbols;
        private char[] _arrChar;

        private float step_x = 0.045f;
        private float step_y = 0.08f;

        public enum FontType : int
        {
            English = 26,
            EnglishWithNumbers = 36,
            EnglishWithNumbersAndPunctuation = 67,
            FullSet = 100
        }

        public InfoPanel(FontType type)
        {
            _symbols = new List<GameObj>();
            SymbolArrayOfVertices.LoadTexture("C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\font.png");

            for (int i = 0; i < (int)type; i++)
            {
                _symbols.Add(new Symbol(i));
            }
        }

        public void PutLineAndDraw(string line)
        {
            line = line.ToLower();
            _arrChar = line.ToCharArray();

            int line_num = 0, symbol_num = 0;

            for (int i = 0; i < _arrChar.Length; i++)
            {
                int symbol = GetSymbolNumber(_arrChar[i]) - 1;

                if (symbol == 100)
                {
                    symbol_num = 0;
                    line_num++;
                    continue;
                }
                else if (symbol >= _symbols.Count)
                {
                    Console.WriteLine("Символ \'" + _arrChar[i] + "\' не инициализирован в коллекции.");
                }
                _symbols[symbol].pipeline.SetPosition(-0.8f + symbol_num * step_x, 0.43f - line_num * step_y, -1);
                _symbols[symbol].Draw();
                symbol_num++;
            }
        }

        private int GetSymbolNumber(char c)
        {
            switch (c)
            {
                case 'a':
                    return 1;
                case 'b':
                    return 2;
                case 'c':
                    return 3;
                case 'd':
                    return 4;
                case 'e':
                    return 5;
                case 'f':
                    return 6;
                case 'g':
                    return 7;
                case 'h':
                    return 8;
                case 'i':
                    return 9;
                case 'k':
                    return 10;
                case 'j':
                    return 26;
                case 'l':
                    return 11;
                case 'm':
                    return 12;
                case 'n':
                    return 13;
                case 'o':
                    return 14;
                case 'p':
                    return 15;
                case 'q':
                    return 16;
                case 'r':
                    return 17;
                case 's':
                    return 18;
                case 't':
                    return 19;
                case 'u':
                    return 20;
                case 'v':
                    return 21;
                case 'w':
                    return 22;
                case 'x':
                    return 23;
                case 'y':
                    return 24;
                case 'z':
                    return 25;
                case '0':
                    return 36;
                case '1':
                    return 27;
                case '2':
                    return 28;
                case '3': 
                    return 29;
                case '4': 
                    return 30;
                case '5': 
                    return 31;
                case '6':
                    return 32;
                case '7':
                    return 33;
                case '8':
                    return 34;
                case '9':
                    return 35;
                case ',':
                    return 51;
                case '.':
                    return 37;
                case '+':
                    return 38;
                case '-':
                    return 39;
                case '*':
                    return 40;
                case '/':
                    return 41;
                case '=':
                    return 42;
                case '(':
                    return 43;
                case ')':
                    return 44;
                case '[':
                    return 45;
                case ']':
                    return 46;
                case '?':
                    return 47;
                case '!':
                    return 48;
                case ';':
                    return 49;
                case ':':
                    return 50;
                case '#':
                    return 52;
                case '$':
                    return 53;
                case '%':
                    return 54;
                case '^':
                    return 55;
                case '&':
                    return 56;
                case '_':
                    return 57;
                case '\'':
                    return 58;
                case '\\':
                    return 59;
                case ' ':
                    return 60;
                case '"':
                    return 61;
                case '`':
                    return 62;
                case '~':
                    return 63;
                case '<':
                    return 64;
                case '>':
                    return 65;
                case 'π':
                    return 66;
                case '@':
                    return 67;
                case 'а':
                    return 94;
                case 'б':
                    return 95;
                case 'в':
                    return 96;
                case 'г':
                    return 97;
                case 'д':
                    return 98;
                case 'е':
                    return 99;
                case 'ё':
                    return 100;
                case 'ж':
                    return 68;
                case 'з':
                    return 69;
                case 'и':
                    return 70;
                case 'й':
                    return 71;
                case 'к':
                    return 72;
                case 'л':
                    return 73;
                case 'м':
                    return 74;
                case 'н':
                    return 75;
                case 'о':
                    return 76;
                case 'п':
                    return 77;
                case 'р':
                    return 78;
                case 'с':
                    return 79;
                case 'т':
                    return 80;
                case 'у':
                    return 81;
                case 'ф':
                    return 82;
                case 'х':
                    return 83;
                case 'ц':
                    return 84;
                case 'ч':
                    return 85;
                case 'ш':
                    return 86;
                case 'щ':
                    return 87;
                case 'ъ':
                    return 88;
                case 'ы':
                    return 89;
                case 'ь':
                    return 90;
                case 'э':
                    return 91;
                case 'ю':
                    return 92;
                case 'я':
                    return 93;

                default:
                    return 60;
                case '\n':
                    return 101;
            }
        }

        public void OnClear()
        {
            for (int i = _symbols.Count - 1; i >= 0; i--)
            {
                _symbols[i].OnDelete();
            }
            _symbols.Clear();
        }
    }
```

## Класс Symbol
При инициализации символа мы вводим его порядковый номер в массиве `_symbols` класса `InfoPanel`. По этому номеру мы получаем местоположение текстуры символа в `font.png` (столбец и строка).
```c#
    public class Symbol : Aim
    {
        public Symbol(int sym_number) 
        {
            mesh = new SymbolMesh(sym_number % 10, 9 - sym_number / 10);
            pipeline = new Pipeline();

            pipeline.SetScale(0.05f);
        }

        public override void OnDelete()
        {
            base.OnDelete();
        }
    }
```

## Класс SymbolMesh
По полученным столбцу и строке мы для нашего символа получаем нужный кусочек текстуры `font.png`. 
```c#
    public class SymbolMesh : AimMesh
    {
        public SymbolMesh(int col, int raw)
        {
            SymbolArrayOfVertices.GetVertices(col, raw, out float[] vertices, out int[] indices);
            Vertices = vertices;
            Indices = indices;
            texture = SymbolArrayOfVertices.texture;
            pers_proj = pers_mat();

            Load();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
```

## Класс SymbolArrayOfVertices
В этом классе мы получаем VAO для нашего символа с помощью функции `GetVertices`. Так как изображение содержит 10 на 10 символов, то и `stepScale` равен 1/10.
```c#
    public static class SymbolArrayOfVertices
    {
        private static float[] cords = new float[]
        {
            //cords
            -1, -1,  0,
            -1,  1,  0,
             1,  1,  0,
             1, -1,  0
        };

        private static float stepScale = 0.1f;

        private static int[] inds 
        {
            get
            {
                return new int[]
                {
                    0, 1, 3,
                    1, 2, 3
                };
            }
        }

        public static void GetVertices(int col, int row, out float[] vertices, out int[] indices)
        {
            vertices = new float[20];

            indices = inds;

            float[] textures = new float[]
            {
                stepScale * col,                stepScale * row,
                stepScale * col,                stepScale * row + stepScale,
                stepScale * col + stepScale,    stepScale * row + stepScale,
                stepScale * col + stepScale,    stepScale * row,
            };

            for (int i = 0; i < 4; i++)
            {
                vertices[i * 5] =     cords[i * 3];
                vertices[i * 5 + 1] = cords[i * 3 + 1];
                vertices[i * 5 + 2] = cords[i * 3 + 2];

                vertices[i * 5 + 3] = textures[i * 2];
                vertices[i * 5 + 4] = textures[i * 2 + 1];
            }
        }

        public static Texture texture { get; private set; }

        public static void LoadTexture(string path)
        {
            texture = Texture.Load(
                path,
                PixelInternalFormat.Rgba,
                TextureUnit.Texture0,
                false);
        }
    }
```

## Вывод строки
В функции `OnLoad()` мы объявляем наш экран отладки на все 100 символов.
```c#
            Console.WriteLine("Загрузка шрифта...");
            info = new InfoPanel(InfoPanel.FontType.FullSet);
```
В функции рендера загружаем в панель нашу строку. Пусть это будут положение камеры, средний FPS и текущее время.
```c#
            info.PutLineAndDraw(
                "x: " + Math.Round(Camera.Pos.x)   + "\n" +
                "y: " + Math.Round(Camera.Pos.y)   + "\n" +
                "z: " + Math.Round(Camera.Pos.z)   + "\n" +
                "FPS: " + Math.Round(fps_out)      + "\n" +
                DateTime.Now );
```

## Результат
![ip](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/info_panel.gif)

<a name="s16"></a>
# Свет: окружающий, рассеянный, отраженный
## Окружающий свет
Базовый  (окружающий)  свет  содержит  только  интенсивность  и  цвет окружающего света. Это минимальное освещение примитивов всех объектов, даже когда свет на них не попадает или они находятся в тени. Итоговый цвет пикселя получается путем умножения исходного цвета пикселя на цвет света и величину интенсивности (яркости) света. 
### Структура BaseLight
Структура базового света. Она состоит из цвета, а также интенсивностей окружающего и рассеянного освещения. Из базовых параметров для окружающего света понадобятся только цвет и интенсивность окружающего освещения.
```c#
﻿using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct BaseLight
    {
        public vector3f Color;
        public float AmbientIntensity;
        public float DiffuseIntensity;

        public BaseLight(vector3f Color, float AmbientIntensity, float DiffuseIntensity)
        {
            this.Color = Color;
            this.AmbientIntensity = AmbientIntensity;
            this.DiffuseIntensity = DiffuseIntensity;
        }

        public BaseLight()
        {
            Color = vector3f.Zero;
            AmbientIntensity = 0f;
            DiffuseIntensity = 0f;
        }
    }

    public struct BaseLightLocations
    {
        public int Color;
        public int AmbientIntensity;
        public int DiffuseIntensity;
    }
}
```
### Класс LightingTechnique
Класс LightingTechnique хранит расположения параметров окружающего света в шейдере, а также устанавливает эти параметры.
```c#
public class LightingTechnique
    {
        BaseLightLocations _baseLightLocations;

        public LightingTechnique()
        {
            _baseLightLocations = new BaseLightLocations();
            Init();
        }

        private void Init()
        {
            //baseLight
            _baseLightLocations.Color = CentralizedShaders.ObjectShader.GetUniformLocation("gBaseLight.Color");
            _baseLightLocations.AmbientIntensity = CentralizedShaders.ObjectShader.GetUniformLocation("gBaseLight.AmbientIntensity");
        }

        public void SetBaseLight(BaseLight baseLight)
        {
            GL.Uniform3(_baseLightLocations.Color, baseLight.Color.x, baseLight.Color.y, baseLight.Color.z);
            GL.Uniform1(_baseLightLocations.AmbientIntensity, baseLight.AmbientIntensity);
        }

        public void SetBaseLight(vector3f Color, float AmbientIntensity)
        {
            GL.Uniform3(_baseLightLocations.Color, Color.x, Color.y, Color.z);
            GL.Uniform1(_baseLightLocations.AmbientIntensity, AmbientIntensity);
        }
    }
```
### Изменения во фрагментном шейдере
Изменения во фрагментном шейдере состоят в том, что также инициализируется структура базового света, а цвет и интенсивность окружающего света влияют на выходной цвет. 
```hlsl
#version 330
out vec4 outputColor;

in vec2 texCoord;
in vec3 Normal0;
in vec3 Tangent0;

struct BaseLight
{
    vec3 Color;
    float AmbientIntensity;
    float DiffuseIntensity;
};

uniform sampler2D gDiffuseMap;
uniform BaseLight gBaseLight;

void main() 
{ 
    vec4 texel = texture(gDiffuseMap, texCoord);

    if (texel.a < 0.3) discard;

	outputColor = texel * vec4(gBaseLight.Color, 1.0) * gBaseLight.AmbientIntensity;
}
```
### Включение в движке
В классе движка включаем окружающий свет, задав его параметры.
```c#
	protected override void OnLoad() {
		. . .
            lightConfig = new LightingTechnique();

            BaseLight baseLight = new BaseLight();
            baseLight.Color = new vector3f(1, 1, 1);
            baseLight.AmbientIntensity = 0.3f;

            CentralizedShaders.ObjectShader.Use();
            lightConfig.SetBaseLight(baseLight);
		. . .
	}
```
## Рассеянный свет
В отличие от базового света, направленный свет основывается на направлении лучей и делает ярче только те примитивы объекта, которые прошли проверку. Яркость и наличие направленного света на том или ином примитиве зависит от того, под каким углом луч направленного света падает на поверхность. Для этого у каждой вершины существует нормализованный вектор нормали, который всегда перепендикулярен поверхности вершины.
Когда направленный свет попадает на примитив, коэффициент влияния света высчитывается через косинус угла между лучами и нормалью поверхности, который равен скалярному произведению нормализованному вектору нормали поверхности и нормализованному обратному вектору направления лучей света. Если коэффициент влияния света DiffuseFactor меньше 0, значит угол между направленным светом и нормалью тупой. Если DiffuseFactor = 0, значит угол прямой. В этих случаях влияния света не будет. Если же DiffuseFactor больше 0, то цвет рассеивания равен произведению цвета света на интенсивность рассеивания, уменьшенный на коэффициент рассеивания.
### Структура DirectionalLight
Структура рассеянного освещения DirectionalLight включает в себя направление, а также элементы базового света.
```c#
﻿using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct DirectionalLight
    {
        public vector3f Direction;
        public BaseLight BaseLight;
        public DirectionalLight(vector3f Color, float AmbientIntensity, float DiffuseIntensity, vector3f Direction)
        {
            this.BaseLight = new BaseLight(Color, AmbientIntensity, DiffuseIntensity);
            this.Direction = Direction;
        }

        public DirectionalLight(BaseLight BaseLight, vector3f Direction)
        {
            this.BaseLight = BaseLight;
            this.Direction = Direction;
        }

        public DirectionalLight()
        {
            BaseLight = new BaseLight();
            Direction = vector3f.Zero;
        }
    }

    public struct DirectionalLightLocations
    {
        public BaseLightLocations BaseLightLocations;
        public int Direction;
    }
}
```
### Изменения в классе LightingTechnique
Изменения в классе LightingTechnique заключаются в добавлении сохранения локаций параметров рассеянного света в функции инициализации и добавлении функций установки параметров.
```c#
        private void Init()
        {
		. . .            
            //dirLight
            _directionalLightLocations.BaseLightLocations.Color = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Base.Color");
            _directionalLightLocations.BaseLightLocations.AmbientIntensity = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Base.AmbientIntensity");
            _directionalLightLocations.BaseLightLocations.DiffuseIntensity = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Base.DiffuseIntensity");
            _directionalLightLocations.Direction = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Direction");
		. . .
        }
```
```c#
	. . .
        public void SetDirectionalLight(DirectionalLight directionalLight)
        {
            Use();
            GL.Uniform3(_directionalLightLocations.BaseLightLocations.Color, 
                directionalLight.BaseLight.Color.x, 
                directionalLight.BaseLight.Color.y, 
                directionalLight.BaseLight.Color.z);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.AmbientIntensity, 
                directionalLight.BaseLight.AmbientIntensity);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.DiffuseIntensity,
                directionalLight.BaseLight.DiffuseIntensity);
            GL.Uniform3(_directionalLightLocations.Direction,
                directionalLight.Direction.x,
                directionalLight.Direction.y,
                directionalLight.Direction.z);
        }

        public void SetDirectionalLight(vector3f Color, float AmbientIntensity, float DiffuseIntensity ,vector3f Direction)
        {
            Use();
            GL.Uniform3(_directionalLightLocations.BaseLightLocations.Color,
                Color.x,
                Color.y,
                Color.z);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.AmbientIntensity,
                AmbientIntensity);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.DiffuseIntensity,
                DiffuseIntensity);
            GL.Uniform3(_directionalLightLocations.Direction,
                Direction.x,
                Direction.y,
                Direction.z);
        }

        private void Use() => CentralizedShaders.ObjectShader.Use();
	. . .
```
### Изменения во фрагментном шейдере
Во фрагментном шейдере была добавлена новая структура рассеянного света, включающая параметры базового света.
```hlsl
. . .
struct DirectionalLight
{
    vec3 Direction;
    BaseLight Base;
};
uniform DirectionalLight gDirectionalLight;
. . .
```
На основе параметров света была высчитана интенсивность рассеянного освещения на том или ином участке объекта. Если результат скалярного произведения вектора нормали и вектора направления света меньше 0, то рассеянное освещение на этом участке отсутствует. В результате окружающий и рассеянный света складываются друг с другом.
```hlsl
. . .
void main() 
{ 
    vec4 texel = texture(gDiffuseMap, texCoord);

    if (texel.a < 0.3) discard;

    vec4 AmbientColor = vec4(gBaseLight.Color, 1.0) * gBaseLight.AmbientIntensity;

    float DiffuseFactor = dot(Normal0, -gDirectionalLight.Direction);

    vec4 DiffuseColor;

    if (DiffuseFactor > 0)
    {
        DiffuseColor = 
            vec4(gDirectionalLight.Base.Color, 1.0) * 
            gDirectionalLight.Base.DiffuseIntensity *
            DiffuseFactor;
    }
    else
    {
        DiffuseColor = vec4(0, 0, 0, 0);
    }

	outputColor = texel * (AmbientColor + DiffuseColor);
}
. . .
```
### Включение в движке
В классе движка был создан и установлен рассеянный свет.
```c#
        // Загрузка окна
        protected override void OnLoad()
        {
		. . .
            directionalLight = new DirectionalLight();
            directionalLight.BaseLight.Color = vector3f.One;
            directionalLight.BaseLight.DiffuseIntensity = 0.6f;
            directionalLight.Direction = new vector3f(1, -0.7f, -1);
            lightConfig.SetDirectionalLight(directionalLight);
 		. . .
        }
```
## Отраженный свет
Имитация отражения луча света от поверхности зависит от позиции камеры и направления луча от источника света до поверхности. Проверка наблюдения отраженного света реализуется с помощью вектора нормали, перпендикулярного поверхности. Сначала высчитывается нормализованный вектор V из точки на поверхности до камеры. Далее высчитывается нормализованный вектор R направления луча, отраженного от поверхности под тем же углом, что был при падении. Потом высчитывается косинус угла между векторами V и R, являющийся SpecularFactor. 
![image](https://github.com/user-attachments/assets/50f8bbfc-7d14-4eed-9f3e-fd8897ae0a8a)
Если SpecularFactor больше 0, то есть если угол между векторами R и V острый или равен 0, то SpecularFactor усиляется параметром SpecPower, являющимся силой отражения материала.
### Изменения в классе LightingTechnique
Были объявлены расположения в шейдере параметров позиции камеры, интенсивности и силы отражения света.  
```c#
    public class LightingTechnique
    {
	. . .
        int _cameraPositionLocation;
        int _matSpecularIntensityLocation;
        int _matSpecularPowerLocation;
	. . .
```
Далее в функции Init параметры были инициализированы. 
```c#
        private void Init()
        {
		. . .
            //specular
            _cameraPositionLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gCameraPos");
            _matSpecularIntensityLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gMatSpecularIntensity");
            _matSpecularPowerLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gMatSpecularPower");
        }
```
Также была добавлена функция установки параметров.
```c#
	. . .
        //specular
        public void SetSpecular(vector3f Position, float SpecularIntensity, float SpecularPower)
        {
            Use();
            GL.Uniform3(_cameraPositionLocation, Position.x, Position.x, Position.z);
            GL.Uniform1(_matSpecularIntensityLocation, SpecularIntensity);
            GL.Uniform1(_matSpecularPowerLocation, SpecularPower);
        }

        public void SetCameraPosition(vector3f Position)
        {
            Use();
            GL.Uniform3(_cameraPositionLocation, Position.x, Position.x, Position.z);
        }

        public void SetMatSpecularIntensity(float SpecularIntensity)
        {
            Use();
            GL.Uniform1(_matSpecularIntensityLocation, SpecularIntensity);
        }

        public void SetMatSpecularPower(float SpecularPower)
        {
            Use();
            GL.Uniform1(_matSpecularPowerLocation, SpecularPower);
        }
	. . .
```
### Изменения в вершинном шейдере
В вершинном шейдере высчитывается параметр WorldPos0, хранящий расположение вершины с учетом преобразований и отправляющийся во фрагментный шейдер.
```hlsl
. . .
out vec3 WorldPos0;
. . .
void main()                                            
{        
	. . .
	WorldPos0 = (vec4(aPosition, 1.0) * world).xyz;
}
```
### Изменения во фрагментном шейдере
Если скалярное произведение вектора нормали и вектора направления рассеянного света больше 0, то происходит подсчет вектора VertexToEye из вершины в мировом пространстве до позиции камеры. Затем вычисляется вектор отражения LightReflect с помощью функции reflect, которая принимает два параметра: вектор направления света и нормаль к поверхности. Если скалярное произведение вектором VertexToEye и LightReflect больше 0, то итоговый отраженный цвет вычисляется через произведение цвета света на интенсивность отражения материала и сила отражения и добавляется к освещению при создании итогового цвета.
```hlsl
...
in vec3 WorldPos0;
uniform vec3 gCameraPos;
uniform float gMatSpecularIntensity;
uniform float gMatSpecularPower;
...
```
```hlsl
void main() 
{ 
    vec4 texel = texture(gDiffuseMap, texCoord);

    if (texel.a < 0.3) discard;

    vec4 AmbientColor = vec4(gBaseLight.Color, 1.0) * gBaseLight.AmbientIntensity;

    float DiffuseFactor = dot(Normal0, -gDirectionalLight.Direction);

    vec4 DiffuseColor = vec4(0, 0, 0, 0);
    vec4 SpecularColor = vec4(0, 0, 0, 0);

    if (DiffuseFactor > 0)
    {
        DiffuseColor = 
            vec4(gDirectionalLight.Base.Color, 1.0) * 
            gDirectionalLight.Base.DiffuseIntensity *
            DiffuseFactor;

        vec3 VertexToEye = normalize(gCameraPos - WorldPos0);
        vec3 LightReflect = normalize(reflect(gDirectionalLight.Direction, Normal0));
        float SpecularFactor = dot(VertexToEye, LightReflect);
        SpecularFactor = pow(SpecularFactor, gMatSpecularPower);

        if (SpecularFactor > 0) {
            SpecularColor = 
                vec4(gDirectionalLight.Base.Color, 1.0f) * 
                gMatSpecularIntensity *
                SpecularFactor;
        }
    }
    outputColor = texel * (AmbientColor + DiffuseColor + SpecularColor);
}
```
### Включение в движке
В классе движка устанавливаются параметры отраженного света.
```c#
        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
		. . .
            lightConfig.SetSpecular(Camera.Pos, 4, 64);
		. . .
	}
```
## Результаты 
![difli](https://github.com/galeevlxix/game_engine/blob/Light/screens/diflights.jpg)  

![spec](https://github.com/galeevlxix/game_engine/blob/Light/screens/specularlight.jpg)

<a name="s17"></a>
# Свет: Point Light
## Теория  
Точечный источник света схож со стандартной лампочкой, но он представляется как материальная точка. У него есть позиция в пространстве, он освещает во всех направлениях, а сила освещения обратно пропорциональна квадрату расстояния от источника света.  
Вектор направления луча света от точечного источника определяется разностью позиции конкретной вершины объекта и позиции источника света. Длина этого вектора равна дистанции:    
![image](https://github.com/user-attachments/assets/df4ce9b0-efe3-4bc0-892b-1c8e3ac0f954)  
Так же, как и с направленным светом (2.2.2 и 2.2.3), косинус угла между направлением света и нормалью поверхности определяет коэффициент влияния света на поверхность. Если коэффициент больше 0, то цвет от точечного света равен произведению цвета света на интенсивность, уменьшенный на коэффициент рассеивания и экспоненциально от дистанции:  
![image](https://github.com/user-attachments/assets/c73571f8-8afc-4019-8f74-92b290cf3eb9)  
Например, пусть существует точечный источник света, у которого белый цвет Color = (1, 1, 1), интенсивность Intensity = 1. Пусть DiffuseFactor = =1, то есть луч света падает под прямым углом на поверхность. Пусть параметры затухания определены как: Constant = 1, Linear = 0,09, Exp = 0,032. Тогда итоговый цвет от освещения поверхности зависит от расстояния до точечным источником света следующим образом.  
![image](https://github.com/user-attachments/assets/7cfab7ac-1b37-40ab-a4e8-5f4bfe2c9c09)  
## Реализация  
Структура точечного источника света выглядит следующим образом. Он имеет вектор позиции, структуру BaseLight с интенсивностью и цветом света, а также Attenuation с параметрами константного, линейного и экспоненциального затухания.
```c#
    public struct PointLight
    {
        public vector3f Position;
        public BaseLight BaseLight;
        public Attenuation Attenuation;
    }

    public struct PointLightLocations
    {
        public BaseLightLocations BaseLightLocations;
        public AttenuationLocations Attenuation;
        public int Position;
    }

    public struct Attenuation
    {
        public float Constant;
        public float Linear;
        public float Exp;
    }

    public struct AttenuationLocations
    {
        public int Constant;
        public int Linear;
        public int Exp;
    }
```
В программе шейдера инициализируется структура точечного света, а также максимальное количество, массив и текущее количество источников точечного света.
```c
struct PointLight
{
    BaseLight Base;
    vec3 Position;
    Attenuation Atten;
};
const int MAX_POINT_LIGHTS = 10;

uniform PointLight gPointLights[MAX_POINT_LIGHTS];
uniform int gNumPointLights;
```
В функции CalcLightInternal на основе направления луча света и вектора нормали текущей поверхности вычисляется влияние диффузного и отраженного освещения на поверхность от всех типов источника света. В функции CalcPointLight задается направление луча для вычисления его диффузного и отраженного освещения, а также задается затухание света, зависящее от расстояния.
```c
vec4 CalcLightInternal(BaseLight Light, vec3 pLightDirection, vec3 Normal)
{
    vec3 LightDirection = normalize(pLightDirection);

    float DiffuseFactor = dot(Normal, -LightDirection);

    vec4 DiffuseColor = vec4(0, 0, 0, 0);
    vec4 SpecularColor = vec4(0, 0, 0, 0);

    if (DiffuseFactor > 0)
    {
        DiffuseColor = vec4(Light.Color, 1.0) * Light.Intensity * DiffuseFactor;

        vec3 VertexToEye = normalize(gCameraPos - WorldPos0);
        vec3 LightReflect = normalize(reflect(LightDirection, Normal));
        float SpecularFactor = dot(VertexToEye, LightReflect);

        if (SpecularFactor > 0) 
        {
            SpecularFactor = pow(SpecularFactor, gMaterial.SpecularPower);
            SpecularColor = vec4(Light.Color, 1.0f) * Light.Intensity * SpecularFactor;
        }
    }
    return (DiffuseColor + SpecularColor);
}
vec4 CalcPointLight(PointLight pLight, vec3 Normal)
{
    vec3 LightDirection = WorldPos0 - pLight.Position;
    float Distance = length(LightDirection);

    vec4 Color = CalcLightInternal(pLight.Base, LightDirection, Normal);
    
    float Attenuation =  pLight.Atten.Constant + 
                         pLight.Atten.Linear * Distance +
                         pLight.Atten.Exp * Distance * Distance;
    return Color / Attenuation;
}
void main()
{
	. . .
    for (int i = 0; i < gNumPointLights; i++)
    {
        TotalLight += CalcPointLight(gPointLights[i], Normal);
    }
    	. . .
}
```
При инициализации позиции для шейдерных переменных сохраняются.
```c
_numPointLightsLocation = CentralizedShaders.GetUniformLocation( ShaderName.AssimpShader, "gNumPointLights");
for (int i = 0; i < MAX_POINT_LIGHTS; i++)
{
	_pointLightLocations[i].BaseLightLocations.Color = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gPointLights[" + i + "].Base.Color");
	_pointLightLocations[i].BaseLightLocations.Intensity = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gPointLights[" + i + "].Base.Intensity");
	_pointLightLocations[i].Position = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gPointLights[" + i + "].Position");
	_pointLightLocations[i].Attenuation.Exp = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gPointLights[" + i + "].Atten.Exp");
	_pointLightLocations[i].Attenuation.Linear = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gPointLights[" + i + "].Atten.Linear");
	_pointLightLocations[i].Attenuation.Constant = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gPointLights[" + i + "].Atten.Constant");
}
```
Установка параметров точечного света.
```c#
	public void SetPointLights(PointLight[] pointLights)
        {
            Use();
            GL.Uniform1(_numPointLightsLocation, pointLights.Length);

            for (int i = 0; i < pointLights.Length; i++)
            {
                GL.Uniform3(
                    _pointLightLocations[i].BaseLightLocations.Color, 
                    pointLights[i].BaseLight.Color.x, 
                    pointLights[i].BaseLight.Color.y, 
                    pointLights[i].BaseLight.Color.z);
                GL.Uniform1(
                    _pointLightLocations[i].BaseLightLocations.Intensity, 
                    pointLights[i].BaseLight.Intensity);
                GL.Uniform3(
                    _pointLightLocations[i].Position, 
                    pointLights[i].Position.x, 
                    pointLights[i].Position.y, 
                    pointLights[i].Position.z);
                GL.Uniform1(
                    _pointLightLocations[i].Attenuation.Exp, 
                    pointLights[i].Attenuation.Exp);
                GL.Uniform1(
                    _pointLightLocations[i].Attenuation.Linear, 
                    pointLights[i].Attenuation.Linear);
                GL.Uniform1(
                    _pointLightLocations[i].Attenuation.Constant, 
                    pointLights[i].Attenuation.Constant);
            }
}
```

```c#
            pointLights[0].Position = new vector3f(-5, 2, 0);
            pointLights[0].Attenuation.Exp = 0.032f;
            pointLights[0].Attenuation.Linear = 0.09f;
            pointLights[0].Attenuation.Constant = 1;
            pointLights[0].BaseLight.Color = new vector3f(1, 0, 0);
            pointLights[0].BaseLight.Intensity = 1f;

            pointLights[1].Position = new vector3f(5, 2, 0);
            pointLights[1].Attenuation.Exp = 0.032f;
            pointLights[1].Attenuation.Linear = 0.09f;
            pointLights[1].Attenuation.Constant = 1;
            pointLights[1].BaseLight.Color = new vector3f(0, 1, 1);
            pointLights[1].BaseLight.Intensity = 1f;

            lightConfig.SetPointLights(pointLights);
```
<a name="s18"></a>
# Свет: Spot Light 
## Теория  
Прожекторный свет, как и точечный свет, имеет позицию, затухание, цвет и интенсивность, а также он имеет направление и коэффициент обрезки света Cutoff. Чтобы определить влияние прожекторного света на поверхность, первым делом необходимо вычислить косинус угла между нормализованным вектором направления луча света прожектора от начала источника до пикселя поверхности с самим направлением прожектора.  
Если косинус меньше, чем коэффициент обрезки света, то пиксель находится вне круга прожектора. А если больше, значит пиксель находится в пределах круга, и необходимо вычислить влияние света на него так, как если бы у нас был точечный источник света	, учитывая угол падения луча, дистанцию до пикселя, коэффициенты затухания и интенсивность свечения. Полученный результат умножается на значение, линейно интерполируемое от 0 до 1 в зависимости от SpotFactor и Cutoff.  
![image](https://github.com/user-attachments/assets/fcde44d1-67ab-4a36-ab72-8b04a527554b)  
## Реализация  
Структура прожекторного источника света выглядит следующим образом. Он имеет вектор направления, параметр отсечения и структуру Point Light. Усеченный и направленный точечный свет определяет прожекторный свет. 
```c#
    public struct Spotlight
    {
        public vector3f Direction;
        public float Cutoff1;
        public PointLight PointLight;
    }

    public struct SpotlightLocations
    {
        public PointLightLocations PointLightLocations;
        public int Direction;
        public int Cutoff1;
    }
```
В программе шейдера инициализируется структура прожекторного света, а также максимальное количество, массив и текущее количество источников прожекторного света.  
```c
struct SpotLight
{
    PointLight Base;
    vec3 Direction;
    float Cutoff1;
};

const int MAX_SPOT_LIGHTS = 10;

uniform SpotLight gSpotLights[MAX_SPOT_LIGHTS];
uniform int gNumSpotLights;
```
Вектор LightToPixel может не совпадать с направлением прожектора. Он лишь указывает направление текущего луча от источника света до пикселя объекта. Если этот луч находится в области усечения, цвет от прожекторного свет считается, как от точечного. Далее границы освещения делаются более плавными.  
```c
vec4 CalcSpotLight(SpotLight sLight, vec3 Normal) 
{
    vec3 LightToPixel = normalize(WorldPos0 - sLight.Base.Position);
    float SpotFactor = dot(LightToPixel, sLight.Direction);

    if (SpotFactor > sLight.Cutoff1)
    {
        vec4 Color = CalcPointLight(sLight.Base, Normal);
        return Color * (1.0 - (1.0 - SpotFactor) * 1.0 / (1.0 - sLight.Cutoff1));
    }

    return vec4(0, 0, 0, 0);
}
. . .
    for (int i = 0; i < gNumSpotLights; i++)
    {
        TotalLight += CalcSpotLight(gSpotLights[i], Normal0);
    }
```
При инициализации сохраняем позиции для шейдерных переменных.  
```c#
            _numSpotLightsLocation = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gNumSpotLights");

            for (int i = 0; i < MAX_SPOT_LIGHTS; i++)
            {
		_spotlightLocations[i].PointLightLocations.BaseLightLocations.Color = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gSpotLights[" + i + "].Base.Base.Color");
                _spotlightLocations[i].PointLightLocations.BaseLightLocations.Intensity = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gSpotLights[" + i + "].Base.Base.Intensity");
                _spotlightLocations[i].PointLightLocations.Position = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gSpotLights[" + i + "].Base.Position");
                _spotlightLocations[i].PointLightLocations.Attenuation.Exp = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gSpotLights[" + i + "].Base.Atten.Exp");
                _spotlightLocations[i].PointLightLocations.Attenuation.Constant = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gSpotLights[" + i + "].Base.Atten.Constant");
                _spotlightLocations[i].PointLightLocations.Attenuation.Linear = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gSpotLights[" + i + "].Base.Atten.Linear");
                _spotlightLocations[i].Direction = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gSpotLights[" + i + "].Direction");
                _spotlightLocations[i].Cutoff1 = CentralizedShaders.GetUniformLocation(ShaderName.AssimpShader, "gSpotLights[" + i + "].Cutoff1");
            }
```
Установка параметров прожекторного света выглядит следующим образом.  
```c#
        public void SetSpotLights(Spotlight[] spotLights)
        {
            Use();
            GL.Uniform1(_numSpotLightsLocation, spotLights.Length);
            for (int i = 0; i < spotLights.Length; i++)
            {
                GL.Uniform3(
              _spotlightLocations[i].PointLightLocations.BaseLightLocations.Color, 
                    spotLights[i].PointLight.BaseLight.Color.x, 
                    spotLights[i].PointLight.BaseLight.Color.y, 
                    spotLights[i].PointLight.BaseLight.Color.z);
                GL.Uniform1(
_spotlightLocations[i].PointLightLocations.BaseLightLocations.Intensity,
                    spotLights[i].PointLight.BaseLight.Intensity);
                GL.Uniform3(
                    _spotlightLocations[i].PointLightLocations.Position,
                    spotLights[i].PointLight.Position.x,
                    spotLights[i].PointLight.Position.y,
                    spotLights[i].PointLight.Position.z);
                GL.Uniform1(
                    _spotlightLocations[i].PointLightLocations.Attenuation.Exp,
                    spotLights[i].PointLight.Attenuation.Exp);
                GL.Uniform1(
   _spotlightLocations[i].PointLightLocations.Attenuation.Constant,
                    spotLights[i].PointLight.Attenuation.Constant);
                GL.Uniform1(
                    _spotlightLocations[i].PointLightLocations.Attenuation.Linear,
                    spotLights[i].PointLight.Attenuation.Linear);
                GL.Uniform3(
                    _spotlightLocations[i].Direction,
                    spotLights[i].Direction.x,
                    spotLights[i].Direction.y,
                    spotLights[i].Direction.z);
                GL.Uniform1(
                    _spotlightLocations[i].Cutoff1,
                    spotLights[i].Cutoff1);
        }
```

```c#
            spotlights[0].PointLight.Position = new vector3f(-10, 1, 25);
            spotlights[0].PointLight.BaseLight.Color = new vector3f(1, 1, 0);
            spotlights[0].PointLight.BaseLight.Intensity = 1;
            spotlights[0].PointLight.Attenuation.Constant = 1;
            spotlights[0].PointLight.Attenuation.Linear = 0.027f;
            spotlights[0].PointLight.Attenuation.Exp = 0.0028f;
            spotlights[0].Direction = new vector3f(0, -1, 0);
            spotlights[0].Cutoff1 = 0.3f; 
            spotlights[1].PointLight.Position = Camera.Pos;
            spotlights[1].PointLight.BaseLight.Color = new vector3f(1, 0, 1);
            spotlights[1].PointLight.BaseLight.Intensity = 0;
            spotlights[1].PointLight.Attenuation.Constant = 1;
            spotlights[1].PointLight.Attenuation.Linear = 0.027f;
            spotlights[1].PointLight.Attenuation.Exp = 0.0028f;
            spotlights[1].Direction = -Camera.Target;
            spotlights[1].Cutoff1 = 0.97f; 
            lightConfig.SetSpotLights(spotlights);
```
## Результат  
![image](https://github.com/user-attachments/assets/8b51e7b5-63cf-48ca-a7aa-e6aeb57b8b81)
![gif](https://github.com/galeevlxix/game_engine/blob/diplom%2B/Files/Screenshots/pointspotlight-ezgif.com-video-to-gif-converter.gif)
<a name="s19"></a>
# Карты нормали (Normal Map)
## Теория  
Карты нормалей не изменяют реальный 3D-объект. Они лишь создают за счет света и теней иллюзию существования шероховатости, рельефа и прочих мелких деталей, когда на самом деле эти мелкие детали не были смоделированы, а хранятся в карте нормали в виде текстуры. Благодаря технике использования карты нормали можно значительно повысить детализацию объектов и улучшить графику, без значительного влияния на производительность.  
![image](https://github.com/user-attachments/assets/12fdf19c-4188-4ce4-87b4-61fd7ada82ab)  
Достигается это путем заимствования нормалей вершин из текстуры, называемой картой нормали. Каждый тексел из этой текстуры, который имеет цвет (R, G, B), определяет направление вектора нормали в координатах (X, Y, Z). Данный вектор находится в диапазоне [0; 1], так как величины Red, Green или Blue находятся в диапазоне [0; 1], поэтому необходимо перевести вектор в диапазон [-1; 1].  
![image](https://github.com/user-attachments/assets/5bf9faa2-3d7e-4aa3-9336-12f512309edb)  
Однако все нормали, выбранные из карты нормали, находятся в пространстве текстуры ориентированы вдоль положительной оси Oz. Это может плохо повлиять на результат освещения поверхности объекта, когда при изменении объекта в мировом пространстве нормаль его какой-то поверхности не будет сонаправлена с осью Oz.   
TBN-матрица строится из векторов Tangent, Bitangent и Normal. Вектор нормали текстуры умножается на TBN-матрицу для преобразования из пространства текстуры в пространство модели и нормализуется, чтобы гарантировать единичную длину. Далее полученный вектор нормали текстуры будет использоваться во всех вычислениях диффузного освещения вместо вектора нормали поверхности.  
![image](https://github.com/user-attachments/assets/fde5c98a-a5c0-4be4-802f-cd08bd49e955)  
## Реализация  
Каждый Mesh содержит свой объект класса материала, который в свою очередь содержит пути текстур этого Mesh и силу отражения материала, а также функции для работы с ними.  
```c#
    public class AMaterial
    {
        private ModelTexturePaths m_paths;
        private float m_specular_power;

        private AMaterial()
        {
            m_paths = new ModelTexturePaths();
        }

        public static AMaterial Init(ModelTexturePaths paths)
        {
            AMaterial material = new AMaterial();
            material.m_paths = paths;
            material.LoadTextures();

            return material;
        }

        public void Use()
        {
            UseTextures();
            LightningManager.lightConfig.SetMatSpecularPower(m_specular_power);
        }

        private void UseTextures()
        {
            TextureHeap.Use(m_paths);
        }
        
        private void LoadTextures()
        {
            //missing maps
            if (m_paths._NormalPath == string.Empty) m_paths._NormalPath = TextureHeap.empty_normal_map;
            if (m_paths._SpecularPath == string.Empty) m_paths._SpecularPath = TextureHeap.empty_specular_map;

            //add
            TextureHeap.Add(m_paths._DiffusePath);
            TextureHeap.Add(m_paths._NormalPath, TextureUnit.Texture1);
        }

        public void SetSpecularPower(float value)
        {
            m_specular_power = value;
        }
    }
```
Все текстуры, которые есть в проекте, хранятся в «куче текстур», где каждая текстура уникальная и идентифицируется путем к ее файлу. В материале хранятся пути к файлам текстур, и когда надо активировать какую-то текстуру материала, можно просто обратиться к этой куче. Сделано это для того, чтобы не создавать и не хранить одни и те же тяжелые текстуры разных материалов.  
```c#
    public static class TextureHeap
    {
        private static Dictionary<string, Texture> _textureHeap = new Dictionary<string, Texture>();
        public static string empty_normal_map = "..\\..\\..\\Files\\Textures\\EmptyNormalMap.png";
        public static string empty_specular_map = "..\\..\\..\\Files\\Textures\\white_list2.bmp";
public static void Add(string file_path, TextureUnit unit = TextureUnit.Texture0, PixelInternalFormat format = PixelInternalFormat.Rgba)
        {
            if (!_textureHeap.ContainsKey(file_path))
            {
                _textureHeap.Add(file_path, Texture.Load(file_path, format, unit));
            }
        }

        public static void Use(string file_path)
        {
            _textureHeap[file_path].Use();
        }

        public static void Use(ModelTexturePaths paths)
        {
            _textureHeap[paths._DiffusePath].Use();
            _textureHeap[paths._NormalPath].Use();
        }
    }
```
При загрузке объекта с Assimp создаются и загружаются текстурные карты и сила отражения материалов на основе данных из файла материала (.mtl) объекта.   
```c#
            if (mesh.MaterialIndex >= 0)
            {
                // Textures
                Material input_material = _scene.Materials[mesh.MaterialIndex];
                texturesPaths = ProcessTextures(input_material.GetAllMaterialTextures());
                shininess = input_material.Shininess;                
            }

            material = AMaterial.Init(texturesPaths);
            material.SetSpecularPower(shininess);

            _entries.Add(new AEntry(vertices, indices, material));
```
В шейдерной программе нормаль меняется на результат функции CalcBumpedNormal.  
```c
vec3 CalcBumpedNormal()
{
    vec3 Normal = normalize(Normal0); 
    vec3 Tangent = normalize(Tangent0);
    Tangent = normalize(Tangent - dot(Tangent, Normal) * Normal);
    vec3 Bitangent = cross(Tangent, Normal);
    vec3 BumpMapNormal = (texture2D(gMaterial.NormalMap, texCoord.xy)).xyz;
    BumpMapNormal = 2.0 * BumpMapNormal - vec3(1.0, 1.0, 1.0);
    vec3 NewNormal;                              
    mat3 TBN = mat3(Tangent, Bitangent, Normal);    
    NewNormal = TBN * BumpMapNormal;                    
    NewNormal = normalize(NewNormal);               
    return NewNormal;  
}
void main()
{
    vec3 Normal = CalcBumpedNormal();
. . .
}
```
## Результат
![image](https://github.com/user-attachments/assets/3864426e-7b7f-4808-bf85-217b3700b3b0)
<a name="s20"></a>
# Карта отражения (Specular Map)
## Теория
Specular Map накладывается на объект, как и диффузная карта, но хранит в себе не цвета пикселей объекта, а степень отражения материала в данном участке. Отличие Specular Map от параметра силы отражения материала Specular Power в том, что второй действует по всем поверхностям одного материла и усиливает отражение. А Specular Map показывает, где отражение должно быть сильнее, а где должно быть меньше. Например, металлическое лезвие ножа должно отражать свет сильно, а деревянная ручка – слабо. Каждый пиксель карты отражения может отображаться в виде цветового вектора, где черный представляет цветовой вектор vec3 (0, 0, 0), а белый - цветовой вектор vec3 (1, 1, 1), например.  
## Реализация  
При загрузке и хранении текстур берется в расчет еще наличие specular map.  
```c#
        private void LoadTextures()
        {
            //missing maps
            if (m_paths._NormalPath == string.Empty) m_paths._NormalPath = TextureHeap.empty_normal_map;
            if (m_paths._SpecularPath == string.Empty) m_paths._SpecularPath = TextureHeap.empty_specular_map;

            //add
            TextureHeap.Add(m_paths._DiffusePath);
            TextureHeap.Add(m_paths._NormalPath, TextureUnit.Texture1);
            TextureHeap.Add(m_paths._SpecularPath, TextureUnit.Texture2);
        }
```
```c#
        public static void Use(ModelTexturePaths paths)
        {
            _textureHeap[paths._DiffusePath].Use();
            _textureHeap[paths._NormalPath].Use();
            _textureHeap[paths._SpecularPath].Use();
        }
```
Во фрагментном шейдере SpecularColor умножается еще на тексел карты отражения.
```c
SpecularColor = vec4(Light.Color, 1.0f) * Light.Intensity * SpecularFactor * texture2D(gMaterial.SpecularMap, texCoord.xy);
```
## Результат
![image](https://github.com/user-attachments/assets/e6c62442-7822-439f-aa81-09d05a58e684)
<a name="s21"></a>
# Тени (Shadow Mapping)
## Теория
Одним из самых распространенных техник построения теней является использование карты теней. Метод реализуется за счет повторного рендеринга сцены с объектами. Для исследования данной техники был выбран такой тип источника света как прожектор, так как он схож с камерой и удобен для построения матрицы проекции.  
Во время первого рендеринга сцена отрисовывается с точки зрения источника света. Все ближайшие к источнику света пиксели попадают в отдельный буфер глубины. Значит мы получим наименьшие значения глубины, которые видно с точки зрения источника света. Текстура, получающаяся в итоге и не имеющая цвета, называется картой теней и связана с одним конкретным источником света. Размер текстуры указывается при создании матрицы проекции света.  
![image](https://github.com/user-attachments/assets/153b8bc0-708b-4060-8dfe-67f118494d99)  
Во время второго рендеринга сцена отрисовывается обычно – с точки зрения камеры. Буфер глубины используется во фрагментном шейдере для получения соответствующего значения глубины для каждого рисуемого пикселя. Например, сначала необходимо перевести пиксель в точке P в пространство источника света. Так как точка P не видна из точки зрения света, её координата z в нашем примере будет 0,9. По координатам точки x, y мы можем заглянуть в карту глубины и узнать, что ближайшая к источнику света точка — C с глубиной 0,4. Это значение меньше, чем для точки P, поэтому точка P находится в тени.  
![image](https://github.com/user-attachments/assets/e153f61b-c408-4ea1-b51d-6cc7013d4dfe)  
## Реализация
Сначала создаётся кадровый буфер для рисования карты глубины и 2D текстуру, чтобы использовать её качестве буфера глубины для кадрового буфера. Здесь устанавливается высота и ширина текстуры и указывается формат текстуры GL_DEPTH_COMPONENT.  
```c#
    public class ShadowMapFBO
    {
        private int m_shadowWidth;
        private int m_shadowHeight;
        private int m_fbo;
        private int m_shadowMap; // фактический буфер глубины
        public ShadowMapFBO(int width = 0, int height = 0)
        {
            m_shadowWidth = width;
            m_shadowHeight = height;
            m_fbo = 0; 
            m_shadowMap = 0;
            Init();
        }
	. . . 
	}
```
Затем необходимо присоединить текстуру глубины к кадровому буферу в качестве буфера глубины.
```c#
	private void Init()
        {
            m_fbo = GL.GenFramebuffer();
            m_shadowMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, m_shadowMap);
            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                PixelInternalFormat.DepthComponent32,
                m_shadowWidth,
                m_shadowHeight, 
                0, 
                PixelFormat.DepthComponent, 
                PixelType.Float, 
                IntPtr.Zero);
	    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, m_shadowMap, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ShadowMapFBO error: " + status.ToString());
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
```
Функция BindForWriting, вызываемая перед первым проходом, необходима для переключения рендера в карту теней. Функция BindForReading, вызываемая перед вторым проходом, необходима для привязывания карты теней для чтения.  
```c#
        public void BindForWriting()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, m_fbo); 
        }
        public void BindForReading(TextureUnit unit) 
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, m_shadowMap);
        }
        public void Dispose()
        {
            GL.DeleteTexture(m_shadowMap);
        }
```
Сначала устанавливаются матрицы проекции перспективы и матрицы пространства камеры для камеры наблюдателя и для источника света отдельно.  
```c#
	var p = new matrix4f();
	p.InitPersProjTransform(120, shadow_size_x, shadow_size_y, 0.1f, 100);
	Matrix4 projMatrixFromLight = p.ToOpenTK();
	Matrix4 projMatrix = mPersProj.PersProjMatrix.ToOpenTK();
	vector3f pos = LightningManager.spotlights[0].PointLight.Position;
	vector3f tar = LightningManager.spotlights[0].Direction;
	matrix4f LightSpacePos = new matrix4f();
	LightSpacePos.InitTranslationTransform(-pos);
	matrix4f LightSpaceTarget = new matrix4f();
	LightSpaceTarget.InitCameraTransform(-tar, vector3f.Up);
	Matrix4 viewMatrixFromLight = (LightSpacePos * LightSpaceTarget).ToOpenTK();
	Matrix4 viewMatrix = (Camera.CameraTranslation * Camera.CameraRotation).ToOpenTK();
	Shader shadowShader = CentralizedShaders.GetShader(ShaderName.ShadowShader);
	Shader normalShader = CentralizedShaders.GetShader(ShaderName.AssimpShader);
```
Сцена рендерится первый раз с точки зрения прожекторного источника света с использованием шейдера теней.  
```c#
	GL.CullFace(CullFaceMode.Front);
	shadowMap.BindForWriting();
	GL.Viewport(0, 0, shadow_size_x, shadow_size_y);
	GL.Clear(ClearBufferMask.DepthBufferBit);
	shadowShader.Use();
	Draw(shadowShader, ball, viewMatrixFromLight, projMatrixFromLight);
	Draw(shadowShader, back, viewMatrixFromLight, projMatrixFromLight);
	Matrix4 wvpMatrixFromLight_ball = ball._pipeline.getWorld() * viewMatrixFromLight * projMatrixFromLight;
	Matrix4 wvpMatrixFromLight_back = back._pipeline.getWorld() * viewMatrixFromLight * projMatrixFromLight;
```
Затем при обычном рендеринге передается в шейдер еще и матрица WVP с точки зрения света.  
```c#
	GL.CullFace(CullFaceMode.Back);
	GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	GL.Viewport(0, 0, WindowWidth, WindowHeight);
	GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
	normalShader.Use();
	shadowMap.BindForReading(TextureUnit.Texture3);
	normalShader.setValue("light_wvp", wvpMatrixFromLight_ball);
	Draw(normalShader, ball, viewMatrix, projMatrix);
	normalShader.setValue("light_wvp", wvpMatrixFromLight_back);
	Draw(normalShader, back, viewMatrix, projMatrix);
```
Шейдерная функция CalcShadowFactor проверяет, находится ли пиксель в тени.  
```c
float CalcShadowFactor(vec4 LightSpacePos)
{
    vec3 ProjCoords = LightSpacePos.xyz / LightSpacePos.w;
    ProjCoords = 0.5 * ProjCoords + 0.5;
    float Depth = texture2D(gShadowMap, ProjCoords.xy).r;
    if (Depth + 0.0001 < ProjCoords.z)
        return 0;
    else
        return 1.0;
}

void main()
{
	. . .
    TotalLight += shadowFactor * CalcSpotLight(gSpotLights[0], Normal);
. . .
}
```
## Результат
![image](https://github.com/user-attachments/assets/6b509d8c-db81-48f0-b02a-dcc9ebf5f88f)  
<a name="s22"></a>
# PCF  
## Теория
Так как карта теней (глубины) имеет постоянное разрешение, часто тексель карты глубины охватывает более одного текселя фрагмента объекта. Это приводит к тому, что несколько текселей фрагмента объекта могут извлекать одно и то же значение из карты глубины, что приводит к появлению этих неровных блочных краев.   
PCF (Percentage Closer Filtering) является одним из методов решения этой проблемы. Самая простая реализация этого метода заключается в том, чтобы взять соседние тексели текущего текселя карты глубины и усреднить результат в текущем текселе.  
## Реализация  
В переменной shadow суммируются текущий тексель карты глубины и 8 соседних. Размытость теней можно настраивать с помощью переменной PСF_Power. Например, при PСF_Power = 2 усредняться будут уже 25 текселей.  
```c
int PСF_Power = 1;
float CalcShadowFactor(vec4 LightSpacePos, sampler2D ShadowMap)
{
    vec3 ProjCoords = LightSpacePos.xyz / LightSpacePos.w;
    ProjCoords = 0.5 * ProjCoords + 0.5;
    float shadow = 0.0;
    vec2 texSize = 1.0 / textureSize(ShadowMap, 0);
    for (int x = -PСF_Power ; x <= PСF_Power ; ++x)
    {
        for (int y = -PСF_Power ; y <= PСF_Power ; ++y)
        {
            float pcfDepth = texture(ShadowMap, ProjCoords.xy + vec2(x, y) * texSize).r;
            shadow += ProjCoords.z <= pcfDepth + 0.00001 ? 0.0 : 1.0;
        }
    }
    shadow /= ((PСF_Power * 2 + 1) * (PСF_Power * 2 + 1));
    return (1 - shadow);
}
```
## Результат
![image](https://github.com/user-attachments/assets/88a53195-6df2-4193-bfb8-3f171115c3e5)
<a name="s23"></a>
# Shadow Mapping для нескольких источников света  
## Реализация  
Для того, чтобы реализовать тени от нескольких источников света необходимо включить в структуру света карту теней и матрицу WVP для источника света. 
```c#
public class Spotlight
{
    public vector3f Direction;
    public float Cutoff1;
    public PointLight PointLight;
    public ShadowMapFBO ShadowMapSpotlight;
    
    public Dictionary<string, Matrix4> wvpMatrixFromLight;
    public Spotlight()
    {
        Direction = new vector3f();
        Cutoff1 = 1;
        PointLight = new PointLight();
        
        ShadowMapSpotlight = new ShadowMapFBO(2048, 2048);
        wvpMatrixFromLight = new Dictionary<string, Matrix4>();
    }
}
```
Для каждого источника света устанавливаются значения сэмплера карты теней.
```c#
private static void SetValuesForSamplers()
{
    SetValue(ShaderName.AssimpShader, "gMaterial.DiffuseMap", 0);
    SetValue(ShaderName.AssimpShader, "gMaterial.NormalMap", 1);
    SetValue(ShaderName.AssimpShader, "gMaterial.SpecularMap", 2);

    for (int i = 0; i < LightningManager.SpotlightsCount; i++)
    {
        SetValue(ShaderName.AssimpShader, "gSpotLights[" + i + "].gShadowMap", 10 + i);
    }
}   
```
В функции создания теней DrawShadows() для каждого источника света строятся матрица проекции и матрица вида с указанием свойств spotlight. Далее все объекты рисуются с точки зрения источника света в буфер глубины этого источника. 
```c#
public static void DrawShadows()
{
    shadowShader.Use();

    foreach (Spotlight spotlight in LightningManager.spotlights)
    {
        int shadowMapSize = spotlight.ShadowMapSpotlight.Size;
        // CREATE MATRICES
        Matrix4 projMatrixFromLight = matrix4f.GetInitPersProjTransform(100, shadowMapSize, shadowMapSize, 0.1f, 100).ToOpenTK();
        vector3f pos = spotlight.PointLight.Position;
        vector3f up = vector3f.Cross(tar, vector3f.Right);
				Matrix4 viewMatrixFromLight = (matrix4f.GetInitTranslationTransform(-pos) * matrix4f.GetInitCameraTransform(-tar, -up)).ToOpenTK();
        // RENDER SHADOWS 
        spotlight.ShadowMapSpotlight.BindForWriting();
        GL.Viewport(0, 0, shadowMapSize, shadowMapSize);
        GL.Clear(ClearBufferMask.DepthBufferBit);
        if (spotlight.wvpMatrixFromLight.Count > 0)
            spotlight.wvpMatrixFromLight.Clear();
        foreach (string obj_name in obj_list.Keys)
        {
            obj_list[obj_name].Draw(shadowShader, viewMatrixFromLight, projMatrixFromLight);
            spotlight.wvpMatrixFromLight.Add(obj_name, obj_list[obj_name]._pipeline.getWorld() * viewMatrixFromLight * projMatrixFromLight);
        }
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
}
```
В функции отрисовки объектов на экране DrawScene() сначала все карты теней, записанные раннее в буфер глубины, считываются для отправки в шейдер. При отрисовке каждого объекта его матрица отображения WVP с точки зрения каждого источника света отправляется в шейдер.
```c#
public static void DrawScene()
{
    normalShader.Use();
    for (int i = 0; i < LightningManager.SpotlightsCount; i++)
    {
        LightningManager.spotlights[i].ShadowMapSpotlight.BindForReading(TextureUnit.Texture10 + i);
    }
    foreach (string obj_name in obj_list.Keys)
    {
        for (int i = 0; i < LightningManager.SpotlightsCount; i++)
        {
            if (LightningManager.spotlights[i].wvpMatrixFromLight.Count > 0)
                normalShader.setValue("gSpotLights[" + i + "].LightWVP", LightningManager.spotlights[i].wvpMatrixFromLight[obj_name]);
        }
        obj_list[obj_name].Draw(normalShader);
    }
}  
```
Во фрагментном шейдере структура источника света теперь имеет карту теней и матрицу отображения WVP текущего рисуемого объекта. 
```c
struct SpotLight
{
    PointLight Base;
    vec3 Direction;
    float Cutoff1;
    
    sampler2D gShadowMap;
    mat4 LightWVP;
};  
```
Функция CalcShadowFactor, проверяющая, находится ли пиксель в тени, имеет мягкие тени благодаря методу PCF и разбиралась в предыдущем радзеле. Теперь вызов этой функции происходит непосредственно в функции вычисления света от источника света этого типа.
```c
vec4 CalcSpotLight(SpotLight sLight, vec3 Normal) 
{
    vec3 LightToPixel = normalize(WorldPos0 - sLight.Base.Position);
    float SpotFactor = dot(LightToPixel, sLight.Direction);
    if (SpotFactor > sLight.Cutoff1)
    {
        float shadow = CalcShadowFactor(Position0 * sLight.LightWVP, sLight.gShadowMap);
        vec4 Color = shadow * CalcPointLight(sLight.Base, Normal);
        return Color * (1.0 - (1.0 - SpotFactor) * 1.0 / (1.0 - sLight.Cutoff1));
    }

    return vec4(0, 0, 0, 0);
}
```
## Реультат  
![image](https://github.com/user-attachments/assets/906e0b7d-87c4-4f43-9a9b-c089ce32595e)  
<a name="s24"></a>
# 3D выбор  
## Реализация  
Выбор трехмерного объекта с помощью курсора или прицела необходим для интерактивного взаимодействия с экспонатами виртуального музея. Реализация 3D выбора начинается с создания класса SelectingMapFBO, включающего в себя карту глубины m_depthMap, кадровый буфер m_fbo для рисования m_depthMap и буфера цвета для хранения информации визуализированных треугольников. 
```c#
public class SelectingMapFBO
{
    private int m_fbo;
    private int m_selectMap;
    private int m_depthMap;
    public SelectingMapFBO()
    {
        m_fbo = 0;
        m_selectMap = 0;
        m_depthMap = 0;
    }
    . . .
```
В функции инициализации Init сначала создается FBO. 
```c#
public void Init(int WindowWidth, int WindowHeight)
{
    m_fbo = GL.GenFramebuffer();
    GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_fbo);
    m_selectMap = GL.GenTexture();
    GL.BindTexture(TextureTarget.Texture2D, m_selectMap);
    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32ui, WindowWidth, WindowHeight, 0, PixelFormat.RgbInteger, PixelType.UnsignedInt, IntPtr.Zero);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
    GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, m_selectMap, 0);
    m_depthMap = GL.GenTexture();
    GL.BindTexture(TextureTarget.Texture2D, m_depthMap);
    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, WindowWidth, WindowHeight, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
    GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, m_depthMap, 0);
    var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
    if (status != FramebufferErrorCode.FramebufferComplete) Console.WriteLine("ShadowMapFBO error: " + status.ToString());
    GL.BindTexture(TextureTarget.Texture2D, 0);
    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
}
```
Далее происходит инициализация объекта текстуры для буфера с информацией о примитиве. Потом инициализируется объект текстуры для буфера глубины. А в конце происходит проверка успеха инициализации и развязка от текстуры и буфера глубины. Функции Enable() и Disable() используются для включения и отключения записи в буфер глубины. 
```c#
public void Enable()
{
    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, m_fbo);
}

public void Disable()
{
    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
}
```
Функция ReadPixel необходима для получения информации об объекте из пикселя в центре экрана. Вместо информации о цвете объекта RGB пиксель содержит информацию об индексе объекта, его MeshEntry и номер примитива.
```c#
public unsafe PixelInfo ReadPixel(int x, int y)
{
    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, m_fbo);
    GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

    PixelInfo[] pixels = new PixelInfo[1];

    pixels[0] = new PixelInfo();
    
    GL.ReadPixels(x, y, 1, 1, PixelFormat.RgbInteger, PixelType.UnsignedInt, pixels);

    GL.ReadBuffer(ReadBufferMode.None);
    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);

    return pixels[0];
}
public struct PixelInfo
{
    public int ObjectID;
    public int DrawID;
    public int PrimID;
    public PixelInfo()
    {
        ObjectID = 0;
        DrawID = 0;
        PrimID = 0;
    }
}
```
В вершинном шейдере карты выбора вершины просто умножаются на матрицу отображения WVP от камеры. 
```c
#version 410 core

layout (location = 0) in vec3 aPosition;

uniform mat4 wvp;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * wvp;
}
```
А во фрагментном шейдере карты выбора вместо цвета фрагмента на выход поступает информация об объекте.
```c
#version 330 core

out uvec4 FragColor;

uniform int gDrawIndex;

uniform int gObjectIndex;

void main()
{
    FragColor = uvec4(gObjectIndex, gDrawIndex, gl_PrimitiveID + 1, 1.0f);
}
```
Данная функция записывает в карту выбора информацию об объектах на экране и возвращает информацию об объекте, пиксель которого находится в центре экрана. 
```c#
public static SelectingMapFBO.PixelInfo GetSelectedPixel()
{
    selectingShader.Use();
    selectMap.Enable();
    GL.Viewport(0, 0, WindowWidth, WindowHeight);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    int i = 0;
    foreach (AObject obj in obj_list.Values)
    {
        selectingShader.setValue("gObjectIndex", i);
        i++;
        obj.Draw(selectingShader);
    }
    selectMap.Disable();
    return selectMap.ReadPixel(WindowWidth / 2, WindowHeight / 2);
}
```
При отрисовке, когда зажата ЛКМ, в консоль выводится информация об объекте, который находится на прицеле.
```c#
public static void DrawScene(bool isPressed)
{
    if (isPressed)
    {
        SelectingMapFBO.PixelInfo pixel = GetSelectedPixel();

        if (pixel.PrimID != 0)
        {
            switch(pixel.ObjectID)      //логика
            {
                case 0:
                    Console.WriteLine("Музей");
                    break;
                case 1:
                    Console.WriteLine("Скульптура");
                    break;
                case 2:
                    Console.WriteLine("Стол");
                    break;
            }
        }
    }
    . . .
```
## Реультат  
![gif](https://github.com/galeevlxix/game_engine/blob/diplom%2B/Files/Screenshots/3D-ezgif.com-video-to-gif-converter.gif)
<a name="s25"></a> 
# Компилятор 
_Находится в разработке..._
## Теория  
Фактически это не является компилятором, а называется так для простоты. Это скорее мини-язык программирования внутри проекта движка, предназначенный для тестирования функционала графического приложения. "Компилятор" читает строку команды, введенную в консоль, или несколько строк команд, сохраненных в файле, и в реальном времени выполняет соответствующие действия. Например, изменить цвет направленного света **без необходимости менять код и перезапускать приложение**, тратя много времени на ожидание загрузки моделей и текстур.  

В данный момент компилятор может:  
1. `alter` - Установить параметры 3D сцены (Н-р, объектов/света)  
2. `get` - Получить информацию о параметрах 3D сцены (Н-р, камеры/FPS)  
3. `load` - Загрузить команды настройки 3D сцены из файла и выполнить (Н-р, объектов/света)  
4. `save` - Сохранить команды настройки 3D сцены из консоли в файл (Н-р, объектов/света)  
5. `compress` - Сжать файл 3D-модели формата _obj_  
6. `help` - Вызов помощника **CompilerHelper**  

Для более подробного изучения синтаксиса языка обращайтесь к помощнику _CompilerHelper_. Пример использования помощника:
1. Ввод:  
   `help`  
   Вывод:  
   ```
   CompilerHelper > Последующие команды:
	1) alter
	2) get
	3) load
	4) save
	5) compress
   ```
2. Ввод:  
   `help load`  
   Вывод:  
   ```
   CompilerHelper > Последующие команды:
       1) light
       2) object
       3) all
   CompilerHelper:Description > Загрузить команды настройки 3D сцены из файла
   ```
3. Ввод:  
   `help alter light pointlight`  
   Вывод:  
   ```
   CompilerHelper > Последующие команды:
       1) <index> color <r g b> {END_OF_LINE}
       2) <index> intensity <value> {END_OF_LINE}
       3) <index> position <x y z> {END_OF_LINE}
       4) <index> move <x y z> {END_OF_LINE}
       5) <index> constant <value> {END_OF_LINE}
       6) <index> linear <value> {END_OF_LINE}
       7) <index> exp <value> {END_OF_LINE}
   CompilerHelper:Description > Установить параметры точечного источника света
   ```
_{END_OF_LINE}_ - конец строки.  

## Реализация  
### ConsoleCompiler
Класс содержит пути к файлам для сохранения команд для настройки 3D сцены: _light_configuration.txt_ и _object_configuration.txt_. `CompilerHelper` - пользовательский помощник, хранящий дерево всех возможных команд компилятора, доступных для использования.  
```c#
    public static class ConsoleCompiler
    {
        private static string? line = "";
        private static bool isExecuting = false;
        private static string light_configuration_file = "..\\..\\..\\Files\\CompilerFiles\\log_config\\light_configuration.txt";
        private static string object_configuration_file = "..\\..\\..\\Files\\CompilerFiles\\log_config\\object_configuration.txt";

        private static CompilerHelper helper = new CompilerHelper();
```
После загрузки всех компонентов графического приложения запускается компилятор.
```c#
        public static void Run()
        {
            while (!isExecuting)
            {
                line = Console.ReadLine();
            }
        }
```
```c#
        // Загрузка окна
        protected override async void OnLoad()
        {
            . . .
            await Task.Run(() => ConsoleCompiler.Run());
        }
```
Функция выполнения команды. Если введенная строка не пуста, а в данный момент не выполняется другая команда, выполняется команда из консоли. После выполнения команда попадает в словарь `commands`, который хранит каждую команду из консоли, выполненную во время работы приложения, и ее соответствующий уникальный номер (ключ).  

Если новая команда уже есть в словаре, то она заменяет старую команду. Например, если сначала мы установили position для объекта в точке (1, 1, 1), а потом установили position в точке (2, 2, 2). В этом случае нам не нужна старая информация о position для объекта в точке (1, 1, 1).  

Но есть исключения, когда мы двигаем, расширяем или поворачиваем объект. Тогда старые команды удалять нельзя, так как, например, если мы сначала двигали объект на (dx=1, dy=0, dz=0), а потом на (dx=0, dy=0, dz=1), то в конечном итоге мы подвинули объект на (dx=1, dy=0, dz=1).
```c#
	private static Dictionary<string, string> commands = new Dictionary<string, string>();

        private static List<string> prohibited_to_delete = new List<string>()
        {
            "2_4", "2_5", "2_6"
        };
        private static int unique_num = 0;

        public static void Execute()
        {
            if (line != "" && !isExecuting)
            {
                isExecuting = true;
                string feedback = ParseLine(line);
                Console.WriteLine(feedback);
                string[] parts = feedback.Split('#');

                //save commands to logs
                if (parts.Length > 1 && parts[0] != "0")
                {
                    string key = parts[0];
                    for (int i = 1; i < parts.Length - 1; i++)
                    {
                        key += "_" + parts[i];
                    }
                    
                    if(!prohibited_to_delete.Contains(parts[0] + "_" + parts[1]))
                    {
                        if (commands.ContainsKey(key))
                        {
                            commands.Remove(key);
                        }
                        commands.Add(key, line);
                    }
                    else
                    {
                        unique_num++;
                        commands.Add(key + "_" + unique_num, line);
                    }

                }

                line = "";
                isExecuting = false;
            }
        }
```
```c#
        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            . . .
            ConsoleCompiler.Execute();
            . . .
        }
```
В этой функции строка команды разбивается на части (ключевые слова), разделенные пробелами. А затем все эти слова обрабатываются в соответствующих функциях. 
```c#
        private static string ParseLine(string? line)
        {
            line = line.Trim();
            line = line.Replace("  ", " ");
            string[] parts = line.Split(' ');
            if (parts.Length == 0) return "Пустая строка";

            switch (parts[0])
            {
                case "alter":
                    return AlterChoice(parts);
                case "get":
                    return GetChoice(parts);
                case "save":
                    return SaveChoice(parts);
                case "load":
                    return LoadChoice(parts);
                case "compress":
                    return CompressObjFile(parts[1], parts[2]);
                case "#":
                    return "";
                case "help":
                    return helper.PrintCommands(parts);
            }
            return "0# Неизвестное действие";
        }
```
Получить информацию о камере или FPS.  
```c#
        //GET
        private static string GetChoice(string[] parts)
        {
            if (parts.Length == 1) return "0# Незаконченное get действие";
            switch (parts[1])
            {
                case "camera":
                    return GetCameraChoice(parts);
                case "fps":
                    return FPSMeter.Int_FPS + " FPS";
                case "object":
                case "light":
                    return "0# Недоступно";
            }
            return "0# Неизвестное get действие";
        }

        private static string GetCameraChoice(string[] parts)
        {
            if (parts.Length == 2) return "0# Незаконченное get camera действие";

            switch (parts[2])
            {
                case "position":
                    return Camera.Pos.ToStr();
                case "target":
                    return (-Camera.Target).ToStr();
                case "up":
                    return Camera.Up.ToStr();
                case "persproj":
                    return GetPersProj(parts);
            }
            return "0# Неизвестное get camera действие";
        }

        private static string GetPersProj(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное get camera persproj действие";

            switch (parts[3])
            {
                case "fov":
                    return "FOV: " + PersProjMat.GetFOV;
                case "width":
                    return "WIDTH: " + PersProjMat.GetWidth;
                case "height":
                    return "HEIGHT: " + PersProjMat.GetHeight;
                case "znear":
                    return "ZNEAR: " + PersProjMat.GetZNear;
                case "zfar":
                    return "ZFAR: " + PersProjMat.GetZFar;
            }

            return "0# Неизвестное get camera persproj действие";
        }
```
Сохранить команды из `commands` в файлы.
```c#
        //SAVE
        private static string SaveChoice(string[] parts)
        {
            switch (parts[1])
            {
                case "light":
                    using (StreamWriter sw = new StreamWriter(light_configuration_file))
                    {
                        foreach (KeyValuePair<string, string> item in commands)
                        {                            
                            if (item.Key.Split('_')[0] == "1") sw.WriteLine(item.Value);
                        }
                        sw.Close();
                    }
                    return "Команды настройки света сохранены в файл";
                case "object":
                    using (StreamWriter sw = new StreamWriter(object_configuration_file))
                    {
                        foreach (KeyValuePair<string, string> item in commands)
                        {
                            if (item.Key.Split('_')[0] == "2") sw.WriteLine(item.Value);
                        }
                        sw.Close();
                    }
                    return "Команды настройки объектов сохранены в файл";
                case "all":
                    return SaveChoice(new string[] { "save", "light" }) + "\n" + SaveChoice(new string[] { "save", "object" });
            }
            return "0# Неизвестное save действие";
        }
```
Загрузить сохраненные команды из файла и выполнить их. 
```c#
        //LOAD
        private static string LoadChoice(string[] parts)
        {
            switch (parts[1])
            {
                case "light":
                    string? line;
                    using (StreamReader sr = new StreamReader(light_configuration_file))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            ParseLine(line);
                        }
                    }
                    return "Команды настройки света загружены из файла";
                case "object":
                    string? line1;
                    using (StreamReader sr = new StreamReader(object_configuration_file))
                    {
                        while ((line1 = sr.ReadLine()) != null)
                        {
                            ParseLine(line1);
                        }
                    }
                    return "Команды настройки объектов загружены из файла";
                case "all":
                    return LoadChoice(new string[] { "save", "light" }) + "\n" + LoadChoice(new string[] { "save", "object" });
            }
            return "0# Неизвестное load действие";
        }
```
Установить параметры света или объектов, проинициализированных в `LightningManager` и `ObjectArray` соответственно.
```c#
        //ALTER
        private static string AlterChoice(string[] parts)
        {
            if (parts.Length == 1) return "0# Незаконченное alter действие";
            switch (parts[1])
            {
                case "light":
                    return AlterLightChoice(parts);
                case "object":  //material, scale, angle, position
                    if (parts.Length == 2 || parts[2] == null || parts[2] == string.Empty) return "0# Незаконченное alter object действие -> Необходимо указать имя объекта";
                    if (ObjectArray.Count == 0) return "0# Массив Assimp-объектов пуст";
                    if (!ObjectArray.Exists(parts[2])) return "0# Объекта " + parts[2] + " не существует в массиве Assimp-объектов";
                    return AlterObjectChoice(parts, parts[2]);
                case "camera":  //position, target, 
                    return "0# Недоступно";
            }
            return "0# Неизвестное alter действие";
        }

        private static string AlterObjectChoice(string[] parts, string obj)
        {
            if (parts.Length == 3) return "0# Незаконченное alter object действие -> Необходимо указать изменяемый параметр и его значение";
            switch (parts[3])
            {
                //УСТАНОВИТЬ
                case "scale":
                    if (parts.Length == 5)
                    {
                        float value;
                        try
                        {
                            value = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.SetScale(obj, value, value, value);
                        return "2#1#" + obj + "# Масштаб объекта изменен на Scale(" + value + "," + value + "," + value + ")";
                    }
                    else if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.SetScale(obj, x, y, z);
                        return "2#1#" + obj + "# Масштаб объекта изменен на Scale(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " scale действие -> Необходимо ввести корректное значение";
                    }                    
                case "angle":
                    if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.SetAngle(obj, x, y, z);
                        return "2#2#" + obj + "# Угол объекта изменен на Angle(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " angle действие -> Необходимо ввести корректное значение";
                    }
                case "position":
                    if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.SetPosition(obj, x, y, z);
                        return "2#3#" + obj + "# Позиция объекта изменена на Position(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " position действие -> Необходимо ввести корректное значение";
                    }
                //НЕМЕДЛЕННО ОБНОВИТЬ
                case "expand":
                    if (parts.Length == 5)
                    {
                        float value;
                        try
                        {
                            value = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.ExpandImmediately(obj, value, value, value);
                        return "2#4#" + obj + "# Масштаб объекта увеличен на Expand(" + value + "," + value + "," + value + ")";
                    }
                    else if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.ExpandImmediately(obj, x, y, z);
                        return "2#4#" + obj + "# Масштаб объекта увеличен на Expand(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " expand действие -> Необходимо ввести корректное значение";
                    }
                case "rotate":
                    if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.RotateImmediately(obj, x, y, z);
                        return "2#5#" + obj + "# Угол объекта увеличен на Rotate(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " rotate действие -> Необходимо ввести корректное значение";
                    }
                case "move":
                    if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.MoveImmediately(obj, x, y, z);
                        return "2#6#" + obj + "# Позиция объекта изменена на Move(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " move действие -> Необходимо ввести корректное значение";
                    }
            }
            return "0# Неизвестное alter object действие";
        }

        private static string AlterLightChoice(string[] parts)
        {
            if (parts.Length == 2) return "0# Незаконченное alter light действие";

            switch (parts[2])
            {
                case "baselight":
                    return AlterBaseLightChoice(parts);
                case "directionallight":
                    return AlterDirectionalLightChoice(parts);
                case "pointlight":
                    if (int.TryParse(parts[3], out int result))
                    {
                        if (result >= 0 && result < LightningManager.PointlightsCount)
                            return AlterPointLightChoice(parts, result);
                        else return "0# Индекс за пределами массива pointlights";
                    }
                    else return "0# Индекс pointlight должен быть числом";
                case "spotlight":
                    if (int.TryParse(parts[3], out int res))
                    {
                        if (res >= 0 && res < LightningManager.SpotlightsCount)
                            return AlterSpotLightChoice(parts, res);
                        else return "0# Индекс за пределами массива spotlights";
                    }
                    else return "0# Индекс spotlight должен быть числом";
            }
            return "0# Неизвестное alter light действие";
        }

        //ALTER -> BASE LIGHT
        private static string AlterBaseLightChoice(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное alter light действие";

            switch (parts[3])
            {
                case "color":
                    float red;
                    float green;
                    float blue;
                    try
                    {
                        red = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        green = float.Parse(parts[5], CultureInfo.InvariantCulture);
                        blue = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return "0# Не удается преобразовать в числовое значение";
                    }
                    vector3f color = new vector3f(red, green, blue);
                    LightningManager.lightConfig.SetBaseLightColor(color);
                    return "1#1#1# Окружающий свет изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    LightningManager.lightConfig.SetBaseLightIntensity(intensity);
                    return "1#1#2# Яркость окружающего света изменена на " + intensity;
            }
            return "0# Неизвестное alter light действие";
        }

        //ALTER -> DIRECTIONAL LIGHT
        private static string AlterDirectionalLightChoice(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное alter light действие";
            switch (parts[3])
            {
                case "color":
                    float red;
                    float green;
                    float blue;
                    try
                    {
                        red = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        green = float.Parse(parts[5], CultureInfo.InvariantCulture);
                        blue = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return "0# Не удается преобразовать в числовое значение";
                    }
                    vector3f color = new vector3f(red, green, blue);
                    LightningManager.lightConfig.SetDirectionalLightColor(color);
                    return "1#2#1# Напраленный свет изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    LightningManager.lightConfig.SetDirectionalLightIntensity(intensity);
                    return "1#2#2# Яркость направленного света изменена на " + intensity;
                case "direction":
                    float x;
                    float y;
                    float z;
                    try
                    {
                        x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                        z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return "0# Не удается преобразовать в числовое значение";
                    }
                    vector3f dir = new vector3f(x, y, z);
                    LightningManager.lightConfig.SetDirectionalLightDirection(dir);
                    return "1#2#3# Направление света изменено на Direction(" + x + ", " + y + ", " + z + ")";
            }
            return "0# Неизвестное alter light действие";
        }

        //ALTER POINT LIGHT
        private static string AlterPointLightChoice(string[] parts, int index)
        {
            if (parts.Length == 4) return "0# Незаконченное alter light действие";
            switch (parts[4])
            {
                case "position":
                    float x = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].SetPosition(x, y, z);
                    return "1#4#1#" + index + "# Позиция точечного света " + index + " изменена на Position(" + x + ", " + y + ", " + z + ")";
                case "move":
                    float x1 = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y1 = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z1 = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Move(x1, y1, z1);
                    return "1#4#2#" + index + "# Позиция точечного света " + index + " смещена на +Position(" + x1 + ", " + y1 + ", " + z1 + ")";
                case "color":
                    float red = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float green = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float blue = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].SetColor(red, green, blue);
                    return "1#4#3#" + index + "# Цвет точечного света " + index + " изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].SetIntensity(intensity);
                    return "1#4#4#" + index + "# Интенсивность точечного света " + index + " изменена на " + intensity;
                case "constant":
                    float constant = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Attenuation.Constant = constant;
                    return "1#4#5#" + index + "# Постоянное затухание точечного света " + index + " изменена на " + constant;
                case "linear":
                    float linear = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Attenuation.Linear = linear;
                    return "1#4#6#" + index + "# Линейное затухание точечного света " + index + " изменена на " + linear;
                case "exp":
                    float exp = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Attenuation.Exp = exp;
                    return "1#4#7#" + index + "# Экспоненциальное затухание точечного света " + index + " изменена на " + exp;
            }
            return "0# Неизвестное alter light действие";
        }

        //ALTER SPOT LIGHT
        private static string AlterSpotLightChoice(string[] parts, int index)
        {
            if (parts.Length == 4) return "0# Незаконченное alter light действие";
            switch (parts[4])
            {
                case "position":
                    float x = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.SetPosition(x, y, z);
                    return "1#5#1#" + index + "# Позиция прожекторного света " + index + " изменена на Position(" + x + ", " + y + ", " + z + ")";
                case "move":
                    float x1 = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y1 = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z1 = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Move(x1, y1, z1);
                    return "1#5#2#" + index + "# Позиция прожекторного света " + index + " смещена на +Position(" + x1 + ", " + y1 + ", " + z1 + ")";
                case "color":
                    float red = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float green = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float blue = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.SetColor(red, green, blue);
                    return "1#5#3#" + index + "# Цвет прожекторного света " + index + " изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.SetIntensity(intensity);
                    return "1#5#4#" + index + "# Интенсивность прожекторного света " + index + " изменена на " + intensity;
                case "constant":
                    float constant = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Attenuation.Constant = constant;
                    return "1#5#5#" + index + "# Постоянное затухание прожекторного света " + index + " изменена на " + constant;
                case "linear":
                    float linear = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Attenuation.Linear = linear;
                    return "1#5#6#" + index + "# Линейное затухание прожекторного света " + index + " изменена на " + linear;
                case "exp":
                    float exp = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Attenuation.Exp = exp;
                    return "1#5#7#" + index + "# Экспоненциальное затухание прожекторного света " + index + " изменена на " + exp;
                case "direction":
                    float x2 = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y2 = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z2 = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    vector3f dir = new vector3f(x2, y2, z2);
                    dir.Normalize();
                    LightningManager.spotlights[index].Direction = dir;
                    return "1#5#8#" + index + "# Направление прожекторного света изменено на Direction(" + x2 + ", " + y2 + ", " + z2 + ")";
                case "cutoff":
                    float cutoff = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].Cutoff1 = cutoff;
                    return "1#5#9#" + index + "# Cutoff прожекторного света " + index + " изменен на " + cutoff;
            }
            return "0# Неизвестное alter light действие";
        }
```
Для компрессии файла 3D-модели формата _obj_. Новый файл создается из старого файла, из которого удаляются неиспользуемые вершины, координаты текстур и нормали.
```c#
        //функция для создания нового скомпрессированного файла obj. UPD: Больше не используется.
        private static string CompressObjFile(string oldFilePath, string newFilePath)
        {
            string? line;

            string faceSector = "";
            string verticesSector = "";

            List<string> old_vertices = new List<string>();
            List<string> old_text_cords = new List<string>();
            List<string> old_normals = new List<string>();

            using (TextReader reader = new StreamReader(oldFilePath))
            {
                bool exit = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (exit) break;

                    line = line.Trim();
                    line = line.Replace("  ", " ");

                    string[] parts = line.Split(' ');
                    switch (parts[0])
                    {
                        case "v":
                            old_vertices.Add(line);
                            break;
                        case "vt":
                            old_text_cords.Add(line);
                            break;
                        case "vn":
                            old_normals.Add(line);
                            break;
                        case "f":
                            //exit = true;
                            break;
                    }
                }
            }

            Dictionary<string, int> vert = new Dictionary<string, int>();
            Dictionary<string, int> text = new Dictionary<string, int>();
            Dictionary<string, int> norm = new Dictionary<string, int>();

            int i = 1;
            foreach (string vert_line in old_vertices)
            {
                if (!vert.ContainsKey(vert_line))
                {
                    vert.Add(vert_line, i++);
                }
            }

            i = 1;
            foreach (string text_line in old_text_cords)
            {
                if (!text.ContainsKey(text_line))
                {
                    text.Add(text_line, i++);
                }
            }

            i = 1;
            foreach (string norm_line in old_normals)
            {
                if (!norm.ContainsKey(norm_line))
                {
                    norm.Add(norm_line, i++);
                }
            }

            using (TextReader reader = new StreamReader(oldFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    string[] parts = line.Split(' ');
                    switch (parts[0])
                    {
                        case "v":
                        case "vt":
                        case "vn":
                            break;
                        case "f":
                            line = line.Trim('f').Trim();
                            string[] f_parts = line.Split(' ');
                            string newline = "f";

                            foreach (string f_part in f_parts)
                            {
                                string[] _f = f_part.Split('/');

                                int v_ind = vert[old_vertices[int.Parse(_f[0]) - 1]];
                                int t_ind = text[old_text_cords[int.Parse(_f[1]) - 1]];
                                int n_ind = norm[old_normals[int.Parse(_f[2]) - 1]];

                                newline += " " + v_ind + "/" + t_ind + "/" + n_ind;
                            }
                            faceSector += newline + "\n";
                            break;

                        case "g":       //сохранить в faceSector
                        case "s":
                        case "usemtl":
                            faceSector += line + "\n";
                            break;
                        default:
                            verticesSector += line + "\n";
                            break;
                    }
                }
            }

            File.Delete(newFilePath);

            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                sw.WriteLine(verticesSector);

                foreach (string v_line in vert.Keys)
                {
                    sw.WriteLine(v_line);
                }

                foreach (string t_line in text.Keys)
                {
                    sw.WriteLine(t_line);
                }

                foreach (string n_line in norm.Keys)
                {
                    sw.WriteLine(n_line);
                }

                sw.WriteLine(faceSector);

                sw.Close();
            }

            return "Файл размером " + (new FileInfo(oldFilePath).Length / 1024).ToString()
                + "KB сжат в файл размером " + (new FileInfo(newFilePath).Length / 1024).ToString() + "KB";
        }
```
### CompilerHelper
Данный класс хранит всё дерево команд, описанное в _ConsoleCompiler_, в корневом узле `root`. Каждый узел имеет ключевое слово команды, его описание и множество последующих узлов (ключевых слов, следующих после данного). 
```c#
    public class CompilerHelper
    {
        private CommandNode root;
        private const string command_list_file = "..\\..\\..\\Files\\CompilerFiles\\compiler_command_list.txt";
        StreamReader sr;

        public CompilerHelper()
        {
            root = new CommandNode();
            root.name = "root";
            sr = new StreamReader(command_list_file);
            root.commandNodes = InitNodes();
            sr.Close();
        }

        private class CommandNode
        {
            public string? name;
            public string? description;
            public List<CommandNode> commandNodes = new List<CommandNode>();
        }
```
Все дерево команд с описаниями каждого ключевого слова хранится в файле _compiler_command_list.txt_.
```c
alter
[D]
Установить параметры 3D сцены
{
    light
    [D]
    Установить параметры различных типов света
    {
        baselight 
        [D]
        Установить параметры окружающего света
        {
            color <r g b> {END_OF_LINE}
            intensity <value> {END_OF_LINE}
        }
        directionallight
        [D]
        Установить параметры направленного света
        {
            color <r g b> {END_OF_LINE}
            intensity <value> {END_OF_LINE}
            direction <dir.x dir.y dir.z> {END_OF_LINE}
        }
        pointlight 
        [D]
        Установить параметры точечного источника света
        {
            <index> color <r g b> {END_OF_LINE}
            <index> intensity <value> {END_OF_LINE}
            <index> position <x y z> {END_OF_LINE}
            <index> move <x y z> {END_OF_LINE}
            <index> constant <value> {END_OF_LINE}
            <index> linear <value> {END_OF_LINE}
            <index> exp <value> {END_OF_LINE}
        }
        spotlight
        [D]
        Установить параметры направленного источника света
        {
            <index> color <r g b> {END_OF_LINE}
            <index> intensity <value> {END_OF_LINE}
            <index> position <x y z> {END_OF_LINE}
            <index> move <x y z> {END_OF_LINE}
            <index> constant <value> {END_OF_LINE}
            <index> linear <value> {END_OF_LINE}
            <index> exp <value> {END_OF_LINE}
            <index> direction <dir.x dir.y dir.z> {END_OF_LINE}
            <index> cutoff {END_OF_LINE}
        }
    }
    object
    [D]
    Установить параметры объекта
    {
        <object_name> scale <x y z> {END_OF_LINE}
        <object_name> scale <value> {END_OF_LINE}
        <object_name> angle <x y z> {END_OF_LINE}
        <object_name> position <x y z> {END_OF_LINE}
        <object_name> expand <x y z> {END_OF_LINE}
        <object_name> expand <value> {END_OF_LINE}
        <object_name> rotate <x y z> {END_OF_LINE}
        <object_name> move <x y z> {END_OF_LINE}
        material 
        [D]
        Установить параметры материала объекта
        {
            --Недоступно
        }
    }
    camera
    [D]
    Установить параметры камеры 
    {
        --Недоступно
    }
}
get
[D]
Получить информацию о параметрах 3D сцены
{
    light
    [D]
    Получить сведения об типе освещения
    {
        --Недоступно
    }
    object
    [D]
    Получить сведения об объекте
    {
        --Недоступно
    }
    camera
    [D]
    Получить сведения о камере
    {
        position
        [D]
        Получить вектор позиции камеры
        {
            {END_OF_LINE}
        }
        target
        [D]
        Получить вектор направления камеры
        {
            {END_OF_LINE}
        }
        up 
        [D]
        Получить вектор Up камеры
        {
            {END_OF_LINE}
        }
        persproj
        [D]
        Получить сведения о проекции перспективы камеры
        {
            fov
            [D]
            Получить величину поля зрения камеры
            {
                {END_OF_LINE}
            }
            width
            [D]
            Получить ширину камеры
            {
                {END_OF_LINE}
            }
            height
            [D]
            Получить высоту камеры
            {
                {END_OF_LINE}
            }
            znear
            [D]
            Получить близкую границу отсечения сцены
            {
                {END_OF_LINE}
            }
            zfar
            [D]
            Получить дальнюю границу отсечения сцены
            {
                {END_OF_LINE}
            }
        }
    }
    fps
    [D]
    Получить значение FPS
    {
        {END_OF_LINE}
    }
}
load
[D]
Загрузить команды настройки 3D сцены из файла
{
    light
    [D]
    Загрузить команды настройки света из файла
    {
        {END_OF_LINE}
    }
    object
    [D]
    Загрузить команды настройки объектов из файла
    {
        {END_OF_LINE}
    }
    all
    [D]
    Загрузить все команды настройки 3D сцены из файла
    {
        {END_OF_LINE}
    }
}
save
[D]
Сохранить команды настройки 3D сцены в файл
{
    light
    [D]
    Сохранить команды настройки света в файл
    {
        {END_OF_LINE}
    }
    object
    [D]
    Сохранить команды настройки объектов в файл
    {
        {END_OF_LINE}
    }
    all
    [D]
    Сохранить все команды настройки 3D сцены в файл
    {
        {END_OF_LINE}
    }
}
compress
[D]
Сжать файл 3D-модели формата obj
{
    <old_file_name> <new_file_name> {END_OF_LINE}
}
```
Функция `InitNodes` рекурсивно инициализирует каждый узел дерева, читая файл.
```c#
        private List<CommandNode> InitNodes()
        {
            List<CommandNode> child_nodes = new List<CommandNode>();
            while (true)
            {
                string? line = sr.ReadLine();
                if (line == null) return child_nodes;
                line = line.Trim();

                switch (line)
                {
                    case "{":
                        if (child_nodes.Count > 0) child_nodes[child_nodes.Count - 1].commandNodes = InitNodes();
                        break;
                    case "}":
                        return child_nodes;
                    case "[D]":
                        if (child_nodes.Count > 0)
                        {
                            line = sr.ReadLine();
                            line = line.Trim();
                            child_nodes[child_nodes.Count - 1].description = line;
                        }
                        break;
                    default:
                        CommandNode chn = new CommandNode();
                        chn.name = line;
                        child_nodes.Add(chn);
                        break;
                }
            }
        }
```
Функция `PrintCommands` принимает на вход команду типа _help ..._ и начинает поиск команд в поддереве соответствующего узла. Если команда типа _help_, то поиск производится в корневом узле.
```c#
        public string PrintCommands(string[] parts)
        {
            if (parts.Length == 1)
            {
                string output = "CompilerHelper > Последующие команды:";
                int i = 0;
                foreach (CommandNode node in root.commandNodes)
                {
                    i++;
                    output += "\n" + i + ") " + node.name;
                }
                return output.Trim(' ').Trim(',');
            }
            return FindCommands(parts, 1, root);
        }
```
Функция `FindCommands` рекурсивно ищет ключевые слова (если они есть), следующие после данного слова. То есть проходится по всем веткам данного узла. В конечном итоге выводит последующие ключевые слова и описание данной подкоманды.
```c#
        private string FindCommands(string[] parts, int index, CommandNode parrent_node)
        {
            foreach(CommandNode child_node in parrent_node.commandNodes)
            {
                if (child_node.name == parts[index])
                {
                    if (parts.Length == index + 1)
                    {
                        if (child_node.commandNodes.Count == 0)
                        {
                            return "CompilerHelper > " + parts[index] + " является настраиваемым параметром." + (child_node.description != null ? "\nCompilerHelper > " + child_node.description : null);
                        }
                        string output = "CompilerHelper > Последующие команды:";
                        int i = 0;
                        foreach (CommandNode node in child_node.commandNodes)
                        {
                            i++;
                            output += "\n       " + i + ") " + node.name ;
                        }
                        return output.Trim(' ').Trim(',') + (child_node.description != null ? "\nCompilerHelper:Description > " + child_node.description : null);
                    }
                    else
                    {
                        return FindCommands(parts, index + 1, child_node);
                    }
                }
            }
            return "CompilerHelper > " + "Команды " + parts[index] + " не обнаружено.";
        }
```
## Реультат  
### ConsoleCompiler
![gif](https://github.com/galeevlxix/game_engine/blob/diplom%2B/Files/Screenshots/bandicam2024-07-1823-07-44-491-ezgif.com-video-to-gif-converter.gif)
### CompilerHelper
![gif](https://github.com/galeevlxix/game_engine/blob/diplom%2B/Files/Screenshots/bandicam2024-07-1823-08-22-069-ezgif.com-video-to-gif-converter.gif)
