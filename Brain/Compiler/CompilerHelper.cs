namespace game_2.Brain.Compiler
{
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


        private class CommandNode
        {
            public string? name;
            public string? description;
            public List<CommandNode> commandNodes = new List<CommandNode>();
        }
    }
}
