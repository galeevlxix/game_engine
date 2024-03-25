using game_2.Brain.ObjectFolder;

namespace game_2.Brain.InfoPanelFolder
{
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
            SymbolArrayOfVertices.LoadTexture("..\\..\\..\\Files\\Textures\\font.png");

            for (int i = 0; i < (int)type; i++)
            {
                _symbols.Add(new Symbol(i));

                if (i == 25)
                {
                    Console.WriteLine("     + английские символы");
                }
                else if (i == 35)
                {
                    Console.WriteLine("     + цифры");
                }
                else if (i == 66)
                {
                    Console.WriteLine("     + знаки пунктуации");
                }
                else if (i == 99)
                {
                    Console.WriteLine("     + русские символы");
                }
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
}
