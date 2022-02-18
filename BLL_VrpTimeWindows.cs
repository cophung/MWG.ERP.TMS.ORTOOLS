using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using MWG.Library.Core.Data;
using MWG.Library.Core.Utils;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes; // Duration

namespace MWG.ERP.TMS.ORTOOLS
{
    public class BLL_VrpTimeWindows
    {
        public static ML_VrpTimeWindows_IndexRoute FindRoutes(ML_VrpTimeWindows objML_VrpTimeWindows, ref ResultMessage objResultMessage)
        {
            ML_VrpTimeWindows_IndexRoute objML_VrpTimeWindows_IndexRoute = new ML_VrpTimeWindows_IndexRoute();

            try
            {
                // Create Routing Index Manager
                RoutingIndexManager manager =
                    new RoutingIndexManager(objML_VrpTimeWindows.TimeMatrix.GetLength(0), objML_VrpTimeWindows.VehicleNumber, objML_VrpTimeWindows.Depot);

                // Create Routing Model.
                RoutingModel routing = new RoutingModel(manager);

                // Create and register a transit callback.
                int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable Index to time matrix NodeIndex.
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return objML_VrpTimeWindows.TimeMatrix[fromNode][toNode];
                });

                // Define cost of each arc.
                routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

                // Add Time constraint.
                routing.AddDimension(transitCallbackIndex, // transit callback
                                     objML_VrpTimeWindows.WaitingTime, // allow waiting time
                                     objML_VrpTimeWindows.VehicleCapacities, // vehicle maximum capacities
                                     false,                // start cumul to zero
                                     "Time");
                RoutingDimension timeDimension = routing.GetMutableDimension("Time");

                // Add time window constraints for each location except depot.
                for (int i = 1; i < objML_VrpTimeWindows.TimeWindows.GetLength(0); ++i)
                {
                    long index = manager.NodeToIndex(i);
                    timeDimension.CumulVar(index).SetRange(objML_VrpTimeWindows.TimeWindows[i][0], objML_VrpTimeWindows.TimeWindows[i][1]);
                }

                // Add time window constraints for each vehicle start node.
                for (int i = 0; i < objML_VrpTimeWindows.VehicleNumber; ++i)
                {
                    long index = routing.Start(i);
                    timeDimension.CumulVar(index).SetRange(objML_VrpTimeWindows.TimeWindows[0][0], objML_VrpTimeWindows.TimeWindows[0][1]);
                }

                // Instantiate route start and end times to produce feasible times.
                for (int i = 0; i < objML_VrpTimeWindows.VehicleNumber; ++i)
                {
                    routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(routing.Start(i)));
                    routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(routing.End(i)));
                }

                // Setting first solution heuristic.
                RoutingSearchParameters searchParameters =
                    operations_research_constraint_solver.DefaultRoutingSearchParameters();
                searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.Automatic;
                searchParameters.TimeLimit = new Duration { Seconds = 20 };

                // Solve the problem.
                Assignment solution = routing.SolveWithParameters(searchParameters);

                // Print solution
                if (solution == null)
                {
                    objResultMessage.IsError = true;
                    objResultMessage.Message = "Không tìm được phân tuyến tối ưu";
                    return objML_VrpTimeWindows_IndexRoute;
                }
                else
                {
                    long totalTime = 0;
                    for (int i = 0; i < objML_VrpTimeWindows.VehicleNumber; ++i)
                    {
                        Console.WriteLine("Route for Vehicle {0}:", i);
                        ML_VrpTimeWindows_IndexRouteItem objML_VrpTimeWindows_IndexRouteItem = new ML_VrpTimeWindows_IndexRouteItem();
                        var index = routing.Start(i);
                        while (routing.IsEnd(index) == false)
                        {
                            var timeVar = routing.GetMutableDimension("Time").CumulVar(index);
                            //Console.Write("{0} Time({1},{2}) -> ", manager.IndexToNode(index), solution.Min(timeVar), solution.Max(timeVar));
                            objML_VrpTimeWindows_IndexRouteItem.ListNodeIndex.Add(manager.IndexToNode(index));
                            index = solution.Value(routing.NextVar(index));
                        }

                        var endTimeVar = routing.GetMutableDimension("Time").CumulVar(index);
                        //Console.WriteLine("{0} Time({1},{2})", manager.IndexToNode(index), solution.Min(endTimeVar), solution.Max(endTimeVar));
                        objML_VrpTimeWindows_IndexRouteItem.ListNodeIndex.Add(manager.IndexToNode(index));
                        //Console.WriteLine("Time of the route: {0}min", solution.Min(endTimeVar));
                        objML_VrpTimeWindows_IndexRouteItem.TotalTimeRoute = solution.Min(endTimeVar);

                        objML_VrpTimeWindows_IndexRoute.ListML_VrpTimeWindows_IndexRouteItem.Add(objML_VrpTimeWindows_IndexRouteItem);

                        totalTime += solution.Min(endTimeVar);
                    }

                    objML_VrpTimeWindows_IndexRoute.ListML_VrpTimeWindows_IndexRouteItem = objML_VrpTimeWindows_IndexRoute.ListML_VrpTimeWindows_IndexRouteItem.OrderByDescending(o => o.ListNodeIndex.Count).ToList();
                    for (int i = 0; i < objML_VrpTimeWindows_IndexRoute.ListML_VrpTimeWindows_IndexRouteItem.Count; i++)
                    {
                        objML_VrpTimeWindows_IndexRoute.ListNodeIndex.Add(objML_VrpTimeWindows_IndexRoute.ListML_VrpTimeWindows_IndexRouteItem[i].ListNodeIndex);
                        objML_VrpTimeWindows_IndexRoute.ListTimeRoute.Add(objML_VrpTimeWindows_IndexRoute.ListML_VrpTimeWindows_IndexRouteItem[i].TotalTimeRoute);
                    }

                    //Console.WriteLine("Total time of all routes: {0}min", totalTime);
                    objML_VrpTimeWindows_IndexRoute.TotalTimeAllRoute = totalTime;

                    return objML_VrpTimeWindows_IndexRoute;
                }
            }
            catch (Exception objEx)
            {
                objResultMessage.IsError = true;
                objResultMessage.Message = "Lỗi xử lý phân tuyến, vui lòng liên hệ IT";
                objResultMessage.MessageDetail = objEx.ToString();
                return objML_VrpTimeWindows_IndexRoute;
            }

        }
    }
}
