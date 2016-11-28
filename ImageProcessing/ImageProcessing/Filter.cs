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
        [Description("Min Filter")]
        minFilter=0,
        
        [Description("Max Filter")]
        maxFilter=1,

        [Description("Median Filter")]
        medianFilter=2,

        [Description("Average Filter")]
        averageFilter=3,

        [Description("Smoothing Filter")]
        smoothing=4,

        [Description("Thresholding Filter")]
        thresholding=5

    }
}
