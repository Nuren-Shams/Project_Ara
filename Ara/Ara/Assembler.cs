using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ara {
    public static class Assembler {
        private static readonly string CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789($)";
        private static string Format(string Instruction) {
            Instruction = Instruction.ToUpper();
            StringBuilder InstructionBuilder = new StringBuilder(Instruction);
            for(int Iterator=0; Iterator<InstructionBuilder.Length; Iterator++) {
                if (!CharacterSet.Contains(InstructionBuilder[Iterator])) {
                    InstructionBuilder[Iterator] = ' ';
                }
            }
            Instruction = InstructionBuilder.ToString();
            Instruction = Regex.Replace(Instruction, "\\s+", " ");
            Instruction = Instruction.Trim();
            return Instruction;
        }
    }
}