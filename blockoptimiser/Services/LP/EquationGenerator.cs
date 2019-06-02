using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Services.LP
{
    public class EquationGenerator
    {
        private String _line = "";
        private int _lineMaxLength = 200;
        private String _fileName = "";
        
        public String FileName
        {
            get
            {
                return _fileName;
            }
            
        }
        ExecutionContext context;
        public void Generate(ExecutionContext context)
        {
            this.context = context;
            FileStream fs = CreateFile();

            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("maximize");
                WriteObjectiveFunction(sw);
                Write("", sw);
                sw.WriteLine("Subject to");
                WriteConstraints(sw);
                sw.WriteLine("end");
            }
        }

        private FileStream CreateFile()
        {
            Directory.CreateDirectory(@"C:\\blockoptimiser");
            _fileName = @"C:\\blockoptimiser\\blockoptimiser-dump_" + context.Period+".lp";

            return File.Create(_fileName);
        }

        private void WriteObjectiveFunction(StreamWriter sw)
        {

            List<Process> processes = context.GetProcessList();
            Dictionary<int, String> modelProcessFilterMap = new Dictionary<int, string>();
            double F = (1 / Math.Pow(Convert.ToDouble(1 + context.DiscountFactor), Convert.ToDouble(context.Period)));
            foreach( Model model in context.GetModels())
            {
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(model.Id);
                foreach (int ii in blocks.Keys)
                {
                    foreach (int jj in blocks[ii].Keys)
                    {
                        foreach (int kk in blocks[ii][jj].Keys)
                        {
                            Block block = blocks[ii][jj][kk];
                            if (!context.IsValid(block, model.Id)) continue;
                            Decimal minigCost = GetMiningCost(block, context.Year);
                            if(block.Processes != null && block.Processes.Count > 0 )
                            {
                                foreach(Process process in block.Processes)
                                {
                                    Decimal processValue = GetProcessValue(block, process, context.Year) - minigCost;

                                    if (processValue != 0)
                                    {
                                        processValue = processValue * (Decimal)F;
                                        if (processValue < 0)
                                        {
                                            Write(RoundOff(processValue) + " B" + block.Id + "p" + process.ProcessNumber, sw);
                                        }
                                        else
                                        {
                                            Write(" + " + RoundOff(processValue) + " B" + block.Id + "p" + process.ProcessNumber, sw);
                                        }
                                    }
                                    if (minigCost != 0)
                                        Write(" - " + RoundOff(minigCost * (Decimal)F) + " B" + block.Id + "s1", sw);
                                }
                            } else
                            {
                                if (minigCost > 0)
                                    Write(" - " + RoundOff(minigCost * (Decimal)F) + " B" + block.Id + "w1", sw);
                            }
                            
                        }
                    }
                }

            }            
        }

        private void WriteConstraints(StreamWriter sw)
        {
            WriteGeotechConstraints(sw);
            Write("", sw);
            WriteReserveConstraints(sw);
            Write("", sw);
            WriteProcessLimitConstraints(sw);
            Write("", sw);
            WriteGradeLimitConstraints(sw);
            Write("", sw);
        }

        private void WriteGeotechConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\ Geotech");
            List<Model> models = context.GetModels();
            foreach(Model model in models)
            {
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(model.Id);
                foreach (int ii in blocks.Keys)
                {
                    foreach (int jj in blocks[ii].Keys)
                    {
                        foreach (int kk in blocks[ii][jj].Keys)
                        {

                            Block b = blocks[ii][jj][kk];
                            if (!context.IsValid(b, model.Id)) continue;
                            decimal xorth = (decimal)b.data["Xortho"];
                            decimal yorth = (decimal)b.data["Yortho"];
                            decimal tonneswt = context.GetTonnesWtForBlock(b);
                            List<Block> dependentBlocks = b.DependentBlocks;
                            foreach(Block ub in dependentBlocks)
                            {
                                if (!context.IsValid(ub, model.Id)) continue;

                                decimal ubtonneswt = context.GetTonnesWtForBlock(ub);
                                if (ubtonneswt == 0) continue;
                                decimal ratio = tonneswt / ubtonneswt;
                                ratio = RoundOff(ratio);
                                if (b.Processes != null)
                                {
                                    foreach (Process process in b.Processes)
                                    {
                                        Write(" + B" + b.Id + "p" + process.ProcessNumber + " + B" + b.Id + "s1", sw);
                                    }

                                }
                                else
                                {
                                    Write(" + B" + b.Id + "w1", sw);
                                }
                                if (ub.Processes != null)
                                {
                                    foreach (Process process in ub.Processes)
                                    {
                                        Write(" - " + ratio + "B" + ub.Id + "p" + process.ProcessNumber + " - " + ratio + "B" + ub.Id + "s1", sw);
                                    }
                                }
                                else
                                {
                                    Write(" - " + ratio + "B" + ub.Id + "w1", sw);
                                }

                                Write(" <= 0", sw);
                                Write("", sw);
                            }
                        }
                    }

                }

            }

        }

        private void WriteReserveConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\Reserve Constraints");
            List<Model> models = context.GetModels();
            foreach (Model model in models)
            {
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(model.Id);
                foreach (int ii in blocks.Keys)
                {
                    foreach (int jj in blocks[ii].Keys)
                    {
                        foreach (int kk in blocks[ii][jj].Keys)
                        {
                            Block block = blocks[ii][jj][kk];
                            if (!context.IsValid(block, model.Id)) continue;
                            decimal tonneswt = context.GetTonnesWtForBlock(block);
                            if (block.Processes != null )
                            {
                                String eqn = "";
                                foreach (Process process in block.Processes)
                                {
                                    Write("B" + block.Id + "p" + process.ProcessNumber + " >= 0", sw);
                                    Write("", sw);
                                    Write("B" + block.Id + "s1 >= 0", sw);
                                    Write("", sw);
                                    eqn = eqn + " + B" + block.Id + "p" + process.ProcessNumber + " + B" + block.Id + "s1";
                                }
                                Write(eqn + " <= " + tonneswt, sw);
                                Write("", sw);
                            } else
                            {
                                Write("B" + block.Id + "w1 >= 0 ", sw);
                                Write("", sw);
                                Write("B" + block.Id + "w1 <= "+tonneswt, sw);
                                Write("", sw);
                            }
                        }
                    }
                }
            }
        }
        private void WriteProcessLimitConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\Process Limits");
            foreach(ProcessLimit processLimit in context.GetProcessLimtis())
            {
                if (!processLimit.IsUsed) continue;

                Decimal processLimitValue = 0;
                foreach (var mapping in processLimit.ProcessLimitYearMapping)
                {
                    if (mapping.Year == context.Year)
                    {
                        processLimitValue = mapping.Value;
                        break;
                    }
                }

                if (processLimit.ItemType == ProcessLimit.ITEM_TYPE_PROCESS)
                {
                    Process process = context.GetProcessById(processLimit.ItemId);

                    foreach (Model model in context.GetModels())
                    {
                        Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(model.Id);
                        foreach (int ii in blocks.Keys)
                        {
                            foreach (int jj in blocks[ii].Keys)
                            {
                                foreach (int kk in blocks[ii][jj].Keys)
                                {
                                    Block b = blocks[ii][jj][kk];
                                    if (!context.IsValid(b, model.Id)) continue;
                                    if(b.Processes != null)
                                    {
                                        foreach(Process p in b.Processes)
                                        {
                                            if(p.Id == process.Id)
                                            {
                                                Write(" + B" + b.Id + "p" + process.ProcessNumber, sw);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (processLimit.ItemType == ProcessLimit.ITEM_TYPE_PRODUCT)
                {
                    Product product = context.GetProductById(processLimit.ItemId);
                    if (product == null) continue;
                    foreach(int processId in product.ProcessIds)
                    {
                        Process process = context.GetProcessById(processId);

                        foreach (Model model in context.GetModels())
                        {
                            Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(model.Id);
                            foreach (int ii in blocks.Keys)
                            {
                                foreach (int jj in blocks[ii].Keys)
                                {
                                    foreach (int kk in blocks[ii][jj].Keys)
                                    {
                                        Block b = blocks[ii][jj][kk];
                                        if (!context.IsValid(b, model.Id)) continue;
                                        if (b.Processes != null)
                                        {
                                            foreach (Process p in b.Processes)
                                            {
                                                if (p.Id == process.Id)
                                                {
                                                    Decimal value = context.GetFieldValueforBlock(b, product.UnitName);
                                                    Decimal tonnesWt = context.GetTonnesWtForBlock(b);
                                                    Write(" + " + RoundOff(value / tonnesWt) + " B" + b.Id + "p" + process.ProcessNumber, sw);
                                                }
                                            }
                                        }
                                                    
                                    }
                                }
                            }
                        }
                    }                                    
                }
                else if (processLimit.ItemType == ProcessLimit.ITEM_TYPE_PRODUCT_JOIN)
                {
                    List<String> productNames = context.GetProductsInProductJoin(processLimit.ItemName);
                    foreach (String productName in productNames)
                    {
                        Product product = context.GetProductByName(productName);
                        if (product == null) continue;
                        foreach (int processId in product.ProcessIds)
                        {
                            Process process = context.GetProcessById(processId);

                            foreach (Model model in context.GetModels())
                            {
                                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(model.Id);
                                foreach (int ii in blocks.Keys)
                                {
                                    foreach (int jj in blocks[ii].Keys)
                                    {
                                        foreach (int kk in blocks[ii][jj].Keys)
                                        {
                                            Block b = blocks[ii][jj][kk];
                                            if (!context.IsValid(b, model.Id)) continue;
                                            if (b.Processes != null)
                                            {
                                                foreach (Process p in b.Processes)
                                                {
                                                    if (p.Id == process.Id)
                                                    {
                                                        Decimal value = context.GetFieldValueforBlock(b, product.UnitName);
                                                        Decimal tonnesWt = context.GetTonnesWtForBlock(b);
                                                        Write(" + " + RoundOff(value / tonnesWt) + " B" + b.Id + "p" + process.ProcessNumber, sw);
                                                    }
                                                }
                                            }
                                                        
                                        }
                                    }
                                }
                            }                           
                        }
                    }
                }
                if (processLimit.ItemType == ProcessLimit.ITEM_TYPE_MODEL)
                {
                    int modelId = processLimit.ItemId;
                    Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(modelId);
                    foreach (int ii in blocks.Keys)
                    {
                        foreach (int jj in blocks[ii].Keys)
                        {
                            foreach (int kk in blocks[ii][jj].Keys)
                            {
                                Block b = blocks[ii][jj][kk];
                                if(context.IsValid(b, modelId))
                                {
                                    if (b.Processes != null )
                                    {
                                        foreach (Process process in b.Processes)
                                        {
                                            Write(" + B" + b.Id + "p" + process.ProcessNumber , sw);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Write(" <=  "+processLimitValue, sw);
                Write("", sw);
            }
        }

        private void WriteGradeLimitConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\ Grade Limits");
            foreach (GradeLimit gradeLimit in context.GetGradeLimtis())
            {
                if (!gradeLimit.IsUsed) continue;
                Decimal targetGrade = 0;
                foreach(var mapping in gradeLimit.GradeLimitYearMapping)
                {
                    if(mapping.Year == context.Year)
                    {
                        targetGrade = mapping.Value;
                        break;
                    }
                }
                if (gradeLimit.ItemType == GradeLimit.ITEM_TYPE_PRODUCT)
                {
                    Product product = context.GetProductByName(gradeLimit.ItemName);
                    if (product == null) continue;
                    
                    foreach(int processId in product.ProcessIds)
                    {
                        Process process = context.GetProcessById(processId);

                        foreach (Model model in context.GetModels())
                        {
                            Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(model.Id);
                            foreach (int ii in blocks.Keys)
                            {
                                foreach (int jj in blocks[ii].Keys)
                                {
                                    foreach (int kk in blocks[ii][jj].Keys)
                                    {
                                        Block b = blocks[ii][jj][kk];
                                        if (!context.IsValid(b, model.Id)) continue;
                                        if (b.Processes != null)
                                        {
                                            foreach (Process p in b.Processes)
                                            {
                                                if (p.Id == process.Id)
                                                {
                                                    Decimal tonnesWt = context.GetTonnesWtForBlock(b);
                                                    Decimal processRatio = 0;
                                                    if (tonnesWt > 0)
                                                        processRatio = context.GetFieldValueforBlock(b, product.UnitName) / tonnesWt;

                                                    Decimal blockGrade = context.GetFieldValueforBlock(b, gradeLimit.GradeName);
                                                    Decimal coeff = processRatio * (targetGrade - blockGrade);
                                                    coeff = RoundOff(coeff);
                                                    if (coeff < 0)
                                                    {
                                                        Write(" " + coeff + "B" + b.Id + "p" + process.ProcessNumber, sw);
                                                    }
                                                    else
                                                    {
                                                        Write(" +" + coeff + "B" + b.Id + "p" + process.ProcessNumber, sw);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    if(gradeLimit.IsMax)
                    {
                        Write(" <= 0 ", sw);                        
                    } else
                    {
                        Write(" >= 0 ", sw);
                    }
                    Write("", sw);
                }
                else if (gradeLimit.ItemType == GradeLimit.ITEM_TYPE_PRODUCT_JOIN)
                {
                    List<String> productNames = context.GetProductsInProductJoin(gradeLimit.ItemName);
                    List<ProductJoinGradeAliasing> gradeAlises = context.GetGradeAlisesByProductJoinName(gradeLimit.ItemName);
                    int gradeIndex = -1;
                    foreach(var gradeAlias in gradeAlises)
                    {
                        if (gradeAlias.GradeAliasName.Equals(gradeLimit.GradeName))
                        {
                            gradeIndex = gradeAlias.GradeAliasNameIndex;
                            break;
                        }
                    }
                    if (gradeIndex == -1) continue;

                    foreach(String productName in productNames)
                    {
                        Product product = context.GetProductByName(productName);
                        if (product == null) continue;
                        List<String> gradeNames = context.GetGradeFieldsByAssociatedFieldName(product.UnitName);
                        if (gradeNames.Count == 0 || gradeNames.Count < gradeIndex) continue;
                        String gradeName = gradeNames.ElementAt(gradeIndex-1);

                        foreach (int processId in product.ProcessIds)
                        {
                            Process process = context.GetProcessById(processId);
                            foreach (Model model in context.GetModels())
                            {
                                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = context.GetBlocks(model.Id);
                                foreach (int ii in blocks.Keys)
                                {
                                    foreach (int jj in blocks[ii].Keys)
                                    {
                                        foreach (int kk in blocks[ii][jj].Keys)
                                        {
                                            Block b = blocks[ii][jj][kk];
                                            if (!context.IsValid(b, model.Id)) continue;
                                            if (b.Processes != null)
                                            {
                                                foreach (Process p in b.Processes)
                                                {
                                                    if (p.Id == process.Id)
                                                    {
                                                        Decimal tonnesWt = context.GetTonnesWtForBlock(b);
                                                        Decimal processRatio = 0;
                                                        if (tonnesWt > 0)
                                                            processRatio = context.GetFieldValueforBlock(b, product.UnitName) / tonnesWt;
                                                        Decimal blockGrade = context.GetFieldValueforBlock(b, gradeName);
                                                        Decimal coeff = processRatio * (targetGrade - blockGrade);
                                                        coeff = RoundOff(coeff);
                                                        if (coeff < 0)
                                                        {
                                                            Write(" " + coeff + "B" + b.Id + "p" + process.ProcessNumber, sw);
                                                        }
                                                        else
                                                        {
                                                            Write(" +" + coeff + "B" + b.Id + "p" + process.ProcessNumber, sw);
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (gradeLimit.IsMax)
                    {
                        Write(" <= 0 ", sw);
                    }
                    else
                    {
                        Write(" >= 0 ", sw);
                    }
                    Write("", sw);
                }
            }
        }

        private Decimal GetProcessValue(Block b, Process process, int year)
        {
            Decimal value = 0;
            if (process == null) return value;
            Decimal totalrevenue = 0, pcost = 0;

            List<Opex> opexList = context.getOpexList();
            Decimal tonnes = context.GetTonnesWtForBlock(b);
            foreach(Opex opex in opexList)
            {
                if (!opex.IsUsed) continue;

                if (opex.CostType == Opex.REVENUE)
                {
                    if (opex.FilterType == Opex.FILTER_TYPE_PRODUCT && !(context.IsProductAssociatedToProcess(opex.FilterName, process.Id)))
                    {
                        continue;

                    }
                    else if (opex.FilterType == Opex.FILTER_TYPE_PRODUCT_JOIN)
                    {
                        if (!context.IsProductJoinAssociatedToProcess(opex.FilterName, process.Id))
                        {
                            continue;
                        }
                    }
                    Decimal revExprValue = context.GetUnitValueforBlock(b, opex.UnitType, opex.UnitId);
                    Decimal revenue = 0;
                    foreach(var costData in opex.CostData)
                    {
                        if(costData.Year == year)
                        {
                            revenue = costData.Value;
                            break;
                        }
                    }
                    if( tonnes != 0 )
                        totalrevenue = totalrevenue + revenue * revExprValue / tonnes;
                }
                else
                {
                    if((opex.FilterType == Opex.FILTER_TYPE_PROCESS && opex.FilterName.Equals(process.Name))
                        || (opex.FilterType == Opex.FILTER_TYPE_PROCESS_JOIN && context.IsProcessInProcessJoin(opex.FilterName, process.Name)))  {
                        foreach (var costData in opex.CostData)
                        {
                            if (costData.Year == year)
                            {
                                pcost = pcost + costData.Value;
                                break;
                            }
                        }
                    }

                }

            }
            return totalrevenue - pcost;
        }

        public Decimal GetMiningCost(Block b, int year)
        {
            foreach (Opex opex in context.getOpexList())
            {
                if (!opex.IsUsed) continue;

                if (opex.CostType == Opex.MINING_COST)
                {
                    Decimal miningCost = 0;
                    foreach (var costData in opex.CostData)
                    {
                        if (costData.Year == year)
                        {
                            miningCost = costData.Value;
                            break;
                        }
                    }
                    /*if( opex.UnitType > 0 )
                    {
                        Decimal revExprValue = context.GetUnitValueforBlock(b, opex.UnitType, opex.UnitId);
                        miningCost = miningCost* revExprValue;
                    }*/
                    
                    return miningCost;
                }
            }
            return 0;
        }
        private Boolean IsProcessAssociatedToProductJoin()
        {

            return false;
        }
        private void Write(String str, StreamWriter sw)
        {
            _line = _line + str;
            if (str.Length == 0 || _line.Length > _lineMaxLength)
            {
                sw.WriteLine(_line);
                _line = "";
            }                    
        }

        private Decimal RoundOff(Decimal value)
        {
            return (Decimal)RoundOff((double)value);
        }
        private double RoundOff(double value)
        {
            return Math.Round(value, 3);
        }

    }
}
