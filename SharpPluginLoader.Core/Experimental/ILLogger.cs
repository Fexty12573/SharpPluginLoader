using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Experimental
{
    internal class ILLogger
    {
        private readonly ILGenerator _gen;

        public ILLogger(ILGenerator gen) => _gen = gen;

        public void Emit(OpCode opcode)
        {
            Log.Info($"{opcode}");
            _gen.Emit(opcode);
        }

        public void Emit(OpCode opcode, FieldInfo field)
        {
            Log.Info($"{opcode} {field}");
            _gen.Emit(opcode, field);
        }

        public void Emit(OpCode opcode, MethodInfo method)
        {
            Log.Info($"{opcode} {method}");
            _gen.Emit(opcode, method);
        }

        public void Emit(OpCode opcode, ConstructorInfo con)
        {
            Log.Info($"{opcode} {con}");
            _gen.Emit(opcode, con);
        }

        public void Emit(OpCode opcode, int pos)
        {
            Log.Info($"{opcode}.{pos}");
            _gen.Emit(opcode, pos);
        }
    }
}
