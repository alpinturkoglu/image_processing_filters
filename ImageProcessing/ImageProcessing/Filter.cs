using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    enum Filter
    {
        
        [Description("Negative")]
        negative=0,

        [Description("Median")]
        median=1,

        [Description("Mean")]
        mean=2,

        [Description("Smoothing")]
        smoothing=3,

        [Description("Blur")]
        blur=4

    }
}
