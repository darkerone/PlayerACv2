using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerACv2.Enums
{
    /// <summary>
    /// Enum des sources effectuant les mises à jour de la position de la musique
    /// </summary>
    public enum PositionUpdateSourceEnum
    {
        Slider = 0,
        Player = 1,
        ThumbDrag = 2,
        Jump = 3
    }
}
