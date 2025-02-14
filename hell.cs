﻿using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetSafer_StringDeobfuscator
{
    public class hell
    {
        #region cflowfix
        private static List<MethodDef> methods = new List<MethodDef>();
        private static Local local_variable = null;
        private static List<int> toberemoved;
        private static List<int> integer_values_1;
        private static List<int> for_rem;
        private static List<Instruction> switchinstructions;
        private static IList<Instruction> instructions;
        private static List<int> toberemovedindex;
        private static List<int> toberemovedvalues;
        private static MethodDef method;
        private static List<Instruction> conditionalinstructions;
        private static List<Instruction> brinstructions;
        private static List<Instruction> realbrinstructions;
        private static List<int> local_values;

        public static void AddMethods(TypeDef type)
        {
            if (type.HasMethods)
            {
                foreach (MethodDef methodDef in type.Methods)
                {
                    if (methodDef.HasBody)
                    {
                        methods.Add(methodDef);
                    }
                }
            }
            if (type.HasNestedTypes)
            {
                foreach (TypeDef type2 in type.NestedTypes)
                {
                    AddMethods(type2);
                }
            }
        }

        public static void InstructionParseNoLocal(int ins_index)
        {
            for (int i = ins_index; i < instructions.Count; i++)
            {
                Instruction instruction = instructions[i];
                MethodDef methodDef = method;
                if (!toberemovedindex.Contains(i))
                {
                    if (instructions[i].IsBr())
                    {
                        Instruction item = instructions[i].Operand as Instruction;
                        if (!brinstructions.Contains(item) && !realbrinstructions.Contains(item))
                        {
                            realbrinstructions.Add(item);
                            int ins_index2 = instructions.IndexOf(item);
                            InstructionParseNoLocal(ins_index2);
                        }
                        break;
                    }
                    if (instructions[i].IsConditionalBranch() || instructions[i].IsLeave())
                    {
                        Instruction item = instructions[i].Operand as Instruction;
                        if (!conditionalinstructions.Contains(item))
                        {
                            conditionalinstructions.Add(item);
                            int ins_index3 = instructions.IndexOf(item);
                            InstructionParseNoLocal(ins_index3);
                            if (i + 1 < instructions.Count)
                            {
                                int ins_index4 = i + 1;
                                InstructionParseNoLocal(ins_index4);
                            }
                        }
                    }
                    else
                    {
                        if (instructions[i].OpCode == OpCodes.Ret)
                        {
                            break;
                        }
                        if (instructions[i].IsLdcI4())
                        {
                            uint num = 0U;
                            if (instructions[i].IsLdcI4())
                            {
                                num = (uint)instructions[i].GetLdcI4Value();
                            }
                            int num2 = i + 1;
                            if (instructions[i + 1].IsBr())
                            {
                                Instruction item2 = instructions[i + 1].Operand as Instruction;
                                num2 = instructions.IndexOf(item2);
                            }
                            if (instructions[num2].IsLdcI4())
                            {
                                uint num3 = 0U;
                                if (instructions[num2].IsLdcI4())
                                {
                                    num3 = (uint)instructions[num2].GetLdcI4Value();
                                }
                                uint num4 = 0U;
                                if ((instructions[num2 + 1].OpCode == OpCodes.Mul && instructions[num2 + 2].IsLdcI4()) || (instructions[num2 + 1].IsLdcI4() && instructions[num2 + 2].OpCode == OpCodes.Mul) || instructions[num2 + 1].OpCode == OpCodes.Xor)
                                {
                                    if (instructions[num2 + 1].OpCode != OpCodes.Xor)
                                    {
                                        if (instructions[num2 + 1].OpCode == OpCodes.Mul && instructions[num2 + 2].IsLdcI4())
                                        {
                                            num4 = (uint)instructions[num2 + 2].GetLdcI4Value();
                                        }
                                        if (instructions[num2 + 1].IsLdcI4() && instructions[num2 + 2].OpCode == OpCodes.Mul)
                                        {
                                            num4 = (uint)instructions[num2 + 1].GetLdcI4Value();
                                        }
                                    }
                                    if (instructions[num2 + 3].OpCode == OpCodes.Xor || instructions[num2 + 1].OpCode == OpCodes.Xor)
                                    {
                                        for (int j = 0; j < toberemoved.Count; j++)
                                        {
                                            if ((instructions[num2 + 4].IsBr() && instructions[num2 + 4].Operand as Instruction == instructions[toberemoved[j]]) || num2 + 4 == toberemoved[j] || (instructions[num2 + 1].OpCode == OpCodes.Xor && num2 == toberemoved[j]))
                                            {
                                                uint num5;
                                                if (instructions[num2 + 1].OpCode == OpCodes.Xor)
                                                {
                                                    num5 = (num ^ num3);
                                                }
                                                else if (instructions[num2 + 1].IsLdcI4() || instructions[num2 + 1].IsLdloc())
                                                {
                                                    num5 = (num4 * num3 ^ num);
                                                }
                                                else
                                                {
                                                    num5 = (num * num3 ^ num4);
                                                }
                                                uint num6;
                                                if (instructions[num2 + 1].OpCode != OpCodes.Xor)
                                                {
                                                    num6 = (num5 ^ (uint)integer_values_1[j]);
                                                }
                                                else
                                                {
                                                    num6 = num5;
                                                }
                                                uint num7 = num6 % (uint)for_rem[j];
                                                Instruction[] array = switchinstructions[j].Operand as Instruction[];
                                                Instruction item3 = array[(int)((UIntPtr)num7)];
                                                if (toberemovedindex.Contains(i))
                                                {
                                                }
                                                toberemovedindex.Add(i);
                                                toberemovedvalues.Add((int)num7);
                                                bool flag = false;
                                                if (brinstructions.IndexOf(item3) != -1)
                                                {
                                                    flag = true;
                                                }
                                                if (flag)
                                                {
                                                    brinstructions.Add(item3);
                                                    InstructionParseNoLocal(instructions.IndexOf(item3));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (instructions[i].OpCode == OpCodes.Switch)
                        {
                            bool flag2;
                            if (i - 4 < 0)
                            {
                                flag2 = false;
                            }
                            else
                            {
                                flag2 = false;
                                for (int j = 0; j < toberemoved.Count; j++)
                                {
                                    int num8 = toberemoved[j];
                                    if (i - 4 == toberemoved[j])
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                            }
                            if (!flag2)
                            {
                                foreach (Instruction item4 in instructions[i].Operand as Instruction[])
                                {
                                    InstructionParseNoLocal(instructions.IndexOf(item4));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void InstructionParse2(int ins_index, uint local_value)
        {
            for (int i = ins_index; i < instructions.Count; i++)
            {
                Instruction instruction = instructions[i];
                MethodDef methodDef = method;
                string text = methodDef.Name;
                string fullName = methodDef.DeclaringType.FullName;
                if (!toberemovedindex.Contains(i))
                {
                    if (instructions[i].IsBr())
                    {
                        Instruction item = instructions[i].Operand as Instruction;
                        if (!brinstructions.Contains(item) && !realbrinstructions.Contains(item))
                        {
                            realbrinstructions.Add(item);
                            int ins_index2 = instructions.IndexOf(item);
                            InstructionParse2(ins_index2, local_value);
                        }
                        break;
                    }
                    if (instructions[i].IsConditionalBranch() || instructions[i].IsLeave())
                    {
                        Instruction item = instructions[i].Operand as Instruction;
                        if (!conditionalinstructions.Contains(item))
                        {
                            conditionalinstructions.Add(item);
                            int ins_index3 = instructions.IndexOf(item);
                            InstructionParse2(ins_index3, local_value);
                            if (i + 1 < instructions.Count)
                            {
                                int ins_index4 = i + 1;
                                InstructionParse2(ins_index4, local_value);
                            }
                        }
                    }
                    else
                    {
                        if (instructions[i].OpCode == OpCodes.Ret)
                        {
                            break;
                        }
                        if (instructions[i].IsLdcI4() && i + 1 < instructions.Count && instructions[i + 1].IsStloc() && instructions[i + 1].GetLocal(method.Body.Variables) == local_variable)
                        {
                            local_value = (uint)instructions[i].GetLdcI4Value();
                        }
                        else if (instructions[i].IsLdcI4() || (instructions[i].IsLdloc() && instructions[i].GetLocal(method.Body.Variables) == local_variable))
                        {
                            uint num;
                            if (instructions[i].IsLdcI4())
                            {
                                num = (uint)instructions[i].GetLdcI4Value();
                            }
                            else
                            {
                                num = local_value;
                            }
                            int num2 = i + 1;
                            if (instructions[i + 1].IsBr())
                            {
                                Instruction item2 = instructions[i + 1].Operand as Instruction;
                                num2 = instructions.IndexOf(item2);
                            }
                            if (instructions[num2].IsLdcI4() || (instructions[num2].IsLdloc() && instructions[num2].GetLocal(method.Body.Variables) == local_variable))
                            {
                                uint num3;
                                if (instructions[num2].IsLdcI4())
                                {
                                    num3 = (uint)instructions[num2].GetLdcI4Value();
                                }
                                else
                                {
                                    num3 = local_value;
                                }
                                uint num4 = 0U;
                                if ((instructions[num2 + 1].OpCode == OpCodes.Mul && instructions[num2 + 2].IsLdcI4()) || (instructions[num2 + 1].IsLdcI4() && instructions[num2 + 2].OpCode == OpCodes.Mul) || instructions[num2 + 1].OpCode == OpCodes.Xor)
                                {
                                    if (instructions[num2 + 1].OpCode != OpCodes.Xor)
                                    {
                                        if (instructions[num2 + 1].OpCode == OpCodes.Mul && instructions[num2 + 2].IsLdcI4())
                                        {
                                            num4 = (uint)instructions[num2 + 2].GetLdcI4Value();
                                        }
                                        if (instructions[num2 + 1].IsLdcI4() && instructions[num2 + 2].OpCode == OpCodes.Mul)
                                        {
                                            num4 = (uint)instructions[num2 + 1].GetLdcI4Value();
                                        }
                                    }
                                    if (instructions[num2 + 3].OpCode == OpCodes.Xor || instructions[num2 + 1].OpCode == OpCodes.Xor)
                                    {
                                        for (int j = 0; j < toberemoved.Count; j++)
                                        {
                                            if ((instructions[num2 + 4].IsBr() && instructions[num2 + 4].Operand as Instruction == instructions[toberemoved[j]]) || num2 + 4 == toberemoved[j] || (instructions[num2 + 1].OpCode == OpCodes.Xor && num2 == toberemoved[j]))
                                            {
                                                uint num5;
                                                if (instructions[num2 + 1].OpCode == OpCodes.Xor)
                                                {
                                                    num5 = (num ^ num3);
                                                }
                                                else if (instructions[num2 + 1].IsLdcI4() || instructions[num2 + 1].IsLdloc())
                                                {
                                                    num5 = (num4 * num3 ^ num);
                                                }
                                                else
                                                {
                                                    num5 = (num * num3 ^ num4);
                                                }
                                                if (instructions[num2 + 1].OpCode != OpCodes.Xor)
                                                {
                                                    local_value = (num5 ^ (uint)integer_values_1[j]);
                                                }
                                                else
                                                {
                                                    local_value = num5;
                                                }
                                                uint num6 = local_value % (uint)for_rem[j];
                                                Instruction[] array = switchinstructions[j].Operand as Instruction[];
                                                Instruction item3 = array[(int)((UIntPtr)num6)];
                                                if (toberemovedindex.Contains(i))
                                                {
                                                }
                                                toberemovedindex.Add(i);
                                                toberemovedvalues.Add((int)num6);
                                                bool flag = false;
                                                int num7 = brinstructions.IndexOf(item3);
                                                if (num7 != -1)
                                                {
                                                    int num8 = local_values[num7];
                                                    if ((long)num8 != (long)((ulong)local_value))
                                                    {
                                                        flag = true;
                                                    }
                                                }
                                                else
                                                {
                                                    flag = true;
                                                }
                                                if (flag)
                                                {
                                                    brinstructions.Add(item3);
                                                    local_values.Add((int)local_value);
                                                    InstructionParse2(instructions.IndexOf(item3), local_value);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (instructions[i].OpCode == OpCodes.Switch)
                        {
                            bool flag2;
                            if (i - 4 < 0)
                            {
                                flag2 = false;
                            }
                            else
                            {
                                flag2 = false;
                                for (int j = 0; j < toberemoved.Count; j++)
                                {
                                    int num9 = toberemoved[j];
                                    if (i - 6 == toberemoved[j])
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                            }
                            if (!flag2)
                            {
                                foreach (Instruction item4 in instructions[i].Operand as Instruction[])
                                {
                                    InstructionParse2(instructions.IndexOf(item4), local_value);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void fixcflow(ModuleDefMD module)
        {
            ModuleDef manifestModule = module;
            if (manifestModule.IsILOnly)
            {
                methods = new List<MethodDef>();
                if (manifestModule.HasTypes)
                {
                    foreach (TypeDef type in manifestModule.Types)
                    {
                        AddMethods(type);
                    }
                }
                BlocksCflowDeobfuscator blocksCflowDeobfuscator = new BlocksCflowDeobfuscator();
                for (int i = 0; i < methods.Count; i++)
                {
                    try
                    {
                        Blocks blocks = new Blocks(methods[i]);
                        blocksCflowDeobfuscator.Initialize(blocks);
                        blocksCflowDeobfuscator.Deobfuscate();
                        blocks.RepartitionBlocks();
                        IList<Instruction> list;
                        IList<ExceptionHandler> exceptionHandlers;
                        blocks.GetCode(out list, out exceptionHandlers);
                        DotNetUtils.RestoreBody(methods[i], list, exceptionHandlers);
                    }
                    catch { }
                }
                for (int i = 0; i < methods.Count; i++)
                {
                    for (int j = 0; j < methods[i].Body.Instructions.Count; j++)
                    {
                        if (methods[i].Body.Instructions[j].IsLdcI4() && j + 1 < methods[i].Body.Instructions.Count && methods[i].Body.Instructions[j + 1].OpCode == OpCodes.Pop)
                        {
                            methods[i].Body.Instructions[j].OpCode = OpCodes.Nop;
                            methods[i].Body.Instructions[j + 1].OpCode = OpCodes.Nop;
                            for (int k = 0; k < methods[i].Body.Instructions.Count; k++)
                            {
                                if (methods[i].Body.Instructions[k].OpCode == OpCodes.Br || methods[i].Body.Instructions[k].OpCode == OpCodes.Br_S)
                                {
                                    if (methods[i].Body.Instructions[k].Operand is Instruction)
                                    {
                                        Instruction instruction = methods[i].Body.Instructions[k].Operand as Instruction;
                                        if (instruction == methods[i].Body.Instructions[j + 1])
                                        {
                                            if (k - 1 >= 0 && methods[i].Body.Instructions[k - 1].IsLdcI4())
                                            {
                                                methods[i].Body.Instructions[k - 1].OpCode = OpCodes.Nop;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (methods[i].Body.Instructions[j].OpCode == OpCodes.Dup && j + 1 < methods[i].Body.Instructions.Count && methods[i].Body.Instructions[j + 1].OpCode == OpCodes.Pop)
                        {
                            methods[i].Body.Instructions[j].OpCode = OpCodes.Nop;
                            methods[i].Body.Instructions[j + 1].OpCode = OpCodes.Nop;
                            for (int k = 0; k < methods[i].Body.Instructions.Count; k++)
                            {
                                if (methods[i].Body.Instructions[k].OpCode == OpCodes.Br || methods[i].Body.Instructions[k].OpCode == OpCodes.Br_S)
                                {
                                    if (methods[i].Body.Instructions[k].Operand is Instruction)
                                    {
                                        Instruction instruction = methods[i].Body.Instructions[k].Operand as Instruction;
                                        if (instruction == methods[i].Body.Instructions[j + 1])
                                        {
                                            if (k - 1 >= 0 && methods[i].Body.Instructions[k - 1].OpCode == OpCodes.Dup)
                                            {
                                                methods[i].Body.Instructions[k - 1].OpCode = OpCodes.Nop;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < methods.Count; i++)
                {
                    try
                    {
                        Blocks blocks = new Blocks(methods[i]);
                        blocksCflowDeobfuscator.Initialize(blocks);
                        blocksCflowDeobfuscator.Deobfuscate();
                        blocks.RepartitionBlocks();
                        IList<Instruction> list;
                        IList<ExceptionHandler> exceptionHandlers;
                        blocks.GetCode(out list, out exceptionHandlers);
                        DotNetUtils.RestoreBody(methods[i], list, exceptionHandlers);
                    }
                    catch { }
                }
                for (int i = 0; i < methods.Count; i++)
                {
                    List<Instruction> list2 = new List<Instruction>();
                    List<Instruction> list3 = new List<Instruction>();
                    Local local = null;
                    List<int> list4 = new List<int>();
                    List<int> list5 = new List<int>();
                    for (int j = 0; j < methods[i].Body.Instructions.Count; j++)
                    {
                        if (j + 6 < methods[i].Body.Instructions.Count && methods[i].Body.Instructions[j].IsLdcI4())
                        {
                            if (methods[i].Body.Instructions[j + 1].OpCode == OpCodes.Xor)
                            {
                                if (methods[i].Body.Instructions[j + 2].OpCode == OpCodes.Dup)
                                {
                                    if (methods[i].Body.Instructions[j + 3].IsStloc())
                                    {
                                        if (methods[i].Body.Instructions[j + 4].IsLdcI4())
                                        {
                                            if (methods[i].Body.Instructions[j + 5].OpCode == OpCodes.Rem_Un)
                                            {
                                                if (methods[i].Body.Instructions[j + 6].OpCode == OpCodes.Switch)
                                                {
                                                    list2.Add(methods[i].Body.Instructions[j]);
                                                    list4.Add(methods[i].Body.Instructions[j].GetLdcI4Value());
                                                    local = methods[i].Body.Instructions[j + 3].GetLocal(methods[i].Body.Variables);
                                                    list5.Add(methods[i].Body.Instructions[j + 4].GetLdcI4Value());
                                                    list3.Add(methods[i].Body.Instructions[j + 6]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (list2.Count > 0)
                    {
                        for (int j = 0; j < methods[i].Body.Instructions.Count; j++)
                        {
                            if (j + 1 < methods[i].Body.Instructions.Count && methods[i].Body.Instructions[j].IsLdcI4())
                            {
                                if (methods[i].Body.Instructions[j + 1].IsBr())
                                {
                                    Instruction instruction = methods[i].Body.Instructions[j + 1].Operand as Instruction;
                                    for (int k = 0; k < list2.Count; k++)
                                    {
                                        if (instruction == list2[k])
                                        {
                                            MethodDef methodDef = methods[i];
                                            int ldcI4Value = methods[i].Body.Instructions[j].GetLdcI4Value();
                                            uint num = (uint)(ldcI4Value ^ list4[k]);
                                            uint num2 = num % (uint)list5[k];
                                            methods[i].Body.Instructions[j].OpCode = OpCodes.Ldc_I4;
                                            methods[i].Body.Instructions[j].Operand = (int)num;
                                            methods[i].Body.Instructions[j + 1].Operand = OpCodes.Br;
                                            Instruction[] array = list3[k].Operand as Instruction[];
                                            methods[i].Body.Instructions[j + 1].Operand = array[(int)((UIntPtr)num2)];
                                            methods[i].Body.Instructions.Insert(j + 1, OpCodes.Stloc_S.ToInstruction(local));
                                            j++;
                                        }
                                    }
                                }
                            }
                        }
                        methods[i].Body.SimplifyBranches();
                        methods[i].Body.OptimizeBranches();
                    }
                }
                for (int i = 0; i < methods.Count; i++)
                {
                    try
                    {
                        Blocks blocks = new Blocks(methods[i]);
                        blocksCflowDeobfuscator.Initialize(blocks);
                        blocksCflowDeobfuscator.Deobfuscate();
                        blocks.RepartitionBlocks();
                        IList<Instruction> list;
                        IList<ExceptionHandler> exceptionHandlers;
                        blocks.GetCode(out list, out exceptionHandlers);
                        DotNetUtils.RestoreBody(methods[i], list, exceptionHandlers);
                    }
                    catch { }
                }
                for (int i = 0; i < methods.Count; i++)
                {
                    Dictionary<Instruction, Instruction> dictionary = new Dictionary<Instruction, Instruction>();
                    for (int j = 0; j < methods[i].Body.Instructions.Count; j++)
                    {
                        if (methods[i].Body.Instructions[j].IsConditionalBranch())
                        {
                            Instruction instruction2 = methods[i].Body.Instructions[j];
                            for (int k = 0; k < methods[i].Body.Instructions.Count; k++)
                            {
                                if (methods[i].Body.Instructions[k].IsBr())
                                {
                                    Instruction instruction3 = methods[i].Body.Instructions[k];
                                    Instruction instruction4 = methods[i].Body.Instructions[k].Operand as Instruction;
                                    if (instruction4 == instruction2)
                                    {
                                        if (!dictionary.ContainsKey(instruction4))
                                        {
                                            methods[i].Body.Instructions[k].OpCode = instruction2.GetOpCode();
                                            methods[i].Body.Instructions[k].Operand = instruction2.GetOperand();
                                            methods[i].Body.Instructions.Insert(k + 1, OpCodes.Br.ToInstruction(methods[i].Body.Instructions[j + 1]));
                                            k++;
                                            dictionary.Add(instruction4, methods[i].Body.Instructions[k]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    methods[i].Body.SimplifyBranches();
                    methods[i].Body.OptimizeBranches();
                }
                for (int i = 0; i < methods.Count; i++)
                {
                    try
                    {
                        Blocks blocks = new Blocks(methods[i]);
                        blocksCflowDeobfuscator.Initialize(blocks);
                        blocksCflowDeobfuscator.Deobfuscate();
                        blocks.RepartitionBlocks();
                        IList<Instruction> list;
                        IList<ExceptionHandler> exceptionHandlers;
                        blocks.GetCode(out list, out exceptionHandlers);
                        DotNetUtils.RestoreBody(methods[i], list, exceptionHandlers);
                    }
                    catch { }
                }
                int num3 = 0;
                for (int i = 0; i < methods.Count; i++)
                {
                    toberemoved = new List<int>();
                    integer_values_1 = new List<int>();
                    for_rem = new List<int>();
                    switchinstructions = new List<Instruction>();
                    for (int j = 0; j < methods[i].Body.Instructions.Count; j++)
                    {
                        if (j + 6 < methods[i].Body.Instructions.Count && methods[i].Body.Instructions[j].IsLdcI4())
                        {
                            if (methods[i].Body.Instructions[j + 1].OpCode == OpCodes.Xor)
                            {
                                if (methods[i].Body.Instructions[j + 2].OpCode == OpCodes.Dup)
                                {
                                    if (methods[i].Body.Instructions[j + 3].IsStloc())
                                    {
                                        if (methods[i].Body.Instructions[j + 4].IsLdcI4())
                                        {
                                            if (methods[i].Body.Instructions[j + 5].OpCode == OpCodes.Rem_Un)
                                            {
                                                if (methods[i].Body.Instructions[j + 6].OpCode == OpCodes.Switch)
                                                {
                                                    toberemoved.Add(j);
                                                    integer_values_1.Add(methods[i].Body.Instructions[j].GetLdcI4Value());
                                                    local_variable = methods[i].Body.Instructions[j + 3].GetLocal(methods[i].Body.Variables);
                                                    for_rem.Add(methods[i].Body.Instructions[j + 4].GetLdcI4Value());
                                                    switchinstructions.Add(methods[i].Body.Instructions[j + 6]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (switchinstructions.Count > 0)
                    {
                        toberemovedindex = new List<int>();
                        toberemovedvalues = new List<int>();
                        conditionalinstructions = new List<Instruction>();
                        brinstructions = new List<Instruction>();
                        realbrinstructions = new List<Instruction>();
                        local_values = new List<int>();
                        instructions = methods[i].Body.Instructions;
                        method = methods[i];
                        InstructionParse2(0, 0U);
                        num3 += toberemovedindex.Count;
                        if (toberemovedindex.Count > 0)
                        {
                            for (int l = 0; l < toberemoved.Count; l++)
                            {
                                for (int j = 0; j < 6; j++)
                                {
                                    methods[i].Body.Instructions[j + toberemoved[l]].OpCode = OpCodes.Nop;
                                    methods[i].Body.Instructions[j + toberemoved[l]].Operand = null;
                                }
                            }
                            for (int j = 0; j < toberemovedindex.Count; j++)
                            {
                                methods[i].Body.Instructions[toberemovedindex[j]].OpCode = OpCodes.Ldc_I4;
                                methods[i].Body.Instructions[toberemovedindex[j]].Operand = toberemovedvalues[j];
                                if (!methods[i].Body.Instructions[toberemovedindex[j] + 1].IsBr())
                                {
                                    for (int k = 0; k < 4; k++)
                                    {
                                        methods[i].Body.Instructions[toberemovedindex[j] + k + 1].OpCode = OpCodes.Nop;
                                        methods[i].Body.Instructions[toberemovedindex[j] + k + 1].Operand = null;
                                    }
                                }
                            }
                        }
                    }
                    toberemoved = new List<int>();
                    integer_values_1 = new List<int>();
                    for_rem = new List<int>();
                    switchinstructions = new List<Instruction>();
                    for (int j = 0; j < methods[i].Body.Instructions.Count; j++)
                    {
                        if (j + 6 < methods[i].Body.Instructions.Count && methods[i].Body.Instructions[j].IsLdcI4())
                        {
                            if (methods[i].Body.Instructions[j + 1].OpCode == OpCodes.Xor)
                            {
                                if (methods[i].Body.Instructions[j + 2].IsLdcI4())
                                {
                                    if (methods[i].Body.Instructions[j + 3].OpCode == OpCodes.Rem_Un)
                                    {
                                        if (methods[i].Body.Instructions[j + 4].OpCode == OpCodes.Switch)
                                        {
                                            toberemoved.Add(j);
                                            integer_values_1.Add(methods[i].Body.Instructions[j].GetLdcI4Value());
                                            for_rem.Add(methods[i].Body.Instructions[j + 2].GetLdcI4Value());
                                            switchinstructions.Add(methods[i].Body.Instructions[j + 4]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (switchinstructions.Count > 0)
                    {
                        toberemovedindex = new List<int>();
                        toberemovedvalues = new List<int>();
                        conditionalinstructions = new List<Instruction>();
                        brinstructions = new List<Instruction>();
                        realbrinstructions = new List<Instruction>();
                        instructions = methods[i].Body.Instructions;
                        method = methods[i];
                        InstructionParseNoLocal(0);
                        num3 += toberemovedindex.Count;
                        if (toberemovedindex.Count > 0)
                        {
                            for (int l = 0; l < toberemoved.Count; l++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    methods[i].Body.Instructions[j + toberemoved[l]].OpCode = OpCodes.Nop;
                                    methods[i].Body.Instructions[j + toberemoved[l]].Operand = null;
                                }
                            }
                            for (int j = 0; j < toberemovedindex.Count; j++)
                            {
                                methods[i].Body.Instructions[toberemovedindex[j]].OpCode = OpCodes.Ldc_I4;
                                methods[i].Body.Instructions[toberemovedindex[j]].Operand = toberemovedvalues[j];
                                if (!methods[i].Body.Instructions[toberemovedindex[j] + 1].IsBr())
                                {
                                    for (int k = 0; k < 4; k++)
                                    {
                                        methods[i].Body.Instructions[toberemovedindex[j] + k + 1].OpCode = OpCodes.Nop;
                                        methods[i].Body.Instructions[toberemovedindex[j] + k + 1].Operand = null;
                                    }
                                }
                            }
                        }
                    }
                    try
                    {
                        Blocks blocks = new Blocks(methods[i]);
                        blocksCflowDeobfuscator.Initialize(blocks);
                        blocksCflowDeobfuscator.Deobfuscate();
                        blocks.RepartitionBlocks();
                        IList<Instruction> list;
                        IList<ExceptionHandler> exceptionHandlers;
                        blocks.GetCode(out list, out exceptionHandlers);
                        DotNetUtils.RestoreBody(methods[i], list, exceptionHandlers);
                    }
                    catch { }
                    methods[i].Body.SimplifyBranches();
                    methods[i].Body.OptimizeBranches();

                }
                for (int i = 0; i < methods.Count; i++)
                {
                    try
                    {
                        Blocks blocks = new Blocks(methods[i]);
                        blocksCflowDeobfuscator.Initialize(blocks);
                        blocksCflowDeobfuscator.Deobfuscate();
                        blocks.RepartitionBlocks();
                        IList<Instruction> list;
                        IList<ExceptionHandler> exceptionHandlers;
                        blocks.GetCode(out list, out exceptionHandlers);
                        DotNetUtils.RestoreBody(methods[i], list, exceptionHandlers);
                    }
                    catch { }
                }
            }
        }
        #endregion
    }
}
