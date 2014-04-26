using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public enum SavePropertyState
    {
        /// <summary>
        /// The save bundle does not modify the value of this property.
        /// </summary>
        Unchanged,

        /// <summary>
        /// The save bundle has a value for this property that has not yet been
        /// applied or discarded.
        /// </summary>
        Pending,

        /// <summary>
        /// The save bundle has a value for this property that has been set to
        /// the entity via a call to SaveProperty.Apply.
        /// </summary>
        Applied,

        /// <summary>
        /// The save bundle has a value for this property that has been explicity
        /// ignored via a call to SaveProperty.Ignore.
        /// </summary>
        Ignored
    }
}
