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

        ExecutionContext context;
        public void Generate(ExecutionContext context)
        {
            this.context = context;
            //Scenario scenario = new ScenarioDataAccess().Get(Context.ScenarioId);
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
            Directory.CreateDirectory(@"C:\\blockoptimizor");
            String fileName = @"C:\\blockoptimizor\\blockoptimizor-dump_"+context.Period+".lp";

            return File.Create(fileName);
        }

        private void WriteObjectiveFunction(StreamWriter sw)
        {

            List<Process> processes = context.GetProcessList();
            int count = 1;
            Dictionary<int, String> modelProcessFilterMap = new Dictionary<int, string>();
            double F = (1 / Math.Pow(Convert.ToDouble(1 + context.DiscountFactor), Convert.ToDouble(context.Period)));
            foreach (Process process in processes)
            {
                foreach(ProcessModelMapping mapping in process.Mapping)
                {
                    if(!modelProcessFilterMap.ContainsKey(mapping.ModelId))
                    {
                        modelProcessFilterMap.Add(mapping.ModelId, " not ( " + mapping.FilterString + ") ");
                    } else
                    {
                        String condition = modelProcessFilterMap[mapping.ModelId] + " AND not(" + mapping.FilterString + ") ";
                        modelProcessFilterMap[mapping.ModelId] = condition;
                    }
                    List<Block> blocks = context.GetBlocks(mapping.ModelId, mapping.FilterString);

                    foreach(Block block in blocks)
                    {
                        if(block.ProcessNos == null)
                        {
                            block.ProcessNos = new List<int>();
                        }
                        block.ProcessNos.Add(process.ProcessNumber);
                        Decimal minigCost = GetMiningCost(block, context.Year);
                        Decimal processValue = GetProcessValue(block, process, context.Year) - minigCost;
                        
                        if (processValue != 0 )
                        {
                            processValue = processValue * (Decimal)F;
                            if (processValue < 0)
                            {
                                Write( processValue +" B" + block.Id + "p" + count, sw);
                            } else
                            {
                                Write(" + " + processValue +" B" + block.Id + "p" + count, sw);
                            }
                        }
                        if(minigCost != 0) 
                            Write(" - "+ minigCost * (Decimal)F  + " B" + block.Id + "s1", sw);
                    }
                }
                count++;
            }

            foreach(var ModelId in modelProcessFilterMap.Keys)
            {
                List<Block> blocks = context.GetBlocks(ModelId, modelProcessFilterMap[ModelId]);
                foreach (Block block in blocks)
                {
                    Decimal minigCost = GetMiningCost(block, context.Year);
                    if (minigCost > 0)
                        Write(" - " + minigCost * (Decimal)F + " B" + block.Id + "w1", sw);
                }
            }
            
        }

        private void WriteConstraints(StreamWriter sw)
        {
            WriteGeotechConstraints(sw);
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
                decimal xinc = 0, yinc = 0, zinc = 0, max_dim = 0;
                decimal xm = 0, ym = 0, zm = 0;
                List<ModelDimension> modelDimensions = context.GetModelDimension(model.Id);
                foreach(ModelDimension dimension in modelDimensions)
                {
                    if (dimension.Type.Equals("Origin"))
                    {
                        xm = dimension.XDim;
                        ym = dimension.YDim;
                        zm = dimension.ZDim;
                    }
                    else if (dimension.Type.Equals("Dimensions"))
                    {
                        xinc = dimension.XDim;
                        yinc = dimension.YDim;
                        zinc = dimension.ZDim;
                        if(xinc > yinc)
                        {
                            max_dim = xinc;
                        } else
                        {
                            max_dim = yinc;
                        }
                    }
                }
                
                Geotech geotech = context.GetGeotechByModel(model.Id);
                String selectstr = "max ( ";
                if (!geotech.UseScript)
                {
                    String columnName = context.GetColumnNameById(geotech.Id, model.Id);
                    if(columnName == null)
                    {
                        throw new Exception("Please check your geotech configuration.");
                    }
                    selectstr = selectstr +  context.GetColumnNameById(geotech.Id, model.Id) + ") as angle";
                } else
                {
                    selectstr = selectstr + geotech.Script + ") as angle";
                }

                double max_ira = context.GetIRA(selectstr, model.Id) * Math.PI / 180; ;

                int nbenches = (int)Math.Ceiling(max_dim / (zinc / (decimal)Math.Tan(max_ira)));
                
                List<Block> blocks = context.GetGeotechBlocks(model.Id);
                foreach (Block b in blocks)
                {
                    decimal dist = zinc / (decimal)Math.Tan(max_ira);
                    decimal xorth =(decimal) b.data["xortho"];
                    decimal yorth = (decimal) b.data["yortho"];
                    int imin = (int)((xorth - dist + xinc - xm)/xinc );
                    int imax = (int)((xorth + dist + xinc - xm) / xinc);
                    int jmin = (int)((yorth + dist + yinc - ym) / yinc);
                    int jmax = (int)((yorth + dist + yinc - ym) / yinc);
                    if (imin <= 0) imin = 1;
                    if (jmin <= 0) jmin = 1;
                    
                    for (int k = 1; k <= nbenches - 1; k++)
                    {
                        for (int i = imin; i <= imax; i++)
                        {
                            for(int j = jmin; j <= jmax; j++)
                            {
                                foreach(var bb in blocks)
                                {
                                    if(((int)bb.data["i"] >= imin && (int)bb.data["i"] <= imax ) 
                                        && ((int)bb.data["j"] >= jmin && (int)bb.data["j"] <= jmax)
                                        && ((int)bb.data["k"] <= (int)b.data["k"] + k - 1))
                                    {
                                        Block upperblock = null;
                                        foreach (var bi in blocks)
                                        {
                                            if (((int)bb.data["i"] == (int)bi.data["i"])
                                                && ((int)bb.data["j"] == (int)bi.data["j"])
                                                && ((int)bb.data["k"] == (int)bi.data["k"] + 1))
                                            {
                                                upperblock = bi;
                                                break;
                                            }
                                        }
                                        if (upperblock == null) continue;
                                        decimal tonneswt = context.GetTonnesWtForBlock(bb);
                                        decimal tonneswtba = context.GetTonnesWtForBlock(upperblock);
                                        if (tonneswtba == 0) continue;
                                        decimal ratio = tonneswt / tonneswtba;
                                        List<int> processNos = bb.ProcessNos;
                                        if (processNos != null && processNos.Count > 0 )
                                        {
                                            foreach(int processNo in processNos)
                                            {
                                                Write(" + B" + bb.Id + "p" + processNo + " + B" + bb.Id + "s1", sw);
                                            }
                                           
                                        } else
                                        {
                                            Write(" + B" + bb.Id + "w1", sw);
                                        }
                                        List<int> ubprocessNos = upperblock.ProcessNos;
                                        if (ubprocessNos != null && ubprocessNos.Count > 0)
                                        {
                                            foreach (int processNo in ubprocessNos)
                                            {
                                                Write(" - " + ratio + "B" + upperblock.Id + "p" + processNo + " - " + ratio + "B" + upperblock.Id + "s1", sw);
                                            }                                           
                                        }
                                        else
                                        {
                                            Write(" - " + ratio + "B" + upperblock.Id + "w1", sw);
                                        }

                                        Write(" <= 0", sw);
                                        Write("", sw);
                                    } 
                                }
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
                    foreach(var mapping in process.Mapping)
                    {
                        List<Block> blocks = context.GetBlocks(mapping.ModelId, mapping.FilterString);
                        foreach (Block b in blocks)
                        {
                            Write(" + B" + b.Id + "p" + process.ProcessNumber + " + B" + b.Id + "s1", sw);
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
                        foreach (var mapping in process.Mapping)
                        {
                            List<Block> blocks = context.GetBlocks(mapping.ModelId, mapping.FilterString);
                            foreach (Block b in blocks)
                            {
                                Decimal value = context.GetFieldValueforBlock(b, product.UnitName);
                                Decimal tonnesWt = context.GetTonnesWtForBlock(b);

                                Write(" + "+ (value /tonnesWt)+ " B" + b.Id + "p" + process.ProcessNumber + " + B" + b.Id + "s1", sw);
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
                            foreach (var mapping in process.Mapping)
                            {
                                
                                List<Block> blocks = context.GetBlocks(mapping.ModelId, mapping.FilterString);
                                foreach (Block b in blocks)
                                {
                                    Decimal value = context.GetFieldValueforBlock(b, product.UnitName);
                                    Decimal tonnesWt = context.GetTonnesWtForBlock(b);
                                    Write(" + " + (value / tonnesWt) + " B" + b.Id + "p" + process.ProcessNumber + " + B" + b.Id + "s1", sw);
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
                        foreach (var mapping in process.Mapping)
                        {
                            List<Block> blocks = context.GetBlocks(mapping.ModelId, mapping.FilterString);
                            foreach (Block b in blocks)
                            {
                                Decimal processRatio = context.GetFieldValueforBlock(b, product.UnitName);
                                Decimal blockGrade = context.GetFieldValueforBlock(b, gradeLimit.GradeName);
                                Decimal coeff = processRatio * (targetGrade * blockGrade);
                                if (coeff < 0 )
                                {
                                    Write(" "+coeff+"B" + b.Id + "p" + process.ProcessNumber +  " "+coeff+ "B" + b.Id + "s1", sw);
                                } else
                                {
                                    Write(" +" + coeff + "B" + b.Id + "p" + process.ProcessNumber + " +" + coeff + "B" + b.Id + "s1", sw);
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
                    foreach(String productName in productNames)
                    {
                        Product product = context.GetProductByName(productName);
                        if (product == null) continue;

                        foreach (int processId in product.ProcessIds)
                        {
                            Process process = context.GetProcessById(processId);
                            foreach (var mapping in process.Mapping)
                            {
                                List<Block> blocks = context.GetBlocks(mapping.ModelId, mapping.FilterString);
                                foreach (Block b in blocks)
                                {
                                    Decimal processRatio = context.GetUnitValueforBlock(b, product.UnitType, product.UnitId);
                                    Decimal blockGrade = context.GetFieldValueforBlock(b, gradeLimit.GradeName);
                                    Decimal coeff = processRatio * (targetGrade * blockGrade);
                                    if (coeff < 0)
                                    {
                                        Write(" " + coeff + "B" + b.Id + "p" + process.ProcessNumber + " " + coeff + "B" + b.Id + "s1", sw);
                                    }
                                    else
                                    {
                                        Write(" +" + coeff + "B" + b.Id + "p" + process.ProcessNumber + " +" + coeff + "B" + b.Id + "s1", sw);
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
                    if( opex.UnitType > 0 )
                    {
                        Decimal revExprValue = context.GetUnitValueforBlock(b, opex.UnitType, opex.UnitId);
                        miningCost = miningCost* revExprValue;
                    }
                    
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

    }
}
