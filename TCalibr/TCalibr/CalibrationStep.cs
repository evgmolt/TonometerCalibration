using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCalibr
{
    internal enum CalibrationStep
    {
        NoConnected,
        Step015,
        Step025,
        Step035,
        ReadyToRecord,
        Completed
    }
}
