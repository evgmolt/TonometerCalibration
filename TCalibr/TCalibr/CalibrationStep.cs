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
        Step01,
        Step02,
        Step03,
        ReadyToRecord,
        Completed
    }
}
