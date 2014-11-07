using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongDownloadConsole
{
    public enum ProcessingStateEnum :byte
    {
        Neu = 0,
        Heruntergeladen = 1,
        BereitZumLoeschen = 2,
        Geloescht = 3
    }

    public enum RecordingQualityFlagEnum
    {
        // ReSharper disable InconsistentNaming
        NQ = 1,
        HQ = 2,
        HD = 4,
        HQ_NQ = 3,
        HD_NQ = 5,
        HD_HQ = 6,
        HD_HQ_NQ = 7
        // ReSharper restore InconsistentNaming
    }
}
