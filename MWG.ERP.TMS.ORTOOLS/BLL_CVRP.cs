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
    public class BLL_CVRP
    {
        /// <summary>
        /// The capacitated vehicle routing problem
        /// </summary>
        /// <param name="objML_CVRP"></param>
        /// <returns></returns>
        public static ML_CVRP_IndexRoute FindRoutes(ML_CVRP objML_CVRP, ref ResultMessage objResultMessage)
        {
            ML_CVRP_IndexRoute objML_CVRP_IndexRoute = new ML_CVRP_IndexRoute(); // kết quả phân tuyến
            const long penalty = 50000; // giá trị penalty

            try
            {
                // Create Routing Index Manager
                RoutingIndexManager manager =
                    new RoutingIndexManager(objML_CVRP.DistanceMatrix.GetLength(0), objML_CVRP.VehicleCapacities.Length, objML_CVRP.Depot);

                // Create Routing Model.
                RoutingModel routing = new RoutingModel(manager);

                // To use the routing solver, you need to create a distance (or transit) callback: a function that takes any pair of locations and returns the distance between them. The easiest way to do this is using the distance matrix
                int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable Index to distance matrix NodeIndex.
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return objML_CVRP.DistanceMatrix[fromNode][toNode];
                });

                // Define cost of each arc.
                routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

                // Add Capacity constraint.
                int demandCallbackIndex = routing.RegisterUnaryTransitCallback((long fromIndex) =>
                {
                    // Convert from routing variable Index to demand NodeIndex.
                    var fromNode = manager.IndexToNode(fromIndex);
                    return objML_CVRP.Demands[fromNode];
                });
                routing.AddDimensionWithVehicleCapacity(demandCallbackIndex,
                                                        0, // null capacity slack
                                                        objML_CVRP.VehicleCapacities, // vehicle maximum capacities
                                                        true,                   // start cumul to zero
                                                        "Capacity");

                // Allow to drop nodes.
                for (int i = 1; i < objML_CVRP.DistanceMatrix.GetLength(0); ++i)
                {
                    routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, penalty);
                }

                // Setting first solution heuristic.
                RoutingSearchParameters searchParameters =
                    operations_research_constraint_solver.DefaultRoutingSearchParameters();
                searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.Automatic;
                searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.Automatic;
                searchParameters.TimeLimit = new Duration { Seconds = 20 };

                // Solve the problem.
                Assignment solution = routing.SolveWithParameters(searchParameters);


                // Print solution
                if (solution == null)
                {
                    objResultMessage.IsError = true;
                    objResultMessage.Message = "Không tìm được phân tuyến tối ưu";
                    return objML_CVRP_IndexRoute;
                }
                else
                {
                    // Mã vận đơn bị bỏ sót
                    for (int index = 0; index < routing.Size(); ++index)
                    {
                        if (routing.IsStart(index) || routing.IsEnd(index))
                        {
                            continue;
                        }
                        if (solution.Value(routing.NextVar(index)) == index)
                        {
                            objML_CVRP_IndexRoute.ListDroppedNodes.Add(manager.IndexToNode(index));
                        }
                    }

                    // in ra danh sách phân tuyến
                    long longTotalDistance = 0;
                    long longTotalLoad = 0;
                    for (int i = 0; i < objML_CVRP.VehicleCapacities.Length; ++i)
                    {
                        long routeDistance = 0;
                        long routeLoad = 0;
                        List<long> routeShipmentOrderID = new List<long>();

                        var index = routing.Start(i);

                        while (routing.IsEnd(index) == false)
                        {
                            long nodeIndex = manager.IndexToNode(index);
                            routeLoad += objML_CVRP.Demands[nodeIndex];
                            routeShipmentOrderID.Add(nodeIndex);

                            var previousIndex = index;
                            index = solution.Value(routing.NextVar(index));
                            routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                        }

                        //objML_CVRP_IndexRoute.ListNodeIndex.Add(routeShipmentOrderID);
                        //objML_CVRP_IndexRoute.ListTotalDistance.Add(routeDistance);
                        //objML_CVRP_IndexRoute.ListTotalLoad.Add(routeLoad);
                        ML_CVRP_IndexRouteItem objML_CVRP_IndexRouteItem = new ML_CVRP_IndexRouteItem
                        {
                            ListNodeIndex = routeShipmentOrderID,
                            TotalDistance = routeDistance,
                            TotalLoad = routeLoad
                        };
                        objML_CVRP_IndexRoute.ListML_CVRP_IndexRouteItem.Add(objML_CVRP_IndexRouteItem);
                        longTotalDistance += routeDistance;
                        longTotalLoad += routeLoad;
                    }

                    objML_CVRP_IndexRoute.ListML_CVRP_IndexRouteItem = objML_CVRP_IndexRoute.ListML_CVRP_IndexRouteItem.OrderByDescending(o => o.ListNodeIndex.Count).ToList();
                    for (int i = 0; i < objML_CVRP_IndexRoute.ListML_CVRP_IndexRouteItem.Count; i++)
                    {
                        objML_CVRP_IndexRoute.ListNodeIndex.Add(objML_CVRP_IndexRoute.ListML_CVRP_IndexRouteItem[i].ListNodeIndex);
                        objML_CVRP_IndexRoute.ListTotalDistance.Add(objML_CVRP_IndexRoute.ListML_CVRP_IndexRouteItem[i].TotalDistance);
                        objML_CVRP_IndexRoute.ListTotalLoad.Add(objML_CVRP_IndexRoute.ListML_CVRP_IndexRouteItem[i].TotalLoad);
                    }

                    objML_CVRP_IndexRoute.TotalDistance = longTotalDistance;
                    objML_CVRP_IndexRoute.TotalLoad = longTotalLoad;
                }

                return objML_CVRP_IndexRoute;
            }
            catch (Exception objEx)
            {
                objResultMessage.IsError = true;
                objResultMessage.Message = "Lỗi xử lý phân tuyến, vui lòng liên hệ IT";
                objResultMessage.MessageDetail = objEx.ToString();
                return objML_CVRP_IndexRoute;
            }
        }
    }
}
