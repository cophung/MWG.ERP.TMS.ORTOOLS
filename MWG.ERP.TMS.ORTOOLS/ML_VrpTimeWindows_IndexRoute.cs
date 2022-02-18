using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWG.ERP.TMS.ORTOOLS
{
    public class ML_VrpTimeWindows_IndexRouteItem
    {
        private List<long> lstNodeIndex = new List<long>(); // danh sách index của 1 tuyến
        private long longTotalTimeRoute = 0; // tổng thời gian di chuyển của 1 tuyến

        public List<long> ListNodeIndex { get => lstNodeIndex; set => lstNodeIndex = value; }
        public long TotalTimeRoute { get => longTotalTimeRoute; set => longTotalTimeRoute = value; }
    }

    public class ML_VrpTimeWindows_IndexRoute
    {
        private List<ML_VrpTimeWindows_IndexRouteItem> lstML_VrpTimeWindows_IndexRouteItem = new List<ML_VrpTimeWindows_IndexRouteItem>(); // danh sách: index + tổng thời gian di chuyển của từng tuyến

        private List<List<long>> lstNodeIndex = new List<List<long>>(); // danh sách index đã phân tuyến
        private List<long> lstListTimeRoute = new List<long>(); // danh sách thời gian di chuyển của từng tuyến
        private long longTotalTimeAllRoute = 0; // tổng thời gian của phân tuyến


        public List<ML_VrpTimeWindows_IndexRouteItem> ListML_VrpTimeWindows_IndexRouteItem { get => lstML_VrpTimeWindows_IndexRouteItem; set => lstML_VrpTimeWindows_IndexRouteItem = value; } // danh sách: index + tổng thời gian di chuyển của từng tuyến

        public List<List<long>> ListNodeIndex { get => lstNodeIndex; set => lstNodeIndex = value; }
        public List<long> ListTimeRoute { get => lstListTimeRoute; set => lstListTimeRoute = value; }
        public long TotalTimeAllRoute { get => longTotalTimeAllRoute; set => longTotalTimeAllRoute = value; }
    }
}
