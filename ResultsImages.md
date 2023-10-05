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

<a name="s1"></a>
# Первые шаги и треугольник 
## Создание окна
![st1](https://github.com/galeevlxix/game_engine/blob/master/screens/st1.png)
## Треугольник 
![st2](https://github.com/galeevlxix/game_engine/blob/master/screens/tria.png)
<a name="s3"></a> 
# Индексная отрисовка
![st3](https://github.com/galeevlxix/game_engine/blob/master/screens/2tria.png)
<a name="s4"></a> 
# Интерполяционный цвет
![st4](https://github.com/galeevlxix/game_engine/blob/master/screens/interp.png)
<a name="s5"></a> 
# Вращение, перемещение, масштаб
![cube](https://github.com/galeevlxix/game_engine/blob/master/screens/anim.gif)
<a name="s6"></a>
# Добавим класс GameObj
![2obj](https://github.com/galeevlxix/game_engine/blob/master/screens/bandicam%202023-06-28%2016-01-06-972.gif)
<a name="s7"></a>
# Разные форматы 3D-моделей
### .obj
Тут сразу 3 объекта на экране (мужик, машина и пол это все один объект)
![objform](https://github.com/galeevlxix/game_engine/blob/master/screens/scene1.gif)
### .fbx
![fbxform](https://github.com/galeevlxix/game_engine/blob/master/screens/fbxfi.png)
### .ply
![plyform](https://github.com/galeevlxix/game_engine/blob/master/screens/ply.png)
На этом моменте моя работа в рамках летней практики заканчивается. Но я намерен дальше развивать проект и добавлять в него кучу всего.
<a name = "s8"></a>
# Камера
![camera](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/camera%20(1).gif)
<a name = "s9"></a>
# Текстуры
![tex](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/textures.png)
<a name = "s10"></a>
# Нормали
Нормали — это векторы, которые используются для определения того, как свет отражается от поверхности.
![normals](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/bandicam%202023-07-21%2002-51-44-481.gif)
<a name="s11"></a>
# Извлечение текстур и нормалей из obj
![monkey](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/обезьяна.gif)
<a name="s12"></a>
# DeltaTime
![dt](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/DeltaTimeScene%20(online-video-cutter.com).gif)
<a name="s13"></a>
# Скайбокс и прицел
![skb](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/skybox.gif)
<a name="s14"></a>
# Загрузка моделей через Assimp
До библиотеки Assimp мы могли загрузить только одну текстуру на модель, в результате чего некоторые области модели выглядили нелепо, потому что тектура находилась не на своем месте.
![warr](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/warr_man.png)
<a name="s15"></a>
# Информационная панель
### `font.png`:
![font](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/font.png)
![ip](https://github.com/galeevlxix/game_engine/blob/WorkingWithTheModel/screens/info_panel.gif)
<a name="s16"></a>
# Окружающее освещение, рассеянное освещение, отраженный свет
![difli](https://github.com/galeevlxix/game_engine/blob/Light/screens/diflights.jpg)
![spec](https://github.com/galeevlxix/game_engine/blob/Light/screens/specularlight.jpg)
