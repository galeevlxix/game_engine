using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public static class Storage
    {
        public static string vertexShader =
            "#version 330                                           \n" +
            "layout (location = 0) in vec3 aPosition;               \n" +
            "out vec4 vertexColor;                                  \n" +
            "uniform mat4 mvp;                                      \n" +
            "void main()                                            \n" +
            "{                                                      \n" +
            "   gl_Position = vec4(aPosition, 1.0) * mvp;           \n" +
            "   vertexColor = vec4(clamp(aPosition, 0.0, 1.0), 1.0);\n" +
            "}";

        public static string fragmentShader =
            "#version 330                                           \n" +
            "in vec4 vertexColor;                                   \n" +
            "void main() { gl_FragColor = vertexColor; }            \n";

        public static float[] cubeVertices = new float[]{    //куб
                  0.5f, -0.5f, -0.5f,
                  0.5f, -0.5f,  0.5f,
                 -0.5f, -0.5f,  0.5f,
                 -0.5f, -0.5f, -0.5f,
                  0.5f,  0.5f, -0.5f,
                  0.5f,  0.5f,  0.5f,
                 -0.5f,  0.5f,  0.5f,
                 -0.5f,  0.5f, -0.5f
                };
        public static int[] cubeIndices = new int[]
        {
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
    }
}
