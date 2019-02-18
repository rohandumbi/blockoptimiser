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
            String fileName = @"C:\\blockoptimizor\\blockoptimizor-dump.lp";

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
                    String condition = modelProcessFilterMap[mapping.ModelId];
                    if(condition == null)
                    {
                        modelProcessFilterMap.Add(mapping.ModelId, " not ( " + mapping.FilterString + ") ");
                    } else
                    {
                        condition = condition + " AND not(" + mapping.FilterString + ") ";
                        modelProcessFilterMap.Add(mapping.ModelId, condition);
                    }
                    List<Block> blocks = context.GetBlocks(mapping.ModelId, mapping.FilterString);

                    foreach(Block block in blocks)
                    {
                        List<int> blockIds = context.processBlockMapping[process.Id];
                        if(blockIds == null)
                        {
                            blockIds = new List<int>();
                            context.processBlockMapping.Add(process.Id, blockIds);
                        }
                        blockIds.Add(block.Id);
                        Decimal minigCost = GetMiningCost(block, context.Year);
                        Decimal processValue = GetProcessValue(block, process, context.Year) - minigCost;
                        processValue = processValue * (Decimal)F;
                        if (processValue != 0 )
                        {
                            if(processValue < 0)
                            {
                                Write( processValue +" B" + block.Id + "p" + count, sw);
                            } else
                            {
                                Write(" + " + processValue +" B" + block.Id + "p" + count, sw);
                            }
                        }
                        
                        Write(" - "+ minigCost + " B" + block.Id + "s" + count, sw);
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
                    Write(" - " + minigCost + " B" + block.Id + "w" + count, sw);
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
                double xinc = 0, yinc = 0, zinc = 0, max_dim = 0;
                List<ModelDimension> modelDimensions = context.GetModelDimension(model.Id);
                foreach(ModelDimension dimension in modelDimensions)
                {
                    if(dimension.Type.Equals("Dimensions"))
                    {
                        xinc = (double)dimension.XDim;
                        yinc = (double)dimension.YDim;
                        zinc = (double)dimension.ZDim;
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
                if (geotech.Type == 1)
                {
                    String columnName = context.GetColumnNameById(geotech.Id, model.Id);
                    if(columnName == null)
                    {
                        throw new Exception("Please check your geotech configuration.");
                    }
                    selectstr = context.GetColumnNameById(geotech.Id, model.Id) + ") as angle";
                } else
                {
                    selectstr = geotech.Script + ") as angle";
                }

                double max_ira = context.GetIRA(selectstr, model.Id) * Math.PI / 180; ;

                int nbenches = (int)Math.Ceiling(max_dim / (zinc / Math.Tan(max_ira)));
            }

        }
        private void WriteProcessLimitConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\Process Limits");
        }

        private void WriteGradeLimitConstraints(StreamWriter sw)
        {
            sw.WriteLine("\\ Grade Limits");
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
                    Decimal revExprValue = context.GetUnitValueforBlock(b, opex.UnitType, opex.UnitId);
                    return miningCost * revExprValue;
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
