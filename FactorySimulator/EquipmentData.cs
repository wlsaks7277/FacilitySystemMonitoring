using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySimulator
{
    public class EquipmentData
    {
        public string EquipmentId { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public int ProductionCount { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
