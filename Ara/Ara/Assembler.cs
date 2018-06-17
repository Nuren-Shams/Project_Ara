using System.Linq;

namespace Ara {
    public static class Assembler {
        //Exhaustive list of the tokens that are permissible for use in the assembly language.
        private static readonly string AuthorizedTokens = "ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789($)";
        private static readonly int RegisterDataWidth = 8; //In bits.
        private static readonly int RegisterAddressWidth = 3; //In bits.
        private static readonly int MemoryAddressWidth = 12; //In bits.
        private static readonly int InstructionWidth = 16; //In bits.

        private static string[][] InstructionTemplate;
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

            InstructionTemplate = new string[BulkData.Length][];
            for(int Iterator=0; Iterator<BulkData.Length; Iterator++) {
                InstructionTemplate[Iterator] = new string[BulkData[Iterator].Split(' ').Length];
                for(int SubIterator=0; SubIterator<InstructionTemplate[Iterator].Length; SubIterator++) {
                    InstructionTemplate[Iterator][SubIterator] = (string)BulkData[Iterator].Split(' ').GetValue(SubIterator);
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

        private static string GetInstructionType(string Mnemonic) {
            string Type = "";
            for(int Iterator=0; Iterator<(InstructionType.Length/2); Iterator++) {
                if (Mnemonic==InstructionType[Iterator,1]) {
                    Type = InstructionType[Iterator, 0];
                    break;
                }
            }
            return Type;
        }

        private static string GetOpCode(string Mnemonic) {
            string OpCode = "";
            for(int Iterator=0; Iterator<(OCMap.Length/2); Iterator++) {
                if (Mnemonic==OCMap[Iterator,1]) {
                    OpCode = OCMap[Iterator, 0];
                    break;
                }
            }
            return OpCode;
        }

        private static int ParseOperandCount(string Input) {
            string NumericTokens = "0123456789";
            System.Text.StringBuilder InputBuilder = new System.Text.StringBuilder(Input);
            for(int Iterator=0; Iterator<InputBuilder.Length; Iterator++) {
                if (!NumericTokens.Contains(InputBuilder[Iterator])) { InputBuilder[Iterator] = ' '; }
            }
            Input = InputBuilder.ToString();
            Input = Input.Trim();
            return System.Int32.Parse(Input);
        }

        private static string GetRegisterAddress(string RegisterName) {
            string RegisterAddress = "";
            for(int Iterator=0; Iterator<(RFMap.Length/2); Iterator++) {
                if (RegisterName==RFMap[Iterator,1]) {
                    RegisterAddress = RFMap[Iterator, 0];
                    break;
                }
            }
            return RegisterAddress;
        }

        private static string BinaryExtension(string BinaryNumber, int Length) {
            if ((Length-BinaryNumber.Length)<=0) { return BinaryNumber; }
            else { return BinaryExtension(("0"+BinaryNumber),Length); }
        }

        private static string GetFuncCode(string Mnemonic) {
            string FuncCode = "";
            for(int Iterator=0; Iterator<(FCMap.Length/2); Iterator++) {
                if (Mnemonic==FCMap[Iterator,1]) {
                    FuncCode = FCMap[Iterator, 0];
                    break;
                }
            }
            return FuncCode;
        }

        public static int Assemble() {
            /*\
             * Return Codes
             * ------------
             * 
             *  0: No Errors
             *  1: Assembly.txt couldn't be read
             *  2: unrecognized mnemonic
             *  3: Too few operands
             *  4: Too many operands
             *  5: Operand mismatch
             *  6: Unrecognized register name
             *  7: Invalid radix of immediate value
             *  8: Invalid immediate value
             *  9: Immediate value overflow
             * 10: Instruction width mismatch
            \*/
            LoadInstructionTemplate();
            LoadInstructionType();
            LoadOCMap();
            LoadFCMap();
            LoadRFMap();

            string[] RawAssemblyCode;

            try {
                RawAssemblyCode = System.IO.File.ReadAllLines("Assembly.txt");
            } catch(System.Exception EXCEPTION) {
                System.Console.WriteLine(EXCEPTION.Message);
                return 1;
            }

            string[] BinaryMachineCode = new string[RawAssemblyCode.Length];
            string[] HexadecimalMachineCode = new string[RawAssemblyCode.Length];
            for (int Iterator=0; Iterator<RawAssemblyCode.Length; Iterator++) {
                string ProcessedAssemblyCode = Format(RawAssemblyCode[Iterator]);
                string Mnemonic = (string)ProcessedAssemblyCode.Split(' ').GetValue(0);
                string Type = GetInstructionType(Mnemonic);

                //Catch mnemonic errors
                string OpCode;
                if (Type=="") {
                    System.Console.WriteLine("ERROR: \"" + Mnemonic + "\" at line {0} is not recognized as a valid mnemonic for a processor instruction.\n-----", (Iterator + 1));
                    return 2;
                } else {
                    /*No Error*/
                    OpCode = GetOpCode(Mnemonic);
                }

                //Catch operand count errors
                int ExpectedOperandCount = ParseOperandCount(Type);
                int ActualOperandCount = ProcessedAssemblyCode.Split(' ').Length - 1;
                if (ActualOperandCount<ExpectedOperandCount) {
                    System.Console.WriteLine("ERROR: Too few operands at line {0}.\n-----", (Iterator + 1));
                    return 3;
                } else if (ActualOperandCount>ExpectedOperandCount) {
                    System.Console.WriteLine("ERROR: Too many operands at line {0}.\n-----", (Iterator + 1));
                    return 4;
                } else {/*No Error*/}

                //Preprocess operands and catch errors
                string[] Operands = new string[ExpectedOperandCount];
                if (ExpectedOperandCount!=0) {
                    for(int SubIterator=0; SubIterator<ExpectedOperandCount; SubIterator++) {
                        Operands[SubIterator] = (string)ProcessedAssemblyCode.Split(' ').GetValue(SubIterator + 1);

                        //Catch operand mismatch errors
                        switch(Type[0]) {
                            case 'R':
                                if (Operands[SubIterator][0]!='$') {
                                    System.Console.WriteLine("ERROR: Operand mismatch at line {0}.\n-----", (Iterator + 1));
                                    return 5;
                                } else {/*No Error*/}
                                break;
                            case 'I':
                                if (InstructionTemplate[1][(SubIterator+2)]=="RegisterName") {
                                    if (Operands[SubIterator][0]!='$') {
                                        System.Console.WriteLine("ERROR: Operand mismatch at line {0}.\n-----", (Iterator + 1));
                                        return 5;
                                    } else {/*No Error*/}
                                } else if (InstructionTemplate[1][(SubIterator+2)]=="ImmediateValue") {
                                    if (Operands[SubIterator][0]!='(') {
                                        System.Console.WriteLine("ERROR: Operand mismatch at line {0}.\n-----", (Iterator + 1));
                                        return 5;
                                    } else {/*No Error*/}
                                }
                                break;
                            case 'J':
                                if (Operands[SubIterator][0]!='(') {
                                    System.Console.WriteLine("ERROR: Operand mismatch at line {0}.\n-----", (Iterator + 1));
                                    return 5;
                                } else {/*No Error*/}
                                break;
                        }

                        //Preprocess register names and catch errors
                        if (Operands[SubIterator][0]=='$') {
                            Operands[SubIterator] = GetRegisterAddress(Operands[SubIterator]);
                            if (Operands[SubIterator]=="") {
                                System.Console.WriteLine("ERROR: Unrecognized register name at line {0}.\n-----", (Iterator + 1));
                                return 6;
                            } else {/*No Error*/}
                        }

                        //Preprocess immediate values and catch errors
                        else if (Operands[SubIterator][0]=='(') {
                            int Radix;
                            try {
                                Radix = System.Int32.Parse(
                                    Operands[SubIterator].Substring(
                                        (Operands[SubIterator].LastIndexOf(')')+1), (Operands[SubIterator].Length-Operands[SubIterator].LastIndexOf(')')-1)
                                    )
                                );
                            } catch(System.FormatException) {
                                System.Console.WriteLine("ERROR: Invalid radix of immediate value at line {0}.\n-----", (Iterator + 1));
                                return 7;
                            }

                            int Value;
                            try {
                                Value = System.Convert.ToInt32(
                                    Operands[SubIterator].Substring(
                                        1, (Operands[SubIterator].LastIndexOf(')')-1)
                                    ), Radix
                                );

                                //Check immediate value overflow
                                if ((OpCode=="0011") || (OpCode=="0100") || (OpCode=="0110")) {
                                    if (Value>=((int)System.Math.Pow(2,RegisterDataWidth))) {
                                        System.Console.WriteLine("ERROR: Immediate value overflow at line {0}. Maximum permissible value is ({1})10\n-----", (Iterator + 1), ((int)System.Math.Pow(2,RegisterDataWidth)-1));
                                        return 9;
                                    } else {/*No Error*/}
                                } else if ((OpCode=="0101") || (OpCode=="1010")) {
                                    if (Value>RegisterDataWidth) {
                                        System.Console.WriteLine("ERROR: Immediate value overflow at line {0}. Maximum permissible value is ({1})10\n-----", (Iterator + 1), RegisterDataWidth);
                                        return 9;
                                    } else {/*No Error*/}
                                } else if (Type[0]=='J') {
                                    if (Value>=((int)System.Math.Pow(2,MemoryAddressWidth))) {
                                        System.Console.WriteLine("ERROR: Immediate value overflow at line {0}. Maximum permissible value is ({1})10\n-----", (Iterator + 1), ((int)System.Math.Pow(2,MemoryAddressWidth)-1));
                                        return 9;
                                    } else {/*No Error*/}
                                }

                                Operands[SubIterator] = System.Convert.ToString(Value, 2);
                            } catch(System.ArgumentException) {
                                System.Console.WriteLine("ERROR: Invalid radix of immediate value at line {0}.\n-----", (Iterator + 1));
                                return 7;
                            } catch(System.FormatException) {
                                System.Console.WriteLine("ERROR: Invalid immediate value at line {0}.\n-----", (Iterator + 1));
                                return 8;
                            }
                        }
                    }
                }

                //Construct the machine codes in binary
                switch(Type[0]) {
                    case 'R':
                        Type = "0";
                        break;
                    case 'I':
                        Type = "1";
                        break;
                    case 'J':
                        Type = "2";
                        break;
                }
                for(int SubIterator=1; SubIterator<InstructionTemplate[System.Int32.Parse(Type)].Length; SubIterator++) {
                    if (InstructionTemplate[System.Int32.Parse(Type)][SubIterator]=="Mnemonic") {
                        BinaryMachineCode[Iterator] = OpCode;
                    } else if (InstructionTemplate[System.Int32.Parse(Type)][SubIterator]=="RegisterName") {
                        try {
                            BinaryMachineCode[Iterator] += Operands[ExpectedOperandCount - ActualOperandCount];
                            ActualOperandCount--;
                        } catch(System.IndexOutOfRangeException) {
                            for(int TertiaryIterator=0; TertiaryIterator<RegisterAddressWidth; TertiaryIterator++) {
                                BinaryMachineCode[Iterator] += "0";
                            }
                        }
                    } else if (InstructionTemplate[System.Int32.Parse(Type)][SubIterator]=="ImmediateValue") {
                        try {
                            BinaryMachineCode[Iterator] += BinaryExtension(Operands[ExpectedOperandCount - ActualOperandCount], (InstructionWidth - BinaryMachineCode[Iterator].Length));
                            ActualOperandCount--;
                        } catch(System.IndexOutOfRangeException) {
                            for(int TertiaryIterator=BinaryMachineCode[Iterator].Length; TertiaryIterator<InstructionWidth; TertiaryIterator++) {
                                BinaryMachineCode[Iterator] += "0";
                            }
                        }
                    } else if (InstructionTemplate[System.Int32.Parse(Type)][SubIterator]=="FuncCode") {
                        BinaryMachineCode[Iterator] += GetFuncCode(Mnemonic);
                    }
                }

                //Catch instruction width mismatch errors
                if (BinaryMachineCode[Iterator].Length!=InstructionWidth) {
                    System.Console.WriteLine("ERROR: Instruction width mismatch detected at line {0}.\n-----  The assembler’s reference libraries are corrupted.", (Iterator + 1));
                    return 10;
                }

                //Construct the machine codes in hexadecimal
                HexadecimalMachineCode[Iterator] = System.String.Format("{0:X}", (System.Convert.ToInt32(BinaryMachineCode[Iterator], 2)));
            }

            //Preprocess the generated machine codes
            string BulkBinaryMachineCode = "";
            string BulkHexadecimalMachineCode = "v2.0 raw\n";
            for(int Iterator=0; Iterator<RawAssemblyCode.Length; Iterator++) {
                BulkBinaryMachineCode += BinaryMachineCode[Iterator];
                BulkHexadecimalMachineCode += HexadecimalMachineCode[Iterator];
                if (Iterator!=(RawAssemblyCode.Length-1)) {
                    BulkBinaryMachineCode += "\n";
                    BulkHexadecimalMachineCode += "\n";
                }
            }

            //Output the machine codes to files
            System.IO.File.WriteAllText("BinaryMachineCode.txt", BulkBinaryMachineCode);
            System.IO.File.WriteAllText("HexadecimalMachineCode.hex", BulkHexadecimalMachineCode);

            return 0;
        }
    }
}