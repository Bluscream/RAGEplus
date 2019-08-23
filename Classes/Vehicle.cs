using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAGEplus
{
    public static class Vehicle
    {
        public static void BurstTires(this Rage.Vehicle vehicle, int wheelCount = 4) { for (int i = 0; i < wheelCount; i++) { try { vehicle.Wheels[i].BurstTire(); } catch { } } }
    }
}
