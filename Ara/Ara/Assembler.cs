using System.Linq;

namespace Ara {
    public static class Assembler {
        //Exhaustive list of the tokens that are permissible for use in the assembly language.
        private static readonly string AuthorizedTokens = "ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789($)";

        private static string[,] InstructionTemplate;
        private static string[,] InstructionType;
        private static string[,] OCMap;
        private static string[,] FCMap;
        private static string[,] RFMap;

        //Loads the instruction templates into memory.
        private static bool LoadInstructionTemplate() {
            string[] BulkData;

            try {
                BulkData = System.IO.File.ReadAllLines("Instruction.template");
            } catch(System.Exception EXCEPTION) {
                System.Console.WriteLine(EXCEPTION.Message);
                return false;
            }

            int MaxElementCount = System.Int32.MinValue;
            for(int Iterator=0; Iterator<BulkData.Length; Iterator++) {
                if (BulkData[Iterator].Split(' ').Length>MaxElementCount) { MaxElementCount = BulkData[Iterator].Split(' ').Length; }
            }

            InstructionTemplate = new string[BulkData.Length, MaxElementCount];
            for(int RowIterator=0; RowIterator<BulkData.Length; RowIterator++) {
                for(int ColumnIterator=0; ColumnIterator<MaxElementCount; ColumnIterator++) {
                    try {
                        InstructionTemplate[RowIterator, ColumnIterator] = (string)BulkData[RowIterator].Split(' ').GetValue(ColumnIterator);
                    } catch(System.IndexOutOfRangeException) { InstructionTemplate[RowIterator, ColumnIterator] = ""; }
                }
            }

            return true;
        }

        //Loads the list that states the instruction types into memory.
        private static bool LoadInstructionType() {
            string[] BulkData;

            try {
                BulkData = System.IO.File.ReadAllLines("Instruction.type");
            } catch(System.Exception EXCEPTION) {
                System.Console.WriteLine(EXCEPTION.Message);
                return false;
            }

            int MaxElementCount = System.Int32.MinValue;
            for(int Iterator=0; Iterator<BulkData.Length; Iterator++) {
                if (BulkData[Iterator].Split(' ').Length>MaxElementCount) { MaxElementCount = BulkData[Iterator].Split(' ').Length; }
            }

            InstructionType = new string[BulkData.Length, MaxElementCount];
            for(int RowIterator=0; RowIterator<BulkData.Length; RowIterator++) {
                for(int ColumnIterator=0; ColumnIterator<MaxElementCount; ColumnIterator++) {
                    try {
                        InstructionType[RowIterator, ColumnIterator] = (string)BulkData[RowIterator].Split(' ').GetValue(ColumnIterator);
                    } catch(System.IndexOutOfRangeException) { InstructionType[RowIterator, ColumnIterator] = ""; }
                }
            }

            return true;
        }

        //Loads the list that maps the mnemonics to their corresponding OpCodes into memory.
        private static bool LoadOCMap() {
            string[] BulkData;

            try {
                BulkData = System.IO.File.ReadAllLines("OperationCode.map");
            } catch(System.Exception EXCEPTION) {
                System.Console.WriteLine(EXCEPTION.Message);
                return false;
            }

            int MaxElementCount = System.Int32.MinValue;
            for(int Iterator=0; Iterator<BulkData.Length; Iterator++) {
                if (BulkData[Iterator].Split(' ').Length>MaxElementCount) { MaxElementCount = BulkData[Iterator].Split(' ').Length; }
            }

            OCMap = new string[BulkData.Length, MaxElementCount];
            for(int RowIterator=0; RowIterator<BulkData.Length; RowIterator++) {
                for(int ColumnIterator=0; ColumnIterator<MaxElementCount; ColumnIterator++) {
                    try {
                        OCMap[RowIterator, ColumnIterator] = (string)BulkData[RowIterator].Split(' ').GetValue(ColumnIterator);
                    } catch(System.IndexOutOfRangeException) { OCMap[RowIterator, ColumnIterator] = ""; }
                }
            }

            return true;
        }

        //Loads the list that maps the mnemonics to their corresponding FuncCodes into memory.
        private static bool LoadFCMap() {
            string[] BulkData;

            try {
                BulkData = System.IO.File.ReadAllLines("FunctionCode.map");
            } catch(System.Exception EXCEPTION) {
                System.Console.WriteLine(EXCEPTION.Message);
                return false;
            }

            int MaxElementCount = System.Int32.MinValue;
            for(int Iterator=0; Iterator<BulkData.Length; Iterator++) {
                if (BulkData[Iterator].Split(' ').Length>MaxElementCount) { MaxElementCount = BulkData[Iterator].Split(' ').Length; }
            }

            FCMap = new string[BulkData.Length, MaxElementCount];
            for(int RowIterator=0; RowIterator<BulkData.Length; RowIterator++) {
                for(int ColumnIterator=0; ColumnIterator<MaxElementCount; ColumnIterator++) {
                    try {
                        FCMap[RowIterator, ColumnIterator] = (string)BulkData[RowIterator].Split(' ').GetValue(ColumnIterator);
                    } catch(System.IndexOutOfRangeException) { FCMap[RowIterator, ColumnIterator] = ""; }
                }
            }

            return true;
        }

        //Loads the list that maps the register names to their corresponding addresses into memory.
        private static bool LoadRFMap() {
            string[] BulkData;

            try {
                BulkData = System.IO.File.ReadAllLines("RegisterFile.map");
            } catch(System.Exception EXCEPTION) {
                System.Console.WriteLine(EXCEPTION.Message);
                return false;
            }

            int MaxElementCount = System.Int32.MinValue;
            for(int Iterator=0; Iterator<BulkData.Length; Iterator++) {
                if (BulkData[Iterator].Split(' ').Length>MaxElementCount) { MaxElementCount = BulkData[Iterator].Split(' ').Length; }
            }

            RFMap = new string[BulkData.Length, MaxElementCount];
            for(int RowIterator=0; RowIterator<BulkData.Length; RowIterator++) {
                for(int ColumnIterator=0; ColumnIterator<MaxElementCount; ColumnIterator++) {
                    try {
                        RFMap[RowIterator, ColumnIterator] = (string)BulkData[RowIterator].Split(' ').GetValue(ColumnIterator);
                    } catch(System.IndexOutOfRangeException) { RFMap[RowIterator, ColumnIterator] = ""; }
                }
            }

            return true;
        }

        private static string Format(string Instruction) {
            Instruction = Instruction.ToUpper();
            System.Text.StringBuilder InstructionBuilder = new System.Text.StringBuilder(Instruction);
            for(int Iterator=0; Iterator<InstructionBuilder.Length; Iterator++) {
                if (!AuthorizedTokens.Contains(InstructionBuilder[Iterator])) {
                    InstructionBuilder[Iterator] = ' ';
                }
            }
            Instruction = InstructionBuilder.ToString();
            Instruction = System.Text.RegularExpressions.Regex.Replace(Instruction, "\\s+", " ");
            Instruction = System.Text.RegularExpressions.Regex.Replace(Instruction, "\\) ", ")");
            Instruction = Instruction.Trim();
            return Instruction;
        }
    }
}