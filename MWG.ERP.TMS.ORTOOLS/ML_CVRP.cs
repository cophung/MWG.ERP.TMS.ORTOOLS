using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWG.ERP.TMS.ORTOOLS
{
    public class ML_CVRP
    {
        private long longVehicleNumber = 0;
        private int intDepot = 0;

        public long[][] DistanceMatrix { get; set; }
        public long[] Demands { get; set; }
        public long[] VehicleCapacities { get; set; }
        public long VehicleNumber { get => longVehicleNumber; set => longVehicleNumber = value; }
        public int Depot { get => intDepot; set => intDepot = value; }

        public ML_CVRP () {
        
        }
    }
}
