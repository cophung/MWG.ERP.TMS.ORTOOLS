using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWG.ERP.TMS.ORTOOLS
{
    public class ML_VrpTimeWindows
    {
        private int intVehicleNumber = 1; // The number of vehicles in the fleet
        private int intDepot = 0; // The index of the depot
        private long intWaitingTime = 30; // Waiting Time   
        private long intVehicleCapacities = 50; // vehicle maximum capacities (50: khối lượng cho xe máy)

        public long[][] TimeMatrix { get; set; } // An array of travel times between locations
        public long[][] TimeWindows { get; set; } // An array of time windows for the locations
        public int VehicleNumber { get => intVehicleNumber; set => intVehicleNumber = value; }
        public int Depot { get => intDepot; set => intDepot = value; }
        public long WaitingTime { get => intWaitingTime; set => intWaitingTime = value; }
        public long VehicleCapacities { get => intVehicleCapacities; set => intVehicleCapacities = value; }
    }
}
