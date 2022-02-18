using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWG.ERP.TMS.ORTOOLS
{
    public class ML_CVRP_IndexRouteItem
    {
        private List<long> lstNodeIndex = new List<long>(); // danh sách index của 1 tuyến
        private long longTotalDistance = 0; // tổng quảng đường của 1 tuyến
        private long longTotalLoad = 0; // tổng khối lượng của 1 tuyến

        public List<long> ListNodeIndex { get => lstNodeIndex; set => lstNodeIndex = value; }
        public long TotalDistance { get => longTotalDistance; set => longTotalDistance = value; }
        public long TotalLoad { get => longTotalLoad; set => longTotalLoad = value; }
    }


    public class ML_CVRP_IndexRoute
    {
        private List<ML_CVRP_IndexRouteItem> lstML_CVRP_IndexRouteItem = new List<ML_CVRP_IndexRouteItem>(); // danh sách index + tổng khoảng cách + khối lượng của 1 tuyến
        private List<List<long>> lstNodeIndex = new List<List<long>>(); // danh sách index đã phân tuyến
        private List<long> lstTotalDistance = new List<long>(); // danh sách tổng quảng đường của phân tuyến
        private List<long> lstTotalLoad = new List<long>(); // danh sách tổng khối lượng của phân tuyến
        private List<int> lstDroppedNodes = new List<int>(); // danh sách index bị bỏ sót trong quá trình phân tuyến
        private long longTotalDistance = 0; // tổng quảng đường của phân tuyến
        private long longTotalLoad = 0; // tổng khối lượng của phân tuyến

        public List<ML_CVRP_IndexRouteItem> ListML_CVRP_IndexRouteItem { get => lstML_CVRP_IndexRouteItem; set => lstML_CVRP_IndexRouteItem = value; }
        public List<List<long>> ListNodeIndex { get => lstNodeIndex; set => lstNodeIndex = value; }
        public List<long> ListTotalDistance { get => lstTotalDistance; set => lstTotalDistance = value; }
        public List<long> ListTotalLoad { get => lstTotalLoad; set => lstTotalLoad = value; }
        public List<int> ListDroppedNodes { get => lstDroppedNodes; set => lstDroppedNodes = value; }
        public long TotalDistance { get => longTotalDistance; set => longTotalDistance = value; }
        public long TotalLoad { get => longTotalLoad; set => longTotalLoad = value; }
    }
}
